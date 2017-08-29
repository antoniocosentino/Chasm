///-----------------------------------------------------------------
///   Class:            Chasm BOT
///   Description:      cAlgo trading bot
///   Author:           Antonio Cosentino
///   Version:          1.0
///   Updated:          29/08/2017
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
        public int long_stop { get; set; }

        [Parameter("SHORT Trailing Stop (%)", DefaultValue = 4, MinValue = 1, MaxValue = 50)]
        public int short_stop { get; set; }

        [Parameter("Diff Ticks", DefaultValue = 1, MinValue = 1E-07, MaxValue = 1000)]
        public double diffticks { get; set; }

        [Parameter("GAP Up Long?", DefaultValue = true)]
        public bool gap_up_mode { get; set; }

        [Parameter("GAP Down Long?", DefaultValue = true)]
        public bool gap_down_mode { get; set; }

        protected override void OnStart()
        {
            is_position_open = false;
            kindex = 0;

            Print("Chasm {0} Started", version_number);
            Print("Server time is {0}", Server.Time.AddHours(0));
        }

        protected override void OnBar()
        {
            kindex = MarketSeries.Close.Count - 1;
            today_open = MarketSeries.Open[kindex];
            yesterday_close = MarketSeries.Close[kindex - 1];
            yesterday_high = MarketSeries.High[kindex - 1];
            yesterday_low = MarketSeries.Low[kindex - 1];


            Print("Open price is: {0}", today_open);
            Print("Yesterday Close is: {0}", yesterday_close);
            Print("Yesterday High is: {0}", yesterday_high);
            Print("Yesterday Low is: {0}", yesterday_low);


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

            Print("Today High is: {0}", today_high);
            Print("Today Low is: {0}", today_low);

            if (scenario == "GAPUP" && gap_up_mode)
            {
                Print("Buy and close at {0}", today_high + diffticks);
            }
            else if (scenario == "GAPUP" && !gap_up_mode)
            {
                Print("Sell and close at {0}", today_low - diffticks);
            }
            else if (scenario == "GAPDOWN" && gap_down_mode)
            {
                Print("Buy and close at {0}", yesterday_low + diffticks);
            }
            else if (scenario == "GAPDOWN" && !gap_down_mode)
            {
                Print("Sell and close at {0}", today_low - diffticks);
            }


            Print("- - -");
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


        protected override void OnStop()
        {
            Print("Chasm Stopped");
        }
    }
}
