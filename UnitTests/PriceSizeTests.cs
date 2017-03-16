using Microsoft.VisualStudio.TestTools.UnitTesting;
using BetfairNG.Data;

namespace UnitTests
{
    [TestClass]
    public class PriceSizeTests
    {
        [TestMethod]
        public void EqualPriceSizesAreEqual()
        {
            //arange
            PriceSize first_pc = new PriceSize() {Price = 1.0, Size = 100 };
            PriceSize second_pc = new PriceSize() {Price = 1.0, Size = 100 };

            //act
            bool equal = first_pc.Equals(second_pc);

            //assert
            Assert.IsTrue(equal);
        }
    }
}
