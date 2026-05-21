using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

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

IReadOnlyList<AssemblyReferenceInfo> refs;
try
{
    refs = ReadAssemblyReferences(dllPath);
}
catch (Exception ex) when (ex is BadImageFormatException || ex is IOException || ex is UnauthorizedAccessException)
{
    Console.Error.WriteLine($"FAIL: could not parse assembly metadata for {dllPath}: {ex.Message}");
    return 2;
}

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

static IReadOnlyList<AssemblyReferenceInfo> ReadAssemblyReferences(string dllPath)
{
    using var stream = File.OpenRead(dllPath);
    using var peReader = new PEReader(stream);
    if (!peReader.HasMetadata)
    {
        throw new BadImageFormatException("file does not contain .NET metadata");
    }

    var metadataReader = peReader.GetMetadataReader();
    if (!metadataReader.IsAssembly)
    {
        throw new BadImageFormatException("metadata does not describe a managed assembly");
    }

    var refs = new List<AssemblyReferenceInfo>();
    foreach (var referenceHandle in metadataReader.AssemblyReferences)
    {
        var reference = metadataReader.GetAssemblyReference(referenceHandle);
        var name = metadataReader.GetString(reference.Name);
        refs.Add(new AssemblyReferenceInfo(name, $"{name}, Version={reference.Version}"));
    }

    return refs;
}

readonly record struct AssemblyReferenceInfo(string Name, string FullName);
