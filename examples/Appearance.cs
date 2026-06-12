// ATick for .NET example — full appearance via the Style options.
using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class Appearance
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf3 = File.ReadAllBytes(Path.Combine("samples", "blank3.pdf"));
            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            string pl = "[[1,[320,600,575,720]],[2,[40,360,295,480]],[3,[170,55,425,175]]]";
            byte[] signed = Atick.SignPfx(pdf3, pfx, "{\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\","
                + "\"org\":\"Acme Corp CA\",\"ou\":\"Class 3\",\"location\":\"New Delhi, India\","
                + "\"reason\":\"Approved for release\",\"text\":\"Verified by ATick\",\"date\":\"" + now
                + "\",\"width\":240,\"height\":120,\"placements\":" + pl + ",\"mode\":\"single\",\"pades\":true}");
            Save("03_appearance.pdf", signed);
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
