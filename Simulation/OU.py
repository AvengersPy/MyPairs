from scipy.stats import mstats
from pylab import plot, title, xlabel, ylabel, show
from numpy import array, zeros, sqrt, shape


epsilon[]


MEAN_REVERTING_PERIOD = 60
epsilon = list[]




X_t = list[]
for i in range(0, MEAN_REVERTING_PERIOD-1):
	X_t[i] = sum(epsilon[0:i])

X_t
	
# Gradient & intercept
# a = 0.000194054, b = 0.840379173
beta0 = 0.000194054
beta = 0.840379173
residual = mmm_data[1] -beta0- beta*spy_data[1]
n  = 1964
for i in np.arange(0,n):
	residual = mmm_data[i] -beta0- beta*spy_data[i]
	print residual

a = 0.000194054 # Gradient
b = 0.840379173 # intercept
K = -log(b)*252
m = a/(1-b)

residual.lag= residual[2] - residual[1]
for j in np.arrange(1,n+1):
	residual.lag = residual[j+1]-residual[j]

residual.quantile= mstats.mquantiles(residual.lag)
sigma = sqrt(residual.quantile*2*k/(1-b^2))
sigma.eq = sqrt(residual.quantile/(1-b^2))
s = (residual-m)/sigma.eq
s.mod = s -beta0/(k*sigma.eq)

plot(t, residual,'-+r')
plot(t, s,'-b')
plot(t, s.mod, '-g')

title('Simulations')
xlabel('time')
ylabel('S-score')
show()





