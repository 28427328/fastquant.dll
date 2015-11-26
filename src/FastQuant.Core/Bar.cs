﻿// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace SmartQuant
{
    public enum BarData
    {
        Close,
        Open,
        High,
        Low,
        Median,
        Typical,
        Weighted,
        Average,
        Volume,
        OpenInt,
        Range,
        Mean,
        Variance,
        StdDev
    }

    public enum BarType : byte
    {
        Time = 1,
        Tick,
        Volume,
        Range,
        Session
    }

    public class Bar : DataObject
    {
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Open { get; set; }

        public double Range => High - Low;
    }
}