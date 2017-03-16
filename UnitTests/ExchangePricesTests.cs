using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BetfairNG.Data;
using CommonLibrary;
using System.Collections.ObjectModel;

namespace UnitTests
{
    [TestClass]
    public class ExchangePricesTests
    {
        [TestMethod]
        public void EqualExchangePricesAreEqual()
        {
            //arange
            ExchangePrices first_ep = GetStandartObject();
            ExchangePrices second_ep = GetStandartObject();
            
            //act
            bool equal = first_ep.Equals(second_ep);

            //assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void IsNotNullCorrect()
        {
            //arrange
            ExchangePrices first_ep = new ExchangePrices();
            PrivateObject pr = new PrivateObject(first_ep);

            ExchangePrices second_ep = new ExchangePrices()
            {
                AvailableToBack = new ObservableCollection<PriceSize>
                    {
                        new PriceSize() {Price = 1.0, Size = 100.0 } ,
                        new PriceSize() {Price = 1.0, Size = 100.0 } ,
                        new PriceSize() {Price = 1.0, Size = 100.0 }
                    },

                AvailableToLay = new ObservableCollection<PriceSize>
                    {
                        new PriceSize() {Price = 1.0, Size = 100.0 } ,
                        new PriceSize() {Price = 1.0, Size = 100.0 } ,
                        new PriceSize() {Price = 1.0, Size = 100.0 }
                    }
            };

            //act
            bool equal = (bool)pr.Invoke("isOverExchangePricesNotNull", second_ep);

            //correct
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void AvailableToBackCountsAreCorrect()
        {
            //arange
            ExchangePrices first_ep = GetStandartObject();
            PrivateObject pr = new PrivateObject(first_ep);

            ExchangePrices second_ep = GetStandartObject();
            
            //act
            bool equal = (bool)pr.Invoke("isOverExchangePricesHasEqualCount",
                first_ep.AvailableToBack.Count, second_ep.AvailableToBack.Count);

            //assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void AvailableToLayCountsAreCorrect()
        {
            //arange
            ExchangePrices first_ep = GetStandartObject();
            PrivateObject pr = new PrivateObject(first_ep);

            ExchangePrices second_ep = GetStandartObject();
            
            //act
            bool equal = (bool)pr.Invoke("isOverExchangePricesHasEqualCount",
                first_ep.AvailableToLay.Count, second_ep.AvailableToLay.Count);

            //assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void AvailableToBackPriceSizesAreEqualCorrect()
        {
            //arange
            ExchangePrices first_ep = GetStandartObject();
            PrivateObject pr = new PrivateObject(first_ep);

            ExchangePrices second_ep = GetStandartObject();

            //act
            bool equal = (bool)pr.Invoke("isPriceSizeCollectionsEqual",
                first_ep.AvailableToBack, second_ep.AvailableToBack);

            //assert
            Assert.IsTrue(equal);

        }

        private ExchangePrices GetStandartObject()
        {
            return new ExchangePrices()
            {
                AvailableToBack = new ObservableCollection<PriceSize>
                    {
                        new PriceSize() {Price = 1.0, Size = 100.0 } ,
                        new PriceSize() {Price = 1.0, Size = 100.0 } ,
                        new PriceSize() {Price = 1.0, Size = 100.0 }
                    },

                AvailableToLay = new ObservableCollection<PriceSize>
                    {
                        new PriceSize() {Price = 1.0, Size = 100.0 } ,
                        new PriceSize() {Price = 1.0, Size = 100.0 } ,
                        new PriceSize() {Price = 1.0, Size = 100.0 }
                    }
            };
        }
    }
}
