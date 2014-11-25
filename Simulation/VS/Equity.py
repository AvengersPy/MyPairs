class Equity(object):
    """Stock or ETF"""
    def __init__(self, Symbol):
        if type(Symbol) is not str:
            raise Exception("Mast have a symbol with type STRING for an Equity!")

        self.symbol = Symbol
        self.openPrice = 0
        self.closePrice = 0
        self.share = 0
        self.initBP = 0
        self.unRealizedPNL = 0