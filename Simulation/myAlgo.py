# import os
# os.chdir(r'/Users/yubing/MyPairs/Simulation /Users/yubing/MyPairs/Simulation')
from zipline.api import order, record, symbol,history,add_history
from onePair import *

#from pprint import pprint

def initialize(context):
    context.count = 252
    #context.stock = symbol('CSCO')
    
    context.stock = symbol('MMM')
    add_history(bar_count=252, frequency='1d', field='price')
    #spy_data = history(252, '1d', 'price')

    context.other_stocks = {'AAPL'}
    add_history(bar_count=252, frequency='1d', field='price')
    #etf_data = history(252, '1d', 'price')


    #context.ptime = context.trainingSize - 1
    #pprint(vars(context))



def handle_data(context, data):
    # generate signal
    # print regression(etf_history,stock_history,100)
    #price_history = history(bar_count=252, frequency='1d', field='price')
    #print price_history
    # PTime = 100
    spy_data, mmm_data = readCSV()
    PTime = 100
    beta0, beta1, epsilon = regression(spy_data, mmm_data, PTime)
    sscore = score(epsilon, PTime)
    print sscore
    #print context.count
    #print data['AAPL'].price
    #print context.ptime
    # print data
    
    #switch signal:
    #    order(symbol('AAPL'), 10)
    #//record(AAPL=data[symbol('AAPL')].price)
