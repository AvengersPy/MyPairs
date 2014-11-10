/* Copyright (C) 2014 Interactive Brokers LLC. All rights reserved.  This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IBApi;
using MYLogger;

namespace HelloIBCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            //IB's main object
            const string symbolFile = @"C:\Users\Zhe\Documents\GitHub\MyPairs\testSymbol.csv";
            const string quoteDir = @"C:\Users\Zhe\Documents\GitHub\MyPairs\tmp_quotes";
            
            // TODO: remove this max quote to somewhere else
            const int maxQuote = 60;
            EWrapperImpl ibClient = new EWrapperImpl(symbolFile, quoteDir, maxQuote);
            
            ibClient.ClientSocket.eConnect("127.0.0.1", 7496, 0);
            Thread.Sleep(2000);

            #region Test Yahoo
            //Console.WriteLine(ibClient.PairPosDict[8].pairStkLeg.Symbol);
<<<<<<< HEAD
            for (int i = 0; i < 10; i++)
            {
                if (i == 60)
                {
                    Console.ReadKey();
                }
                ibClient.getAllQuote();
            }
=======
            //ibClient.PairPosDict[8].getPairQuote();
>>>>>>> origin/master
            #endregion

            #region testProcessSignal
//             PairSignal tmpSignal = new PairSignal();
//             tmpSignal.StkTID = 8;   // CSCO
//             tmpSignal.EtfTID = 1;
//             tmpSignal.TrSignal = PairType.openLong;
// 
//             ibClient.processSignal(tmpSignal);
//             Console.ReadKey();
// 
//             tmpSignal.TrSignal = PairType.closeLong;
//             ibClient.processSignal(tmpSignal);
// 
//             Console.ReadKey();
            #endregion


            #region Test req mkt data
//             Contract generalContract = new Contract();
//             generalContract.SecType = "CASH";
//             generalContract.Currency = "USD";
//             generalContract.Exchange = "IDEALPRO";
//             generalContract.Symbol = "EUR";
//             ibClient.ClientSocket.reqMktData(1, generalContract, "", false, null);
//             Console.ReadKey();
            #endregion


            #region reqMktData

//             Contract generalContract = new Contract();
//             generalContract.SecType = "STK";
//             generalContract.Currency = "USD";
//             generalContract.Exchange = "SMART";
// 
// 
//             // request one symbol
//             generalContract.Symbol = "GOOG";
//             ibClient.ClientSocket.reqMktData(1, generalContract, "", false, null);
//             Console.ReadKey();

            // request a loop

//             for (int tID = 0; tID < NUM_QUOTES; tID++)
//             {
//                 Contract c1 = generalContract;
//                 //c1.Symbol = ibClient.TickerSymbolDict.ElementAt(tID).Value;
//                 c1.Symbol = "GOOG";
// 
//                 ibClient.ClientSocket.reqMktData(ibClient.TickerSymbolDict.ElementAt(tID).Key, c1, "", false, null);
//             }
//             Thread.Sleep(30000);
//             Console.ForegroundColor = ConsoleColor.Red;
//             Console.WriteLine("Finish requesting quote data! Now write to file.\n\n");
//             Console.ResetColor();
// 
//             foreach (int tID in ibClient.QuoteDict.Keys)
//             {
//                 ibClient.ClientSocket.cancelMktData(tID);
//             }
// 
//             foreach (int tID in ibClient.QuoteDict.Keys)
//             {
//                 Console.WriteLine("{0}, {1}, {2}", tID, ibClient.TickerSymbolDict[tID], ibClient.QuoteDict[tID].Count);
//                 ibClient.CSVWriter(tID);
//             }
            #endregion

            #region Market Order

            ////Create and define a contract to fetch data for
//             Contract contract = new Contract();
//             contract.Symbol = "CSCO";
//             contract.SecType = "STK";
//             contract.Currency = "USD";
//             contract.Exchange = "SMART";
// 
//             Order tmpOrder = new Order();
//             //tmpOrder.ClientId = 0;
//             tmpOrder.OrderId = ibClient.NextOrderId;
//             tmpOrder.Action = "BUY";
//             tmpOrder.TotalQuantity = 100;
//             tmpOrder.OrderType = "MKT";
//             
//             ibClient.ClientSocket.placeOrder(ibClient.NextOrderId, contract, tmpOrder);
//             
//             Thread.Sleep(3000);

            ////ibClient.ClientSocket.cancelOrder(ibClient.NextOrderId);

            //Contract contract1 = new Contract();
            //contract1.Symbol = "AAPL";
            //contract1.SecType = "STK";
            //contract1.Currency = "USD";
            //contract1.Exchange = "SMART";

            
            //tmpOrder.OrderId = ibClient.NextOrderId + 1;
            //tmpOrder.Action = "BUY";
            //tmpOrder.TotalQuantity = 100;
            //tmpOrder.OrderType = "LMT";
            //tmpOrder.LmtPrice = 90;

            //ibClient.ClientSocket.placeOrder(ibClient.NextOrderId, contract, tmpOrder);

            //Thread.Sleep(10000);
            #endregion
            Console.WriteLine("The End.");
        }
    }
}