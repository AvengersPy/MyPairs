/* Copyright (C) 2014 Interactive Brokers LLC. All rights reserved.  This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IBApi;
using MYLogger;
using PairObj;

namespace HelloIBCSharp
{
    public class EWrapperImpl : EWrapper
    {
        EClientSocket clientSocket;
        private int nextOrderId;
        public EClientSocket ClientSocket
        {
            get { return clientSocket; }
            set { clientSocket = value; }
        }
        public int NextOrderId
        {
            get { return nextOrderId; }
            set { nextOrderId = value; }
        }

        #region Constructor Singleton
        private static EWrapperImpl instance;
        public static EWrapperImpl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EWrapperImpl();
                }
                return instance;
            }
        }
        private EWrapperImpl()
        {
            //this.MAX_QUOTE_LIST = maxQuote;
            this.clientSocket = new EClientSocket(this);
            this.equityDict = new Dictionary<int, Equities>();
            this.PairPosDict = new Dictionary<int, PairPos>();
            File.Copy("mylogger.txt", "backup_logger.txt", true);
            MyLogger.Instance.Open("mylogger.txt", false);   // create/open logger file

            this.readSymbols(SYMBOL_FILE_DIR);
        }
        ~EWrapperImpl()
        {
            //MyLogger.Instance.BackupAndClear("mylogger.txt");
            MyLogger.Instance.Close();
        }
        #endregion

        #region Pairs Trading Object
        const string SYMBOL_FILE_DIR = @"C:\Users\Zhe\Documents\GitHub\MyPairs\IBAPIpy\IBAPIpy\bin\dow.csv";    // symbol file
        
        Dictionary<int, Equities> equityDict;   // key tID, value equities including stk and etf
        Dictionary<int, PairPos> pairPosDict;   // key, stkID, recording all positions in pair.

        #region Encap
        public Dictionary<int, Equities> EquityDict
        {
            get { return equityDict; }
            set { equityDict = value; }
        }
        public Dictionary<int, PairPos> PairPosDict
        {
            get { return pairPosDict; }
            set { pairPosDict = value; }
        }
        #endregion
        #endregion

        #region Pairs Trading Helper Functions
        // read csv symbol to dictionary
        // the logic of this part is determined by the structure of the symbol file
        // etf: odd tickerID, stk: even tickerID
        public void readSymbols(string csvDir)
        {
            try
            {
                using (var reader = new StreamReader(csvDir))
                {
                    List<string> listA = new List<string>();
                    List<string> listB = new List<string>();

                    var title = reader.ReadLine();  // jump over the title

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var cells = line.Split(',');
                        int etfID = Convert.ToInt32(cells[0]);
                        int stkID = Convert.ToInt32(cells[2]);

                        int tmpTickerID = new int();
                        // digit 0: stk 0 ; etf 1
                        if (stkID == 0)
                        {
                            tmpTickerID = tmpTickerID | 1;
                        }
                        // digit from 1 to 9: stk ID
                        tmpTickerID = tmpTickerID | (stkID << 1);
                        // digit from 10 to 13: etf ID
                        tmpTickerID = tmpTickerID | (etfID << 10);
                        Equities tmpEquity = new Equities(tmpTickerID, cells[3]);
                        this.equityDict.Add(tmpTickerID, tmpEquity);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("File doesn't exist: {0}", csvDir);
                throw (e);
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("General Exception");
                throw (e);
            }
        }
        public PairPos createPair(PairSignal oneSignal)
        {
            Equities etfLeg = this.equityDict[oneSignal.EtfTID];
            Equities stkLeg = this.equityDict[oneSignal.StkTID];

            PairPos newPair = new PairPos(etfLeg, stkLeg);
            return newPair;
            
            // blah blah
            //foreach (var KeyValuePair in this.TickerSymbolDict)
            //{
            //    // skip etf
            //    if ((KeyValuePair.Key & 1) == 1)
            //        continue;

            //    PairPos newPair = new PairPos();
            //    newPair.PairBeta = 0;   // beta initialize

            //    int tmpEtfTID = (KeyValuePair.Key & 1024) + 1;      // clear all digits in 1 to 9. Aka, clear stk infomation
            //    newPair.pairEtfLeg.TickerID = tmpEtfTID;
            //    try
            //    {
            //        newPair.pairEtfLeg.Symbol = this.TickerSymbolDict[tmpEtfTID];
            //    }
            //    catch (Exception)
            //    {
            //        throw;
            //    }
            //    newPair.pairStkLeg.TickerID = KeyValuePair.Key;
            //    newPair.pairStkLeg.Symbol = KeyValuePair.Value;

            //    this.PairPosDict.Add(KeyValuePair.Key, newPair);
            //}
        }
        public void processSignal(PairSignal oneSignal)
        {
            if (oneSignal.TrSignal == PairType.nullType)
                // do nothing
                return;

            if (!PairPosDict.ContainsKey(oneSignal.StkTID))
            {
                PairPos newPair = this.createPair(oneSignal);
                PairPosDict.Add(oneSignal.StkTID, newPair);
            }

            // retrieve Pair information from pair dict, based on this signal
            PairPos tmpPair = this.PairPosDict[oneSignal.StkTID];
            tmpPair.ThisPairStatus = oneSignal.TrSignal;
            
            // TODO: remove this line later, this is just for test
            tmpPair.PairBeta = 1;

            if (tmpPair.PairBeta == 0)
            {
                // the beta of this pair is zero, this pair hasn't be completely set up. exit application
                throw new Exception("Please build your model first!");
            }

            // create contract objects for both stk and etf
            Contract etfContract = new Contract();
            etfContract.Symbol = tmpPair.EtfLeg.Symbol;
            etfContract.SecType = "STK";        // Both etf and stk are using the same security type, required by IB
            etfContract.Currency = "USD";
            etfContract.Exchange = "SMART";
            Contract stkContract = new Contract();
            stkContract.Symbol = tmpPair.StkLeg.Symbol;
            stkContract.SecType = "STK";
            stkContract.Currency = "USD";
            stkContract.Exchange = "SMART";

            // create order objects for both stk and etf
            Order etfOrder = new Order();
            etfOrder.OrderId = this.NextOrderId;
            etfOrder.OrderType = "MKT";
            Order stkOrder = new Order();
            stkOrder.OrderId = this.NextOrderId + 1;
            stkOrder.OrderType = "MKT";

            // pass order ID to Pair Dictionary
            switch (oneSignal.TrSignal)
            {
                case PairType.openLong:
                    tmpPair.ThisPairStatus = PairType.openLong;
                    tmpPair.EtfLeg.OpenOrderID= this.nextOrderId;
                    tmpPair.StkLeg.OpenOrderID = this.nextOrderId + 1;
                    
                    tmpPair.CurrHoldingPeriod = 0;  // open, holding period = 0
                    break;
                case PairType.openShort:
                    tmpPair.ThisPairStatus = PairType.openShort;
                    tmpPair.EtfLeg.OpenOrderID= this.nextOrderId;
                    tmpPair.StkLeg.OpenOrderID = this.nextOrderId + 1;
                    
                    tmpPair.CurrHoldingPeriod = 0;
                    break;
                case PairType.closeLong:
                    tmpPair.ThisPairStatus = PairType.closeLong;
                    tmpPair.EtfLeg.CloseOrderID = this.nextOrderId;
                    tmpPair.StkLeg.CloseOrderID = this.nextOrderId + 1;

                    tmpPair.CurrHoldingPeriod = -1; // close, holding period = -1
                    break;
                case PairType.closeShort:
                    tmpPair.ThisPairStatus = PairType.closeShort;
                    tmpPair.EtfLeg.CloseOrderID = this.nextOrderId;
                    tmpPair.StkLeg.CloseOrderID = this.nextOrderId + 1;

                    tmpPair.CurrHoldingPeriod = -1;
                    break;
                default:
                    Console.WriteLine("Wrong signal type. Press any key to start over...");
                    Console.ReadKey();
                    Environment.Exit(-2);
                    break;
            }

            this.PairPosDict[oneSignal.StkTID] = tmpPair;

            switch (oneSignal.TrSignal)
            {
                case PairType.openLong:
                    // long etf short stk
                    stkOrder.Action = "SELL";
                    stkOrder.TotalQuantity = Convert.ToInt32(tmpPair.InitEtfShare * tmpPair.PairBeta);
                    stkOrder.OrderType = "MKT";
                    etfOrder.Action = "BUY";
                    etfOrder.TotalQuantity = tmpPair.InitEtfShare;
                    etfOrder.OrderType = "MKT";
                    break;
                case PairType.openShort:
                    // short etf long stk
                    stkOrder.Action = "BUY";
                    stkOrder.TotalQuantity = Convert.ToInt32(tmpPair.InitEtfShare * tmpPair.PairBeta);
                    stkOrder.OrderType = "MKT";
                    etfOrder.Action = "SSHORT";
                    etfOrder.TotalQuantity = tmpPair.InitEtfShare;
                    etfOrder.OrderType = "MKT";
                    break;
                case PairType.closeLong:
                    // close, sell etf buy cover stk
                    stkOrder.Action = "BUY";
                    stkOrder.TotalQuantity = Convert.ToInt32(tmpPair.InitEtfShare * tmpPair.PairBeta);
                    stkOrder.OrderType = "MKT";
                    etfOrder.Action = "SELL";
                    etfOrder.TotalQuantity = tmpPair.InitEtfShare;
                    etfOrder.OrderType = "MKT";
                    break;
                case PairType.closeShort:
                    // close, buy cover etf sell stk
                    stkOrder.Action = "SELL";
                    stkOrder.TotalQuantity = Convert.ToInt32(tmpPair.InitEtfShare * tmpPair.PairBeta);
                    stkOrder.OrderType = "MKT";
                    etfOrder.Action = "BUY";
                    etfOrder.TotalQuantity = tmpPair.InitEtfShare;
                    etfOrder.OrderType = "MKT";
                    break;
                default:
                    // not gonna happen
                    throw new Exception("Wrong signal!");
                    break;
            }

            // TODO: add log file here
            this.ClientSocket.placeOrder(etfOrder.OrderId, etfContract, etfOrder);
            this.ClientSocket.placeOrder(stkOrder.OrderId, stkContract, stkOrder);
            this.ClientSocket.reqIds(1);
        }
        //TODO
        public void getAllQuote()
        {
            foreach (var oneEqu in this.equityDict.Values)
            {
                oneEqu.getQuoteYahoo();
            }
            // calculate uPNL for all pairs
            //TODO
            foreach (var onePair in this.pairPosDict.Values)
            {
                onePair.TotalUnrealizedPNL = onePair.StkLeg.UnrealizedPNL + onePair.EtfLeg.UnrealizedPNL;
                // check stop loss
                if (onePair.exceedMaxLoss())
                {
                    Console.WriteLine("Stop Loss! {0}, {1}, {2}", onePair.StkLeg.Symbol, onePair.StkLeg.Symbol, onePair.TotalUnrealizedPNL);
                    MyLogger.Instance.CreateEntry(String.Format("Stop Loss! {0}, {1}, {2}", onePair.StkLeg.Symbol, onePair.StkLeg.Symbol, onePair.TotalUnrealizedPNL));
                    onePair.closeThisPosition();
                }

                // check max holding time
                if (onePair.CurrHoldingPeriod == -1)
                {
                    // unopened pair position, no holding time
                    continue;
                }

                onePair.CurrHoldingPeriod += 1;

                if (onePair.exceedMaxHoldingTime())
                {
                    Console.WriteLine("Holding too long! {0}, {1}, {2}", onePair.StkLeg.Symbol, onePair.StkLeg.Symbol, onePair.TotalUnrealizedPNL);
                    MyLogger.Instance.CreateEntry(String.Format("Holding too long! {0}, {1}, {2}", onePair.StkLeg.Symbol, onePair.StkLeg.Symbol, onePair.TotalUnrealizedPNL));
                    onePair.closeThisPosition();
                }
            }
        }
        #endregion

        #region EWrapper Methods
        public virtual void error(Exception e)
        {
            Console.WriteLine("Exception thrown: " + e);
            throw e;
        }

        public virtual void error(string str)
        {
            Console.WriteLine("Error: " + str + "\n");
        }

        public virtual void connectionClosed()
        {
            Console.WriteLine("Connection closed.\n");
        }

        public virtual void error(int id, int errorCode, string errorMsg)
        {
            Console.WriteLine("Error. Id: " + id + ", Code: " + errorCode + ", Msg: " + errorMsg + "\n");
        }
        
        public virtual void currentTime(long time)
        {
            DateTime currTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(time).ToLocalTime();
            Console.WriteLine("Current Time: " + currTime + "\n");
        }

        public virtual void managedAccounts(string accountsList)
        {
            Console.WriteLine("Account list: " + accountsList + "\n");
        }

        public virtual void nextValidId(int orderId)
        {
            Console.WriteLine("Next Valid Id: " + orderId + "\n");
            NextOrderId = orderId;
        }

        public virtual void tickPrice(int tickerId, int tickType, double price, int canAutoExecute)
        {
            Console.WriteLine("Tick Price. Ticker Id:" + tickerId + ", tickType: " + tickType + ", Price: " + price + ", CanAutoExecute: " + canAutoExecute + "\n");
//             if (tickType == 4)     // field == 4, Last_Price
//             {
//                 //MyLogger.Instance.CreateEntry(string.Format("Tick Price. Ticker Id:" + tickerId + ", tickType: " + tickType + ", Price: " + price + ", CanAutoExecute: " + canAutoExecute + "\n"));
//                 Console.WriteLine("Tick Price. Ticker Id:" + tickerId + ", tickType: " + tickType + ", Price: " + price + ", CanAutoExecute: " + canAutoExecute + "\n");
// 
//                 if (!this.QuoteDict.ContainsKey(tickerId))
//                 {
//                     // if new tickerID...Error, new ticker id should be added during tickString already
//                     // tickPrice and tickSize should appear after tickString
//                     Console.ForegroundColor = ConsoleColor.Red;
//                     Console.WriteLine("Ticker Price. Error: tickPrice received after tickString");
//                     Console.ResetColor();
//                     //Console.WriteLine("Enter any key to stop...");
//                     //Console.ReadKey();
//                     // ignore this data
//                     return;
//                 }
// 
//                 // A "strange way" of modifying the last item in the list
//                 int listCnt = QuoteDict[tickerId].Count;
//                 QuoteTick tmpQtItm = QuoteDict[tickerId].LastOrDefault();
//                 if (tmpQtItm.QtLastPrice!= 0)   // already received tickPrice
//                     return;
//                 
//                 tmpQtItm.QtLastPrice = price;
//                 QuoteDict[tickerId].RemoveAt(listCnt - 1);
//                 QuoteDict[tickerId].Add(tmpQtItm);
//             }
        }

        public virtual void tickSize(int tickerId, int tickType, int size)
        {
            Console.WriteLine("Tick Size. Ticker Id:" + tickerId + ", tickType: " + tickType + ", Size: " + size + "\n");
//          if (tickType == 5)
//             {
//                 //MyLogger.Instance.CreateEntry(string.Format("Tick Size. Ticker Id:" + tickerId + ", tickType: " + tickType + ", Size: " + size + "\n"));
//                 Console.WriteLine("Tick Size. Ticker Id:" + tickerId + ", tickType: " + tickType + ", Size: " + size + "\n");
//                 
//                 if (!this.QuoteDict.ContainsKey(tickerId))
//                 {
//                     // if new tickerID...Error, new ticker id should be added during tickString already
//                     // tickPrice and tickSize should appear after tickString
// 
//                     Console.ForegroundColor = ConsoleColor.Red;
//                     Console.WriteLine("Ticker Price. Error: tickPrice received after tickString");
//                     Console.ResetColor();
//                     //Console.WriteLine("Enter any key to stop...");
//                     //Console.ReadKey();
//                     // ignore this data
//                     return;
//                 }
// 
//                 int listCnt = QuoteDict[tickerId].Count;
//                 QuoteTick tmpQtItm = QuoteDict[tickerId].LastOrDefault();
//                 if (tmpQtItm.QtLastSize != 0)   // already received tickSize
//                     return;
// 
//                 tmpQtItm.QtLastSize = size;     // actually we don't care about size in pairs trading
//                 QuoteDict[tickerId].RemoveAt(listCnt - 1);
//                 QuoteDict[tickerId].Add(tmpQtItm);
//             }
        }

        public virtual void tickString(int tickerId, int tickType, string value)
        {
            long tmpTime = Convert.ToInt64(value);
            DateTime tickTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(tmpTime).ToLocalTime();
            //MyLogger.Instance.CreateEntry("Tick string. Ticker Id:" + tickerId + ", Type: " + tickType + ", Value: " + tickTime + "\n");
            Console.WriteLine("Tick string. Ticker Id:" + tickerId + ", Type: " + tickType + ", Value: " + tickTime + "\n");
            
//             if (tickType != 45)
//             {
//                 Console.ForegroundColor = ConsoleColor.Red;
//                 Console.WriteLine("Tick string. TICKTYPE NOT 45! Type: {0}", tickType);
//                 Console.ResetColor();
//             }
// 
// 
//             if (!QuoteDict.ContainsKey(tickerId))
//             {
//                 // if new tID, add new element to QuoteDict
//                 List<QuoteTick> tmpList = new List<QuoteTick>();
//                 QuoteTick tmpQt = new QuoteTick();
//                 tmpQt.QtTime = tickTime;
//                 tmpList.Add(tmpQt);
// 
//                 QuoteDict.Add(tickerId, tmpList);
//             }
//             else
//             {
//                 // if existing tID, add one more element to the QuoteList
//                 // before adding, check if the previous quote in QtList is complete or not
//                 QuoteTick tmpQtItm = QuoteDict[tickerId].LastOrDefault();
//                 if (tmpQtItm.QtLastPrice == 0 | tmpQtItm.QtLastSize == 0)
//                 {
//                     int listCnt = QuoteDict[tickerId].Count;
//                     QuoteDict[tickerId].RemoveAt(listCnt - 1);
//                 }
// 
//                 QuoteTick tmpQt = new QuoteTick();
//                 tmpQt.QtTime = tickTime;
// 
//                 QuoteDict[tickerId].Add(tmpQt);
// 
//                 while (QuoteDict[tickerId].Count > MAX_QUOTE_LIST)
//                 {
//                     // Remove the first element if recorded too many quotes
//                     QuoteDict[tickerId].RemoveAt(0);
//                 }
//             }
        }

        public virtual void tickGeneric(int tickerId, int field, double value)
        {
            Console.WriteLine("Tick Generic. Ticker Id:" + tickerId + ", Field: " + field + ", Value: " + value + "\n");
        }

        public virtual void tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture, int holdDays, string futureExpiry, double dividendImpact, double dividendsToExpiry)
        {
            Console.WriteLine("TickEFP. " + tickerId + ", Type: " + tickType + ", BasisPoints: " + basisPoints + ", FormattedBasisPoints: " + formattedBasisPoints + ", ImpliedFuture: " + impliedFuture + ", HoldDays: " + holdDays + ", FutureExpiry: " + futureExpiry + ", DividendImpact: " + dividendImpact + ", DividendsToExpiry: " + dividendsToExpiry + "\n");
        }

        public virtual void tickSnapshotEnd(int tickerId)
        {
            Console.WriteLine("TickSnapshotEnd: " + tickerId + "\n");
        }
        
        public virtual void deltaNeutralValidation(int reqId, UnderComp underComp)
        {
            Console.WriteLine("DeltaNeutralValidation. " + reqId + ", ConId: " + underComp.ConId + ", Delta: " + underComp.Delta + ", Price: " + underComp.Price + "\n");
        }
        
        public virtual void tickOptionComputation(int tickerId, int field, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
        {
            Console.WriteLine("TickOptionComputation. TickerId: " + tickerId + ", field: " + field + ", ImpliedVolatility: " + impliedVolatility + ", Delta: " + delta
                + ", OptionPrice: " + optPrice + ", pvDividend: " + pvDividend + ", Gamma: " + gamma + ", Vega: " + vega + ", Theta: " + theta + ", UnderlyingPrice: " + undPrice + "\n");
        }

        public virtual void accountSummary(int reqId, string account, string tag, string value, string currency)
        {
            Console.WriteLine("Acct Summary. ReqId: " + reqId + ", Acct: " + account + ", Tag: " + tag + ", Value: " + value + ", Currency: " + currency + "\n");
        }

        public virtual void accountSummaryEnd(int reqId)
        {
            Console.WriteLine("AccountSummaryEnd. Req Id: " + reqId + "\n");
        }

        public virtual void updateAccountValue(string key, string value, string currency, string accountName)
        {
            Console.WriteLine("UpdateAccountValue. Key: " + key + ", Value: " + value + ", Currency: " + currency + ", AccountName: " + accountName + "\n");
        }

        public virtual void updatePortfolio(Contract contract, int position, double marketPrice, double marketValue, double averageCost, double unrealisedPNL, double realisedPNL, string accountName)
        {
            Console.WriteLine("UpdatePortfolio. " + contract.Symbol + ", " + contract.SecType + " @ " + contract.Exchange
                + ": Position: " + position + ", MarketPrice: " + marketPrice + ", MarketValue: " + marketValue + ", AverageCost: " + averageCost
                + ", UnrealisedPNL: " + unrealisedPNL + ", RealisedPNL: " + realisedPNL + ", AccountName: " + accountName + "\n");
        }

        public virtual void updateAccountTime(string timestamp)
        {
            Console.WriteLine("UpdateAccountTime. Time: " + timestamp + "\n");
        }

        public virtual void accountDownloadEnd(string account)
        {
            Console.WriteLine("Account download finished: " + account + "\n");
        }

        public virtual void orderStatus(int orderId, string status, int filled, int remaining, double avgFillPrice, 
                                        int permId, int parentId, double lastFillPrice, int clientId, string whyHeld)
        {
            Console.WriteLine("OrderStatus. Id: " + orderId + ", Status: " + status + ", Filled: " + filled + ", Remaining: " + remaining
                + ", AvgFillPrice: " + avgFillPrice + ", PermId: " + permId + ", ParentId: " + parentId + ", LastFillPrice: " + lastFillPrice 
                + ", ClientId: " + clientId + ", WhyHeld: " + whyHeld + "\n");
            MyLogger.Instance.CreateEntry(string.Format("OrderStatus. Id: " + orderId + ", Status: " + status + ", Filled: " + filled + ", Remaining: " + remaining
                + ", AvgFillPrice: " + avgFillPrice + ", PermId: " + permId + ", ParentId: " + parentId + ", LastFillPrice: " + lastFillPrice
                + ", ClientId: " + clientId + ", WhyHeld: " + whyHeld + "\n"));


            if (remaining != 0)
                return;     // if this order not filled, do nothing

            foreach (var tmpPosPair in PairPosDict.Values)
            {
                // save price and share to PosPair obj
                tmpPosPair.savePriceNShare(orderId, avgFillPrice, filled);
            }
        }

        public virtual void openOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
            MyLogger.Instance.CreateEntry("OpenOrder. ID: " + orderId + ", " + contract.Symbol + ", " + contract.SecType + " @ " + contract.Exchange + ": " + order.Action + ", " + order.OrderType + " " + order.TotalQuantity + ", " + orderState.Status + "\n");
            Console.WriteLine("OpenOrder. ID: " + orderId + ", " + contract.Symbol + ", " + contract.SecType + " @ " + contract.Exchange + ": " + order.Action + ", " + order.OrderType + " " + order.TotalQuantity + ", " + orderState.Status + "\n");
        }

        public virtual void openOrderEnd()
        {
            Console.WriteLine("OpenOrderEnd");
        }

        public virtual void contractDetails(int reqId, ContractDetails contractDetails)
        {
            Console.WriteLine("ContractDetails. ReqId: " + reqId + " - " + contractDetails.Summary.Symbol + ", " + contractDetails.Summary.SecType + ", ConId: " + contractDetails.Summary.ConId + " @ " + contractDetails.Summary.Exchange + "\n");
        }

        public virtual void contractDetailsEnd(int reqId)
        {
            Console.WriteLine("ContractDetailsEnd. " + reqId + "\n");
        }

        public virtual void execDetails(int reqId, Contract contract, Execution execution)
        {
            Console.WriteLine("ExecDetails. " + reqId + " - " 
                               + contract.Symbol + ", " + contract.SecType + ", " + contract.Currency + " - " 
                               + execution.ExecId + ", " + execution.OrderId + ", " + execution.Shares + "\n");
            MyLogger.Instance.CreateEntry("ExecDetails. " + reqId + " - "
                               + contract.Symbol + ", " + contract.SecType + ", " + contract.Currency + " - "
                               + execution.ExecId + ", " + execution.OrderId + ", " + execution.Shares + "\n");

            foreach (var tmpPosPair in PairPosDict.Values)
            {
                // save price and share to PosPair obj
                tmpPosPair.saveExecID(execution.OrderId, execution.ExecId);
            }
        }

        public virtual void execDetailsEnd(int reqId)
        {
            Console.WriteLine("ExecDetailsEnd. " + reqId + "\n");
        }

        public virtual void commissionReport(CommissionReport commissionReport)
        {
            MyLogger.Instance.CreateEntry("CommissionReport. " + commissionReport.ExecId + " - " + commissionReport.Commission + " " + commissionReport.Currency + " RPNL " + commissionReport.RealizedPNL + "\n");
            Console.WriteLine("CommissionReport. " + commissionReport.ExecId + " - " + commissionReport.Commission + " " + commissionReport.Currency + " RPNL " + commissionReport.RealizedPNL + "\n");
            foreach (var tmpPosPair in PairPosDict.Values)
            {
                // save price and share to PosPair obj
                tmpPosPair.saveCommPNL(commissionReport.ExecId, commissionReport.Commission, commissionReport.RealizedPNL);
            }
        }

        public virtual void fundamentalData(int reqId, string data)
        {
            Console.WriteLine("FundamentalData. " + reqId + "" + data + "\n");
        }

        public virtual void historicalData(int reqId, string date, double open, double high, double low, double close, int volume, int count, double WAP, bool hasGaps)
        {
            Console.WriteLine("HistoricalData. " + reqId + " - Date: " + date + ", Open: " + open + ", High: " + high + ", Low: " + low + ", Close: " + close + ", Volume: " + volume + ", Count: " + count + ", WAP: " + WAP + ", HasGaps: " + hasGaps + "\n");
        }

        public virtual void marketDataType(int reqId, int marketDataType)
        {
            Console.WriteLine("MarketDataType. reqId: " + reqId + ", Type: " + marketDataType + "\n");
        }

        public virtual void updateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
            Console.WriteLine("UpdateMarketDepth. " + tickerId + " - Position: " + position + ", Operation: " + operation + ", Side: " + side + ", Price: " + price + ", Size" + size + "\n");
        }

        public virtual void updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, int size)
        {
            Console.WriteLine("UpdateMarketDepthL2. " + tickerId + " - Position: " + position + ", Operation: " + operation + ", Side: " + side + ", Price: " + price + ", Size" + size + "\n");
        }


        public virtual void updateNewsBulletin(int msgId, int msgType, String message, String origExchange)
        {
            Console.WriteLine("News Bulletins. " + msgId + " - Type: " + msgType + ", Message: " + message + ", Exchange of Origin: " + origExchange + "\n");
        }

        public virtual void position(string account, Contract contract, int pos, double avgCost)
        {
            Console.WriteLine("Position. " + account + " - Symbol: " + contract.Symbol + ", SecType: " + contract.SecType + ", Currency: " + contract.Currency + ", Position: " + pos + ", Avg cost: " + avgCost + "\n");
        }

        public virtual void positionEnd()
        {
            Console.WriteLine("PositionEnd \n");
        }

        public virtual void realtimeBar(int reqId, long time, double open, double high, double low, double close, long volume, double WAP, int count)
        {
            Console.WriteLine("RealTimeBars. " + reqId + " - Time: " + time + ", Open: " + open + ", High: " + high + ", Low: " + low + ", Close: " + close + ", Volume: " + volume + ", Count: " + count + ", WAP: " + WAP + "\n");
        }

        public virtual void scannerParameters(string xml)
        {
            Console.WriteLine("ScannerParameters. " + xml + "\n");
        }

        public virtual void scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
            Console.WriteLine("ScannerData. " + reqId + " - Rank: " + rank + ", Symbol: " + contractDetails.Summary.Symbol + ", SecType: " + contractDetails.Summary.SecType + ", Currency: " + contractDetails.Summary.Currency
                + ", Distance: " + distance + ", Benchmark: " + benchmark + ", Projection: " + projection + ", Legs String: " + legsStr + "\n");
        }

        public virtual void scannerDataEnd(int reqId)
        {
            Console.WriteLine("ScannerDataEnd. " + reqId + "\n");
        }

        public virtual void receiveFA(int faDataType, string faXmlData)
        {
            Console.WriteLine("Receing FA: " + faDataType + " - " + faXmlData + "\n");
        }

        public virtual void bondContractDetails(int requestId, ContractDetails contractDetails)
        {
            Console.WriteLine("Bond. Symbol " + contractDetails.Summary.Symbol + ", " + contractDetails.Summary);
        }

        public virtual void historicalDataEnd(int reqId, string startDate, string endDate)
        {
            Console.WriteLine("Historical data end - " + reqId + " from " + startDate + " to " + endDate);
        }

        public virtual void verifyMessageAPI(string apiData)
        {
            Console.WriteLine("verifyMessageAPI: " + apiData);
        }
        public virtual void verifyCompleted(bool isSuccessful, string errorText)
        {
            Console.WriteLine("verifyCompleted. IsSuccessfule: " + isSuccessful + " - Error: " + errorText);
        }
        public virtual void displayGroupList(int reqId, string groups)
        {
            Console.WriteLine("DisplayGroupList. Request: " + reqId + ", Groups" + groups);
        }
        public virtual void displayGroupUpdated(int reqId, string contractInfo)
        {
            Console.WriteLine("displayGroupUpdated. Request: " + reqId + ", ContractInfo: " + contractInfo);
        }
        #endregion

        // quoteDict has been moved to PairPos Dict
        // CSV writer for checking quotes
        //         public void CSVWriter(int tickerID)
        //         {
        //             // create filename
        //             string fileName = "";
        //             try
        //             {
        //                 fileName = string.Format("{0}{1}_{2}.csv", QUOTE_FOLDER_DIR, tickerID, TickerSymbolDict[tickerID]);
        //             }
        //             catch (Exception)
        //             {
        //                 Console.WriteLine("No ticker ID: {0}", tickerID);
        //                 return;
        //             }
        //             
        //             
        //             // delete the file if exists
        //             if (File.Exists(fileName))
        //             {
        //                 File.Delete(fileName);
        //             }
        // 
        //             try
        //             {
        //                 using (FileStream fs = File.Create(fileName))
        //                 {
        //                     // write title
        //                     Byte[] title = new UTF8Encoding(true).GetBytes("Time, Price, Size\n");
        //                     fs.Write(title, 0, title.Length);
        // 
        //                     // write contents
        //                     for (int i = 0; i < QuoteDict[tickerID].Count; i++)
        //                     {
        //                         QuoteTick tmpQtItm = QuoteDict[tickerID][i];
        //                         string tmpStr = String.Format("{0},{1},{2}\n", tmpQtItm.QtTime, tmpQtItm.QtLastPrice, tmpQtItm.QtLastSize);
        //                         Byte[] line = new UTF8Encoding(true).GetBytes(tmpStr);
        //                         fs.Write(line, 0, line.Length);
        //                     }
        //                 }
        //             }
        //             catch (Exception)
        //             {
        //                 throw;
        //             }
        //         }
        // go through the symbol dictionary, and fill out the PairPosition dictionary
    }
}
