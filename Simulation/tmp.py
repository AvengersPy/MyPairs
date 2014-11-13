import os
os.chdir(r"C:\Users\zhe\Documents\GitHub\MyPairs\Simulation\data")

from regression_overload import *

x, y = readCSV()
print x
print y

beta0, beta1, epsilon = regression(x, y)
print beta0
print beta1
print epsilon