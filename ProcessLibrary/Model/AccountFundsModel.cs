namespace Model
{
    public sealed class AccountFundsModel
    {
        public string AvailableToBetBalance { get; set; }
        public string DiscountRate { get; set; }
        public string Exposure { get; set; }
        public string ExposureLimit { get; set; }
        public string PointsBalance { get; set; }
        public string RetainedCommission { get; set; }

        public AccountFundsModel()
        {
            AvailableToBetBalance = "";
            DiscountRate = "";
            Exposure = "";
            ExposureLimit = "";
            PointsBalance = "";
            RetainedCommission = "";
        }
    }
}
