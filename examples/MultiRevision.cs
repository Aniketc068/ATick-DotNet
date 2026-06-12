// ATick for .NET example — sign, then sign the signed PDF again (revisions).
using System;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class MultiRevision
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] r1 = Atick.SignPfx(File.ReadAllBytes(Path.Combine("samples", "blank.pdf")), pfx, Opt("Rev1", new int[] { 300, 55, 575, 175 })); Save("rev1.pdf", r1);
            byte[] r2 = Atick.SignPfx(r1, pfx, Opt("Rev2", new int[] { 40, 640, 260, 750 })); Save("rev2.pdf", r2);
            byte[] r3 = Atick.SignPfx(r2, pfx, Opt("Rev3", new int[] { 40, 400, 260, 510 })); Save("rev3.pdf", r3);
        }
        static string Opt(string field, int[] rect)
        {
            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            return "{\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\",\"reason\":\"Approved\",\"date\":\"" + now
                + "\",\"field_name\":\"" + field + "\",\"page\":1,\"rect\":[" + rect[0] + "," + rect[1] + "," + rect[2] + "," + rect[3] + "],\"pades\":true}";
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
