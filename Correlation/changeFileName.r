setwd('/Users/yubing/MyPairs/Correlation/SPY')
files<- list.files()
for(f in files)
{
  dframe <- read.csv(f,head=T)
  hate.csv <- unlist(strsplit(f,'[_]'))
  unlike.csv <- unlist(strsplit(hate.csv[1],'[-]'))
  symbol <- unlike.csv[2]
  fname <-paste(symbol,'.csv')
  write.csv(dframe,fname)
}