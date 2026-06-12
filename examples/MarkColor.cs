// ATick for .NET example — the "?" mark colour + gradient.
using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class MarkColor
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf1 = File.ReadAllBytes(Path.Combine("samples", "blank.pdf"));
            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            string baseOpts = "\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\",\"reason\":\"Approved\",\"date\":\"" + now
                + "\",\"page\":1,\"rect\":[300,55,575,175],\"pades\":true";
            string[][] M = {
                new[] { "default", "" },
                new[] { "hex", ",\"mark_color\":\"#3CB371\"" },
                new[] { "named", ",\"mark_color\":\"orange\"" },
                new[] { "rgb255", ",\"mark_color\":[255,80,80]" },
                new[] { "gradient", ",\"mark_gradient\":[\"#FFD700\",\"#FF4500\"]" }
            };
            foreach (var m in M)
                Save("11_mark_" + m[0] + ".pdf", Atick.SignPfx(pdf1, pfx, "{" + baseOpts + m[1] + "}"));
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
