﻿using System;

namespace SmartQuant
{
    public class Account
    {
         
    }

    public class AccountPosition
    {
        public byte CurrencyId { get; }

        public double Value { get; private set; }

        public AccountPosition(byte currencyId, double value)
        {
            CurrencyId = currencyId;
            Value = value;
        }

        public AccountPosition(AccountTransaction transaction)
            : this(transaction.CurrencyId, transaction.Value)
        {
        }

        public void Add(AccountTransaction transaction)
        {
            Value += transaction.Value;
        }
    }

    public class AccountTransaction
    {
        public DateTime DateTime { get; }

        public double Value { get; }

        public byte CurrencyId { get; }

        public string Text { get; }

        public AccountTransaction(DateTime dateTime, double value, byte currencyId, string text)
        {
            this.DateTime = dateTime;
            Value = value;
            CurrencyId = currencyId;
            Text = text;
        }

        public AccountTransaction(Fill fill)
            : this(fill.DateTime, fill.CashFlow, fill.CurrencyId, fill.Text)
        {
        }
    }
}