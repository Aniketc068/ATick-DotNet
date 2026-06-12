using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class FastSigning
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf3 = File.ReadAllBytes(Path.Combine("samples", "blank3.pdf"));
            string pl = "[[1,[40,640,260,750]],[2,[330,380,560,490]],[3,[180,60,400,170]]]";
            Atick.SetFastSigning(true);          // reuse revocation across the batch
            for (int i = 1; i <= 5; i++)
            {
                string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
                string opt = "{\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\",\"reason\":\"Approved\",\"date\":\"" + now
                    + "\",\"placements\":" + pl + ",\"mode\":\"single\",\"pades\":true,\"timestamp\":true,\"ltv\":true}";
                Save("fast_" + i + ".pdf", Atick.SignPfx(pdf3, pfx, opt));
            }
            Atick.SetFastSigning(false);
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
