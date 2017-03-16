namespace Model.Analisys
{
    public sealed class HorizontalAnalisys : IAnalisys
    {
        public string StartRunnerID { get; set; }
        public string CloseRunnerID { get; set; }
        public double StartPrice { get; set; }
        public double ClosePrice { get; set; }
        public double StartSize { get; set; }
        public double CloseSize { get; set; }
        public double MaxObligation { get; set; }

        private readonly double _startObligation;
        private readonly double _closeObligation;

        public double B1Price { get; set; }
        public double B1Size { get; set; }
        public double B2Price { get; set; }
        public double B2Size { get; set; }
        public double L1Price { get; set; }
        public double L1Size { get; set; }
        public double L2Price { get; set; }
        public double L2Size { get; set; }

        public int Gap { get; set; }

        private readonly double _r1_b2Size;
        private readonly double _r1_b3Size;
        private readonly double _r1_l2Size;
        private readonly double _r1_l3Size;

        private double reverseClosePrice
        {
            get { return ClosePrice / (ClosePrice - 1); }
        }

        public HorizontalAnalisys(string startRunnerID, string closeRunnerID, double startPrice, double closePrice, double startSize, double closeSize, double maxObligation,
            double r1_b1Price, double r1_b1Size, double r2_b1Price, double r2_b1Size, double r1_l1Price, double r1_l1Size, double r2_l1Price, double r2_l1Size, int gap,
            double r1_b2Size, double r1_b3Size, double r1_l2Size, double r1_l3Size) // посление 4 для расчета Sum и Total
        {
            StartRunnerID = startRunnerID;
            CloseRunnerID = closeRunnerID;

            StartPrice = startPrice;
            ClosePrice = closePrice;

            StartSize = startSize;
            CloseSize = closeSize;

            MaxObligation = maxObligation;

            B1Price = r1_b1Price;
            B1Size = r1_b1Size;

            B2Price = r2_b1Price;
            B2Size = r2_b1Size;

            L1Price = r1_l1Price;
            L1Size = r1_l1Size;

            L2Price = r2_l1Price;
            L2Size = r2_l1Size;

            Gap = gap;

            _r1_b2Size = r1_b2Size;
            _r1_b3Size = r1_b3Size;

            _r1_l2Size = r1_l2Size;
            _r1_l3Size = r1_l3Size;

            //из-за этого и нужен конструктор
            _startObligation = StartBet;
            _closeObligation = CloseBet;
        }

        public double TradeRatio //Sum
        {
            get { return StartSize + CloseSize; }
        }

        public double WaitingRatio //Total
        {
            get
            {
                double backSum = CloseSize + _r1_b2Size + _r1_b3Size;
                double laySum = StartSize + _r1_l2Size + _r1_l3Size;

                return backSum + laySum;
            }
        }

        public double StartBet
        {
            get
            {
                if (StartPrice < 2)
                    return MaxObligation;
                else
                    return MaxObligation * reverseClosePrice / StartPrice;
            }
        }

        public double CloseBet
        {
            get
            {
                if (StartPrice < 2)
                    return MaxObligation * StartPrice / reverseClosePrice;
                else
                    return MaxObligation;
            }
        }

        public double StartRunnerProfit
        {
            get
            {
                var income = StartBet * StartPrice;
                var semi_profit = income - _startObligation;
                return semi_profit - _closeObligation;
            }
        }

        public double CloseRunnerProfit
        {
            get
            {
                var income = CloseBet * reverseClosePrice;
                var semi_profit = income - _closeObligation;
                return semi_profit - _startObligation;
            }
        }
    }
}
