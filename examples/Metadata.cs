using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class Metadata
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf = File.ReadAllBytes(Path.Combine("samples", "blank.pdf"));
            byte[] withMeta = Atick.SetMetadata(pdf, "{\"title\":\"ATick Demo\",\"author\":\"Aniket Chaturvedi\","
                + "\"subject\":\"Digital signature\",\"keywords\":\"pdf,pades,atick\",\"application\":\"ATick\"}");
            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            byte[] signed = Atick.SignPfx(withMeta, pfx, "{\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\","
                + "\"reason\":\"Approved\",\"date\":\"" + now + "\",\"green_tick\":true,\"page\":1,\"rect\":[300,55,575,175],\"pades\":true}");
            Save("12_metadata.pdf", signed);
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
