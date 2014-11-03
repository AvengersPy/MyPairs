import numpy as np
import matplotlib.pyplot as plt 
from pylab import *

x = [1,2,3,4]
y = [3,5,7,10]

fit = polyfit(x,y,1)
fit_fn = poly1d(fit) # fit_fn is now a function which takes in x and returns an estimate for y

#plot(x,y, 'yo', x, fit_fn(x), '--k')
#xlim(0, 5)
#ylim(0, 12)
print(fit_fn)