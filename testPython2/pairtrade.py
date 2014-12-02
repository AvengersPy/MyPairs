#!/usr/bin/env python
#
# Copyright 2013 Quantopian, Inc.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
import statsmodels.api as sm
from datetime import datetime
import pytz
from scipy import stats
import csv

from zipline.algorithm import TradingAlgorithm
from zipline.transforms import batch_transform
from zipline.utils.factory import load_from_yahoo

from Equity import *
from sscore import *


@batch_transform
def ols_transform(data, sid1, sid2):
    """Computes regression coefficient (slope and intercept)
    via Ordinary Least Squares between two SIDs.
    """

    ## original one
    #p0 = data.price[sid1].values
    #p1 = sm.add_constant(data.price[sid2], prepend=True)
    #slope, intercept = sm.OLS(p0, p1).fit().params


    ##np polyfit
    #pStk = data.price[sid1].values
    #pEtf = data.price[sid2].values

    #para = np.polyfit(pStk, pEtf, 1)
    #slope = para[0]
    #intercept = para[1]

    ## calculate returns
    pStk = data.price[sid1].values
    pStkRtn = np.log(data.price[sid1].values[1:]) - np.log(data.price[sid1].values[0:-1])
    pEtf = data.price[sid2].values
    pEtfRtn = np.log(data.price[sid2].values[1:]) - np.log(data.price[sid2].values[0:-1])
    slope, intercept, r_value, p_value, std_err = stats.linregress(pEtfRtn, pStkRtn)
    
    # record the last prices for calculating returns!
    lastStkPrice = pStk[-1]
    lastEtfPrice = pEtf[-1]
    return slope, intercept, lastStkPrice, lastEtfPrice


class Pairtrade(TradingAlgorithm):
    """Pairtrading relies on cointegration of two stocks.

    The expectation is that once the two stocks drifted apart
    (i.e. there is spread), they will eventually revert again. Thus,
    if we short the upward drifting stock and long the downward
    drifting stock (in short, we buy the spread) once the spread
    widened we can sell the spread with profit once they converged
    again. A nice property of this algorithm is that we enter the
    market in a neutral position.

    This specific algorithm tries to exploit the cointegration of
    CSCO and Coca Cola by estimating the correlation between the
    two. Divergence of the spread is evaluated by z-scoring.
    """

    def initialize(self, stkSymbol, etfSymbol, window_length=50):
        self.spreads = []
        self.invested = 0
        self.window_length = window_length
        self.ols_transform = ols_transform(refresh_period=self.window_length, window_length=self.window_length)

        # create Equity objs
        self.stk = Equity(stkSymbol)
        self.etf = Equity(etfSymbol)

        # need to record the last prices for calculating returns!
        self.lastStkPrice = 0
        self.lastEtfPrice = 0

        # regression parameters
        self.beta = 0
        self.alpha = 0

        ## Stop Loss
        self.STKSHARE = 100     # const stk share
        self.STOPLOSS = -0.05     # stop if loss more than 10%
        self.initBP = 0         # Used buying power after open the position
        self.closeBP = 0
        self.unPNL = 0
        self.PNL = 0

        ## max holding time
        self.holdingTime = 0
        self.MAX_HOLDING = 40



    def handle_data(self, data):
        ######################################################
        # 1. Compute regression coefficients between CSCO and SPY
        params = self.ols_transform.handle_data(data, self.stk.symbol, self.etf.symbol)
        if params is None:
            return

        self.beta, self.alpha, self.lastStkPrice, self.lastEtfPrice = params

        ######################################################
        # 2. Compute spread and zscore
        #zscore = self.compute_zscore(data)
        zscore = self.compute_sscore(data)
        self.record(zscores=zscore)

        ######################################################
        # 3. Place orders
        self.place_orders(data, zscore)
        
        ######################################################
        # 4. Max Holding
        self.maxHoldingTime(data)

        ######################################################
        # 5. Stop Loss
        self.unRealizedPNL(data)


    def unRealizedPNL(self, data):
        if self.invested:
            self.stk.unRealizedPNL = (data[self.stk.symbol].price - self.stk.openPrice) * self.stk.share
            self.etf.unRealizedPNL = (data[self.etf.symbol].price - self.etf.openPrice) * self.etf.share
            self.unPNL = self.stk.unRealizedPNL + self.etf.unRealizedPNL

            if self.unPNL / self.initBP < self.STOPLOSS:
                self.sell_spread()      # close position
                print "{} {}, {} {}, BP {}".format(data[self.stk.symbol].datetime.date(), "Stop Loss", self.stk.symbol, self.etf.symbol, self.initBP)
                self.PNL += self.unPNL
                #print "realized PNL: {}".format(self.unPNL)

    def maxHoldingTime(self, data):
        """close pair position if hold too long
        """
        if self.invested:
            #print "holding: {}".format(self.holdingTime)
            self.holdingTime += 1
        if self.holdingTime > self.MAX_HOLDING:
            self.sell_spread();
            print "{} {}, {} {}, BP {}".format(data[self.stk.symbol].datetime.date(), "Exceeded Max Holding", self.stk.symbol, self.etf.symbol, self.initBP)
            self.PNL += self.unPNL
            #print "realized PNL: {}".format(self.unPNL)

    def compute_zscore(self, data):
        """1. Compute the spread given slope and intercept.
           2. zscore the spread.
        """
        lastStkRtn = np.log(data[self.stk.symbol].price/self.lastStkPrice)
        lastEtfRtn = np.log(data[self.etf.symbol].price/self.lastEtfPrice)

        spread = lastStkRtn - (self.beta * lastEtfRtn + self.alpha)
        self.spreads.append(spread)
        spread_wind = self.spreads[-self.window_length : ]      # the last several residuals
        zscore = (spread - np.mean(spread_wind)) / np.std(spread_wind)
        return zscore



    def compute_sscore(self, data):
        lastStkRtn = np.log(data[self.stk.symbol].price/self.lastStkPrice)
        lastEtfRtn = np.log(data[self.etf.symbol].price/self.lastEtfPrice)

        spread = lastStkRtn - (self.beta * lastEtfRtn + self.alpha)
        self.spreads.append(spread)
        spread_wind = self.spreads[-self.window_length : ]      # the last several residuals

        if (len(spread_wind) >= self.window_length):
            pass

        sscore = score(spread_wind, self.window_length)

        return sscore

    def place_orders(self, data, zscore):
        """Buy spread if zscore is > 2, sell if zscore < .5.
        """

        if zscore >= 2.0 and not self.invested:
            # zscore positive, stock overperformed, short stk
            self.stk.openPrice = data[self.stk.symbol].price
            self.stk.share = -self.STKSHARE
            self.etf.openPrice = data[self.etf.symbol].price
            self.etf.share = int(self.STKSHARE * self.beta * self.stk.openPrice / self.etf.openPrice)

            self.order(self.stk.symbol, self.stk.share)
            self.order(self.etf.symbol, self.etf.share)

            self.stk.initBP = self.stk.share * self.stk.openPrice
            self.etf.initBP = self.etf.share * self.etf.openPrice
            self.initBP = abs(self.stk.initBP) + abs(self.etf.initBP)
           
            print "{} {}, {} {}, BP {}".format(data[self.stk.symbol].datetime.date(), "Long ETF Opened", self.stk.symbol, self.etf.symbol, self.initBP)

            self.invested = True

        elif zscore <= -2.0 and not self.invested:
            # zscore positive, etf overperformed, short etf
            self.stk.openPrice = data[self.stk.symbol].price
            self.stk.share = self.STKSHARE
            self.etf.openPrice = data[self.etf.symbol].price
            self.etf.share = -int(self.STKSHARE * self.beta * self.stk.openPrice / self.etf.openPrice)

            self.order(self.stk.symbol, self.stk.share)
            self.order(self.etf.symbol, self.etf.share)

            self.stk.initBP = self.stk.share * self.stk.openPrice
            self.etf.initBP = self.etf.share * self.etf.openPrice
            self.initBP = abs(self.stk.initBP) + abs(self.etf.initBP)

            print "{} {}, {} {}, BP {}".format(data[self.stk.symbol].datetime.date(), "Long STK Opened", self.stk.symbol, self.etf.symbol, self.initBP)

            self.invested = True
        elif abs(zscore) < .5 and self.invested:
            self.sell_spread()      # close position
            print "{} {}, {} {}, BP {}".format(data[self.stk.symbol].datetime.date(), "Closed", self.stk.symbol, self.etf.symbol, self.initBP)
            self.PNL += self.unPNL
            #print "realized PNL: {}".format(self.unPNL)

    def sell_spread(self):
        """
        decrease exposure, regardless of position long/short.
        buy for a short position, sell for a long.
        """
        etf_amount  = self.portfolio.positions[self.etf.symbol].amount
        self.order(self.etf.symbol, -1 * etf_amount)
        stk_amount = self.portfolio.positions[self.stk.symbol].amount
        self.order(self.stk.symbol, -1 * stk_amount)
        self.initBP = 0         # set buying power to 0
        self.invested = False
        self.holdingTime = 0
        

