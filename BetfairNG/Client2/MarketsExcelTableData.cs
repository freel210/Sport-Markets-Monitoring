using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace BetfairNG.Client2
{
    public sealed class MarketsExcelTableData
    {
        public List<MarketExcelDataRecord> MarketsData { get; private set; }

        public MarketsExcelTableData (DataTable dt)
        {
            //перекладываем полученную таблицу в список
            MarketsData = (from m in dt.AsEnumerable()
                           select new MarketExcelDataRecord(m)).ToList();
        }
    }
}
