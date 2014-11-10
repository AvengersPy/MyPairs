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
            //IB's main object
            EWrapperImpl ibClient = new EWrapperImpl();

            //Connect
            ibClient.ClientSocket.eConnect("127.0.0.1", 7496, 0);

            //Create and define a contract to fetch data for
            Contract contract = new Contract();
            contract.Symbol = "EUR";
            contract.SecType = "CASH";
            contract.Currency = "GBP";
            contract.Exchange = "IDEALPRO";

            //Invoke IB's ClientSocket's data request
            ibClient.ClientSocket.reqMktData(1, contract, "", false, null);

            //Stay alive for a little while
            Thread.Sleep(10000);
        }
    }
}
