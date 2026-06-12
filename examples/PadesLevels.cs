// ATick for .NET example — PAdES B-B / B-T / B-LT / B-LTA.
using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class PadesLevels
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf3 = File.ReadAllBytes(Path.Combine("samples", "blank3.pdf"));
            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            string pl = "[[1,[40,640,260,750]],[2,[330,380,560,490]],[3,[180,60,400,170]]]";
            string baseOpts = "\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\",\"reason\":\"Approved\",\"date\":\"" + now
                + "\",\"placements\":" + pl + ",\"mode\":\"single\",\"pades\":true";
            string[][] L = {
                new[] { "B_B", "" },
                new[] { "B_T", ",\"timestamp\":true" },
                new[] { "B_LT", ",\"timestamp\":true,\"ltv\":true" },
                new[] { "B_LTA", ",\"timestamp\":true,\"lta\":true" }
            };
            foreach (var lv in L)
                Save("02_pades_" + lv[0] + ".pdf", Atick.SignPfx(pdf3, pfx, "{" + baseOpts + lv[1] + "}"));
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
