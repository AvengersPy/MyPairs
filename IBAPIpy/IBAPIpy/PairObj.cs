using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;   // replace multiple values in a string
using Yahoo;



namespace PairObj
{
    public enum PairType
    {
        nullType,   // initial value
        openLong,   // open long    buy etf short stk
        closeLong,  // close long   sell etf buy cover stk
        openShort,  // open short   short etf buy stk
        closeShort  // close short  buy cover etf sell stk
    }
    // quote struct for stock and etf
    public struct Quotes
    {
        DateTime qtTime;
        double qtLastPrice;     // for calculating unrealized PNL
        double qtLastAvgPrice;  // for the strategy
        long qtLastSize;

        #region Encap
        public DateTime QtTime
        {
            get { return qtTime; }
            set { qtTime = value; }
        }
        public long QtLastSize
        {
            get { return qtLastSize; }
            set { qtLastSize = value; }
        }
        public double QtLastPrice
        {
            get { return qtLastPrice; }
            set { qtLastPrice = value; }
        }
        public double QtLastAvgPrice
        {
            get { return qtLastAvgPrice; }
            set { qtLastAvgPrice = value; }
        }
        #endregion
    }
    // signal to be processed
    public class PairSignal
    {
        int stkTID;
        int etfTID;
        PairType trSignal;

        #region Encap
        public PairType TrSignal
        {
            get { return trSignal; }
            set { trSignal = value; }
        }
        public int StkTID
        {
            get { return stkTID; }
            set { stkTID = value; }
        }
        public int EtfTID
        {
            get { return etfTID; }
            set { etfTID = value; }
        }
        #endregion
    }

    // stk or etf object, recognized by even/odd tickerID
    public class Equities
    {
        const int TMP_QUOTE_SIZE = 9;       // calculate the average of how many quotes
        const int MAX_QUOTE_LIST = 60;      // maximum number of quotes in quote list

        int tickerID;
        string symbol;

        int openOrderID;
        int closeOrderID;
        string openExecID;
        string closeExecID;

        double openPrice;
        double closePrice;
        int share;
        double originBP;    // original Buying power of stk

        double openCommission;
        double closeCommission;
        double realizedPNL;
        double unrealizedPNL;   // read only, calculated by get quote function

        List<Quotes> quoteList = new List<Quotes>();

        public Equities()
        {
            throw new Exception("Missing Symbol and/or Ticker ID when createing equity obj!");
        }
        public Equities(int _tickerID, string _symbol)
        {
            this.tickerID = _tickerID;
            this.symbol = _symbol;
        }
        public void getQuoteYahoo()
        {
            // sorted by price, for calculate mean
            List<Quotes> tmpQuoteLst = new List<Quotes>();
            for (int i = 0; i < TMP_QUOTE_SIZE; i++)
            {
                string stkQuote = YahooAPI.getQuote(symbol);
                stkQuote = Regex.Replace(stkQuote, "[\"]|[\r]|[\n]", "");
                string[] entries = stkQuote.Replace("\r", "").Split(',');

                Quotes tmpQuote = new Quotes();
                tmpQuote.QtTime = Convert.ToDateTime(entries[1]);   // why data magically appeared? TODO: figure it out after 12am someday
                tmpQuote.QtLastPrice = Convert.ToDouble(entries[2]);
                tmpQuote.QtLastSize = Convert.ToInt64(entries[3]);
                tmpQuoteLst.Add(tmpQuote);
            }
            List<Quotes> SortedList = tmpQuoteLst.OrderBy(o => o.QtLastPrice).ToList();
            // calculate average, take away the smallest and the largest price
            double avgPrice = 0;
            for (int i = 1; i < TMP_QUOTE_SIZE - 1; i++)
            {
                avgPrice += SortedList[i].QtLastPrice;
            }
            avgPrice /= TMP_QUOTE_SIZE - 2;
            Quotes qtick = new Quotes();
            qtick = tmpQuoteLst[TMP_QUOTE_SIZE - 1];
            qtick.QtLastAvgPrice = avgPrice;

            // push back to quote list
            this.quoteList.Add(qtick);
            while(quoteList.Count > MAX_QUOTE_LIST)
            {
                quoteList.RemoveAt(0);
            }

            // calculate unrealized PNL
            // TODO: this is incorrect. Need bid/ask price for the correct number.
            // probably later
            Quotes lastQuote = quoteList[quoteList.Count - 1];
            this.unrealizedPNL = (lastQuote.QtLastPrice - this.openPrice) * this.share;
        }
        
