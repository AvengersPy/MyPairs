import os
# set up working directory
os.chdir(r"C:\Users\Zhe\Documents\GitHub\MyPairs\Python_Strat\working_dir")

import clr

clr.AddReference("testIBPython")
clr.AddReference("TWSLib")
clr.AddReference("YahooAPI")

clr.AddReference("Microsoft.Build.Utilities.v4.0")
clr.AddReference("Microsoft.Build.Framework")

from IBApi import *
from HelloIBCSharp import *
from Yahoo import *

#System Modules
from System import *
from System.Threading import *
from System.Text import *
from System.IO import *
from System.Collections.Generic import *

from System.Linq import *
from System.Security import *
from Microsoft.Build.Framework import *
from Microsoft.Build.Utilities import *
from System.Threading.Tasks import *
from System.Text.RegularExpressions import *   # replace multiple values in a string


### ============================ how to use logger ============================ 
# MyLogger.Instance.Open(r"C:\Users\Zhe\Documents\GitHub\MyPairs\Python_Strat\Logger\mylogger.txt", False)
# MyLogger.Instance.CreateEntry("Test")

###  ============================ Connect ============================ 
EWrapperImpl.Instance.ClientSocket.eConnect("127.0.0.1", 7496, 0);
Thread.Sleep(1500)
#Console.Read()

###  ============================  Create Signal  ============================ 
# tmpSignal = PairSignal()
# tmpSignal.StkTID = 8;   # CSCO
# tmpSignal.EtfTID = 1;

# tmpSignal.TrSignal = PairType.openLong;	
# EWrapperImpl.Instance.processSignal(tmpSignal);
# Console.Read()

# tmpSignal.TrSignal = PairType.closeLong;
# EWrapperImpl.Instance.processSignal(tmpSignal);
# Console.Read()

# tmpSignal.TrSignal = PairType.openShort;
# EWrapperImpl.Instance.processSignal(tmpSignal);
# Console.Read()

# tmpSignal.TrSignal = PairType.closeShort;
# EWrapperImpl.Instance.processSignal(tmpSignal);
# Console.Read()


### ============================ self close for a pair  ============================ 
# tmpSignal = PairSignal()
# tmpSignal.StkTID = 8;   # CSCO
# tmpSignal.EtfTID = 1;

# tmpSignal.TrSignal = PairType.openLong;	
# EWrapperImpl.Instance.processSignal(tmpSignal);
# Console.Read()

# tmpPair = EWrapperImpl.Instance.PairPosDict[tmpSignal.StkTID]
# tmpPair.closeThisPosition()

# EWrapperImpl.Instance.getAllQuote()
# EWrapperImpl.Instance.getAllQuote()
Thread.Sleep(10000)
Console.WriteLine("The End")