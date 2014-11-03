library ( THAPI )
rdth <- createCredential ( user ="jyang19@stevens.edu", password ="yangjian51")

getAssetDomains(rdth)


snp <- read.csv("s&P500A.CSV")

gets <- function(file, index, number)
{
  s <- ""
  indexnow <- 1
  numbernow <- 0
  for (j in 1:length(file$NYSE))
  {
    c <- ""
    suf <- ""
    if(file$NYSE[j] != "")
    {
      c <- file$NYSE[j]
      suf <- ", "
    }
    else
    
      {
      c <- file$NASDAQ[j]
      suf <- ", "
    }
    numbernow <- numbernow + 1
    if(numbernow > number)
    {
      numbernow <- 1
      indexnow <- indexnow + 1
    }
    if(indexnow > index)
      break
    else if(indexnow == index)
    {
      s <- paste(s, c, sep = "")
      s <- paste(s, suf, sep = "")
    }
  }
  s <- substr(s, 1, nchar(s) - 2)
  return(s)
}

#gets(snp, 1, 50)
#1-50
submitFTPRequest (rdth, "test", instrumentList = gets(snp, 1, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Min: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 1, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Hour: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 1, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="EndOfDay", mktdepth="0",
                  messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")
#51-100
submitFTPRequest (rdth, "test", instrumentList = gets(snp, 2, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Min: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 2, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Hour: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 2, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="EndOfDay", mktdepth="0",
                  messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")
#101-150
submitFTPRequest (rdth, "test", instrumentList = gets(snp, 3, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Min: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 3, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Hour: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 3, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="EndOfDay", mktdepth="0",
                  messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")
#151-200
submitFTPRequest (rdth, "test", instrumentList = gets(snp, 4, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Min: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 4, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Hour: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 4, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="EndOfDay", mktdepth="0",
                  messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")
#201-250
submitFTPRequest (rdth, "test", instrumentList = gets(snp, 5, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Min: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 5, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Hour: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 5, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="EndOfDay", mktdepth="0",
                  messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")
#251-300
submitFTPRequest (rdth, "test", instrumentList = gets(snp, 6, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Min: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 6, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Hour: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 6, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="EndOfDay", mktdepth="0",
                  messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")
#301-350
submitFTPRequest (rdth, "test", instrumentList = gets(snp, 7, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Min: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 7, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Hour: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 7, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="EndOfDay", mktdepth="0",
                  messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")
#351-400
submitFTPRequest (rdth, "test", instrumentList = gets(snp, 7, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Min: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 7, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Hour: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 7, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="EndOfDay", mktdepth="0",
                  messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")
#351-400
submitFTPRequest (rdth, "test", instrumentList = gets(snp, 8, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Min: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 8, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Hour: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 8, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="EndOfDay", mktdepth="0",
                  messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")
#401-450
submitFTPRequest (rdth, "test", instrumentList = gets(snp, 9, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Min: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 9, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Hour: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 9, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="EndOfDay", mktdepth="0",
                  messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")
#451-400
submitFTPRequest (rdth, "test", instrumentList = gets(snp, 10, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Min: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 10, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Hour: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList = gets(snp, 10, 50), 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="EndOfDay", mktdepth="0",
                  messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")
#DIA.P
submitFTPRequest (rdth, "test", instrumentList ="DIA.P", 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Min: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList ="DIA.P", 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Hour: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList ="DIA.P", 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="EndOfDay", mktdepth="0",
                  messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")
#SPY.P
submitFTPRequest (rdth, "test", instrumentList ="SPY.P", 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Min: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList ="SPY.P", 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="Intraday", mktdepth="0",
                  messagetypelist ="Intraday 1Hour: Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")

submitFTPRequest (rdth, "test", instrumentList ="SPY.P", 
                  "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                  reqtype="EndOfDay", mktdepth="0",
                  messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                  reqInGMT="false", disInGMT="false")


#getRequestResult ( rdth , "jyang19@stevens.edu-test-N74475750")
#getRequestResult ( rdth , "jyang19@stevens.edu-test-N74475750")