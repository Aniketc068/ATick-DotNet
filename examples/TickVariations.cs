// ATick for .NET example — green tick variations (green_tick / without_green_tick / always_green_tick).
using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class TickVariations
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf1 = File.ReadAllBytes(Path.Combine("samples", "blank.pdf"));
            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            string baseOpts = "\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\",\"reason\":\"Approved\",\"date\":\"" + now
                + "\",\"page\":1,\"rect\":[300,55,575,175],\"pades\":true";
            Save("green_tick.pdf",         Atick.SignPfx(pdf1, pfx, "{" + baseOpts + ",\"green_tick\":true}"));
            Save("without_green_tick.pdf", Atick.SignPfx(pdf1, pfx, "{" + baseOpts + ",\"green_tick\":false}"));
            Save("always_green_tick.pdf",  Atick.SignPfx(pdf1, pfx, "{" + baseOpts + ",\"always_check\":true}"));
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
