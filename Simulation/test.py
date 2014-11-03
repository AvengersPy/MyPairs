from numpy import *  
import matplotlib.pyplot as plt  
import time  
import math
  
def loadData():  
    train_x = []  
    train_y = []  
    ibm = open('C:\Users\zhe\Documents\GitHub\MyPairs\Simulation\data\ibm.csv') 
    etf = open('C:\Users\zhe\Documents\GitHub\MyPairs\Simulation\data\ETF.csv') 
    for line in ibm.readlines():  
        lineArr1 = line.strip().split()  
        train_x.append(float(lineArr1[2]))
    for line in etf.readlines():  
        lineArr2 = line.strip().split()  
        train_y.append(float(lineArr2[2]))
    return mat(train_x).transpose(), mat(train_y).transpose()
	


def linear_regression(series1, series2):
	sum_x = 0
	sum_y = 0
	n = len(x)
	for x in series1:
		sum_x = sum_x + x
	exp_x = sum_x/n
	for y in series2:
		sum_y = sum_y + y
	exp_y = sum_y/n
	b_nominator = 0
	b_denominator = 0
	for i in range(1,(n)):
		b_nominator = b_nominator + (series1[i]-exp_x)*(series2[i]-exp_y)
		b_denominator = b_denominator + (series1[i]-exp_x)^2
	b = b_nominator/b_denominator
	a = exp_y - b * exp_x
	residual = series2[1] - a - b * series1[1]
	for j in range(2,(n)):
		residual.append(series2[j] - a - b * series1[j])
	return a, b, residual