if __name__ == '__main__':

    start = datetime(2011, 1, 1, 0, 0, 0, 0, pytz.utc)
    end = datetime(2012, 1, 1, 0, 0, 0, 0, pytz.utc)

    sym = list(pd.read_csv('dia.csv')['Symbol'])

    stksymbol = 'AA'
    etfsymbol = 'DIA'

    #errSymbol = []
    #portfResult = {}
    #for i in range(2):
    #    stksymbol = sym[i]

    #    try:
    #        data = load_from_yahoo(stocks=[stksymbol, etfsymbol], indexes={}, start=start, end=end)
    #    except IOError as e:
    #        print "Cannot get {} from yahoo".format(stksymbol)
    #        errSymbol.append(stksymbol)
    #        continue

    #    pairtrade = Pairtrade(stksymbol, etfsymbol)
    #    results = pairtrade.run(data)
    #    data['spreads'] = np.nan
    #    portfResult[stksymbol] = results.portfolio_value[-1] - results.portfolio_value[0]

    #print errSymbol

    #with open('dict.csv', 'w') as outfile:
    #    writer = csv.writer(outfile)
    #    for key, value in portfResult.items():
    #        writer.writerow([key, value])
            

    ############## test one symbol ##########################
    data = load_from_yahoo(stocks=[stksymbol, etfsymbol], indexes={}, start=start, end=end)

    pairtrade = Pairtrade(stksymbol, etfsymbol)
    results = pairtrade.run(data)
    data['spreads'] = np.nan

    print results.portfolio_value
    print results.portfolio_value[-1] - results.portfolio_value[0]

    ax1 = plt.subplot(311)
    data[[stksymbol, etfsymbol]].plot(ax=ax1)
    plt.ylabel('price')
    plt.setp(ax1.get_xticklabels(), visible=False)

    ax2 = plt.subplot(312, sharex=ax1)
    results.zscores.plot(ax=ax2, color='r')
    plt.ylabel('zscored spread')

    ax3 = plt.subplot(313, sharex = ax1)
    results.portfolio_value.plot(ax=ax3)
    plt.ylabel('portfolio value')
    plt.gcf().set_size_inches(18, 8)
    plt.show()


    

