using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;

namespace KataSupermarket.Tests.PBT.Generators;

public static class IntGenerator
{
    public static Gen<int> PositiveInt() =>
        from value in Arb.Generate<int>()
        select Math.Abs(value) + 1;  

    public static int ToInt(this Gen<int> generator) =>
        generator.Sample(1, 1).First();
    
    // public static Arbitrary<int> PositiveInt() =>
    //     (from value in Arb.Generate<int>()
    //     select Math.Abs(value) + 1).ToArbitrary();

    public static Gen<int> PositiveIntLargerThan(int threshold) =>
        Gen.Choose(threshold + 1, int.MaxValue);
    
    public static Gen<int> PositiveIntSmallerThan(int threshold) =>
        Gen.Choose(1, threshold - 1);

    public static Gen<int> PositiveIntBiggerThan(int threshold) =>
        // Gen.Elements(Enumerable.Range(threshold + 1, int.MaxValue - threshold).ToArray());
        Gen.Choose(threshold + 1, threshold * 2);


    public static Gen<int> ZeroOrNegativeInt()
    {
        // var zeroOrNegativeInt = Gen.Frequency(
        //     Tuple.Create(1, Gen.Constant(0)),
        //     Tuple.Create(2, Gen.Choose(int.MinValue, -1))
        // );      
        
        var zeroOrNegativeInt = Gen.Choose(int.MinValue, 0);
        return zeroOrNegativeInt;
    }
}