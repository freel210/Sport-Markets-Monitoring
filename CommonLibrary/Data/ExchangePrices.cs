using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using CommonLibrary;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace BetfairNG.Data
{
    public sealed class ExchangePrices : NotifyPropertyChanger
    {
        private ObservableCollection<PriceSize> _availableToBack;
        private readonly NotifyCollectionChangedEventHandler _availableToBackChangeHandler;
        private readonly PropertyChangedEventHandler _availableToBackPriseSizeHandler;
       
        [JsonProperty(PropertyName = "availableToBack")]
        public ObservableCollection<PriceSize> AvailableToBack 
        {
            get { return _availableToBack; }
            
            set 
            {
                if (_availableToBack == value) return;

                if (_availableToBack != null)
                {
                    _availableToBack.CollectionChanged -= _availableToBackChangeHandler;

                    foreach (var item in _availableToBack)
                    {
                        if (item != null)
                            item.PropertyChanged -= _availableToBackPriseSizeHandler;
                    }
                }
                
                _availableToBack = value;

                if (_availableToBack != null)
                {
                    _availableToBack.CollectionChanged += _availableToBackChangeHandler;

                    foreach (var item in _availableToBack)
                    {
                        if (item != null)
                            item.PropertyChanged += _availableToBackPriseSizeHandler;
                    }
                }

                OnPropertyChanged();
            } 
        }

        private ObservableCollection<PriceSize> _availableToLay;
        private readonly NotifyCollectionChangedEventHandler _availableToLayChangeHandler;
        private readonly PropertyChangedEventHandler _availableToLayPriseSizeHandler;

        [JsonProperty(PropertyName = "availableToLay")]
        public ObservableCollection<PriceSize> AvailableToLay
        {
            get { return _availableToLay; }

            set
            {
                if (_availableToLay == value) return;

                if (_availableToLay != null)
                {
                    _availableToLay.CollectionChanged -= _availableToLayChangeHandler;

                    foreach (var item in _availableToLay)
                    {
                        if (item != null)
                            item.PropertyChanged -= _availableToLayPriseSizeHandler;
                    }
                }

                _availableToLay = value;

                if (_availableToLay != null)
                {
                    _availableToLay.CollectionChanged += _availableToLayChangeHandler;

                    foreach (var item in _availableToLay)
                    {
                        if (item != null)
                            item.PropertyChanged += _availableToLayPriseSizeHandler;
                    }
                }

                OnPropertyChanged();
            }
        }

        private ObservableCollection<PriceSize> _tradedVolume;
        private readonly NotifyCollectionChangedEventHandler _tradedVolumeChangeHandler;
        private readonly PropertyChangedEventHandler _tradedVolumeItemChangeHandler;

        [JsonProperty(PropertyName = "tradedVolume")]
        public ObservableCollection<PriceSize> TradedVolume
        {
            get { return _tradedVolume; } 
            
            set 
            {
                if (_tradedVolume == value) return;

                if (_tradedVolume != null)
                {
                    _tradedVolume.CollectionChanged -= _tradedVolumeChangeHandler;

                    foreach (var item in _tradedVolume)
                    {
                        if (item != null)
                            item.PropertyChanged -= _tradedVolumeItemChangeHandler;
                    }
                }

                _tradedVolume = value;

                if (_tradedVolume != null)
                {
                    _tradedVolume.CollectionChanged += _tradedVolumeChangeHandler;

                    foreach (var item in _tradedVolume)
                    {
                        if (item != null)
                            item.PropertyChanged += _tradedVolumeItemChangeHandler;
                    }
                }

                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder().AppendFormat("{0}", "ExchangePrices");

            if (AvailableToBack != null && AvailableToBack.Count > 0)
            {
                int idx = 0;
                foreach (var availableToBack in AvailableToBack)
                {
                    sb.AppendFormat(" : AvailableToBack[{0}]={1}", idx++, availableToBack);
                }
            }

            if (AvailableToLay != null && AvailableToLay.Count > 0)
            {
                int idx = 0;
                foreach (var availableToLay in AvailableToLay)
                {
                    sb.AppendFormat(" : AvailableToLay[{0}]={1}", idx++, availableToLay);
                }
            }

            if (TradedVolume != null && TradedVolume.Count > 0)
            {
                int idx = 0;
                foreach (var tradedVolume in TradedVolume)
                {
                    sb.AppendFormat(" : TradedVolume[{0}]={1}", idx++, tradedVolume);
                }
            }

            return sb.ToString();
        }

        public ExchangePrices()
        {
            _availableToBackChangeHandler = (sender, e) => OnPropertyChanged("AvailableToBack");
            _availableToLayChangeHandler  = (sender, e) => OnPropertyChanged("AvailableToLay");
            _tradedVolumeChangeHandler    = (sender, e) => OnPropertyChanged("TradedVolume");

            _availableToBackPriseSizeHandler = (sender, e) => OnPropertyChanged("AvailableToBack");
            _availableToLayPriseSizeHandler  = (sender, e) => OnPropertyChanged("AvailableToLay");
            _tradedVolumeItemChangeHandler   = (sender, e) => OnPropertyChanged("TradedVolume");
        }

        public override bool Equals(object obj)
        {
            ExchangePrices other = obj as ExchangePrices;

            //if(other == null) return false;

            //if(AvailableToBack.Count != other.AvailableToBack.Count) return false;
            //if(AvailableToLay.Count  != other.AvailableToLay.Count)  return false;

            //for(int i = 0; i < AvailableToBack.Count; i++)
            //{
            //    if(AvailableToBack[i] != other.AvailableToBack[i])
            //        return false;
            //}

            //for(int i = 0; i < AvailableToLay.Count; i++)
            //{
            //    if(AvailableToLay[i] != other.AvailableToLay[i])
            //        return false;
            //}

            //return true;

            bool otherIsNotNull = isOverExchangePricesNotNull(other);
            if(!otherIsNotNull) return false;

            bool availableToBackCountsAreEqual = isOverExchangePricesHasEqualCount(AvailableToBack.Count, other.AvailableToBack.Count);
            bool availableToLayCountsAreEqual = isOverExchangePricesHasEqualCount(AvailableToLay.Count, other.AvailableToLay.Count);
            if(!availableToBackCountsAreEqual || !availableToLayCountsAreEqual) return false;

            bool availableToBackPriceSizesAreEqual = isPriceSizeCollectionsEqual(AvailableToBack, other.AvailableToBack);
            bool availableToLayPriceSizesAreEqual = isPriceSizeCollectionsEqual(AvailableToLay, other.AvailableToLay);
            if(!availableToBackPriceSizesAreEqual || !availableToLayPriceSizesAreEqual) return false;

            return true;
        }

        private bool isOverExchangePricesNotNull(ExchangePrices ep)
        {
            if(ep != null) return true;

            return false;
        }

        private bool isOverExchangePricesHasEqualCount(int c1, int c2)
        {
            return c1 == c2;
        }

        private bool isPriceSizeCollectionsEqual(IList<PriceSize> l1, IList<PriceSize> l2)
        {
            for(int i = 0; i < l1.Count; i++)
            {
                if(!l1[i].Equals(l2[i]))
                    return false;
            }

            return true;
        }

        public static bool operator == (ExchangePrices ep1, ExchangePrices ep2)
        {
            if(object.ReferenceEquals(ep1, null))
                return object.ReferenceEquals(ep2, null);

            return ep1.Equals(ep2);
        }

        public static bool operator != (ExchangePrices ep1, ExchangePrices ep2)
        {
            return !(ep1 == ep2);
        }

        public override int GetHashCode()
        {
            return new { AvailableToBack, AvailableToLay }.GetHashCode();
        }
    }
}
