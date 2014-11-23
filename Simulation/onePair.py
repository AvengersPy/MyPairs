import os
import csv
import pandas as pd
import numpy as np
from scipy import stats
import matplotlib.pyplot as plt




def regression(listETF, listSTK):
	gradient, intercept, r_value, p_value, std_err = stats.linregress(listETF, listSTK)
	n = len(listETF)
	residure = []
	for i in xrange(0, n):
		x = float(listSTK[i] - gradient * listETF[i] - intercept)
		residure.append(x)
	return intercept, gradient, residure

def readCSV():
	os.chdir(r'/Users/yubing/MyPairs/Simulation/data')
	data1 = pd.read_csv('spy.csv')# pandas dataframe
	data2 = pd.read_csv('csco.csv')
	ETFlist = list(data1['Adj Close'])		# convert to list, you now calculate return and do the regression
	STKlist = list(data2['Adj Close'])
	return ETFlist, STKlist
	
def score(Epsilon, ptime):
	
	if ptime >= len(Epsilon):
		try:
			pass
		except:
			print "Wrong ptime!"

	Xt = []
	result = 0
	for i in range(0,ptime):
		result = result + Epsilon[i]
		Xt.append(result)

	Xt_Lag1 = []
	Xt_Lag0 = []

	for j in range(0,ptime-1):
		Xt_Lag1.append(Xt[j])
		Xt_Lag0.append(Xt[j + 1])

	gradient, intercept, r_value, p_value, std_err = stats.linregress(Xt_Lag0, Xt_Lag1)

	a = intercept
	b = gradient
	k = - np.log(b) * 252
	m = a / (1 - b)

	lam = []

	for j in range(0,ptime-1):
		lam.append(Xt_Lag0[j] - a - b * Xt_Lag1[j])
	lamvar = np.var(lam)
	sigmaeq = np.sqrt(lamvar*2*k/(1-b**2))

	smod = []
	s = []

	for i in range(0, ptime):
		s.append((Epsilon[i]-m)/sigmaeq)
		smod.append(s[i] - a/(k*sigmaeq))

	print m

	return smod

# beta0 = 0.000194054
# beta = 0.840379173
# mmm_data = [77.53,78.4,77.89,77.42,78,77.31,78.05,78.41,79.48,
# 79.33,78.71,79.09,79.25,78.71,
# 78.85,79.56,78.97,78.54,74.92,74.21,74.75,
# 73.87,74.08,73.91,74.3,74.88,74.64,74.55,76,
# 76.43,76.9,76.91,76.5,76.1,76.25,76.49,76.45,
# 76.13,74.25,74,73.25,73.01,73.62,73.5,74.03,
# 74.59,74.61,74.75,74.78,75.8,76.23,75.54,
# 76.49,76.25,77.09,77.59,77.66,77.44,76.74,76.38]

# spy_data = [142.25,141.23,141.33,140.82,141.31,140.58,141.58,
# 142.15,143.07,142.85,143.17,142.54,143.07,142.26,142.97,143.86,
# 142.57,142.19,142.35,142.63,144.15,144.73,144.7,144.97,145.12,
# 144.78,145.06,143.94,143.77,144.8,145.67,145.44,145.56,145.61,
# 146.05,145.74,145.83,143.88,140.39,139.34,140.05,137.93,138.78,
# 139.59,140.54,141.31,140.42,140.18,138.43,138.97,139.31,139.26,
# 140.08,141.1,143.48,143.28,143.5,143.12,142.14,142.54]
spy_data, mmm_data = readCSV()

PTime = 477

spyRtn = np.log(spy_data[1:PTime+1]) - np.log(spy_data[0:PTime])
mmmRtn = np.log(mmm_data[1:PTime+1]) - np.log(mmm_data[0:PTime])

beta0, beta1, epsilon = regression(spyRtn, mmmRtn)
#print spy_data
sscore = score(epsilon, PTime)
print sscore
plt.plot(range(0,len(sscore)),sscore)
#plt.axis([0,len(sscore),min(sscore)*1.2,max(sscore)*1.2])
plt.axis([0,PTime,-0.3,0.3])
plt.show()