// ATick for .NET example — build the ATick appearance + an EMPTY signing
// container (ByteRange + empty Contents) without signing; an external signer fills it later.
using System;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class MakeContainer
    {
        public static void Run()
        {
            byte[] pdf = File.ReadAllBytes(Path.Combine("samples", "blank.pdf"));
            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            // prepare -> (Prepared, BytesToSign); the prepared PDF is the container
            var (prepared, bytesToSign) = Atick.Prepare(pdf, "{\"cn\":\"DS TEST CERTIFICATE 06\",\"reason\":\"Approved\",\"date\":\"" + now
                + "\",\"page\":1,\"rect\":[300,55,575,175],\"pades\":true,\"contents_size\":16384}");
            Save("container.pdf", prepared);
            Console.WriteLine("  (bytes-to-sign: " + bytesToSign.Length + " bytes — hand to an external signer)");
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
