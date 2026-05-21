#!/usr/bin/env bash
# Builds a throwaway consumer of Deque.AxeCore.Selenium against a specific
# Selenium.WebDriver version, then asserts that the build/Deque.AxeCore.Selenium.targets
# picker selected the correct pre-compiled variant.
#
# Assumes Deque.AxeCore.Commons and Deque.AxeCore.Selenium have already been packed
# into their respective packages/<name>/src/bin/Release directories and that the
# AssemblyRefInspector has been built.

set -euo pipefail

if [[ $# -ne 5 ]]; then
    echo "usage: $0 <flavor> <selenium-version> <expected-ref> <rejected-ref> <consumer-tfm>" >&2
    echo "  flavor:           advisory label, e.g. legacy or new" >&2
    echo "  selenium-version: e.g. 4.4.0 or 4.44.0" >&2
    echo "  expected-ref:     simple assembly name the picker should reference (WebDriver or Selenium.WebDriver)" >&2
    echo "  rejected-ref:     simple assembly name the picker should NOT reference" >&2
    echo "  consumer-tfm:     e.g. net471 or netstandard2.0" >&2
    exit 2
fi

flavor="$1"
selenium_version="$2"
expected="$3"
rejected="$4"
consumer_tfm="$5"

repo_root=$(cd "$(dirname "$0")/../.." && pwd)
inspector="$repo_root/.github/smoke/AssemblyRefInspector/bin/Release/net6.0/AssemblyRefInspector.dll"
commons_feed="$repo_root/packages/commons/src/bin/Release"
selenium_feed="$repo_root/packages/selenium/src/bin/Release"

if [[ ! -f "$inspector" ]]; then
    echo "FAIL: AssemblyRefInspector not built. Run: dotnet build $repo_root/.github/smoke/AssemblyRefInspector/AssemblyRefInspector.csproj -c Release" >&2
    exit 2
fi

# Read the just-packed package version so the throwaway consumer pins to it.
pkg_path=$(ls "$selenium_feed"/Deque.AxeCore.Selenium.*.nupkg 2>/dev/null | head -1 || true)
if [[ -z "$pkg_path" ]]; then
    echo "FAIL: no Deque.AxeCore.Selenium nupkg found in $selenium_feed. Pack it first." >&2
    exit 2
fi
pkg_version=$(basename "$pkg_path" | sed -E 's|Deque\.AxeCore\.Selenium\.([0-9].+)\.nupkg|\1|')

tmpdir=$(mktemp -d)
trap 'rm -rf "$tmpdir"' EXIT

cat > "$tmpdir/Consumer.csproj" <<PROJEOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>$consumer_tfm</TargetFramework>
    <RestoreSources>$selenium_feed;$commons_feed;https://api.nuget.org/v3/index.json</RestoreSources>
    <RestorePackagesPath>$tmpdir/packages</RestorePackagesPath>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Selenium.WebDriver" Version="$selenium_version" />
    <PackageReference Include="Deque.AxeCore.Selenium" Version="$pkg_version" />
  </ItemGroup>
</Project>
PROJEOF

cat > "$tmpdir/Hello.cs" <<'CSEOF'
using Deque.AxeCore.Selenium;
using OpenQA.Selenium;
namespace SmokeConsumer
{
    public class Hello
    {
        public AxeBuilder Make(IWebDriver d) => new AxeBuilder(d);
    }
}
CSEOF

echo "=== smoke: flavor=$flavor selenium=$selenium_version tfm=$consumer_tfm ==="

dotnet build "$tmpdir/Consumer.csproj" -c Release

dll="$tmpdir/bin/Release/$consumer_tfm/Deque.AxeCore.Selenium.dll"
if [[ ! -f "$dll" ]]; then
    echo "FAIL: variant DLL was not copied to consumer's bin at $dll" >&2
    exit 1
fi

dotnet "$inspector" "$dll" --expect "$expected" --reject "$rejected"
