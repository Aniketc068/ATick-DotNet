# Quickstart

Sign a PDF with a `.pfx` (or `.p12` / `.pem`) and a visible green-tick appearance.

```csharp
using System.IO;
using Aniketc068.ATick;

byte[] pdf = File.ReadAllBytes("doc.pdf");
byte[] pfx = File.ReadAllBytes("my.pfx");

byte[] signed = Atick.SignPfx(pdf, pfx,
    "{\"password\":\"••••\",\"cn\":\"Aniket Chaturvedi\",\"reason\":\"Approved\"," +
    "\"green_tick\":true,\"page\":1,\"rect\":[300,55,575,175]," +
    "\"pades\":true,\"timestamp\":true,\"ltv\":true}");   // PAdES-B-LT

File.WriteAllBytes("signed.pdf", signed);
```

Open `signed.pdf` in Adobe Reader — for a trusted certificate it shows a valid green tick and
**“Signed and all signatures are valid.”**

## What the options mean

| Option | Meaning |
|---|---|
| `password` | the PFX/P12 password (use `""` for a PEM file) |
| `cn` | the signer name shown in the appearance |
| `reason` | the reason recorded in the signature |
| `green_tick` | draw the `?` mark Adobe greens for a valid+trusted certificate |
| `page`, `rect` | where to draw the appearance — page number and `[x1,y1,x2,y2]` (PDF points) |
| `pades` | produce a PAdES (ETSI) signature |
| `timestamp` | add an RFC-3161 timestamp (PAdES-B-T) |
| `ltv` | embed the chain + revocation for long-term validation (PAdES-B-LT) |

## A minimal, invisible signature

```csharp
byte[] signed = Atick.SignPfx(pdf, pfx,
    "{\"password\":\"••••\",\"placements\":[],\"pades\":true}");   // valid, nothing drawn
```

## Catching errors

```csharp
try
{
    Atick.SignPfx(pdf, pfx, "{\"password\":\"wrong\"}");
}
catch (AtickException e)
{
    Console.WriteLine("signing failed: " + e.Message);
}
```

Next: see [Signing](signing.md) for all the options, or the [API reference](api.md).
