setwd('/Users/yubing/MyPairs/Correlation')
DIA <- read.csv(file="DIA.csv", head = T)

DIA.last <- DIA[,7]

DIA.return <- log(DIA.last[2:(length(DIA.last))]) - log(DIA.last[1:(length(DIA.last)-1)])
setwd('/Users/yubing/MyPairs/Correlation/DIA')
files<- list.files()

beta <- NULL
symbol <- NULL
corr <- NULL
n <- 0

for(f in files)
{
  n <- n+1
  STK <- read.csv(file = f, head = T)
  STK.last <- STK[,8]
  STK.return <- log(STK.last[2:(length(STK.last))]) - log(STK.last[1:(length(STK.last)-1)])
  m1 <- lm(STK.return ~ DIA.return)
  beta <- c(beta, m1$coefficients)
  symbol.csv <- unlist(strsplit(f,'[.]'))
  symbol <- c(symbol, symbol.csv[1])
  corr <- c(corr, cor(STK.return, DIA.return))
}

dataf <- matrix(NA, nrow = n, ncol = 4)
for(i in (1:n))
{
  dataf[i,1] <- symbol[i]
  dataf[i,2] <- "DIA"
  dataf[i,3] <- beta[i]
  dataf[i,4] <- corr[i]
}
setwd('/Users/yubing/MyPairs/Correlation')
write.csv(dataf, file = "STKcorrDIA.csv")
