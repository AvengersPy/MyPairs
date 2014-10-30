library(quantmod)


getSymbols("SPY")
getSymbols("MMM")
spy <- data.frame(SPY)
mmm <- data.frame(MMM)
spy.return <- log(spy$SPY.Adjusted[2:(nrow(spy))]) - log(spy$SPY.Adjusted[1:(nrow(spy)-1)])
mmm.return <- log(mmm$MMM.Adjusted[2:(nrow(spy))]) - log(mmm$MMM.Adjusted[1:(nrow(spy)-1)])


m1 <- lm(mmm.return ~ spy.return)
m1$coefficients
#0.000194054  0.840379173

mean(m1$residuals)


plot(mmm.return, spy.return)

abline(a = 0.000194054, b = 0.840379173)

beta0 <- 0.000194054
beta <- 0.840379173
residual <- mmm.return[1] - beta0 - beta * spy.return[1]
x <- residual
for(i in 2:1965)
{
  residual <- c(residual, mmm.return[i]-beta0-beta*spy.return[i])
  x <- c(x, x[i-1] + residual[i])
}

ab <- lm(x[1:1964] ~ x[2:1965])
ab
#  0.0001036    0.9903414  
a <- 0.0001036
b <- 0.9903414
k <- -log(b)*252
k
#[1] 2.445798
m <- a/(1-b)
m
#[1] 0.01072619
x.lag <- x[2]-x[1]
for(j in 2:1963)
{
  x.lag <- c(x.lag, x[j+1]-x[j])
}
x.lag.var <- quantile(x.lag)
sigma <- sqrt(x.lag.var*2*k/(1-b^2))
sigma.eq <- sqrt(x.lag.var/(1-b^2))
s <- (x-m)/sigma.eq
plot.ts(x)
plot.ts(s)
s.mod <- s - beta0/(k*sigma.eq)
plot.ts(s.mod)
abline(-1.75,0, col="gray60")
abline(-0.25,0, col="lightgray")
abline(2.00,0, col="gray60")
abline(1.25,0, col="lightgray")
