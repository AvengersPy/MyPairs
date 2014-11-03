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