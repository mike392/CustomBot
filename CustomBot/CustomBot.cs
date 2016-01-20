using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using System.Collections.Generic;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class CustomBot : Robot
    {
        [Parameter(DefaultValue = 0.0)]
        public double Parameter { get; set; }
        private int count = 1;
        private double deviation = 0;

        private Laguerre_RSI lag1;
        private Laguerre_RSI lag2;
        private Laguerre laguerre;
        private AwesomeOscillator ao;
        private SampleAlligator all;
        private WellesWilderSmoothing wws;
        private SimpleMovingAverage sma;
        private ExponentialMovingAverage ema;
        private CommodityChannelIndex cci_13, cci_3;
        private Dictionary<TimeFrame, int> dict;

        private int multiplier;
        private int bars_precision = 4;
        private double border_precision = 0.1;
        private int tick_count = 0;
        private int rounding_factor = 4;
        private string crossing_msg = "";
        private List<double> cci3_values;
        private bool cci13_is_above_100 = false;
        private bool cci13_is_below_minus100 = false;
        private string filter_flag = "";
        private string ema_lastvalues = "";
        private string sma_lastvalues = "";

        private enum CrossingEnum
        {
            None,
            EmaHasCrossedSmaFromAbove,
            EmaHasCrossedSmaFromBelow
        }
        private CrossingEnum crossing_index;
        private enum FallingRising
        {
            None,
            Rising,
            Falling
        }
        private FallingRising dirInd;

        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("FastPeriod", DefaultValue = 5)]
        public int FastPeriod { get; set; }

        [Parameter("SlowPeriod", DefaultValue = 34)]
        public int SlowPeriod { get; set; }
        [Parameter("SlowGamma", DefaultValue = 0.9)]
        public double SlowGamma { get; set; }
        [Parameter("FastGamma", DefaultValue = 0.5)]
        public double FastGamma { get; set; }
        protected override void OnStart()
        {
            // Put your initialization logic here
            Print("Started");
            crossing_index = CrossingEnum.None;
            sma = Indicators.SimpleMovingAverage(Source, 100);
            ema = Indicators.ExponentialMovingAverage(Source, 16);

            cci_13 = Indicators.CommodityChannelIndex(MarketSeries, 13);
            cci_3 = Indicators.CommodityChannelIndex(MarketSeries, 3);
            cci3_values = new List<double> 
            {
                (0.0),
                (0.0),
                (0.0),
                (0.0),
                (0.0)
            };
        }

        protected override void OnBar()
        {
            // Put your core logic here
            //Print("CCI 3 current value " + cci_3.Result.LastValue + " Previous value " + cci_3.Result.Last(1));

            //if ((cci_3.Result.LastValue >= 100 && cci_3.Result.Last(2) <= -100) || (cci_3.Result.LastValue >= 100 && cci_3.Result.Last(1) <= -100))
            //{
            //    Print("CCI 3 raised from -100 to 100");
            //    Print("CCI values " + cci_3.Result.LastValue + " " + cci_3.Result.Last(1) + " " + cci_3.Result.Last(2));
            //}
            //if ((cci_3.Result.LastValue <= -100 && cci_3.Result.Last(2) >= 100) || (cci_3.Result.LastValue <= -100 && cci_3.Result.Last(1) >= 100))
            //{
            //    Print("CCI 3 dropped from 100 to -100");
            //    Print("CCI values " + cci_3.Result.LastValue + " " + cci_3.Result.Last(1) + " " + cci_3.Result.Last(2));
            //}

            //ema sma crossing test
            #region CrossingTest
            //if (crossing_index == CrossingEnum.EmaHasCrossedSmaFromAbove && cci13_is_above_100 == true)
            //{
            //    Print("Should sell");
            //}
            //if (crossing_index == CrossingEnum.EmaHasCrossedSmaFromBelow && cci13_is_below_minus100 == true)
            //{
            //    Print("Should buy");
            //}
            #endregion
            #region LevelCrossing
            //if (crossing_cci13_msg != "")
            //{
            //    if (cci_filter_flag != "")
            //    {
            //        cci_filter_flag = "";
            //    }
            //    else
            //    {
            //        Print(crossing_cci13_msg);
            //        Print("CCI 13 values = " + cci13_lastvalues);
            //        crossing_cci13_msg = "";
            //    }
            //}
            //cci_filter_flag = "";

            #endregion




        }
        protected override void OnTick()
        {
            base.OnTick();

            //checking if ema crosses sma from above or from below

            if (CrossedBelow(ema.Result, sma.Result))
            {
                crossing_msg = "ema crossed sma from below";
                //ema_lastvalues = ema.Result.LastValue + " " + ema.Result.Last(1) + " " + ema.Result.Last(2) + " " + ema.Result.Last(3);
                //sma_lastvalues = sma.Result.LastValue + " " + sma.Result.Last(1) + " " + sma.Result.Last(2) + " " + sma.Result.Last(3);
                crossing_index = CrossingEnum.EmaHasCrossedSmaFromBelow;
            }
            if (CrossedAbove(ema.Result, sma.Result))
            {
                crossing_msg = "ema crossed sma from above";
                crossing_index = CrossingEnum.EmaHasCrossedSmaFromAbove;
            }

            //checking for CCI 13 to be above 100 or below -100

            //if (cci_13.Result.LastValue > 100 && cci13_is_above_100 == false)
            //{
            //    //Print("CCI gets above 100");
            //    cci13_is_above_100 = true;
            //}
            //if (cci_13.Result.LastValue < 100 && cci13_is_above_100 == true)
            //{
            //    //Print("CCI left region above 100");
            //    cci13_is_above_100 = false;
            //}
            //if (cci_13.Result.LastValue < -100 && cci13_is_below_minus100 == false)
            //{
            //    //Print("CCI gets below minus 100");
            //    cci13_is_below_minus100 = true;
            //}
            //if (cci_13.Result.LastValue > -100 && cci13_is_below_minus100 == true)
            //{
            //    //Print("CCI left region below minus 100");
            //    cci13_is_below_minus100 = false;
            //}

            //checking whether CCI 3 has hit from -100 to 100 and vice versa
            cci3_values.Insert(0, cci_3.Result.LastValue);
            cci3_values.RemoveAt(cci3_values.Count - 1);
            //foreach (double value in cci3_values)
            //{
            //    Print(value + " #" + cci3_values.IndexOf(value));
            //}

            if ((cci3_values[0] > 100 && cci3_values[1] < -100) || (cci3_values[0] > 100 && cci3_values[2] < -100))
            {
                Print("CCI 3 hit up from -100 to 100");
            }
            else if ((cci3_values[0] < -100 && cci3_values[1] > 100) || (cci3_values[0] < -100 && cci3_values[2] > 100))
            {
                Print("CCI 3 hit down from 100 to -100");
            }



        }
        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
        #region CrossedBelow
        private bool CrossedBelow(DataSeries crossing, DataSeries crossed)
        {
            bool result = false;
            if (Functions.HasCrossedAbove(crossing, crossed, 0))
            {
                result = true;
                filter_flag = "something has recently crossed something";
            }

            return result;
        }
        //private bool CrossedBelow(DataSeries crossing, double value)
        //{
        //    bool result = false;
        //    if (crossing.Last(0) > value && crossing.Last(1) < value)
        //    {
        //        result = true;
        //        cci_filter_flag = "something has recently crossed something";
        //    }

        //    return result;
        //}
        #endregion
        #region CrossedAbove
        private bool CrossedAbove(DataSeries crossing, DataSeries crossed)
        {
            bool result = false;
            if (Functions.HasCrossedBelow(crossing, crossed, 0))
            {
                result = true;
                filter_flag = "something has recently crossed something";
            }

            return result;
        }
        //private bool CrossedAbove(DataSeries crossing, double value)
        //{
        //    bool result = false;
        //    if (crossing.Last(0) < value && crossing.Last(1) > value)
        //    {
        //        result = true;
        //        cci_filter_flag = "something has recently crossed something";
        //    }

        //    return result;
        //}
        #endregion
        #region DecreasingIncreasing
        private bool IsDecreasing(AwesomeOscillator indicator, int iterationNumber)
        {
            bool result = false;
            for (int i = 1; i <= iterationNumber; i++)
            {
                if (indicator.Result.Last(i) < indicator.Result.Last(i + 1))
                {
                    result = true;
                }
                else
                {
                    result = false;
                    break;
                }
            }

            return result;

        }
        private bool IsDecreasing(Laguerre indicator, int iterationNumber)
        {
            bool result = false;
            for (int i = 1; i <= iterationNumber; i++)
            {
                if (indicator.Result.Last(i) < indicator.Result.Last(i + 1))
                {
                    result = true;
                }
                else
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
        private bool IsIncreasing(AwesomeOscillator indicator, int iterationNumber)
        {
            bool result = false;
            for (int i = 1; i <= iterationNumber; i++)
            {
                if (indicator.Result.Last(i) > indicator.Result.Last(i + 1))
                {
                    result = true;
                }
                else
                {
                    result = false;
                    break;
                }
            }

            return result;

        }
        private bool IsIncreasing(Laguerre indicator, int iterationNumber)
        {
            bool result = false;
            for (int i = 1; i <= iterationNumber; i++)
            {
                if (indicator.Result.Last(i) < indicator.Result.Last(i + 1))
                {
                    result = true;
                }
                else
                {
                    result = false;
                    break;
                }
            }

            return result;

        }
        #endregion
        private void initDictionary()
        {
            dict.Add(TimeFrame.Daily, 1440);
            dict.Add(TimeFrame.Hour, 60);
            dict.Add(TimeFrame.Hour2, 120);
            dict.Add(TimeFrame.Minute, 1);
            dict.Add(TimeFrame.Minute10, 10);
            dict.Add(TimeFrame.Minute15, 15);
            dict.Add(TimeFrame.Minute20, 20);
            dict.Add(TimeFrame.Minute30, 30);
            dict.Add(TimeFrame.Minute45, 45);
            dict.Add(TimeFrame.Minute5, 5);
        }
    }


}
