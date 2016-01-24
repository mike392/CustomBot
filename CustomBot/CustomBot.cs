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

        private Laguerre_RSI laguerre;
        private AwesomeOscillator ao;
        private SampleAlligator all;
        private WellesWilderSmoothing wws;
        private SimpleMovingAverage sma;
        private ElliotOscillator eo;
        private ExponentialMovingAverage ema;
        private CommodityChannelIndex cci_13, cci_3;
        private Dictionary<TimeFrame, int> dict;
        private Dictionary<string, List<double>> positions_values;
        private double border_precision = 0.1;
        private int position_counter = 0;
        private int rounding_factor = 4;
        private string crossing_msg = "";
        private List<double> cci3_values;
        private List<double> cci13_values;
        private List<double> ema_values;
        private List<double> sma_values;
        private bool cci13_is_above_100 = false;
        private bool cci13_is_below_minus100 = false;
        private string filter_flag = "";
        private string ema_lastvalues = "";
        private string sma_lastvalues = "";
        private string emasma_state = "";
        private string cci13_state = "";
        private string cci3_state = "";
        private enum EmaSmaCrossingEnum
        {
            None,
            EmaHasCrossedSmaFromAbove,
            EmaHasCrossedSmaFromBelow
        }
        private enum CCI13States
        {
            None,
            EnteredPlus100,
            LeftPlus100,
            EnteredMinus100,
            LeftMinus100
        }
        private enum CCI3States
        {
            None,
            HitUp100,
            Left100,
            HitUp100AfterMinus100,
            DropToMinus100,
            LeftMinus100,
            DropToMinus100After100
        }
        private CCI13States cci13_index;
        private CCI3States cci3_index;
        private EmaSmaCrossingEnum emasma_crossing_index;
        private enum FallingRising
        {
            None,
            Rising,
            Falling
        }
        double[] input = 
        {
            (0.0),
            (0.0),
            (0.0),
            (0.0),
            (0.0)
        };
        private FallingRising dirInd;

        [Parameter("Source")]
        public DataSeries Source { get; set; }
        [Parameter("Position volume", DefaultValue = 100000)]
        public int PositionVolume { get; set; }
        [Parameter("Stop Order Magnitude", DefaultValue = 5)]
        public int StopOrderMagnitude { get; set; }
        [Parameter("Large CCI Index", DefaultValue = 13)]
        public int LargeCCIIndex { get; set; }
        [Parameter("Small CCI Index", DefaultValue = 3)]
        public int SmallCCIIndex { get; set; }
        [Parameter("Laguerre Gamma", DefaultValue = 0.7)]
        public double Gamma { get; set; }
        [Parameter("Laguerre Deviation", DefaultValue = 0.3)]
        public double Deviation { get; set; }
        protected override void OnStart()
        {
            // Put your initialization logic here
            Print("Started");
            Positions.Opened += OnPositionOpened;
            Positions.Closed += OnPositionClosed;
            DefaultIndices();
            laguerre = Indicators.GetIndicator<Laguerre_RSI>(Gamma);
            eo = Indicators.GetIndicator<ElliotOscillator>(MarketSeries.Close, 5, 34);
            sma = Indicators.SimpleMovingAverage(Source, 100);
            ema = Indicators.ExponentialMovingAverage(Source, 16);
            cci_13 = Indicators.CommodityChannelIndex(MarketSeries, LargeCCIIndex);
            cci_3 = Indicators.CommodityChannelIndex(MarketSeries, SmallCCIIndex);
            double[] input = 
            {
                (0.0),
                (0.0),
                (0.0),
                (0.0),
                (0.0)
            };
            cci3_values = new List<double>(input);
            cci13_values = new List<double>(input);
            sma_values = new List<double>(input);
            ema_values = new List<double>(input);
            positions_values = new Dictionary<string, List<double>>();
        }

        protected override void OnBar()
        {
            // Put your core logic here

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

            ListHandler(ema_values, ema.Result);
            ListHandler(sma_values, sma.Result);
            ListHandler(cci3_values, cci_3.Result);

            ListHandler(cci13_values, cci_13.Result);
            //checking if ema crosses sma from above or from below

            if (ema_values[0] > sma_values[0] && ema_values[1] < sma_values[1] & ema_values[2] < sma_values[2] && ema_values[3] < sma_values[3] && ema_values[4] < sma_values[4])
            {
                crossing_msg = "ema crossed sma from below";
                emasma_crossing_index = EmaSmaCrossingEnum.EmaHasCrossedSmaFromBelow;

            }
            if (ema_values[0] < sma_values[0] && ema_values[1] > sma_values[1] & ema_values[2] > sma_values[2] && ema_values[3] > sma_values[3] && ema_values[4] > sma_values[4])
            {
                crossing_msg = "ema crossed sma from above";
                emasma_crossing_index = EmaSmaCrossingEnum.EmaHasCrossedSmaFromAbove;
            }

            //checking for CCI 13 to be above 100 or below -100

            LargeCCIProcess(cci13_values);
            SmallCCIProcess(cci3_values);

            //trailing stop execution
            if (Positions.Count > 0 && positions_values.Count > 0)
            {
                foreach (Position position in Positions)
                {

                    if (position.StopLoss == null)
                    {
                        SetStopLoss(position);
                        //SetTakeProfit(position);
                    }
                    if (PositionHasList(position))
                    {
                        ListHandler(positions_values[position.Label], position.GrossProfit);
                        Print("Latest position " + position.Label + " profit distance vs stop loss magnitude " + ((position.EntryPrice - Symbol.Bid) / Symbol.PipSize) + " " + ((position.EntryPrice - position.StopLoss) / Symbol.PipSize));
                        Print("Last position " + position.Label + " profits " + positions_values[position.Label][0] + " " + positions_values[position.Label][1] + " " + positions_values[position.Label][2]);
                        Print("Position entry price " + position.EntryPrice + " and stop loss " + position.StopLoss + " current price Bid " + Symbol.Bid + " and Ask " + Symbol.Ask);
                        if (Math.Abs((Symbol.Bid - position.EntryPrice) / Symbol.PipSize) >= StopOrderMagnitude)
                        {
                            TrailingStop(position);
                        }
                    }
                }
            }

            #region SomeOldComments
            //Print(cci3_state);

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

            //foreach (double value in cci3_values)
            //{
            //    Print(value + " #" + cci3_values.IndexOf(value));
            //}

            //Print("CCI 3 calc values " + Math.Round(cci_3.Result.LastValue, 5) + " " + Math.Round(cci_3.Result.Last(1), 5) + " " + Math.Round(cci_3.Result.Last(2), 5));
            //Print("CCI 3 List values " + Math.Round(cci3_values[0], 5) + " " + Math.Round(cci3_values[1], 5) + " " + Math.Round(cci3_values[2], 5));
            #endregion
        }
        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
        protected void OnPositionOpened(PositionOpenedEventArgs args)
        {
            base.OnPositionOpened(args.Position);
            Print("Position " + args.Position.Label + " opened!");
            List<double> list;
            list = new List<double>(input);
            Print("Preparing position list");
            positions_values.Add("" + position_counter, list);
            Print("Created position list " + positions_values.Count + "with label " + position_counter);
            position_counter++;
            DefaultIndices();
        }
        protected void OnPositionClosed(PositionClosedEventArgs args)
        {
            base.OnPositionClosed(args.Position);
            Print("Position " + args.Position.Label + "closed!");
            Print("Current positions count = " + Positions.Count);
            positions_values.Remove(args.Position.Label);
            Print("Current lists count = " + positions_values.Count);
        }
        private void TrailingStop(Position pos)
        {
            Print("Position entered Trailing Stop with profit " + pos.GrossProfit);
            if (positions_values[pos.Label][0] > positions_values[pos.Label][1])
            {
                SetStopLoss(pos);
            }
        }
        private bool PositionHasList(Position pos)
        {
            bool result = false;
            result = positions_values.ContainsKey(pos.Label);
            Print("Position " + pos.Label + " has list " + result);
            return result;
        }

        private void LargeCCIProcess(List<double> inputlist)
        {
            if (inputlist[0] > 100 && inputlist[1] < 100 && inputlist[2] < 100)
            {
                cci13_index = CCI13States.EnteredPlus100;
                cci13_state = "CCI 13 entered +100";
            }
            if (inputlist[0] < 100 && inputlist[1] > 100 && inputlist[2] > 100)
            {
                cci13_index = CCI13States.LeftPlus100;
                cci13_state = "CCI 13 left +100";
            }
            if (inputlist[0] < -100 && inputlist[1] > -100 && inputlist[2] > -100)
            {
                cci13_index = CCI13States.EnteredMinus100;
                cci13_state = "CCI 13 entered -100";
            }
            if (inputlist[0] > -100 && inputlist[1] < -100 && inputlist[2] < -100)
            {
                cci13_index = CCI13States.LeftMinus100;
                cci13_state = "CCI 13 left -100";
            }
        }
        private void SmallCCIProcess(List<double> inputlist)
        {
            if (inputlist[0] > 100 && inputlist[1] < 100 && inputlist[2] < 100)
            {
                if (cci3_index == CCI3States.LeftMinus100)
                {
                    cci3_index = CCI3States.HitUp100AfterMinus100;
                    cci3_state = "CCI 3 hit above +100 after -100";
                    OpenPosition(TradeType.Sell);
                }
                else
                {
                    cci3_index = CCI3States.HitUp100;
                    cci3_state = "CCI 3 hit above +100";
                }

            }
            else if (inputlist[0] < -100 && inputlist[1] > -100 && inputlist[2] > -100)
            {

            }
            if (inputlist[0] < -100 && inputlist[1] > -100 && inputlist[2] > -100)
            {
                if (cci3_index == CCI3States.Left100)
                {
                    cci3_index = CCI3States.DropToMinus100After100;
                    cci3_state = "CCI 3 dropped below -100 after +100";
                    OpenPosition(TradeType.Buy);
                }
                else
                {
                    cci3_index = CCI3States.DropToMinus100;
                    cci3_state = "CCI 3 dropped below -100";
                }
            }
            if (inputlist[0] < 100 && inputlist[1] > 100)
            {
                cci3_index = CCI3States.Left100;
                cci3_state = "CCI 3 left 100";

            }
            if (inputlist[0] > -100 && inputlist[1] < -100)
            {
                cci3_index = CCI3States.LeftMinus100;
                cci3_state = "CCI 3 left -100";
            }
            //else
            //{
            //    cci3_index = CCI3States.None;
            //    cci3_state = "";
            //}

        }
        private void OpenPosition(TradeType trade)
        {
            if (cci13_index == CCI13States.LeftPlus100 && emasma_crossing_index == EmaSmaCrossingEnum.EmaHasCrossedSmaFromAbove && trade == TradeType.Sell && laguerre.laguerrersi.LastValue < 0.5 - Deviation)
            {
                ExecuteMarketOrder(TradeType.Sell, this.Symbol, PositionVolume, "" + position_counter);
            }
            if (cci13_index == CCI13States.LeftMinus100 && emasma_crossing_index == EmaSmaCrossingEnum.EmaHasCrossedSmaFromBelow && trade == TradeType.Buy && laguerre.laguerrersi.LastValue > 0.5 + Deviation)
            {

                ExecuteMarketOrder(TradeType.Buy, this.Symbol, PositionVolume, "" + position_counter);
                //Print("OPEN BUY POSITION!");
            }
            //Print("Current positions = " + Positions.Count);
            //Print("Current closed positions = " + Positions.Closed)
            //Print("Current lists = " + positions_values.Count);
            //if (Positions.Count > 2)
            //{
            //    Print("Last 3 positions " + Positions[0].Label + " " + Positions[1].Label + " " + Positions[Positions.Count - 1].Label);
            //}

        }
        private void SetTakeProfit(Position position)
        {
            double takeprofit;
            if (position.TradeType == TradeType.Buy)
            {
                takeprofit = Symbol.Ask + StopOrderMagnitude * Symbol.PipSize;
                ModifyPosition(position, position.StopLoss, takeprofit);
            }
            else
            {
                takeprofit = Symbol.Bid - StopOrderMagnitude * Symbol.PipSize;
                ModifyPosition(position, position.StopLoss, takeprofit);
            }

        }
        private void SetStopLoss(Position position)
        {
            double stoploss;
            if (position.TradeType == TradeType.Buy)
            {
                stoploss = Symbol.Bid - StopOrderMagnitude * Symbol.PipSize;
                ModifyPosition(position, stoploss, position.TakeProfit);
            }
            else
            {
                stoploss = Symbol.Ask + StopOrderMagnitude * Symbol.PipSize;
                ModifyPosition(position, stoploss, position.TakeProfit);
            }

        }
        private void DefaultIndices()
        {
            // emasma_crossing_index = EmaSmaCrossingEnum.None;
            cci3_state = "";
            cci3_index = CCI3States.None;

        }
        private void ListHandler(List<double> inputlist, DataSeries inputseries)
        {
            inputlist.Insert(0, inputseries.LastValue);
            inputlist.RemoveAt(inputlist.Count - 1);
        }
        private void ListHandler(List<double> inputlist, double inputvalue)
        {
            inputlist.Insert(0, inputvalue);
            inputlist.RemoveAt(inputlist.Count - 1);
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
