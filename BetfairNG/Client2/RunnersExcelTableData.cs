using BetfairNG.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BetfairNG.Client2
{
    public sealed class RunnersExcelTableData
    {
        public Dictionary<string, List<RunnerDataExcelRecord>> RunnersData { get; private set; }

        public RunnersExcelTableData(DataTable dt)
        {
            RunnersData = new Dictionary<string, List<RunnerDataExcelRecord>>();

            //перекладываем полученную таблицу в каталог
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                string marketId = row.ItemArray[0].ToString();

                RunnerDataExcelRecord re = new RunnerDataExcelRecord(row);

                if(!RunnersData.ContainsKey(marketId))

                    RunnersData.Add(marketId, new List<RunnerDataExcelRecord>() { re });
                else
                    RunnersData[marketId].Add(re);
            }
        }

        public Dictionary<string, List<Runner>> GetRunners()
        {
            var querry = from r in RunnersData
                         select new KeyValuePair<string, IEnumerable<Runner>>(r.Key,
                            from rd in r.Value
                            select rd.ToRunner());

            return querry.ToDictionary(x => x.Key, x => x.Value.ToList());
        }

        public Dictionary<string, List<RunnerDescription>> GetRunnersDescription()
        {
            var querry = from r in RunnersData
                         select new KeyValuePair<string, IEnumerable<RunnerDescription>>(r.Key,
                            from rd in r.Value
                            select rd.ToRunnerDescription());

            return querry.ToDictionary(x => x.Key, x => x.Value.ToList());
        }
    }
}
