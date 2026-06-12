// ATick for .NET example — Deferred (TWO-STEP) signing for a REMOTE key
// (eSign ESP / HSM / smart card), shown on several pages.
//
// When the private key lives elsewhere, ATick splits signing into two steps:
//   1) Atick.Prepare(pdf, options) -> (Prepared, BytesToSign)
//        BytesToSign      = the exact bytes that must be signed (the ByteRange)
//        sha256(BytesToSign) = their hash -> send THIS to your eSign service if it wants a hash
//   2) your signer produces a DETACHED PKCS#7/CMS over BytesToSign
//   3) Atick.Embed(prepared, cms) -> signedPdf
//
// Below, ATick itself (Atick.CmsPfx) stands in for the external signer so the demo runs with no extra
// setup — replace that block with YOUR eSign ESP / HSM / token call (it just returns a detached CMS
// over BytesToSign).
using System;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class DeferredEsign
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf3 = File.ReadAllBytes(Path.Combine("samples", "blank3.pdf"));

            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);

            // ---- STEP 1: prepare (no key needed) -> appearance on every page + bytes-to-sign ----
            string pl = "[[1,[40,640,260,750]],[2,[330,380,560,490]],[3,[180,60,400,170]]]";
            var (prepared, bytesToSign) = Atick.Prepare(pdf3, "{\"cn\":\"DS TEST CERTIFICATE 06\",\"reason\":\"eSign\",\"date\":\"" + now
                + "\",\"placements\":" + pl + ",\"mode\":\"single\",\"field_name\":\"Signature1\","
                + "\"signer_name\":\"DS TEST CERTIFICATE 06\",\"contents_size\":16384}");
            byte[] digest = SHA256.HashData(bytesToSign);
            Console.WriteLine("STEP 1: hash to send to the signer: " + Convert.ToHexString(digest).ToLowerInvariant());

            // ---- STEP 2: the EXTERNAL signer makes a detached CMS over bytesToSign ----
            //   >>> Replace this whole block with your eSign ESP / HSM / token call. <<<
            //   Here ATick itself stands in for the external signer (Atick.CmsPfx makes a detached CMS over
            //   the given bytes), so the demo runs with no extra setup. In production this CMS comes from the ESP.
            byte[] cms = Atick.CmsPfx(bytesToSign, pfx, "{\"password\":\"ABC12\",\"hash_algo\":\"sha256\"}");

            // ---- STEP 3: embed the CMS ----
            Save("08_deferred.pdf", Atick.Embed(prepared, cms));
            Console.WriteLine("  signed on 3 pages via the two-step eSign flow");
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