        #region Encap
        public int TickerID
        {
            get { return tickerID; }
            set { tickerID = value; }
        }
        public string Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }
        public int OpenOrderID
        {
            get { return openOrderID; }
            set { openOrderID = value; }
        }
        public int CloseOrderID
        {
            get { return closeOrderID; }
            set { closeOrderID = value; }
        }
        public string OpenExecID
        {
            get { return openExecID; }
            set { openExecID = value; }
        }
        public string CloseExecID
        {
            get { return closeExecID; }
            set { closeExecID = value; }
        }
        public double OpenPrice
        {
            get { return openPrice; }
            set { openPrice = value; }
        }
        public double ClosePrice
        {
            get { return closePrice; }
            set { closePrice = value; }
        }
        public int Share
        {
            get { return share; }
            set { share = value; }
        }
        public double OriginBP
        {
            get { return originBP; }
            set { originBP = value; }
        }
        public double OpenCommission
        {
            get { return openCommission; }
            set { openCommission = value; }
        }
        public double CloseCommission
        {
            get { return closeCommission; }
            set { closeCommission = value; }
        }
        public double RealizedPNL
        {
            get { return realizedPNL; }
            set { realizedPNL = value; }
        }
        public double UnrealizedPNL
        {
            get { return unrealizedPNL; }
        }
        #endregion
    }
    // pair position
    public class PairPos
    {
        public PairPos(Equities etf, Equities stk)
        {
            this.EtfLeg = etf;
            this.StkLeg = stk;

            this.sScore = new double[4];
            this.thisPairStatus = PairType.nullType;
        }
        const int MAX_HOLDING_PERIOD = 20;
        const double STOP_LOSS = 0.1;       // maximum loss ratio

        public Equities StkLeg;
        public Equities EtfLeg;

        long openTime;
        long closeTime;
        PairType thisPairStatus;
        
        // parameters from regression
        double pairBeta;
        double pairAlpha;
        double[] sScore;    // open long, close long, open short, close short

        int initEtfShare = 20;   // init share, make it const for now 
        public int InitEtfShare
        {
            get { return initEtfShare; }
            set { initEtfShare = value; }
        }

        double totalBP;
        double totalPNL;
        double totalUnrealizedPNL;
        double totalComm;

        #region Encap
        public long OpenTime
        {
            get { return openTime; }
            set { openTime = value; }
        }
        public long CloseTime
        {
            get { return closeTime; }
            set { closeTime = value; }
        }
        public PairType ThisPairStatus
        {
            get { return thisPairStatus; }
            set { thisPairStatus = value; }
        }

        // parameters from regression
        public double PairBeta
        {
            get { return pairBeta; }
            set { pairBeta = value; }
        }
        public double PairAlpha
        {
            get { return pairAlpha; }
            set { pairAlpha = value; }
        }
        public double[] SScore
        {
            get { return sScore; }
            set { sScore = value; }
        }
        public double TotalBP
        {
            get { return totalBP; }
            set { totalBP = value; }
        }
        public double TotalPNL
        {
            get { return totalPNL; }
            set { totalPNL = value; }
        }
        public double TotalComm
        {
            get { return totalComm; }
            set { totalComm = value; }
        }
        public double TotalUnrealizedPNL
        {
            get { return totalUnrealizedPNL; }
            set { totalUnrealizedPNL = value; }
        }
        
        #endregion

        #region Save CallBack
        public void savePriceNShare(int orderID, double avgFillPrice, int share)
        {
            if (this.EtfLeg.OpenOrderID == orderID)
            {
                this.EtfLeg.OpenPrice = avgFillPrice;
                this.EtfLeg.Share = share;
                this.EtfLeg.OriginBP = avgFillPrice * share;
            }
            else if (this.EtfLeg.CloseOrderID == orderID)
            {
                this.EtfLeg.ClosePrice = avgFillPrice;
                //this.EtfLeg.Share = share;    // don't need to do share again in close
            }
            else if (this.StkLeg.OpenOrderID == orderID)
            {
                this.StkLeg.OpenPrice = avgFillPrice;
                this.StkLeg.Share = share;
                this.StkLeg.OriginBP = avgFillPrice * share;
                if (this.EtfLeg.OriginBP == 0.0)
                {
                    throw new Exception("Don't know ETF BP yet!");
                }
                this.totalBP = this.StkLeg.OriginBP + this.EtfLeg.OriginBP;
            }
            else if (this.StkLeg.CloseOrderID == orderID)
            {
                this.StkLeg.ClosePrice = avgFillPrice;
                //this.StkLeg.Share = share;    // don't need to do share again in close
            }
        }
        public void saveExecID(int orderID, string execID)
        {
            if (this.EtfLeg.OpenOrderID == orderID)
            {
                this.EtfLeg.OpenExecID = execID;
            }
            else if (this.EtfLeg.CloseOrderID == orderID)
            {
                this.EtfLeg.CloseExecID = execID;
            }
            else if (this.StkLeg.OpenOrderID == orderID)
            {
                this.StkLeg.OpenExecID = execID;
            }
            else if (this.StkLeg.CloseOrderID == orderID)
            {
                this.StkLeg.CloseExecID = execID;
            }
        }
        public void saveCommPNL(string execID, double comm, double realizedPNL)
        {
            if (Math.Abs(realizedPNL) > 100000)
            {
                // If realizedPNL is too large, invalid number. 
                // The position has not been realized yet.
                realizedPNL = 0;
            }
            // cannot use switch here since const value is required
            if (this.EtfLeg.OpenExecID == execID)
            {
                this.EtfLeg.OpenCommission = comm;
            }
            else if (this.EtfLeg.CloseExecID == execID)
            {
                this.EtfLeg.CloseCommission = comm;
                this.EtfLeg.RealizedPNL = realizedPNL;
                if (this.StkLeg.RealizedPNL != 0 && this.EtfLeg.RealizedPNL != 0)
                {
                    this.TotalPNL = this.StkLeg.RealizedPNL + this.EtfLeg.RealizedPNL;
                }
            }
            else if (this.StkLeg.OpenExecID == execID)
            {
                this.StkLeg.OpenCommission = comm;
            }
            else if (this.StkLeg.CloseExecID == execID)
            {
                this.StkLeg.CloseCommission = comm;
                this.StkLeg.RealizedPNL = realizedPNL;

                if (this.StkLeg.RealizedPNL != 0 && this.EtfLeg.RealizedPNL != 0)
                {
                    // for test, TODO, remove this after test
                    if (this.thisPairStatus != PairType.closeShort && this.thisPairStatus != PairType.closeLong)
                    {
                        throw new Exception("Error  status when calculating commission and PNL");
                    }
                    // test end

                    this.TotalPNL = this.StkLeg.RealizedPNL + this.EtfLeg.RealizedPNL;
                    this.TotalComm = this.StkLeg.CloseCommission + this.StkLeg.OpenCommission +
                                     this.EtfLeg.CloseCommission + this.EtfLeg.OpenCommission;
                }
            }
        }
        #endregion

        // TODO: add self-close here.
    }

}
