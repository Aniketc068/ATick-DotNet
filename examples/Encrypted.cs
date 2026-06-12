// ATick for .NET example — password-protected output, signature stays valid.
using System;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class Encrypted
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf1 = File.ReadAllBytes(Path.Combine("samples", "blank.pdf"));

            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            string baseOpts = "\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\",\"reason\":\"Approved\",\"date\":\"" + now
                + "\",\"page\":1,\"rect\":[300,55,575,175],\"pades\":true";
            // encrypt_password -> output PDF needs this password to open; signature stays valid (B-B/B-T only)
            Save("10_signed_encrypted.pdf", Atick.SignPfx(pdf1, pfx, "{" + baseOpts + ",\"encrypt_password\":\"secret\"}"));
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
