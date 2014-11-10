using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Yahoo
{
    public class YahooAPI
    {
        public static string getHist(string _symbol, string _yearFrom, string _yearTo)
        {
            // TODO: later. Not for this FE520 Proj

            // get data
//             string csvData = null;
//             using (WebClient web = new WebClient())
//             {
//                 string tmpUrl = string.Format("http://real-chart.finance.yahoo.com/table.csv?s={0}&amp;a=00&amp;b=1&amp;c={1}&amp;d=00&amp;e=1&amp;f={2}&amp;g=d&amp;ignore=.csv",
//                                              _symbol, _yearFrom, _yearTo);
// 
//                 try
//                 {
//                     csvData = web.DownloadString(tmpUrl);
//                 }
//                 catch (System.Net.WebException e)
//                 {
//                     Console.WriteLine(e.Message);
//                     Console.WriteLine("Press any key to terminate.");
//                     Console.ReadKey();
//                     Environment.Exit(-9);
//                 }
//             }
// 
//             // parse data
//             List<StockHist> stockHistLst = new List<StockHist>();
//             string histData = csvData.Replace("r", "");
//             string[] rows = histData.Split('\n');
// 
//             for (int i = 1; i < rows.Length - 1; i++)
//             {
//                 string[] cols = rows[i].Split(',');
//                 StockHist hs = new StockHist();
// 
//                 hs.Symbol = _symbol;
//                 hs.Date = Convert.ToDateTime(cols[0]);
//                 hs.Open = Convert.ToDouble(cols[1]);
//                 hs.High = Convert.ToDouble(cols[2]);
//                 hs.Low = Convert.ToDouble(cols[3]);
//                 hs.Last = Convert.ToDouble(cols[4]);
//                 hs.Volume = Convert.ToInt32(cols[5]);
//                 hs.AdjLast = Convert.ToDouble(cols[6]);
// 
//                 stockHistLst.Add(hs);
//             }
// 
//             return stockHistLst;
            string nullstr = null;
            return nullstr;
        }
        public static string getQuote(string _symbol)
        {
            // get data
            string quoteData = null;
            using (WebClient web = new WebClient())
            {
                string tmpUrl = string.Format("http://download.finance.yahoo.com/d/quotes.csv?s={0}&f=st1l1v0&e=.csv", _symbol);

                try
                {
                    quoteData = web.DownloadString(tmpUrl);
                }
                catch (System.Net.WebException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press any key to terminate.");
                    Console.ReadKey();
                    Environment.Exit(-9);
                }
            }

            return quoteData;
        }
    }
}
