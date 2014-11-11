import clr
# IB Api Modules
clr.AddReference("testIBPython")
clr.AddReference("TWSLib")
clr.AddReference("YahooAPI")
from IBApi import *
from HelloIBCSharp import *

# System Modules
from System import *
from System.Threading import *
from System.IO import *
from System.Text import *
from System.Collections.Generic import *

# connect to IB and get data
# symbolDir = r"C:\Users\zhe\Dropbox\FE520 Proj\ProjPythonScript\PyNet\Dow.csv"
# ibClient = EWrapperImpl(symbolDir)
# ibClient.ClientSocket.eConnect("127.0.0.1", 7496, 0)

# create contract
# generalContract = Contract();
# generalContract.SecType = "STK";
# generalContract.Currency = "USD";
# generalContract.Exchange = "SMART";

# req market data by loop
# for tID in ibClient.TickerSymbolDict.Keys:
	# c1 = generalContract;
	# c1.Symbol = ibClient.TickerSymbolDict[tID];
	# ibClient.ClientSocket.reqMktData(tID, c1, "", False, None);
Console.WriteLine("Sleep")
Thread.Sleep(10000);

# Console.ForegroundColor = ConsoleColor.Red;
# Console.WriteLine("Finish requesting quote data! Now write to file.\n\n");
# Thread.Sleep(3000)
# Console.ResetColor();

# for tID in ibClient.QuoteDict.Keys:
	# ibClient.ClientSocket.cancelMktData(tID);
	
# for tID in ibClient.QuoteDict.Keys:
	# Console.WriteLine("{0}, {1}, {2}", tID, ibClient.TickerSymbolDict[tID], ibClient.QuoteDict[tID].Count);
	# ibClient.CSVWriter(tID);