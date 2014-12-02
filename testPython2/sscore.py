from scipy.stats import mstats
from scipy import stats
from numpy.lib.scimath import logn
from math import e
import numpy as np
from numpy import array, zeros, sqrt, shape

def score(Epsilon, ptime):
	
    if ptime > len(Epsilon):
        return 0

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

    for j in range(0, ptime - 1):
        lam.append(Xt_Lag0[j] - a - b * Xt_Lag1[j])

    lamvar = np.var(lam)

    sigmaeq = sqrt(lamvar*2*k/(1-b**2))


    s = []
    # compute a list of past sscore
    for i in range(0, ptime):
        s.append((Epsilon[i]-m)/sigmaeq)

    #instantaneous sscore
    inst_s = ((Epsilon[-1]-m)/sigmaeq - np.mean(s))/np.std(s)
    
    smod = inst_s - a/k*sigmaeq

    #if abs(inst_s)>100:
    #    pass
    #    return 0

    return s