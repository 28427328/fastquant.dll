﻿using System;
using System.Collections;
using System.Drawing;
using System.Linq;

namespace SmartQuant.Charting
{
    public class TTitleItem
    {
        public string Text { get; set; }

        public Color Color { get; set; }

        public TTitleItem() : this("", Color.Black)
        {
        }

        public TTitleItem(string text) : this(text, Color.Black)
        {
        }

        public TTitleItem(string text, Color color)
        {
            Text = text;
            Color = color;
        }
    }
    [Serializable]
    public class TTitle
    {
        private Pad pad;

        public ArrayList Items { get; private set; }

        public bool ItemsEnabled { get; set; }

        public string Text { get; set; }

        public Font Font { get; set; }

        public Color Color { get; set; }

        public ETitlePosition Position { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width
        {
            get
            {
                return (int)this.pad.Graphics.MeasureString(GetText(), Font).Width;
            }
        }

        public int Height
        {
            get
            {
                return (int)this.pad.Graphics.MeasureString(GetText(), Font).Height;
            }
        }

        public ETitleStrategy Strategy { get; set; }

        public TTitle(Pad pad, string text = "")
        {
            this.pad = pad;
            Text = Text;
            Items = new ArrayList();
            ItemsEnabled = false;
            Font = new Font("Arial", 8f);
            Color = Color.Black;
            Position = ETitlePosition.Left;
            Strategy = ETitleStrategy.Smart;
            X = 0;
            Y = 0;
        }

        public void Add(string Text, Color Color)
        {
            Items.Add(new TTitleItem(Text, Color));
        }

        private string GetText()
        {
            string str = Text;
            foreach (TTitleItem item in Items)
                str = str + " " + item.Text;
            return str;
        }

        public void Paint()
        {
            var brush = new SolidBrush(Color);
            if (!string.IsNullOrEmpty(Text))
                this.pad.Graphics.DrawString(Text, Font, brush, X, Y);
            if (Strategy == ETitleStrategy.Smart && Text == "" && ItemsEnabled && Items.Count != 0)
                this.pad.Graphics.DrawString(((TTitleItem)Items[0]).Text, Font, brush, X, Y);
            if (!ItemsEnabled)
                return;
            string str = Text;
            foreach (TTitleItem item in Items)
            {
                string text = str + " ";
                int num = X + (int)this.pad.Graphics.MeasureString(text, Font).Width;
                this.pad.Graphics.DrawString(item.Text, Font, new SolidBrush(item.Color), num, Y);
                str = text + item.Text;
            }
        }
    }
}
