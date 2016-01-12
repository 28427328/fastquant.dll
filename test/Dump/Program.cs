using System;
using SmartQuant;
using System.IO;


namespace HelloWorldSample 
{
	public static class Program 
	{
		public static void Main() 
		{
            var ms =new MemoryStream();
            var wr = new BinaryWriter(ms);
            wr.Write("FKey");
            Console.WriteLine(ms.ToArray().Length);
         
            var f = Framework.Current;
            var df = new DataFile("d:\\data.quant", f.StreamerManager);
            df.Open();
            df.Dump();
            ObjectKey key;
            var kname = "AAPL.0.Bid";
            df.Keys.TryGetValue(kname, out key);
            if (key != null)
            {
                key.Dump();
                Console.WriteLine(key.DateTime);
                Console.WriteLine(key.CompressionLevel);
                Console.WriteLine(key.CompressionMethod);
            }
            df.Close();
            DumpInstrument();
            Console.ReadLine();
		}

        static void DumpInstrument()
        {
            var f = Framework.Current;
            foreach (var i in f.InstrumentManager.Instruments)
                Console.WriteLine(i.Symbol);
        }
	}
}