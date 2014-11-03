import pandas as pd
import numpy as np

COLUMN_SEPARATOR = ','
mmm_data = pd.DataFrame.from_csv('data/mmm.csv', sep=COLUMN_SEPARATOR, header=None)

#AREA_INDEX = 4
#SELLING_PRICE_INDEX = 13
#x = housing_data[AREA_INDEX]
#y = housing_data[SELLING_PRICE_INDEX]

#regression = np.polyfit(x, y, 1)
#print(mmm_data)
#print(type(mmm_data))#<class 'pandas.core.frame.DataFrame'>
print(mmm_data[])
