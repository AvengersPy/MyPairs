using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;   // replace multiple values in a string
using Yahoo;
using HelloIBCSharp;



namespace HelloIBCSharp
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

        public PairSignal(int _stkID, int _etfID)
        {
            this.stkTID = _stkID;
            this.etfTID = _etfID;
            this.trSignal = PairType.nullType;
        }
        public PairSignal()
        {
            this.stkTID = 0;
            this.etfTID = 0;
            this.trSignal = PairType.nullType;
        }

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
        public Equities()
        {
            throw new Exception("Missing Symbol and/or Ticker ID when creating equity obj!");
        }
        public Equities(int _tickerID, string _symbol)
        {
            this.tickerID = _tickerID;
            this.symbol = _symbol;

            this.unrealizedPNL = -1000;    // initial value of uPNL
        }

        const int TMP_QUOTE_SIZE = 5;       // calculate the average of how many quotes
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
                // TODO: last price only, maybe change later.
                tmpQuote.QtLastPrice = Convert.ToDouble(entries[2]);
                //tmpQuote.QtTime = Convert.ToDateTime(entries[1]);   // why data magically appeared? TODO: figure it out after 12am someday
                //tmpQuote.QtLastSize = Convert.ToInt64(entries[3]);
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
            if (this.openPrice != 0)
            {
                this.unrealizedPNL = (lastQuote.QtLastPrice - this.openPrice) * this.share;
            }
            Console.WriteLine("Getting quote {0}", this.symbol);
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
            this.currHoldingPeriod = -1;
            this.thisPairStatus = PairType.nullType;
        }
        const int MAX_HOLDING_PERIOD = 20;
        const double STOP_LOSS = 0.1;       // maximum loss ratio

        Equities stkLeg;
        Equities etfLeg;

        long openTime;
        long closeTime;
        PairType thisPairStatus;
        int currHoldingPeriod;
        
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
        public Equities StkLeg
        {
            get { return stkLeg; }
            set { stkLeg = value; }
        }
        public Equities EtfLeg
        {
            get { return etfLeg; }
            set { etfLeg = value; }
        }
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
        public int CurrHoldingPeriod
        {
            get { return currHoldingPeriod; }
            set { currHoldingPeriod = value; }
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
                // calculate totalBP twice, one will be deprecated
                this.totalBP = this.StkLeg.OriginBP + this.EtfLeg.OriginBP;
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
                // calculate totalBP twice, one will be deprecated
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
        // close this position, for reasons like stop loss, max holding periods
        public void closeThisPosition()
        {
            PairSignal selfSignal = new PairSignal(this.stkLeg.TickerID, this.etfLeg.TickerID);
            switch (this.thisPairStatus)
            {
                case PairType.openLong:
                    selfSignal.TrSignal = PairType.closeLong;
                    break;
                case PairType.openShort:
                    selfSignal.TrSignal = PairType.closeShort;
                    break;
                default:
                    throw new Exception( String.Format("Cannot close my self! Status = {}", Convert.ToString(this.ThisPairStatus)) );
            }
            // TODO
            EWrapperImpl.Instance.processSignal(selfSignal);
        }
        public bool exceedMaxLoss()
        {
            switch (this.thisPairStatus)
            {
                case PairType.nullType:
                    throw new Exception("Cannot calculate PNL, pair position uninitialized!");
                    break;
                case PairType.closeLong:
                case PairType.closeShort:
                    throw new Exception("Cannot calculate PNL, pair position closed!");
                    break;

            }
            if (this.stkLeg.UnrealizedPNL != -1000 && this.etfLeg.UnrealizedPNL != -1000)
            {
                this.totalUnrealizedPNL = this.stkLeg.UnrealizedPNL + this.etfLeg.UnrealizedPNL;
                if (this.totalUnrealizedPNL / this.totalBP >= STOP_LOSS)
                {
                    // stop loss, close pair position
                    return false;
                }
            }
            else
            {
                throw new Exception("Uninitialized PNL for stk or etf!");
            }
            return true;
        }
        public bool exceedMaxHoldingTime()
        {
            if (this.currHoldingPeriod <= MAX_HOLDING_PERIOD)
            {
                return false;
            }
            return true;
        }
    }
}
