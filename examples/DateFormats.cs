// ATick for .NET example — any custom date string in the appearance.
using System;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class DateFormats
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf = File.ReadAllBytes(Path.Combine("samples", "blank.pdf"));
            string[][] variants =
            {
                new[] { "iso",    Fmt("yyyy-MM-dd HH:mm:ss") },
                new[] { "eu",     Fmt("dd/MM/yyyy HH:mm") },
                new[] { "us",     Fmt("MM-dd-yyyy hh:mm tt") },
                new[] { "words",  Fmt("dddd, dd MMMM yyyy") },
                new[] { "custom", Fmt("'Signed on' dd-MMM-yyyy 'at' hh:mm tt") },
            };
            foreach (string[] v in variants)
            {
                string opt = "{\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\",\"reason\":\"Approved\","
                    + "\"date\":\"" + v[1] + "\",\"page\":1,\"rect\":[300,55,575,175],\"pades\":true}";
                Save("18_date_" + v[0] + ".pdf", Atick.SignPfx(pdf, pfx, opt));
            }
        }
        static string Fmt(string pattern) { return DateTime.Now.ToString(pattern, CultureInfo.InvariantCulture); }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
