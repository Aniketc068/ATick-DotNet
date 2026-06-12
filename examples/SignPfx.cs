// ATick for .NET example — Sign a PDF with a .pfx in one call; same signature shown on 3 pages (default ATick logo).
using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class SignPfx
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf3 = File.ReadAllBytes(Path.Combine("samples", "blank3.pdf"));
            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            string pl = "[[1,[40,640,260,750]],[2,[330,380,560,490]],[3,[180,60,400,170]]]";
            byte[] signed = Atick.SignPfx(pdf3, pfx, "{\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\","
                + "\"reason\":\"Approved\",\"date\":\"" + now + "\",\"placements\":" + pl + ",\"mode\":\"single\",\"pades\":true}");
            Save("01_pfx.pdf", signed);
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
