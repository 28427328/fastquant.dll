﻿using System;
using static System.Math;

namespace SmartQuant.Statistics
{
    public class AnnualReturn : PortfolioStatisticsItem
    {
        protected DateTime dateTime;
        protected double initial;

        public override int Type => PortfolioStatisticsType.AnnualReturn;

        public override bool Show => false;
        public override string Name => "Annual Return";
        public override string Category => "Daily / Annual returns";

        public override void OnEquity(double equity)
        {
            throw new NotImplementedException();
        }
    }

    public class AnnualReturnPercent : PortfolioStatisticsItem
    {
        protected DateTime dateTime;

        protected double initial;

        public override void OnEquity(double equity)
        {
            if (this.dateTime == DateTime.MinValue)
            {
                this.dateTime = Clock.DateTime;
                this.initial = equity;
            }
            if (Clock.DateTime.Year > this.dateTime.Year)
            {
                if (this.initial != 0)
                {
                    this.totalValue = (equity - this.initial) / this.initial;
                    TotalValues.Add(Clock.DateTime, this.totalValue);
                    Emit();
                }
                this.dateTime = Clock.DateTime;
                this.initial = equity;
            }
        }

        public override string Category => "Daily / Annual returns";

        public override string Format => "P2";

        public override string Name => "Annual Return %";

        public override bool Show => false;

        public override int Type => PortfolioStatisticsType.AnnualReturnPercent;
    }

    public class AnnualReturnPercentDownsideRisk : PortfolioStatisticsItem
    {
        public override void OnInit()
        {
            AnnualizedFactor = 252;
            Subscribe(PortfolioStatisticsType.DailyDownsideRisk);
        }

        public override void OnStatistics(PortfolioStatisticsItem statistics)
        {
            if (statistics.Type == PortfolioStatisticsType.DailyDownsideRisk)
            {
                this.totalValue = AnnualizedFactor * statistics.TotalValue;
                TotalValues.Add(Clock.DateTime, this.totalValue);
                Emit();
            }
        }

        public double AnnualizedFactor { get; set; }

        public override string Category => "Daily / Annual returns";

        public override string Name => "Annual Return % Downside Risk";

        public override int Type => PortfolioStatisticsType.AnnualDownsideRisk;
    }

    public class AnnualReturnPercentStdDev : PortfolioStatisticsItem
    {
        public override void OnInit()
        {
            AnnualizedFactor = 252;
            Subscribe(PortfolioStatisticsType.DailyReturnPercentStdDev);
        }

        public override void OnStatistics(PortfolioStatisticsItem statistics)
        {
            if (statistics.Type == PortfolioStatisticsType.DailyReturnPercentStdDev)
            {
                this.totalValue = this.AnnualizedFactor * statistics.TotalValue;
                TotalValues.Add(Clock.DateTime, this.totalValue);
                Emit();
            }
        }

        public double AnnualizedFactor { get; set; }

        public override string Category => "Daily / Annual returns";

        public override string Format => "P2";

        public override string Name => "Annual Return % Standard Deviation";

        public override int Type => PortfolioStatisticsType.AnnualReturnPercentStdDev;
    }

    public class AvgAnnualReturnPercent : PortfolioStatisticsItem
    {
        public override void OnInit()
        {
            AnnualizedFactor = 252;
            Subscribe(PortfolioStatisticsType.AvgDailyReturnPercent);
        }

        public override void OnStatistics(PortfolioStatisticsItem statistics)
        {
            if (statistics.Type == 66)
            {
                this.totalValue = statistics.TotalValue * AnnualizedFactor;
                TotalValues.Add(Clock.DateTime, this.totalValue);
                Emit();
            }
        }

        public double AnnualizedFactor { get; set; }

        public override string Category => "Daily / Annual returns";

        public override string Format => "P2";

        public override string Name => "Average Annual Return %";

        public override int Type => PortfolioStatisticsType.AvgAnnualReturnPercent;
    }

    public class DailyReturnPercent : PortfolioStatisticsItem
    {
        public override void OnEquity(double equity)
        {
            if (this.dateTime == DateTime.MinValue)
            {
                this.dateTime = Clock.DateTime;
                this.initial = equity;
            }
            if (Clock.DateTime.Date > this.dateTime.Date)
            {
                if (this.initial != 0)
                {
                    this.totalValue = (equity - this.initial) / this.initial;
                    TotalValues.Add(Clock.DateTime, this.totalValue);
                    Emit();
                }
                this.dateTime = Clock.DateTime;
                this.initial = equity;
            }
        }

        public override string Category => "Daily / Annual returns";

        public override string Format => "P2";

        public override string Name => "Daily Return %";

        public override bool Show => true;

        public override int Type => PortfolioStatisticsType.DailyReturnPercent;

        protected DateTime dateTime;

        protected double initial;
    }

    public class DailyReturnPercentDownsideRisk : PortfolioStatisticsItem
    {
        public override void OnInit()
        {
            Threshold = 0;
            Subscribe(PortfolioStatisticsType.DailyReturnPercent);
        }

        public override void OnStatistics(PortfolioStatisticsItem statistics)
        {
            if (statistics.Type == PortfolioStatisticsType.DailyReturnPercent)
            {
                this.sumsq += Pow(Max(0, statistics.TotalValue - Threshold), 2);
                if (statistics.TotalValue < 0)
                {
                    this.count++;
                }
                if (this.count > 0)
                {
                    this.totalValue = Sqrt(this.sumsq / this.count);
                    TotalValues.Add(Clock.DateTime, this.totalValue);
                    Emit();
                }
            }
        }

        public override string Category => "Daily / Annual returns";

        public override string Name => "Daily Return % Downside Risk";

        public double Threshold { get; set; }

        public override int Type => PortfolioStatisticsType.DailyDownsideRisk;

        protected int count;

        // Token: 0x04000889 RID: 2185
        [CompilerGenerated]
        private double double_0;

        // Token: 0x04000887 RID: 2183
        protected double sumsq;
    }

}