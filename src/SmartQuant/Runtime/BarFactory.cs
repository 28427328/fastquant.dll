// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace SmartQuant
{
    public class BarFactoryItem
    {
        protected internal BarFactory factory;
        protected internal Instrument instrument;
        protected internal BarType barType;
        protected internal long barSize;
        protected internal BarInput barInput;
        protected internal bool sessionEnabled;
        protected internal TimeSpan session1;
        protected internal TimeSpan session2;
        protected internal int providerId;

        protected Bar bar;

        public Instrument Instrument => this.instrument;

        public BarType BarType => this.barType;

        public long BarSize => this.barSize;

        public bool SessionEnabled
        {
            get
            {
                return this.sessionEnabled;
            }
            set
            {
                this.sessionEnabled = value;
            }
        }

        public TimeSpan Session1
        {
            get
            {
                return this.session1;
            }
            set
            {
                this.session1 = value;
            }
        }

        public TimeSpan Session2
        {
            get
            {
                return this.session2;
            }
            set
            {
                this.session2 = value;
            }
        }

        public int ProviderId { get; set; }

        protected BarFactoryItem(Instrument instrument, BarType barType, long barSize, BarInput barInput = BarInput.Trade, int providerId = -1)
        {
            this.instrument = instrument;
            this.barType = barType;
            this.barSize = barSize;
            this.barInput = barInput;
            this.providerId = providerId;
        }

        protected BarFactoryItem(Instrument instrument, BarType barType, long barSize, BarInput barInput, TimeSpan session1, TimeSpan session2, int providerId = -1)
        {
            this.factory = null;
            this.instrument = instrument;
            this.barType = barType;
            this.barSize = barSize;
            this.barInput = barInput;
            this.sessionEnabled = true;
            this.session1 = session1;
            this.session2 = session2;
            this.providerId = providerId;
        }

        protected virtual bool InSession(DateTime dateTime)
        {
            if (this.sessionEnabled)
            {
                TimeSpan timeOfDay = dateTime.TimeOfDay;
                if (timeOfDay < this.session1 || timeOfDay > this.session2)
                    return false;
            }
            return true;
        }

        internal void method_0(DataObject obj)
        {
            if (this.providerId != -1 && ((Tick)obj).ProviderId != this.providerId)
                return;

            if (!InSession(obj.DateTime))
                return;

            OnData(obj);
        }


        protected virtual void OnData(DataObject obj)
        {
            var tick = obj as Tick;
            if (this.bar == null)
            {
                this.bar = new Bar();
                this.bar.InstrumentId = tick.InstrumentId;
                this.bar.Type = this.barType;
                this.bar.Size = this.barSize;
                this.bar.OpenDateTime = GetBarOpenDateTime(obj);
                this.bar.DateTime = this.GetDataObjectDateTime(obj, ClockType.Local);
                this.bar.Open = tick.Price;
                this.bar.High = tick.Price;
                this.bar.Low = tick.Price;
                this.bar.Close = tick.Price;
                this.bar.Volume = tick.Size;
                this.bar.Status = BarStatus.Open;
                this.factory.Framework.EventServer.OnEvent(this.bar);
            }
            else
            {
                if (tick.Price > this.bar.High)
                    this.bar.High = tick.Price;
                if (tick.Price < this.bar.Low)
                    this.bar.Low = tick.Price;
                this.bar.Close = tick.Price;
                this.bar.Volume += tick.Size;
                this.bar.DateTime = GetDataObjectDateTime(obj, ClockType.Local);
            }
            ++this.bar.N;
        }

        protected internal virtual void OnReminder(DateTime datetime)
        {
            // do nothing
        }

        protected virtual DateTime GetBarOpenDateTime(DataObject obj) => obj.DateTime;

        protected virtual DateTime GetBarCloseDateTime(DataObject obj) => obj.DateTime;

        protected virtual DateTime GetDataObjectDateTime(DataObject obj, ClockType type)
        {
            return type == ClockType.Local ? obj.DateTime : (obj as Tick).ExchangeDateTime;
        }

        protected bool AddReminder(DateTime datetime, ClockType type)
        {
            throw new NotImplementedException();
        }

        protected void EmitBar()
        {
            this.bar.Status = BarStatus.Close;
            this.factory.Framework.EventServer.OnEvent(this.bar);
            this.bar = null;
        }

        public override string ToString() => $"{this.instrument.Symbol} {this.barType} {this.barSize} {this.barInput}";
    }

    public class TimeBarFactoryItem : BarFactoryItem
    {
        private ClockType type;

        public TimeBarFactoryItem(Instrument instrument, long barSize, int providerId = -1) : base(instrument, BarType.Time, barSize, BarInput.Trade, providerId)
        {
        }

        public TimeBarFactoryItem(Instrument instrument, long barSize, ClockType type = ClockType.Local, int providerId = -1)
        : base(instrument, BarType.Time, barSize, BarInput.Trade, providerId)
        {
            this.type = type;
        }

        public TimeBarFactoryItem(Instrument instrument, long barSize, BarInput barInput = BarInput.Trade, int providerId = -1) : base(instrument, BarType.Time, barSize, barInput, providerId)
        {
        }

        public TimeBarFactoryItem(Instrument instrument, long barSize, BarInput barInput = BarInput.Trade, ClockType type = ClockType.Local, int providerId = -1) : base(instrument, BarType.Time, barSize, barInput, providerId)
        {
            this.type = type;
        }

        public TimeBarFactoryItem(Instrument instrument, long barSize, BarInput barInput, TimeSpan session1, TimeSpan session2, int providerId = -1) : base(instrument, BarType.Tick, barSize, barInput, session1, session2, providerId)
        {
        }

        public TimeBarFactoryItem(Instrument instrument, long barSize, BarInput barInput, ClockType type, TimeSpan session1, TimeSpan session2, int providerId = -1) : base(instrument, BarType.Time, barSize, barInput, session1, session2, providerId)
        {
            this.type = type;
        }

        protected override void OnData(DataObject obj)
        {
            base.OnData(obj);
            if (this.bar != null || AddReminder(GetBarCloseDateTime(obj), this.type))
                return;
            if (obj is Tick)
                Console.WriteLine("TimeBarFactoryItem::OnData Can not add reminder. Clock = {0} local datetime = {1} exchange dateTime = {2} object = {3} object exchange datetime = {4} reminder datetime = {5}", this.type, this.factory.Framework.Clock.DateTime, this.factory.Framework.ExchangeClock.DateTime, obj, (obj as Tick).ExchangeDateTime, GetBarCloseDateTime(obj));
            else
                Console.WriteLine("TimeBarFactoryItem::OnData Can not add reminder. Object is not tick! Clock = {0} local datetime = {1} exchange datetime = {2} object = {3} reminder datetime = {4}", this.type, this.factory.Framework.Clock.DateTime, this.factory.Framework.ExchangeClock.DateTime, obj, GetBarCloseDateTime(obj));
        }

        protected override DateTime GetBarOpenDateTime(DataObject obj)
        {
            var t = GetDataObjectDateTime(obj, this.type);
            long seconds = (long)t.TimeOfDay.TotalSeconds / this.barSize * this.barSize;
            return t.Date.AddSeconds(seconds);
        }

        protected override DateTime GetBarCloseDateTime(DataObject obj)
        {
            return GetBarOpenDateTime(obj).AddSeconds(this.barSize);
        }

        protected internal override void OnReminder(DateTime datetime)
        {
            this.bar.DateTime = this.type == ClockType.Local ? datetime : this.factory.Framework.Clock.DateTime;
            EmitBar();
        }
    }

    public class TickBarFactoryItem : BarFactoryItem
    {
        public TickBarFactoryItem(Instrument instrument, long barSize, BarInput barInput = BarInput.Trade, int providerId = -1) : base(instrument, BarType.Tick, barSize, barInput, providerId)
        {
        }

        public TickBarFactoryItem(Instrument instrument, long barSize, BarInput barInput, TimeSpan session1, TimeSpan session2, int providerId = -1) : base(instrument, BarType.Tick, barSize, barInput, session1, session2, providerId)
        {
        }

        protected override void OnData(DataObject obj)
        {
            base.OnData(obj);
            if (this.bar.N == this.barSize)
                EmitBar();
        }
    }

    public class RangeBarFactoryItem : BarFactoryItem
    {
        public RangeBarFactoryItem(Instrument instrument, long barSize, BarInput barInput = BarInput.Trade, int providerId = -1) : base(instrument, BarType.Range, barSize, barInput, providerId)
        {
        }

        public RangeBarFactoryItem(Instrument instrument, long barSize, BarInput barInput, TimeSpan session1, TimeSpan session2, int providerId = -1) : base(instrument, BarType.Tick, barSize, barInput, session1, session2, providerId)
        {
        }

        protected override void OnData(DataObject obj)
        {
            base.OnData(obj);
            if (this.bar.N == this.barSize)
                EmitBar();
        }
    }

    public class VolumeBarFactoryItem : BarFactoryItem
    {
        public VolumeBarFactoryItem(Instrument instrument, long barSize, BarInput barInput = BarInput.Trade, int providerId = -1) : base(instrument, BarType.Volume, barSize, barInput, providerId)
        {
        }

        public VolumeBarFactoryItem(Instrument instrument, long barSize, BarInput barInput, TimeSpan session1, TimeSpan session2, int providerId = -1) : base(instrument, BarType.Tick, barSize, barInput, session1, session2, providerId)
        {
        }

        protected override void OnData(DataObject obj)
        {
            base.OnData(obj);
            if (this.bar.Volume >= this.barSize)
                EmitBar();
        }
    }

    public class SessionBarFactoryItem : BarFactoryItem
    {
        private ClockType type;

        public SessionBarFactoryItem(Instrument instrument, BarInput barInput, TimeSpan session1, TimeSpan session2, int providerId = -1) : base(instrument, BarType.Session, (long)(session2 - session1).TotalSeconds, barInput, session1, session2, providerId)
        {
        }

        public SessionBarFactoryItem(Instrument instrument, BarInput barInput, ClockType type, TimeSpan session1, TimeSpan session2, int providerId = -1) : base(instrument, BarType.Session, (long)(session2 - session1).TotalSeconds, barInput, session1, session2, providerId)
        {
            this.type = type;
        }

        protected override void OnData(DataObject obj)
        {
            bool flag = this.bar == null;
            base.OnData(obj);
            if (!flag || AddReminder(GetBarCloseDateTime(obj), this.type))
                return;
            if (!(obj is Tick))
                Console.WriteLine($"SessionBarFactoryItem::OnData Can not add reminder. Object is not tick! Clock = {this.type} local datetime = {this.factory.Framework.Clock.DateTime} exchange datetime = {this.factory.Framework.ExchangeClock.DateTime} object = {obj} reminder datetime = {GetBarCloseDateTime(obj)}");
            else
                Console.WriteLine($"SessionBarFactoryItem::OnData Can not add reminder. Clock = {this.type} local datetime = {this.factory.Framework.Clock.DateTime} exchange dateTime = {this.factory.Framework.ExchangeClock.DateTime} object = {obj} object exchange datetime = {((Tick)obj).ExchangeDateTime} reminder datetime = {GetBarCloseDateTime(obj)}");
        }

        protected override DateTime GetBarOpenDateTime(DataObject obj)
        {
            return GetDataObjectDateTime(obj, this.type).Date.Add(this.session1);
        }

        protected override DateTime GetBarCloseDateTime(DataObject obj)
        {
            return GetDataObjectDateTime(obj, this.type).Date.Add(this.session2);
        }

        protected internal override void OnReminder(DateTime datetime)
        {
            if (this.type == ClockType.Local)
                this.bar.DateTime = datetime;
            else
                this.bar.DateTime = this.factory.Framework.Clock.DateTime;
            EmitBar();
        }
    }

    public class BarFactory
    {
        private Framework framework;

        private SortedList<DateTime, SortedList<long, List<BarFactoryItem>>> sortedList_0 = new SortedList<DateTime, SortedList<long, List<BarFactoryItem>>>();

        public IdArray<List<BarFactoryItem>> ItemLists { get; } = new IdArray<List<BarFactoryItem>>(10240);

        internal Framework Framework => this.framework;

        public BarFactory(Framework framework)
        {
            this.framework = framework;
        }
        
        // The most important one of all 'Add's
        public void Add(BarFactoryItem item)
        {
            if (item.factory != null)
                throw new InvalidOperationException("BarFactoryItem is already added to another BarFactory instance.");

            item.factory = this;
            int id = item.instrument.Id;
            var items = ItemLists[id] = ItemLists[id] ?? new List<BarFactoryItem>();
            if (items.Exists(match => item.barType == match.barType && item.barSize == match.barSize && item.barInput == match.barInput))
                Console.WriteLine($"{DateTime.Now} BarFactory::Add Item '{item}' is already added");
            else
                items.Add(item);
        }

        public void Add(Instrument instrument, TimeSpan session1, TimeSpan session2, int providerId = -1)
        {
            Add(instrument, BarType.Session, (long)(session2 - session1).Seconds, BarInput.Trade, ClockType.Local, session1, session2, providerId);
        }

        public void Add(string symbol, TimeSpan session1, TimeSpan session2, int providerId = -1)
        {
            Add(this.framework.InstrumentManager[symbol], BarType.Session, (long)(session2 - session1).Seconds, BarInput.Trade, ClockType.Local, session1, session2, providerId);
        }

        public void Add(string symbol, BarType barType, long barSize, BarInput barInput = BarInput.Trade, int providerId = -1)
        {
            Add(this.framework.InstrumentManager[symbol], barType, barSize, barInput, ClockType.Local, providerId);
        }

        public void Add(InstrumentList instruments, BarType barType, long barSize, BarInput barInput = BarInput.Trade, int providerId = -1)
        {
            foreach (var i in instruments)
                Add(i, barType, barSize, barInput, ClockType.Local, providerId);
        }

        public void Add(string[] symbols, BarType barType, long barSize, BarInput barInput = BarInput.Trade, int providerId = -1)
        {
            foreach(var symbol in symbols)
                Add(this.framework.InstrumentManager.Get(symbol), barType, barSize, barInput, ClockType.Local, providerId);
        }

        public void Add(Instrument instrument, BarType barType, long barSize, BarInput barInput = BarInput.Trade, ClockType type = ClockType.Local, int providerId = -1)
        {
            BarFactoryItem item;
            switch (barType)
            {
                case BarType.Time:
                    item = new TimeBarFactoryItem(instrument, barSize, barInput, type, providerId);
                    break;
                case BarType.Tick:
                    item = new TickBarFactoryItem(instrument, barSize, barInput, providerId);
                    break;
                case BarType.Volume:
                    item = new VolumeBarFactoryItem(instrument, barSize, barInput, providerId);
                    break;
                case BarType.Range:
                    item = new RangeBarFactoryItem(instrument, barSize, barInput, providerId);
                    break;
                case BarType.Session:
                    throw new ArgumentException("BarFactory::Add Can not create SessionBarFactoryItem without session parameters");
                default:
                    throw new ArgumentException($"Unknown bar type - {barType}");
            }
            Add(item);
        }

        public void Add(string symbol, BarType barType, long barSize, BarInput barInput = BarInput.Trade, ClockType type = ClockType.Local, int providerId = -1)
        {
            Add(this.framework.InstrumentManager[symbol], barType, barSize, barInput, type, providerId);
        }

        public void Add(InstrumentList instruments, BarType barType, long barSize, BarInput barInput = BarInput.Trade, ClockType type = ClockType.Local, int providerId = -1)
        {
            foreach (var i in instruments)
                Add(i, barType, barSize, barInput, type, providerId);
        }

        public void Add(string[] symbols, BarType barType, long barSize, BarInput barInput = BarInput.Trade, ClockType type = ClockType.Local, int providerId = -1)
        {
            foreach(var symbol in symbols)
                Add(this.framework.InstrumentManager.Get(symbol), barType, barSize, barInput, type, providerId);
        }

        public void Add(Instrument instrument, BarType barType, long barSize, TimeSpan session1, TimeSpan session2, int providerId = -1)
        {
            Add(instrument, barType, barSize, BarInput.Trade, ClockType.Local, session1, session2, providerId);
        }

        public void Add(string symbol, BarType barType, long barSize, TimeSpan session1, TimeSpan session2, int providerId = -1)
        {
            Add(this.framework.InstrumentManager[symbol], barType, barSize, BarInput.Trade, ClockType.Local, session1, session2, providerId);
        }

        public void Add(string symbol, BarInput barInput, ClockType type, TimeSpan session1, TimeSpan session2, int providerId = -1)
        {
            Add(this.framework.InstrumentManager[symbol], BarType.Session, (long)(session2 - session1).Seconds, barInput, type, session1, session2, providerId);
        }

        public void Add(Instrument instrument, BarType barType, long barSize, BarInput barInput, ClockType type, TimeSpan session1, TimeSpan session2, int providerId = -1)
        {
            BarFactoryItem item;
            switch (barType)
            {
                case BarType.Time:
                    item = new TimeBarFactoryItem(instrument, barSize, barInput, type, session1, session2, providerId);
                    break;
                case BarType.Tick:
                    item = new TickBarFactoryItem(instrument, barSize, barInput, session1, session2, providerId);
                    break;
                case BarType.Volume:
                    item = new VolumeBarFactoryItem(instrument, barSize, barInput, session1, session2, providerId);
                    break;
                case BarType.Range:
                    item = new RangeBarFactoryItem(instrument, barSize, barInput, session1, session2, providerId);
                    break;
                case BarType.Session:
                    item = new SessionBarFactoryItem(instrument, barInput, type, session1, session2, providerId);
                    break;
                default:
                    throw new ArgumentException($"Unknown bar type - {barType}");
            }
            Add(item);
        }

        public void Add(string symbol, BarType barType, long barSize, BarInput barInput, ClockType type, TimeSpan session1, TimeSpan session2, int providerId = -1)
        {
            Add(this.framework.InstrumentManager[symbol], barType, barSize, barInput, type, session1, session2, providerId);
        }

        public void Remove(BarFactoryItem item)
        {
            var list = ItemLists[item.Instrument.Id];
            if (list == null)
                return;

            var found = list.Find(x => x.barType == item.barType && x.barSize == item.barSize && x.barInput == item.barInput);
            if (found != null)
                list.Remove(found);
            else
                Console.WriteLine($"{DateTime.Now} BarFactory::Remove Item '{item}' is already removed");
        }

        public void Remove(Instrument instrument, BarType barType, long barSize, BarInput barInput = BarInput.Trade, ClockType type = ClockType.Local)
        {
            BarFactoryItem item;
            switch (barType)
            {
                case BarType.Time:
                    item = new TimeBarFactoryItem(instrument, barSize, barInput, type);
                    break;
                case BarType.Tick:
                    item = new TickBarFactoryItem(instrument, barSize, barInput);
                    break;
                case BarType.Volume:
                    item = new VolumeBarFactoryItem(instrument, barSize, barInput);
                    break;
                case BarType.Range:
                    item = new RangeBarFactoryItem(instrument, barSize, barInput);
                    break;
                case BarType.Session:
                    throw new ArgumentException("BarFactory::Remove Can not create SessionBarFactoryItem without session parameters");
                default:
                    throw new ArgumentException($"Unknown bar type - {barType}");
            }
            Remove(item);
        }

        public void Clear()
        {
            ItemLists.Clear();
            this.sortedList_0.Clear();
        }

        // TODO: review it later
        internal void OnData(DataObject obj)
        {
            var tick = (Tick)obj;
            var list = ItemLists[tick.InstrumentId];
            if (list == null)
                return;

            int i = 0;
            while (i < list.Count)
            {
                var item = list[i];
                switch (item.barInput)
                {
                    case BarInput.Trade:
                        if (tick.TypeId == EventType.Trade)
                        {
                            item.method_0(tick);
                            i++;
                            continue;
                        }
                        break;
                    case BarInput.Bid:
                        if (tick.TypeId == EventType.Bid)
                        {
                            item.method_0(tick);
                            i++;
                            continue;
                        }
                        break;
                    case BarInput.Ask:
                        if (tick.TypeId == EventType.Ask)
                        {
                            item.method_0(tick);
                            i++;
                            continue;
                        }
                        break;
                    case BarInput.Middle:
                        switch (tick.TypeId)
                        {
                            case EventType.Bid:
                                {
                                    var ask = this.framework.DataManager.GetAsk(tick.InstrumentId);
                                    if (ask == null)
                                    {
                                        i++;
                                        continue;
                                    }
                                    tick = new Tick(obj.dateTime, tick.ProviderId, tick.InstrumentId, (ask.Price + tick.Price) / 2.0, (ask.Size + tick.Size) / 2);
                                    break;
                                }
                            case EventType.Ask:
                                {
                                    Bid bid = this.framework.DataManager.GetBid(tick.InstrumentId);
                                    if (bid == null)
                                    {
                                        i++;
                                        continue;
                                    }
                                    tick = new Tick(obj.dateTime, tick.ProviderId, tick.InstrumentId, (bid.Price + tick.Price) / 2.0, (bid.Size + tick.Size) / 2);
                                    break;
                                }
                            case EventType.Trade:
                                i++;
                                continue;
                        }
                        if (obj.TypeId != EventType.Ask)
                        {
                            item.method_0(tick);
                            i++;
                            continue;
                        }
                        break;
                    case BarInput.Tick:
                        {
                            item.method_0(tick);
                            i++;
                            continue;
                        }
                    case BarInput.BidAsk:
                        if (tick.TypeId != EventType.Trade)
                        {
                            item.method_0(tick);
                            i++;
                            continue;
                        }
                        break;
                    default:
                        Console.WriteLine($"BarFactory::OnData BarInput is not supported : {item.barInput}");
                        break;
                }
                i++;
            }
        }

        // TODO: figure out the algo and function name
        internal bool method_1(BarFactoryItem item, DateTime dateTime, ClockType type)
        {
            bool flag = false;
            SortedList<long, List<BarFactoryItem>> sortedList;
            if (!this.sortedList_0.TryGetValue(dateTime, out sortedList))
            {
                sortedList = new SortedList<long, List<BarFactoryItem>>();
                this.sortedList_0.Add(dateTime, sortedList);
                flag = true;
            }
            List<BarFactoryItem> list;
            if (!sortedList.TryGetValue(item.barSize, out list))
            {
                list = new List<BarFactoryItem>();
                sortedList.Add(item.barSize, list);
            }
            list.Add(item);
            if (flag)
            {
                Clock clock;
                switch (type)
                {
                    case ClockType.Local:
                        clock = this.framework.Clock;
                        break;
                    case ClockType.Exchange:
                        clock = this.framework.ExchangeClock;
                        break;
                    default:
                        clock = this.framework.Clock;
                        break;
                }
                return clock.AddReminder((dt, obj) =>
                {
                    SortedList<long, List<BarFactoryItem>> sList;
                    if (this.sortedList_0.TryGetValue(dt, out sList))
                    {
                        this.sortedList_0.Remove(dt);
                        foreach (var lst in sList.Values)
                            foreach (var itm in lst)
                                itm.OnReminder(dt);
                    }
                }, dateTime, null);
            }
            return true;
        }
    }
}
