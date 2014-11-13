from scipy import stats

def loadData():
	x = []
	y = []
	mmm = open('data/mmm.csv')
	spy = open('data/spy.csv')
	
	
	for line in mmm.readlines():
		lineArr1 = line.strip().split('\r')
		for element in lineArr1:
			ele = element.split(',')
			temp = float(ele[5])
			x.append(temp)
#		lineX = map(float, line.strip().split('\n'))
		
#		mmm_open, mmm_high, mmm_low, mmm_close, mmm_volume, mmm_adjusted = line.split(",")
#		x.extend(mmm_adjusted)
	for line in spy.readlines():
		lineArr2 = line.strip().split('\r')
		
		y.extend(float(lineArr2[5]))
#		spy_open, spy_high, spy_low, spy_close, spy_volume, spy_adjusted = line.split(",")
#		y.extend(spy_adjusted)
	return (mat(x).transpose(), mat(y).transpose())

y, x = loadData()

gradient, intercept, r_value, p_value, std_err = stats.linregress(x,y)
print "Gradient and intercept", gradient, intercept
#print "R-squared", r_value**2
#print "p-value", p_value

import pandas as pd
import numpy as np

COLUMN_SEPARATOR = ','
mmm_data = pd.DataFrame.from_csv('data/mmm.csv', sep=COLUMN_SEPARATOR, header=None)

#AREA_INDEX = 4
#SELLING_PRICE_INDEX = 13
#x = housing_data[AREA_INDEX]
#y = housing_data[SELLING_PRICE_INDEX]

#regression = np.polyfit(x, y, 1)
#print(mmm_data)
#print(type(mmm_data))#<class 'pandas.core.frame.DataFrame'>
print(mmm_data[])

COLUMN_SEPARATOR = ','
spy_data = pd.DataFrame.from_csv('data/spy.csv', sep=COLUMN_SEPARATOR, header=None)
print(spy_data[])

from scipy.stats import mstats
from pylab import plot, title, xlabel, ylabel, show
from numpy import array, zeros, sqrt, shape

# Gradient & intercept
# a = 0.000194054, b = 0.840379173
beta0 = 0.000194054
beta = 0.840379173
residual = mmm_data[1] -beta0- beta*spy_data[1]
n= 1964
for i in np.arange(0,n):
	residual = mmm_data[i] -beta0- beta*spy_data[i]

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





