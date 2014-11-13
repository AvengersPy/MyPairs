import os
import csv
import pandas as pd
import numpy as np

os.chdir(r'/Users/yubing/MyPairs/Simulation/data') # set to your own working directory

data1 = pd.read_csv('1.csv')# pandas dataframe
print data1[:4]				# print first a few rows
print data1['Last']			# print one col

mylist1 = data1['Last']		# convert to list, you now calculate return and do the regression
print "mean: ", np.mean(mylist1)


# have a good day :)
