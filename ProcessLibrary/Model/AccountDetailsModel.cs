namespace Model
{
    public sealed class AccountDetailsModel
    {
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LocalCode { get; set; }
        public string Region { get; set; }
        public string TimeZone { get; set; }
        public string DiscountRate { get; set; }

        public AccountDetailsModel()
        {
            CountryCode = "";
            CurrencyCode = "";
            FirstName = "";
            LastName = "";
            LocalCode = "";
            Region = "";
            TimeZone = "";
            DiscountRate = "";
        }
    }
}
