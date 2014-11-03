library(quantmod)
getSymbols("SPY")
getSymbols("MMM")
?write.csv

spy <- data.frame(SPY)
spy$SPY.Adjusted
mmm <- data.frame(MMM)

write.csv(SPY, file="spy.csv", row.names = FALSE)
write.csv(MMM, file="mmm.csv", row.names = FALSE)



m <- read.csv("mmm.csv")
