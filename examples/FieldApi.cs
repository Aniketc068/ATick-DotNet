using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class FieldApi
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf = File.ReadAllBytes(Path.Combine("samples", "blank.pdf"));
            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            // 1) prepare an empty signing field (template) with the ATick appearance
            byte[] template = Atick.PrepareFields(pdf, "{\"cn\":\"DS TEST CERTIFICATE 06\",\"reason\":\"Approved\","
                + "\"date\":\"" + now + "\",\"field_name\":\"Sig1\",\"page\":1,\"rect\":[300,55,575,175],\"pades\":true}");
            Save("14_prepared_fields_template.pdf", template);
            // 2) sign that field
            byte[] signed = Atick.SignField(template, pfx, "{\"password\":\"ABC12\",\"field_name\":\"Sig1\","
                + "\"reason\":\"Approved\",\"pades\":true}");
            Save("14_sign_field.pdf", signed);
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
