using System;
using Xunit;

namespace Drifter.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var myclass = new Drifter.Class2();
            myclass.printsomething();
            Assert.True(true);
        }
    }
}
