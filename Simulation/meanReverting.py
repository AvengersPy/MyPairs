from scipy.stats import mstats


beta0 = 0.000194054
beta = 0.840379173
mmm_data = [77.53,78.4,77.89,77.42,78,77.31,78.05,78.41,79.48,
79.33,78.71,79.09,79.25,78.71,
78.85,79.56,78.97,78.54,74.92,74.21,74.75,
73.87,74.08,73.91,74.3,74.88,74.64,74.55,76,
76.43,76.9,76.91,76.5,76.1,76.25,76.49,76.45,
76.13,74.25,74,73.25,73.01,73.62,73.5,74.03,
74.59,74.61,74.75,74.78,75.8,76.23,75.54,
76.49,76.25,77.09,77.59,77.66,77.44,76.74,76.38]

spy_data = [142.25,141.23,141.33,140.82,141.31,140.58,141.58,
142.15,143.07,142.85,143.17,142.54,143.07,142.26,142.97,143.86,
142.57,142.19,142.35,142.63,144.15,144.73,144.7,144.97,145.12,
144.78,145.06,143.94,143.77,144.8,145.67,145.44,145.56,145.61,
146.05,145.74,145.83,143.88,140.39,139.34,140.05,137.93,138.78,
139.59,140.54,141.31,140.42,140.18,138.43,138.97,139.31,139.26,
140.08,141.1,143.48,143.28,143.5,143.12,142.14,142.54]

#print(len())
#print(mmm_data[59])

Xt = []
result = 0
epsilon = []

for i in range(0,60):
    # epsilon.append(mmm_data[i] - beta0 - beta*spy_data[i])
    result = result + epsilon[i]
    Xt.append(result) 

# print "Xt: "
# print Xt

# print '\n\n\n'
	
x = []
y = []

for j in range(0,59):
    x.append(epsilon[j])
    y.append(epsilon[j + 1])


#import rpy2
from scipy import stats
#def loadData():
gradient, intercept, r_value, p_value, std_err = stats.linregress(x,y)
print "Gradient and intercept", gradient, intercept  

from numpy.lib.scimath import logn
from math import e

a = intercept
b = gradient
k = logn(e, b)*-252
m = a/(1-b)

lam = []
for j in range(0,59):
    lam.append(Xt[j+1] - a - b * Xt[j])


import numpy as np

lamvar = np.var(lam)

print(lamvar)

from numpy import array, zeros, sqrt, shape

sigmaeq = sqrt(lamvar/(1-b**2))

smod = []

s = []



for i in range(0, 60):
    s.append((epsilon[i]-m)/sigmaeq)
    smod.append(s[i] - a/k*sigmaeq)

print (epsilon) 
print (sigmaeq)
print (smod)
print (s)


#from pylab import plot, title, xlabel, ylabel, show

#plot(range(0, 60), epsilon)
#plot(range(0, 60), s)
#title('Simulations')
#xlabel('steps')
#ylabel('S-score')
#show()


#return (mat(H).transpose(), mat(V).transpose())

#V, H = loadData()


#print r.lm(r("H ~ V"), data = r.data_frame(H=H, V=V))['coefficients']












