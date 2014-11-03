rics <- read.csv("RICs.csv", header = T, stringsAsFactors = F)
rics <- as.character(rics[, 1])

# initial request 30
for(i in 1:30)
{
    submitFTPRequest (rdth, friendlyname=paste(rics[i], "daily", sep="-"), instrumentList = rics[i], 
                      "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                      reqtype="EndOfDay", mktdepth="0",
                      messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                      reqInGMT="false", disInGMT="false")
    print(i)
}

idx <- 13
while(TRUE)
{
    status <- getInFlightStatus(rdth)
    inFlightStatus <- as.numeric(status@active)
    
    if (inFlightStatus < 31) {
        cat("Start Downloading, in flight status:", inFlightStatus, "\n")
        for (i in 1:5)
        {
            idx = idx + 1
            cat("Sending Request No", idx, rics[idx], "\n", sep=" ")
            cat("Inflight reqeusts:", inFlightStatus, "\n", sep=" ")
            
            submitFTPRequest (rdth, friendlyname=paste(rics[idx + i], "daily", sep="-"), instrumentList = rics[idx + i], 
                              "2011-01-01", "2014-09-30", "00:00:00", "23:59:59.999", 
                              reqtype="EndOfDay", mktdepth="0",
                              messagetypelist ="End Of Day (from Real-Time): Open, High, Low, Last, Volume",
                              reqInGMT="false", disInGMT="false")
            status <- getInFlightStatus(rdth)
            inFlightStatus <- as.numeric(status@active)
        }    
    } 
    
    cat("Sleeping, in flight status:", inFlightStatus, "\n")
    Sys.sleep(300)
    
    if (idx > length(rics))
    {
        print("Finish Downloading")
    }
}


getRequestResult(rdth, "MMM_N-N74501916")
cleanUp(rdth)


cxlList <- read.csv("Book1.csv", header=F)
cxl <- as.character(cxlList$V1)
for (i in 1:length(cxl))
{
    result <- cancelRequest(rdth, cxl[i])
    cat(i, result)
}
