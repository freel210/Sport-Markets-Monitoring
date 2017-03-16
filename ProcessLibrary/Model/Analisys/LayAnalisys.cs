namespace Model.Analisys
{
    public sealed class LayAnalisys : IAnalisys
    {
        public string StartRunnerID { get; set; }
        public string CloseRunnerID { get; set; }
        public double StartPrice { get; set; }
        public double ClosePrice { get; set; }
        public double StartSize { get; set; }
        public double CloseSize { get; set; }
        public double MaxObligation { get; set; }

        private readonly double _startReversePrice;
        private readonly double _closeReversPrice;

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

        public LayAnalisys(string startRunnerID, string closeRunnerID, double startPrice, double closePrice, double startSize, double closeSize, double maxObligation,
            double b1Price, double b1Size, double b2Price, double b2Size, double l1Price, double l1Size, double l2Price, double l2Size, int gap)
        {
            StartRunnerID = startRunnerID;
            CloseRunnerID = closeRunnerID;

            StartPrice = startPrice;
            ClosePrice = closePrice;

            StartSize = startSize;
            CloseSize = closeSize;

            MaxObligation = maxObligation;

            B1Price = b1Price;
            B1Size = b1Size;

            B2Price = b2Price;
            B2Size = b2Size;

            L1Price = l1Price;
            L1Size = l1Size;

            L2Price = l2Price;
            L2Size = l2Size;

            Gap = gap;

            //из-за этого и нужен конструктор
            _startReversePrice = StartPrice / (StartPrice - 1);
            _closeReversPrice = ClosePrice / (ClosePrice - 1);

            if (_startReversePrice < _closeReversPrice)
            {
                _startObligation = MaxObligation;

                double result = MaxObligation * _startReversePrice / _closeReversPrice; //чтобы было меньше MaxObligation
                //_closeObligation = Math.Round(result, 2, MidpointRounding.AwayFromZero);
                _closeObligation = result;
            }
            else
            {
                double result = MaxObligation * _closeReversPrice / _startReversePrice; //чтобы было меньше MaxObligation
                //_startObligation = Math.Round(result, 2, MidpointRounding.AwayFromZero);
                _startObligation = result;

                _closeObligation = MaxObligation;
            }
        }

        public double TradeRatio
        {
            get { return 1 / _startReversePrice + 1 / _closeReversPrice; }
        }

        public double WaitingRatio
        {
            get { return StartSize / CloseSize; }
        }

        public double StartBet
        {
            get
            {
                double result = _startObligation * (_startReversePrice - 1);
                //return Math.Round(result, 2, MidpointRounding.AwayFromZero);
                return result;
            }
        }

        public double CloseBet
        {
            get
            {
                double result = _closeObligation * (_closeReversPrice - 1);
                //return Math.Round(result, 2, MidpointRounding.AwayFromZero);
                return result;
            }
        }

        public double StartRunnerProfit
        {
            get
            {
                var income = _startObligation * _startReversePrice;
                var semi_profit = income - _startObligation;
                return semi_profit - _closeObligation;
            }
        }

        public double CloseRunnerProfit
        {
            get
            {
                var income = _closeObligation * _closeReversPrice;
                var semi_profit = income - _closeObligation;
                return semi_profit - _startObligation;
            }
        }
    }
}
