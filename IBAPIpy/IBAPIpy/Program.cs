/* Copyright (C) 2014 Interactive Brokers LLC. All rights reserved.  This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IBApi;
using MYLogger;
using Yahoo;
using PairObj;

namespace HelloIBCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //Connect
            EWrapperImpl.Instance.ClientSocket.eConnect("127.0.0.1", 7496, 0);
            Thread.Sleep(2000);

            EWrapperImpl.Instance.getAllQuote();

            PairSignal tmpSignal = new PairSignal();
            tmpSignal.StkTID = 8;   // CSCO
            tmpSignal.EtfTID = 1;
            tmpSignal.TrSignal = PairType.openLong;

            //ibClient.processSignal(tmpSignal);
            Console.ReadKey();

            EWrapperImpl.Instance.getAllQuote();

            tmpSignal.TrSignal = PairType.closeLong;
            EWrapperImpl.Instance.processSignal(tmpSignal);
            
            Console.ReadKey();

            #region request market data IB
            //Create and define a contract to fetch data for
//             Contract contract = new Contract();
//             contract.Symbol = "EUR";
//             contract.SecType = "CASH";
//             contract.Currency = "GBP";
//             contract.Exchange = "IDEALPRO";
// 
//             //Invoke IB's ClientSocket's data request
            //             ibClient.ClientSocket.reqMktData(1, contract, "", false, null);
            #endregion

            //Stay alive for a little while
            Console.ReadKey();
            Console.WriteLine("The End.");
        }
    }
}
