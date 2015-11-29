﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace SmartQuant
{
    public class BarSeries : IEnumerable<Bar>, IIdNamedItem, IDataSeries, ISeries
    {
        protected string name;
        protected string description;
        private int maxLength;

        public int Id { get; }
        public string Name { get { return this.name; } }

        public List<Indicator> Indicators { get; } = new List<Indicator>();
 
        public BarSeries(int maxLength)
        {
        }

        public BarSeries(string name = "", string description = "", int maxLength = -1, int id = -1)
        {
            this.name = name;
            this.description = description;
            this.maxLength = maxLength;
            Id = id;
        }

        public double this[int index, BarData barData]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime DateTime1
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime DateTime2
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Description
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public double First
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime FirstDateTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public double Last
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime LastDateTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        int ISeries.Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        long IDataSeries.Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public double this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DataObject this[long index]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Add(DataObject obj)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Bar> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public double HighestHigh(int nBars)
        {
            throw new NotImplementedException();
        }

        public double LowestLow(int nBars)
        {
            throw new NotImplementedException();
        }

        public int GetIndex(DateTime dateTime, IndexOption option = IndexOption.Null)
        {
            throw new NotImplementedException();
        }

        public long GetIndex(DateTime dateTime, SearchOption option = SearchOption.Prev)
        {
            throw new NotImplementedException();
        }

        public double GetMax(DateTime dateTime1, DateTime dateTime2)
        {
            throw new NotImplementedException();
        }

        public double GetMax(int index1, int index2, BarData barData)
        {
            throw new NotImplementedException();
        }

        public double GetMin(DateTime dateTime1, DateTime dateTime2)
        {
            throw new NotImplementedException();
        }

        public double GetMin(int index1, int index2, BarData barData)
        {
            throw new NotImplementedException();
        }

        public void Remove(long index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}