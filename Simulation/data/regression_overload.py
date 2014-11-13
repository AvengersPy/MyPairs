import os
import csv
import pandas as pd
import numpy as np
from scipy import stats

def regression(listETF, listSTK):
	gradient, intercept, r_value, p_value, std_err = stats.linregress(listETF, listSTK)
	n = len(listETF)
	residure = []
	for i in xrange(0,941):
		x = float(listSTK[i] - gradient * listETF[i] - intercept)
		residure.append(x)
	return intercept, gradient, residure

def readCSV():
	os.chdir(r'C:\Users\zhe\Documents\GitHub\MyPairs\Simulation\data')
	data1 = pd.read_csv('1.csv')# pandas dataframe
	data2 = pd.read_csv('2.csv')
	mylistX = data1['Last']		# convert to list, you now calculate return and do the regression
	mylistY = data2['Last']
	return mylistX, mylistY