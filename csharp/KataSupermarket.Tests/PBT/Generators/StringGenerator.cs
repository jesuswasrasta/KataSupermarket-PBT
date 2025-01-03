using System.Linq;
using FsCheck;


namespace KataSupermarket.Tests.PBT.Generators;

public static class StringGenerator
{
    public static Gen<string> NonEmptyString() =>
        Arb.Default.String().Filter(s => !string.IsNullOrEmpty(s)).Generator;

    public static Gen<string> NonBlankAlphaNumericString()
    {
        // Define a string that holds all alphanumeric characters.
        const string alphanumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        // Define a generator for indexes into the alphanumeric string.
        var indexGen = Gen.Choose(0, alphanumeric.Length - 1);

        // Define a generator for alphanumeric character:
        var alphanumericCharGen = indexGen.Select(i => alphanumeric[i]);

        // Define a generator for non-blank, alphanumeric strings, that involves generating lists of alphanumeric characters and joining them into strings.
        return Gen.NonEmptyListOf(alphanumericCharGen).Select(chars => new string(chars.ToArray()));
    }
    
    public static string ToString(this Gen<string> generator) =>
        generator.Sample(1, 1).First();
}