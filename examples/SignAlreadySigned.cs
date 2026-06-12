using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class SignAlreadySigned
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] alreadySigned = File.ReadAllBytes(Path.Combine("samples", "signed.pdf"));   // an already-signed input
            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            byte[] resigned = Atick.SignPfx(alreadySigned, pfx, "{\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\","
                + "\"reason\":\"Counter-signed\",\"date\":\"" + now + "\",\"field_name\":\"Atick_2\","
                + "\"page\":1,\"rect\":[40,640,260,750],\"pades\":true}");
            Save("added_signature.pdf", resigned);
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
