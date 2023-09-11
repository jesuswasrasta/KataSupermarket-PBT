using FsCheck.Xunit;

namespace KataSupermarket.Tests
{
    public class Tests
    {
        [Property]
        bool sum_is_commutative(int a, int b) => 
            a + b == b + a;
    }
}
