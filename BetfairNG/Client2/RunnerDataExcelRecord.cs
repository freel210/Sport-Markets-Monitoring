using System;
using System.Data;
using BetfairNG.Data;
using CommonLibrary;
using System.Collections.ObjectModel;

namespace BetfairNG.Client2
{
    public sealed class RunnerDataExcelRecord
    {
        private const double GHANGING_PROBABILITY = 0.05;
        private static readonly Random rnd = new Random();

        public long selectionId { get; private set; }
        public string runnerName { get; private set; }

        public double B3p { get; private set; }
        public double B2p { get; private set; }
        public double B1p { get; private set; }

        public double L3p { get; private set; }
        public double L2p { get; private set; }
        public double L1p { get; private set; }


        private double _B3s;
        public double B3s { get { return GetValue(ref _B3s); } }

        private double _B2s;
        public double B2s { get { return GetValue(ref _B2s); } }

        private double _B1s;
        public double B1s { get { return GetValue(ref _B1s); } }


        private double _L3s;
        public double L3s { get { return GetValue(ref _L3s); } }

        private double _L2s;
        public double L2s { get { return GetValue(ref _L2s); } }

        private double _L1s;
        public double L1s { get { return GetValue(ref _L1s); } }

        public RunnerDataExcelRecord(DataRow row)
        {
            selectionId = long.Parse(row.ItemArray[1].ToString());
            runnerName = row.ItemArray[2].ToString();

            B3p  = double.Parse(row.ItemArray[3].ToString());
            B2p  = double.Parse(row.ItemArray[4].ToString());
            B1p  = double.Parse(row.ItemArray[5].ToString());

            L1p  = double.Parse(row.ItemArray[6].ToString());
            L2p  = double.Parse(row.ItemArray[7].ToString());
            L3p  = double.Parse(row.ItemArray[8].ToString());

            _B3s = double.Parse(row.ItemArray[9].ToString());
            _B2s = double.Parse(row.ItemArray[10].ToString());
            _B1s = double.Parse(row.ItemArray[11].ToString());

            _L1s = double.Parse(row.ItemArray[12].ToString());
            _L2s = double.Parse(row.ItemArray[13].ToString());
            _L3s = double.Parse(row.ItemArray[14].ToString());
        }

        private double GetValue(ref double v)
        {
            double p = rnd.NextDouble();

            if (p < GHANGING_PROBABILITY)
                v = rnd.Next(10, 5000);

            return v;
        }

        public Runner ToRunner()
        {
            Runner r = new Runner()
            {
                SelectionId = selectionId,
                ExchangePrices = new ExchangePrices()
                {
                    AvailableToBack = new ObservableCollection<PriceSize>()
                    {
                        new PriceSize() {Price = B1p, Size = B1s},
                        new PriceSize() {Price = B2p, Size = B2s},
                        new PriceSize() {Price = B3p, Size = B3s}

                    },

                    AvailableToLay = new ObservableCollection<PriceSize>()
                    {
                        new PriceSize() {Price = L1p, Size = L1s},
                        new PriceSize() {Price = L2p, Size = L2s},
                        new PriceSize() {Price = L3p, Size = L3s}
                    }
                }
            };

            return r;
        }

        public RunnerDescription ToRunnerDescription()
        {
            RunnerDescription rd = new RunnerDescription()
            {
                SelectionId = selectionId,
                RunnerName = runnerName
            };

            return rd;
        }
    }
}
