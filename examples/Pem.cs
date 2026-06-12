using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class Pem
    {
        public static void Run()
        {
            byte[] pem = File.ReadAllBytes(Path.Combine("samples", "ABC12.pem"));   // a .pem holding the key + cert
            byte[] pdf = File.ReadAllBytes(Path.Combine("samples", "blank.pdf"));
            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            byte[] signed = Atick.SignPfx(pdf, pem, "{\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\","
                + "\"reason\":\"Approved\",\"date\":\"" + now + "\",\"green_tick\":true,\"page\":1,\"rect\":[300,55,575,175],\"pades\":true}");
            Save("18_sign_pem.pdf", signed);
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
