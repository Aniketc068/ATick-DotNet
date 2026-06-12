using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class Verify
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf = File.ReadAllBytes(Path.Combine("samples", "blank.pdf"));
            try
            {
                string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
                byte[] signed = Atick.SignPfx(pdf, pfx, "{\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\","
                    + "\"reason\":\"Approved\",\"date\":\"" + now + "\",\"green_tick\":true,\"page\":1,"
                    + "\"rect\":[300,55,575,175],\"pades\":true,\"verify_expiry\":true}");  // reject if expired
                Save("09_verified.pdf", signed);
            }
            catch (AtickException e)
            {
                Console.WriteLine("  verify rejected this certificate: " + e.Message);
            }
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
