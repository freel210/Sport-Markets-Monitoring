using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using CommonLibrary;

namespace BetfairNG.Data
{
    public sealed class PriceSize : NotifyPropertyChanger
    {
        private double _price;
        [JsonProperty(PropertyName = "price")]
        public double Price
        {
            get { return _price; }
            
            set
            {
                if (_price == value) return;

                _price = value;
                OnPropertyChanged();
            } 
        }

        private double _size;
        [JsonProperty(PropertyName = "size")]
        public double Size
        {
            get { return _size; }
            
            set
            {
                if (_size == value) return;

                _size = value;
                OnPropertyChanged();
            } 
        }

        public override string ToString()
        {
            return new StringBuilder().AppendFormat("{0}@{1}", Size, Price)
                        .ToString();
        }

        public double NextPrice
        {
            get
            {
                if (!PriceHelpers.Table.Contains(Price) || PriceHelpers.Table.Last() == Price)
                    return PriceHelpers.Table.Last();

                int currIndex = Array.IndexOf(PriceHelpers.Table, Price);
                int nxtIndex = currIndex + 1;
                double nxtValue = PriceHelpers.Table[nxtIndex];

                return nxtValue;
            }
        }

        public PriceSize GetNextPriceSize()
        {
            double price = this.NextPrice;
            return new PriceSize() { Price = price};
        }

        public double PreviousPrice
        {
            get
            {
                if (!PriceHelpers.Table.Contains(Price) || PriceHelpers.Table.First() == Price)
                    return PriceHelpers.Table.First();

                int currIndex = Array.IndexOf(PriceHelpers.Table, Price);
                int prevIndex = currIndex - 1;
                double prevValue = PriceHelpers.Table[prevIndex];

                return prevValue;
            }
        }

        public PriceSize GetPreviousPriceSize()
        {
            double price = this.PreviousPrice;
            return new PriceSize() { Price = price };
        }

        public override bool Equals(object obj)
        {
            PriceSize other = obj as PriceSize;

            if(other == null) return false;

            bool pricesAreEqual = _price.Equals(other.Price);
            bool sizesAreEqual = _size.Equals(other.Size);

            return pricesAreEqual && sizesAreEqual;
        }

        public static bool operator ==(PriceSize ep1, PriceSize ep2)
        {
            if(object.ReferenceEquals(ep1, null))
                return object.ReferenceEquals(ep2, null);

            return ep1.Equals(ep2);
        }

        public static bool operator !=(PriceSize ep1, PriceSize ep2)
        {
            return !(ep1 == ep2);
        }

        public override int GetHashCode()
        {
            return new {Price, Size }.GetHashCode();
        }
    }
}
