///-----------------------------------------------------------------
///   Class:            Chasm BOT
///   Description:      cAlgo trading bot
///   Author:           Antonio Cosentino
///   Version:          1.0
///   Updated:          13/08/2017
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
        private bool is_position_open;
        private double init_close_value;
        private int currenthour;
        private int currentminute;
        private string stringdate;
        private int previous_dayotw;
        private bool day_enable = false;
        private double yesterday_high;
        private double yesterday_low;
        private double today_high;
        private double today_low;
        private double bid_price;
        private double today_open_price;
        private bool daily_check_done;
        private string scenario;
        private int kindex;
        ///

        [Parameter("Start time (hh)", DefaultValue = 9, MinValue = 1, MaxValue = 24)]
        public int start_time_hh { get; set; }

        [Parameter("Start time (mm)", DefaultValue = 0, MinValue = 0, MaxValue = 59)]
        public int start_time_mm { get; set; }

        [Parameter("LONG Trailing Stop (%)", DefaultValue = 8, MinValue = 1, MaxValue = 50)]
        public int long_stop { get; set; }

        [Parameter("SHORT Trailing Stop (%)", DefaultValue = 4, MinValue = 1, MaxValue = 50)]
        public int short_stop { get; set; }

        [Parameter("GAP Up Long?", DefaultValue = true)]
        public bool gap_up_mode { get; set; }

        [Parameter("GAP Down Long?", DefaultValue = true)]
        public bool gap_down_mode { get; set; }

        protected override void OnStart()
        {
            init_close_value = MarketSeries.Close.LastValue;
            previous_dayotw = -1;
            is_position_open = false;
            daily_check_done = false;

            Print("Chasm {0} Started", version_number);
            Print("Server time is {0}", Server.Time.AddHours(2));
        }

        protected override void OnBar()
        {
            if (previous_dayotw == -1 || (int)Server.Time.DayOfWeek != previous_dayotw)
            {
                // Day has changed
                previous_dayotw = (int)Server.Time.DayOfWeek;
                if (previous_dayotw != 6 && previous_dayotw != 0)
                {
                    day_enable = true;
                    daily_check_done = false;
                }
            }
        }

        protected override void OnTick()
        {
            // Time Vars
            stringdate = Server.Time.AddHours(2).ToString("HH:mm");
            currenthour = int.Parse(stringdate.Substring(0, 2));
            currenthour = Convert.ToInt32(stringdate.Substring(0, 2));
            currentminute = int.Parse(stringdate.Substring(3, 2));
            currentminute = Convert.ToInt32(stringdate.Substring(3, 2));
            ///

            bid_price = Symbol.Bid;

            if (day_enable)
            {
                if ((currenthour >= start_time_hh && currentminute >= start_time_mm) && daily_check_done == false)
                {
                    kindex = MarketSeries.Close.Count - 1;
                    today_open_price = MarketSeries.Open[kindex];
                    yesterday_high = MarketSeries.High[kindex - 1];
                    yesterday_low = MarketSeries.Low[kindex - 1];

                    if (today_open_price > yesterday_high)
                    {
                        scenario = "FULL_GAP_UP";
                    }
                    else if (today_open_price < yesterday_low)
                    {
                        scenario = "FULL_GAP_DOWN";
                    }
                    else
                    {
                        scenario = null;
                    }
                    Print(scenario);
                    Print("Today open price is {0}, yesterday high is {1}, yesterday low is {2}", today_open_price, yesterday_high, yesterday_low);
                    daily_check_done = true;
                }
            }








        }


        protected override void OnStop()
        {
            Print("Chasm Stopped");
        }
    }
}
