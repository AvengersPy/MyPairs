﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloIBCSharp
{
    public struct QuoteTick
    {
        DateTime qtTime;
        double qtLastPrice;
        double qtLastSize;

        public DateTime QtTime
        {
            get { return qtTime; }
            set { qtTime = value; }
        }
        public double QtLastSize
        {
            get { return qtLastSize; }
            set { qtLastSize = value; }
        }
        public double QtLastPrice
        {
            get { return qtLastPrice; }
            set { qtLastPrice = value; }
        }
    }

    public class PairPos
    {
        public PairPos()
        {
            this.thisPairStatus = PairType.nullType;
            this.pairEtfLeg = new PairEtf();
            this.pairStkLeg = new PairStk();
            // TODO, why need this equals to 0?
            this.pairStkLeg.OpenOrderID = 0;
            this.pairEtfLeg.OpenOrderID = 0;
        }

        public PairStk pairStkLeg;
        public PairEtf pairEtfLeg;

        int currPeriod;         // how many periods since open this position. Cannot exceed the preset maximum period
        long openTime;
        long closeTime;
        PairType thisPairStatus;
        double pairBeta;
        double pairAlpha;
        double[] sScore = new double[4];   // open long, close long, open short, close short

        double totalBP;     // buying power of stock and etf
        double totalPNL;
        double totalComm;

        const double STOPLOSS = 0.1;    // maximum stop loss

        #region Encap
        public int CurrPeriod
        {
            get { return currPeriod; }
            set { currPeriod = value; }
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
        public HelloIBCSharp.PairType ThisPairStatus
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
        public double[] Score
        {
            get { return sScore; }
            set { sScore = value; }
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
        #endregion

        public void savePriceNShare(int orderID, double avgFillPrice, int share)
        {
            if (this.pairEtfLeg.OpenOrderID == orderID)
            {
                this.pairEtfLeg.OpenPrice = avgFillPrice;
                this.pairEtfLeg.Share = share;
                this.pairEtfLeg.OriginBP = avgFillPrice * share;
            }
            else if (this.pairEtfLeg.CloseOrderID == orderID)
            {
                this.pairEtfLeg.ClosePrice = avgFillPrice;
                //this.pairEtfLeg.Share = share;    // don't need to do share again in close
            }
            else if (this.pairStkLeg.OpenOrderID == orderID)
            {
                this.pairStkLeg.OpenPrice = avgFillPrice;
                this.pairStkLeg.Share = share;
                this.pairStkLeg.OriginBP = avgFillPrice * share;
                if (this.pairEtfLeg.OriginBP == 0.0)
                {
                    throw new Exception("Don't know ETF BP yet!");
                }
                this.totalBP = this.pairStkLeg.OriginBP + this.pairEtfLeg.OriginBP;
            }
            else if (this.pairStkLeg.CloseOrderID == orderID)
            {
                this.pairStkLeg.ClosePrice = avgFillPrice;
                //this.pairStkLeg.Share = share;    // don't need to do share again in close
            }
        }
        public void saveExecID(int orderID, string execID)
        {
            if (this.pairEtfLeg.OpenOrderID == orderID)
            {
                this.pairEtfLeg.OpenExecID = execID;
            }
            else if (this.pairEtfLeg.CloseOrderID == orderID)
            {
                this.pairEtfLeg.CloseExecID = execID;
            }
            else if (this.pairStkLeg.OpenOrderID == orderID)
            {
                this.pairStkLeg.OpenExecID = execID;
            }
            else if (this.pairStkLeg.CloseOrderID == orderID)
            {
                this.pairStkLeg.CloseExecID = execID;
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
            if (this.pairEtfLeg.OpenExecID == execID)
            {
                this.pairEtfLeg.OpenCommission = comm;
            }
            else if (this.pairEtfLeg.CloseExecID == execID)
            {
                this.pairEtfLeg.CloseCommission = comm;
                this.pairEtfLeg.RealizedPNL = realizedPNL;
                if (this.pairStkLeg.RealizedPNL != 0 && this.pairEtfLeg.RealizedPNL != 0)
                {
                    this.TotalPNL = this.pairStkLeg.RealizedPNL + this.pairEtfLeg.RealizedPNL;
                }
            }
            else if (this.pairStkLeg.OpenExecID == execID)
            {
                this.pairStkLeg.OpenCommission = comm;
            }
            else if (this.pairStkLeg.CloseExecID == execID)
            {
                this.pairStkLeg.CloseCommission = comm;
                this.pairStkLeg.RealizedPNL = realizedPNL;
                
                if (this.pairStkLeg.RealizedPNL != 0 && this.pairEtfLeg.RealizedPNL != 0)
                {
                    // for test, TODO, remove this after test
                    if (this.ThisPairStatus != PairType.closeShort && this.ThisPairStatus != PairType.closeLong)
                    {
                        throw new Exception("Error pair status when calculating commission and PNL");
                    }
                    // test end

                    this.TotalPNL = this.pairStkLeg.RealizedPNL + this.pairEtfLeg.RealizedPNL;
                    this.TotalComm = this.pairStkLeg.CloseCommission + this.pairStkLeg.OpenCommission + 
                                     this.pairEtfLeg.CloseCommission + this.pairEtfLeg.OpenCommission;
                }
            }
        }
    }
    // PairPos contains one stk leg and one etf leg
    public class PairStk
    {
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
        #endregion
    }
    public class PairEtf
    {
        int tickerID;
        string symbol;

        int openOrderID;
        int closeOrderID;
        string openExecID;
        string closeExecID;

        double openPrice;
        double closePrice;
        int share;
        double originBP;    // original Buying power of etf
        
        double openCommission;
        double closeCommission;
        double realizedPNL;

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
        #endregion
    }

    public enum PairType
    {
        nullType, // initial value
        openLong,   // open long    buy etf short stk
        closeLong,  // close long   sell etf buy cover stk
        openShort,  // open short   short etf buy stk
        closeShort  // close short  buy cover etf sell stk
    }

    // the trading strategy will receive quotes and return this PairSignal obj
    public class PairSignal
    {
        int stkTID;
        int etfTID;
        PairType trSignal;

        public HelloIBCSharp.PairType TrSignal
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
    }
    
}
