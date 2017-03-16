using System;
using System.Data;

namespace BetfairNG.Client2
{
    public sealed class MarketExcelDataRecord
    {
        public string   MarketId        { get; private set; }
        public string   MarketName      { get; private set; }
        public double   TotalMatched    { get; private set; }
        public string   EventName       { get; private set; }
        public string   EventType       { get; private set; }
        public string   Competition     { get; private set; }
        public int      NumberOfWinners { get; private set; }
        public DateTime MarketStartTime { get; private set; }
        public bool     InPlay          { get; private set; }

        public MarketExcelDataRecord(DataRow row)
        {
            MarketId        = row.ItemArray[0].ToString();
            MarketName      = row.ItemArray[1].ToString();
            TotalMatched    = double.Parse(row.ItemArray[2].ToString());
            EventName       = row.ItemArray[3].ToString();
            EventType       = row.ItemArray[4].ToString();
            Competition     = row.ItemArray[5].ToString();
            NumberOfWinners = int.Parse(row.ItemArray[6].ToString());
            MarketStartTime = DateTime.Parse(row.ItemArray[7].ToString());
            InPlay          = bool.Parse(row.ItemArray[8].ToString());
        }
    }
}
