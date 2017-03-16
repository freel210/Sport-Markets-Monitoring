using Model.Analisys;

namespace MVVM
{
    public static class Extensions
    {
        public static TradeCases ToTradeCase(this object o)
        {
            string s = o as string;
            TradeCases result = TradeCases.empty;

            if (s != null)
            {
                switch (s)
                {
                    case "BL1B2":
                        result = TradeCases.BL1B2;
                        break;

                    case "BL2B1":
                        result = TradeCases.BL2B1;
                        break;

                    case "LB1L2":
                        result = TradeCases.LB1L2;
                        break;

                    case "LB2L1":
                        result = TradeCases.LB2L1;
                        break;

                    case "BL1LB1":
                        result = TradeCases.BL1LB1;
                        break;

                    case "BL2LB2":
                        result = TradeCases.BL2LB2;
                        break;

                    default:
                        break;
                }
            }
            return result;
        }
    }
}
