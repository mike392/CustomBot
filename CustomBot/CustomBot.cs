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
        private Dictionary<TimeFrame, int> dict;
        private int multiplier;
        private int bars_precision = 4;
        private double border_precision = 0.1;
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
            //dict = new Dictionary<TimeFrame, int>();
            //initDictionary();
            //multiplier = dict[this.TimeFrame];
            //ind = Indicators.GetIndicator<ElliotOscillator>(Source, FastPeriod, SlowPeriod);
            ao = Indicators.AwesomeOscillator();
            laguerre = Indicators.GetIndicator<Laguerre>();

            lag1 = Indicators.GetIndicator<Laguerre_RSI>(FastGamma);
            lag2 = Indicators.GetIndicator<Laguerre_RSI>(SlowGamma);

        }

        protected override void OnBar()
        {
            // Put your core logic here
            Print("Current 0.5 laguerre value " + lag1.laguerrersi.LastValue);
            Print("Current 0.9 laguerre value " + lag2.laguerrersi.LastValue);
            Print("Current laguerre value " + laguerre.Result.LastValue);
            //if (IsDecreasing(ao, bars_precision) && ao.Result.LastValue > ao.Result.Last(1))
            //{
            //    if (ao.Result.IsRising() && dirInd == FallingRising.Falling)
            //    {
                   
            //if (IsIncreasing(laguerre, bars_precision) && Math.Abs(laguerre.Result.LastValue - 0.15) < border_precision)
            //        {
            //            Print("Open buy position");
            //             Print("Laguerre value = " + laguerre.Result.LastValue);
            //            Print("Is laguerre increasing " + IsIncreasing(laguerre, bars_precision));

            //            //Print("Is AO Rising " + ao.Result.IsRising());
            //            //Print("Previous indicator is red " + (dirInd == FallingRising.Falling));
            //            //Print("Increses Slow Lagguerre " + IsIncreasing(lag2, precision));
            //            //Print("Value of fast Lagguerre = " + lag1.laguerrersi.LastValue);
            //            //Print("Slow Laguerre values " + lag2.laguerrersi.Last(5) + " " + lag2.laguerrersi.Last(4) + " " + lag2.laguerrersi.Last(3) + " " + lag2.laguerrersi.Last(2) + " " + lag2.laguerrersi.Last(1) + " " + lag2.laguerrersi.LastValue);
            //            //Print("Lag1 over bought value " + lag1.overbought);
            //            //Print("Lag1 over sold value " + lag1.oversold);
            //            //Print("Lag2 over bought value " + lag2.overbought);
            //            //Print("Lag2 over sold value " + lag2.oversold);
            //        }
            //    }
            //}

            //if (IsIncreasing(ao, bars_precision) && ao.Result.LastValue < ao.Result.Last(1))
            //{
            //    if (ao.Result.IsFalling() && dirInd == FallingRising.Rising)
            //    {
                    //if (IsDecreasing(laguerre, bars_precision) && Math.Abs(laguerre.Result.LastValue - 0.75) < border_precision)
                    //{
                    //    Print("Open sell position");
                    //    Print("Laguerre value = " + laguerre.Result.LastValue);
                    //    Print("Is laguerre decreasing " + IsDecreasing(laguerre, bars_precision));

                    //    //Print("Decreases Slow Lagguerre " + IsDecreasing(lag2, precision));
                    //    //Print("Value of fast Lagguerre = " + lag1.laguerrersi.LastValue);
                    //    //Print("Slow Laguerre values " + lag2.laguerrersi.Last(5) + " " + lag2.laguerrersi.Last(4) + " " + lag2.laguerrersi.Last(3) + " " + lag2.laguerrersi.Last(2) + " " + lag2.laguerrersi.Last(1) + " " + lag2.laguerrersi.LastValue);
                    //}
            //    }
            //}


            if (ao.Result.IsRising())
            {
                dirInd = FallingRising.Rising;
            }
            else if (ao.Result.IsFalling())
            {
                dirInd = FallingRising.Falling;
            }
            else
            {
                dirInd = FallingRising.None;
            }

            //Print("previous 3th value " + ao.Result.Last(3));
            //Print("previous 2nd value " + ao.Result.Last(2));
            //Print("previous value " + ao.Result.Last(1));
            //Print("current value " + ao.Result.LastValue);
            //if (ao.Result.IsFalling())
            //{
            //    Print("Trend falling");
            //}
            //else if (ao.Result.IsRising())
            //{
            //    Print("Trend rising");
            //}
            //else
            //{
            //    Print("Unknown direction");
            //}
            //Print(ao.Result);


            //Print("Jaws value " + all.Jaws.LastValue);
            //Print("Teeth value " + all.Teeth.LastValue);
            //Print("Lips value " + all.Lips.LastValue);

            //Print("Tick # " + count);
            //count = count + 1;
            //Print("Is DownTrend " + double.IsNaN(ind.DownTrend.LastValue));
            //Print("Elliot Downtrend Last Value " + Math.Round(ind.DownTrend.LastValue, 8));
            //Print("Is UpTrend " + double.IsNaN(ind.UpTrend.LastValue));
            //Print("Elliot Uptrend Last Value " + Math.Round(ind.UpTrend.LastValue, 8));
            //Print("Is Neutral " + double.IsNaN(ind.Neutral.LastValue));
            //Print("Elliot Neutral Last Value " + Math.Round(ind.Neutral.LastValue, 8));
            //if (double.IsNaN(ind.DownTrend.LastValue) && double.IsNaN(ind.UpTrend.LastValue))
            //{
            //    Print("Trend is neutral");

            //}
            //else
            //{
            //    if (double.IsNaN(ind.DownTrend.LastValue) && double.IsNaN(ind.Neutral.LastValue))
            //    {
            //        Print("Trend is Upgoing");
            //    }
            //    else
            //    {
            //        Print("Trnd is downgoing");
            //    }
            //}

            //deviation = Math.Abs(all.Jaws.LastValue - all.Lips.LastValue);
            //Print("Jaws/Lips deviation is " + Math.Round((double)deviation, 8));

        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
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
