// ATick for .NET example — DocMDP certification + lock all fields.
using System;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class CertifyLock
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf3 = File.ReadAllBytes(Path.Combine("samples", "blank3.pdf"));

            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            string pl = "[[1,[40,640,260,750]],[2,[330,380,560,490]],[3,[180,60,400,170]]]";
            string baseOpts = "\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\",\"reason\":\"Approved\",\"date\":\"" + now
                + "\",\"placements\":" + pl + ",\"mode\":\"single\",\"pades\":true";
            // certify = 2 (DocMDP FORM_FILLING — later form fill / signing still allowed)
            Save("04_certified_formfill.pdf", Atick.SignPfx(pdf3, pfx, "{" + baseOpts + ",\"certify\":2}"));
            // certify = 1 (DocMDP NO_CHANGES) and lock every form field
            Save("04_certified_locked.pdf", Atick.SignPfx(pdf3, pfx, "{" + baseOpts + ",\"certify\":1,\"lock_fields\":[\"*\"]}"));
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
