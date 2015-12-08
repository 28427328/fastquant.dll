﻿using System;

namespace SmartQuant
{
    public class ExecutionMessage : DataObject
    {
        public int Id { get; internal set; }

        public Order Order { get; set; }

        public int OrderId { get; set; }

        public string ClOrderId { get; set; }

        public string ProviderOrderId { get; set; }

        public int InstrumentId { get; internal set; }

        public Instrument Instrument { get; set; }

        internal ObjectTable Fields { get; set; }

        public object this[int index]
        {
            get
            {
                return Fields != null ? Fields[index] : null;
            }
            set
            {
                if (Fields == null)
                    Fields = new ObjectTable();
                Fields[index] = value;
            }
        }
    }

    public class ExecutionReport : ExecutionMessage
    {
        public override byte TypeId => DataObjectType.ExecutionReport;

        public byte CurrencyId { get; set; }

        public ExecType ExecType { get; set; }

        public OrderType OrdType { get; set; }

        public OrderSide Side { get; set; }

        public TimeInForce TimeInForce { get; set; }

        public OrderStatus OrdStatus { get; set; }

        public double LastPx { get; set; }

        public double AvgPx { get; set; }

        public double OrdQty { get; set; }

        public double CumQty { get; set; }

        public double LastQty { get; set; }

        public double LeavesQty { get; set; }

        public double Price { get; set; }

        public double StopPx { get; set; }

        public double Commission { get; set; }

        public string Text { get; set; }

        public ExecutionReport()
        {
        }

        public ExecutionReport(ExecutionReport report)
        {
            this.DateTime = report.DateTime;
            this.Instrument = report.Instrument;
            this.Order = report.Order;

            this.CurrencyId = report.CurrencyId;
            this.ExecType = report.ExecType;
            this.OrdType = report.OrdType;
            this.Side = report.Side;
            this.TimeInForce = report.TimeInForce;
            this.OrdStatus = report.OrdStatus;
            this.LastPx = report.LastPx;
            this.AvgPx = report.AvgPx;
            this.OrdQty = report.OrdQty;
            this.CumQty = report.CumQty;
            this.LastQty = report.LastQty;
            this.LeavesQty = report.LeavesQty;
            this.Price = report.Price;
            this.StopPx = report.StopPx;
            this.Commission = report.Commission;
            this.Text = report.Text;
        }

        public override string ToString() => $"{DateTime} {Instrument.Symbol} {ExecType} {Side} {AvgPx}";
    }

    public enum ExecutionCommandType
    {
        Send,
        Cancel,
        Replace
    }

    public class ExecutionCommand : ExecutionMessage
    {
        public override byte TypeId
        {
            get
            {
                return DataObjectType.ExecutionCommand;
            }
        }

        public Portfolio Portfolio { get; private set; }

        public IExecutionProvider Provider { get; private set; }

        public int AlgoId { get; private set; }

        public string OCA { get; internal set; }

        public string Text { get; internal set; }

        public double StopPx { get; internal set; }

        public double Price { get; internal set; }

        public OrderSide Side { get; internal set; }

        public OrderType OrdType { get; internal set; }

        public TimeInForce TimeInForce { get; internal set; }

        public double Qty { get; internal set; }

        public DateTime TransactTime { get; internal set; }

        public ExecutionCommandType Type { get; internal set; }

        public string Account { get; internal set; }

        public string ClientID { get; internal set; }

        public byte ProviderId { get; internal set; }

        public int PortfolioId { get; internal set; }

        public byte RouteId { get; internal set; }

        public ExecutionCommand()
        {
            Text = "";
            OCA = "";
        }

        public ExecutionCommand(ExecutionCommandType type, Order order)
            : this()
        {
            Type = type;
            Order = order;
            OrderId = order.Id;
        }

        public ExecutionCommand(ExecutionCommand command)
            : this()
        {
            Account = command.Account;
            ClientID = command.ClientID;
            RouteId = command.RouteId;
        }
    }

    public delegate void ExecutionCommandEventHandler(object sender, ExecutionCommand command);

}