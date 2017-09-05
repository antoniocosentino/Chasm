///-----------------------------------------------------------------
///   Class:            Chasm BOT
///   Description:      cAlgo trading bot
///   Author:           Antonio Cosentino
///   Version:          1.0
///   Updated:          05/09/2017
///-----------------------------------------------------------------

using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using System.Collections.Generic;


namespace cAlgo.indicators
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Chasm : Robot
    {
        string version_number = "1.0";
        // Declarations
        private int currenthour;
        private int currentminute;
        private string stringdate;

        private bool is_position_open;
        private string scenario;

        private double today_open;
        private double today_high;
        private double today_low;
        private double bid_price;
        private double ask_price;
        private double yesterday_close;
        private double yesterday_high;
        private double yesterday_low;
        private int kindex;
        ///

        [Parameter("LONG Trailing Stop (%)", DefaultValue = 8, MinValue = 1, MaxValue = 50)]
        public double long_stop { get; set; }

        [Parameter("SHORT Trailing Stop (%)", DefaultValue = 4, MinValue = 1, MaxValue = 50)]
        public double short_stop { get; set; }

        [Parameter("Diff Ticks", DefaultValue = 2, MinValue = 1, MaxValue = 1000)]
        public double diffticks { get; set; }

        [Parameter("GAP Up Long?", DefaultValue = true)]
        public bool gap_up_mode { get; set; }

        [Parameter("GAP Down Long?", DefaultValue = false)]
        public bool gap_down_mode { get; set; }

        [Parameter("N. Contracts", DefaultValue = 100, MinValue = 1, MaxValue = 100000)]
        public int ncontracts { get; set; }

        protected override void OnStart()
        {
            is_position_open = false;
            kindex = 0;
            Positions.Closed += PositionsOnClosed;
            Print("Chasm {0} Started", version_number);
            Print("Server time is {0}", Server.Time.AddHours(0));
        }

        protected override void OnBar()
        {
            Print("");
            kindex = MarketSeries.Close.Count - 1;
            today_open = MarketSeries.Open[kindex];
            yesterday_close = MarketSeries.Close[kindex - 1];
            yesterday_high = MarketSeries.High[kindex - 1];
            yesterday_low = MarketSeries.Low[kindex - 1];

            if (today_open > yesterday_close)
            {
                scenario = "GAPUP";
            }
            else if (today_open < yesterday_close)
            {
                scenario = "GAPDOWN";
            }
            else
            {
                scenario = null;
            }

            if (scenario != null)
            {
                Print("Scenario is: {0}", scenario);
                Timer.Start(3600);
            }
        }

        protected override void OnTimer()
        {
            Timer.Stop();
            today_high = MarketSeries.High[kindex];
            today_low = MarketSeries.Low[kindex];

            if (!is_position_open)
            {
                if (scenario == "GAPUP" && gap_up_mode)
                {
                    double stoploss_price = ask_price - (ask_price * (long_stop / 100));
                    double stoploss_pips = (ask_price - stoploss_price) / Symbol.PipSize;
                    double takeprofit_pips = ((today_high / Symbol.PipSize) + diffticks) - (ask_price / Symbol.PipSize);
                    double takeprofit_price = ask_price + (takeprofit_pips * Symbol.PipSize);
                    Print("Buy at {0} with {1} takeprofit and {2} stop loss", ask_price, takeprofit_price, stoploss_price);
                    ExecuteMarketOrder(TradeType.Buy, Symbol, ncontracts, "Chasm", stoploss_pips, takeprofit_pips);
                    is_position_open = true;
                }
                else if (scenario == "GAPUP" && !gap_up_mode)
                {
                    double stoploss_price = bid_price + (bid_price * (short_stop / 100));
                    double stoploss_pips = (stoploss_price - bid_price) / Symbol.PipSize;
                    double takeprofit_pips = (bid_price / Symbol.PipSize) - ((today_low / Symbol.PipSize) - diffticks);
                    double takeprofit_price = bid_price - (takeprofit_pips * Symbol.PipSize);
                    if (takeprofit_price < bid_price)
                    {
                        Print("Sell at {0} with {1} takeprofit and {2} stop loss", ask_price, takeprofit_price, stoploss_price);
                        ExecuteMarketOrder(TradeType.Sell, Symbol, ncontracts, "Chasm", stoploss_pips, takeprofit_pips);
                        is_position_open = true;
                    }
                }
                else if (scenario == "GAPDOWN" && gap_down_mode)
                {
                    double stoploss_price = ask_price - (ask_price * (long_stop / 100));
                    double stoploss_pips = (ask_price - stoploss_price) / Symbol.PipSize;
                    //double takeprofit_pips = ((yesterday_low / Symbol.PipSize) + diffticks) - (ask_price / Symbol.PipSize); // trying instead with today low
                    double takeprofit_pips = ((today_low / Symbol.PipSize) + diffticks) - (ask_price / Symbol.PipSize);
                    double takeprofit_price = ask_price + (takeprofit_pips * Symbol.PipSize);
                    if (takeprofit_price > ask_price)
                    {
                        Print("Buy at {0} with {1} takeprofit and {2} stop loss", ask_price, takeprofit_price, stoploss_price);
                        ExecuteMarketOrder(TradeType.Buy, Symbol, ncontracts, "Chasm", stoploss_pips, takeprofit_pips);
                        is_position_open = true;
                    }
                }
                else if (scenario == "GAPDOWN" && !gap_down_mode)
                {
                    double stoploss_price = bid_price + (bid_price * (short_stop / 100));
                    double stoploss_pips = (stoploss_price - bid_price) / Symbol.PipSize;
                    double takeprofit_pips = (bid_price / Symbol.PipSize) - ((today_low / Symbol.PipSize) - diffticks);
                    double takeprofit_price = bid_price - (takeprofit_pips * Symbol.PipSize);
                    Print("Sell at {0} with {1} takeprofit and {2} stop loss", ask_price, takeprofit_price, stoploss_price);
                    ExecuteMarketOrder(TradeType.Sell, Symbol, ncontracts, "Chasm", stoploss_pips, takeprofit_pips);
                    is_position_open = true;

                }
            }
        }

        protected override void OnTick()
        {
            // Time Vars
            stringdate = Server.Time.AddHours(0).ToString("HH:mm");
            currenthour = int.Parse(stringdate.Substring(0, 2));
            currenthour = Convert.ToInt32(stringdate.Substring(0, 2));
            currentminute = int.Parse(stringdate.Substring(3, 2));
            currentminute = Convert.ToInt32(stringdate.Substring(3, 2));
            ///
            bid_price = Symbol.Bid;
            ask_price = Symbol.Ask;

        }

        private void PositionsOnClosed(PositionClosedEventArgs args)
        {
            var pos = args.Position;
            Print("Position closed with €{0} profit", pos.GrossProfit);
            is_position_open = false;
        }

        protected override void OnStop()
        {
            Print("Chasm Stopped");
        }
    }
}
