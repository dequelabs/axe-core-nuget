using System;
using System.Linq;
using System.Reflection;

// Lists the referenced assemblies of a target DLL and asserts that one specific
// reference is present and another specific reference is absent. Used by the
// dual-selenium-smoke workflow to verify that the .targets picker selected the
// correct pre-compiled variant for a given consumer's Selenium.WebDriver version.
//
// Usage:
//   AssemblyRefInspector <dll-path> --expect <SimpleName> --reject <SimpleName>
//
// Exits 0 on success, 1 on assertion failure, 2 on argument/IO error.

if (args.Length != 5 || args[1] != "--expect" || args[3] != "--reject")
{
    Console.Error.WriteLine("usage: AssemblyRefInspector <dll-path> --expect <SimpleName> --reject <SimpleName>");
    return 2;
}

var dllPath = args[0];
var expected = args[2];
var rejected = args[4];

if (!System.IO.File.Exists(dllPath))
{
    Console.Error.WriteLine($"FAIL: file not found: {dllPath}");
    return 2;
}

var refs = Assembly.LoadFrom(dllPath).GetReferencedAssemblies();
Console.WriteLine($"Referenced assemblies in {dllPath}:");
foreach (var r in refs)
{
    Console.WriteLine($"  {r.FullName}");
}

var sawExpected = refs.Any(r => r.Name == expected);
var sawRejected = refs.Any(r => r.Name == rejected);

if (sawExpected && !sawRejected)
{
    Console.WriteLine($"PASS: references {expected}, does not reference {rejected}.");
    return 0;
}

if (!sawExpected)
{
    Console.Error.WriteLine($"FAIL: expected reference to {expected} was not present.");
}
if (sawRejected)
{
    Console.Error.WriteLine($"FAIL: rejected reference to {rejected} was present (the picker selected the wrong variant).");
}
return 1;
