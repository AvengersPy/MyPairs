from zipline.api import order, record, symbol
from onePair import *

def initialize(context):
    context.trainingSize = 252
    context.ptime = context.trainingSize - 1



def handle_data(context, data):
    # generate signal
    print context.tradingSize
    print context.ptime
    print data
    
    #switch signal:
    #    order(symbol('AAPL'), 10)
    #//record(AAPL=data[symbol('AAPL')].price)
