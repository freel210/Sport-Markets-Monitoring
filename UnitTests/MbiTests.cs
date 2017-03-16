using Microsoft.VisualStudio.TestTools.UnitTesting;
using BetfairNG.Data;
using Model;
using CommonLibrary;
using System.Collections.ObjectModel;

namespace UnitTests
{
    [TestClass]
    public class MbiTests
    {
        [TestMethod]
        public void EqualMbisAreEqual()
        {
            // arrange 
            MarketBookInformation first_mbi = new MarketBookInformation()
            {
                ExPricesRunner0 = new ExchangePrices()
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
                },

                ExPricesRunner1 = new ExchangePrices()
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
                }
            };

            MarketBookInformation second_mbi = new MarketBookInformation()
            {
                ExPricesRunner0 = new ExchangePrices()
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
                },

                ExPricesRunner1 = new ExchangePrices()
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
                }
            };

            //act
            bool equal = first_mbi.Equals(second_mbi);

            // assert 
            Assert.IsTrue(equal);
        }
    }
}
