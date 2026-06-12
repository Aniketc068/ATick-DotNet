using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class DocumentTimestamp
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf = File.ReadAllBytes(Path.Combine("samples", "blank.pdf"));
            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            // 1) sign B-LT
            byte[] signed = Atick.SignPfx(pdf, pfx, "{\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\","
                + "\"reason\":\"Approved\",\"date\":\"" + now + "\",\"page\":1,\"rect\":[300,55,575,175],"
                + "\"pades\":true,\"timestamp\":true,\"ltv\":true}");
            // 2) add a standalone archive DocTimeStamp -> B-LTA
            byte[] lta = Atick.AddDocTimestamp(signed, "{}");
            Save("document_timestamp.pdf", lta);
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
