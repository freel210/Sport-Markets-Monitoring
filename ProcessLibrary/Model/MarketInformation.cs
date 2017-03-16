using System;
using BetfairNG.Data;
using System.ComponentModel;
using System.Collections.Specialized;
using CommonLibrary;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows;

namespace Model
{
    [Serializable]
    public sealed class MarketInformation : NotifyPropertyChanger
    {
        private ModelCore modelCore = new ModelCore(); 
        
        //инфомация из MarketCatalogue
        public string MarketId          { get; set; }
        public string MarketName        { get; set; } //служит для проверки записывалась ли информация в объект или еще нет
        public double TotalMatched      { get; set; }
        public string EventType         { get; set; }
        public string EventName         { get; set; }
        public string Runner0Name       { get; set; }
        public string Runner1Name       { get; set; }        
        public string CompetitionName   { get; set; }
        public DateTime StartMarketTime { get; set; }

        //инфомация из MarketBook
        private ObservableCollection<MarketBookInformation> _mbi;
        private readonly PropertyChangedEventHandler _mbiChangeItemHandler;
        private readonly NotifyCollectionChangedEventHandler _mbiChangeHandler;
        public ObservableCollection<MarketBookInformation> Mbi
        {
            get { return _mbi; }
            set
            {
                if (_mbi == value) return;

                if (_mbi != null)
                {
                    _mbi.CollectionChanged -= _mbiChangeHandler;

                    foreach (var item in _mbi)
                    {
                        item.PropertyChanged -= _mbiChangeItemHandler;
                    }                  
                }

                _mbi = value;

                if (_mbi != null)
                {
                    _mbi.CollectionChanged += _mbiChangeHandler;

                    foreach (var item in _mbi)
                    {
                        item.PropertyChanged += _mbiChangeItemHandler;
                    }
                }

                OnPropertyChanged();
            }
        }

        private bool _isInMonitoring = false;
        public bool IsInMonitoring
        {
            get { return _isInMonitoring; }
            set
            {
                if(value == _isInMonitoring) return;
                _isInMonitoring = value;

                OnPropertyChanged();
            }
        }

        public MarketInformation()
        {
            _mbi = new ObservableCollection<MarketBookInformation>();
          
            MarketBookInformation mbi = new MarketBookInformation()
            {
                MarketName = this.MarketName,
                Runner0Name = this.Runner0Name,
                Runner1Name = this.Runner1Name,
                
                ExPricesRunner0 = new ExchangePrices()
                {
                    AvailableToBack = new ObservableCollection<PriceSize> { new PriceSize(), new PriceSize(), new PriceSize() },
                    AvailableToLay = new ObservableCollection<PriceSize> { new PriceSize(), new PriceSize(), new PriceSize() }
                },

                ExPricesRunner1 = new ExchangePrices()
                {
                    AvailableToBack = new ObservableCollection<PriceSize> { new PriceSize(), new PriceSize(), new PriceSize() },
                    AvailableToLay = new ObservableCollection<PriceSize> { new PriceSize(), new PriceSize(), new PriceSize() }
                }
            };

            Mbi.Add(mbi);

            //обработчик изменений внутри Mdi
            _mbiChangeHandler = (sender, e) => OnPropertyChanged("Mbi");
            _mbiChangeItemHandler = (sender, e) => OnPropertyChanged("Mbi");
        }

        public void UpdateMarket()
        {
            List<MarketInformation> marketBook = modelCore.GetMarketBook(new List<string> {MarketId});

            MarketBookInformation recordJustReceived = marketBook[0].Mbi.Last();
            recordJustReceived.MarketName = MarketName; //проброс MarketName в MarketBookInformation при добавлении новых рыночных данных

            //добавляем только в том случае, если полученные данные отличаются от уже полученных ранее
            if(Mbi.LastOrDefault() != recordJustReceived)
            {
                Mbi.Add(recordJustReceived);
                //MessageBox.Show("Дык!");
            }
        }

        public void SaveHistory()
        {
            try
            {
                string currentPath = Directory.GetCurrentDirectory();
                string path = Path.Combine(currentPath, "History Data");

                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                path = Path.Combine(path, DateTime.Now.ToShortDateString());

                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string n = MarketName;
                DateTime dt = Mbi.Last().LastByte;
                int h = dt.Hour;
                int m = dt.Minute;
                int s = dt.Second;

                string filename = path + @"\" + n + " " + h + "." + m + "." + s + ".xml";

                if(File.Exists(filename)) File.Delete(filename);

                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(typeof(MarketInformation));
                using(MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, this);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(filename);
                    //stream.Close();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

    }
}
