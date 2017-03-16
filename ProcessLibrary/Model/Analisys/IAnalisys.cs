namespace Model.Analisys
{
    public interface IAnalisys
    {
        string StartRunnerID { get; set; }
        string CloseRunnerID { get; set; }
        double StartPrice { get; set; }
        double ClosePrice { get; set; }
        double StartSize { get; set; }
        double CloseSize { get; set; }
        double MaxObligation { get; set; }

        double TradeRatio { get; }
        double WaitingRatio { get; }
        double StartBet { get; }
        double CloseBet { get; }
        double StartRunnerProfit { get; }
        double CloseRunnerProfit { get; }

        double B1Price { get; set; }
        double B1Size { get; set; }
        double B2Price { get; set; }
        double B2Size { get; set; }
        double L1Price { get; set; }
        double L1Size { get; set; }
        double L2Price { get; set; }
        double L2Size { get; set; }

        int Gap { get; set; }
    }
}
