using System.Collections.Generic;
using System.Linq;
using System.Data.OleDb;
using System.IO;
using System.Data;
using BetfairNG.Data;

namespace BetfairNG.Client2
{
    public sealed class miniBF
    {
        static readonly miniBF instance = new miniBF();

        private static readonly RunnersExcelTableData runnersExcelTableData;
        private static readonly MarketsExcelTableData marketsExcelTableData;

        miniBF()
        {

        }

        public static miniBF Instance
        {
            get {return instance;}
        }

        static miniBF()
        {
            DataTable RunnerDescription = new DataTable();
            DataTable MarketCatalogue = new DataTable();

            string startupPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string fileName = startupPath + @"\TestData v4.xlsx";
            string connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 12.0 Xml; HDR=YES'", fileName);

            string qMC = "select * from [MarketCatalogue$]";
            string qRD = "select * from [RunnerDescription$]";

            using(OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();

                using(OleDbDataAdapter sheetAdapter = new OleDbDataAdapter(qRD, conn))
                    sheetAdapter.Fill(RunnerDescription);

                using(OleDbDataAdapter sheetAdapter = new OleDbDataAdapter(qMC, conn))
                    sheetAdapter.Fill(MarketCatalogue);
            }

            //сортируем MarketCatalogue по убыванию TotalMatched
            DataView dv = MarketCatalogue.DefaultView;
            dv.Sort = "TotalMatched desc";
            MarketCatalogue = dv.ToTable();

            //распихиваем полученные таблицы в специальные классы
            runnersExcelTableData = new RunnersExcelTableData(RunnerDescription);
            marketsExcelTableData = new MarketsExcelTableData(MarketCatalogue);
        }

        public List<MarketCatalogue> GetMarketCatalogueResponse()
        {
            //данным по руннерам даем короткое имя
            Dictionary<string, List<RunnerDescription>> rd = runnersExcelTableData.GetRunnersDescription();

            //формируем список MarketCatalogue
            var querry = from m in marketsExcelTableData.MarketsData
                         //orderby m.TotalMatched descending            пусть контрол сортирует
                         select new MarketCatalogue()
                         {
                             MarketId        = m.MarketId,
                             MarketName      = m.MarketName,
                             TotalMatched    = m.TotalMatched,
                             Event           = new Event()       { Name = m.EventName },
                             EventType       = new EventType()   { Name = m.EventType },
                             Competition     = new Competition() { Name = m.Competition },
                             Runners         = rd.ContainsKey(m.MarketId) ? rd[m.MarketId] : new List<RunnerDescription>(),
                             MarketStartTime = m.MarketStartTime
                         };

            return querry.ToList();
        }

        public List<MarketBook> GetMarketBookResponse(IEnumerable<string> mIds)
        {
            //var querry = from m in mIds
            //             join c in marketsExcelTableData.MarketsData
            //             on m equals c.MarketId
            //             select new { MarketID = c.MarketId, IsInplay = c.InPlay };

            //отбираем только рынки из списка mIDs
            var querry = from m in marketsExcelTableData.MarketsData
                         where mIds.Contains(m.MarketId)
                         select new { MarketID = m.MarketId, IsInplay = m.InPlay };

            //отбираем только тех раннеров, которые относятся к рынкам из списка mIDs
            var dictRun = from d in runnersExcelTableData.GetRunners()
                          where mIds.Contains(d.Key) 
                          select d;

            //возможно, одно и тоже значение руннеров берется из-за отсюда
            var querry2 = from d in dictRun
                          join q in querry
                          on d.Key equals q.MarketID
                          select new MarketBook() { MarketId = q.MarketID, Runners = d.Value, NumberOfWinners = 1, IsInplay = q.IsInplay }; 

            return querry2.ToList();
        }
    }
}
