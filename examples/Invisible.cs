// ATick for .NET example — invisible signature (placements: [] -> nothing drawn).
using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class Invisible
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf1 = File.ReadAllBytes(Path.Combine("samples", "blank.pdf"));
            byte[] signed = Atick.SignPfx(pdf1, pfx, "{\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\","
                + "\"reason\":\"Invisible approval\",\"placements\":[],\"field_name\":\"InvisibleSignature\","
                + "\"pades\":true,\"timestamp\":true,\"ltv\":true}");
            Save("16_invisible.pdf", signed);
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
