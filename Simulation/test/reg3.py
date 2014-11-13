import os
import csv
import pandas as pd
import numpy as np
from scipy import stats

os.chdir(r'/Users/yubing/MyPairs/Simulation/data') # set to your own working directory

data1 = pd.read_csv('1.csv')# pandas dataframe
data2 = pd.read_csv('2.csv')
#print data1[:4]				# print first a few rows
#print data1['Last']			# print one col

mylist1 = data1['Last']		# convert to list, you now calculate return and do the regression
mylist2 = data2['Last']
#print "mean: ", np.mean(mylist1)
gradient, intercept, r_value, p_value, std_err = stats.linregress(mylist2,mylist1)
print "Gradient and intercept:", gradient, intercept

n = len(mylist1)
residure = []
#print(n)
for i in xrange(0,941):
	x = float(mylist1[i] - gradient * mylist2[i] - intercept)
	residure.append(x)
print "Residue:", residure


