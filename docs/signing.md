# Signing methods

ATick for .NET signs with a credential file or with an external key holder (USB token, smart-card,
HSM, Windows certificate store). Every signing call takes its configuration as a single **JSON
options string**, and every failure throws `AtickException`.

```csharp
using Aniketc068.ATick;
using System.IO;
```

## 1. PFX / P12 / PEM file

`Atick.SignPfx` is the primary method. It accepts both **PKCS#12** (`.pfx` / `.p12`) and **PEM** —
the format is auto-detected.

```csharp
byte[] pdf = File.ReadAllBytes("in.pdf");
byte[] pfx = File.ReadAllBytes("signer.pfx");

byte[] signed = Atick.SignPfx(pdf, pfx,
    "{\"password\":\"••••\",\"cn\":\"Aniket\",\"reason\":\"Approved\",\"pades\":true}");

File.WriteAllBytes("out.pdf", signed);
```

### PEM credentials

A PEM credential is an unencrypted PKCS#8 / PKCS#1 private key plus one or more `CERTIFICATE`
blocks. Pass its bytes as the `pfx` argument and use an empty `password` (`""`):

```csharp
byte[] pem = File.ReadAllBytes("signer.pem");

byte[] signed = Atick.SignPfx(pdf, pem,
    "{\"password\":\"\",\"cn\":\"Aniket\",\"pades\":true}");
```

```{note}
Because the format is auto-detected, the same `SignPfx` call works for `.pfx`, `.p12`, and `.pem`.
Only the `password` differs: the PKCS#12 passphrase for `.pfx`/`.p12`, and `""` for PEM.
```

## 2. USB token / smart-card / HSM / Windows store (deferred flow)

ATick for .NET does not load PKCS#11 libraries or the Windows store itself. To sign with a key that
never leaves a token, card, HSM, or the OS store, use the **deferred flow**: ATick prepares the
document and hands you the exact bytes to sign, you produce the CMS signature with your own provider
(for example `System.Security.Cryptography`, a PKCS#11 provider, or a vendor SDK), and ATick embeds
it.

```csharp
// Step 1 — prepare. Returns a value tuple (Prepared, BytesToSign).
var (prepared, bytesToSign) = Atick.Prepare(pdf,
    "{\"cn\":\"Aniket\",\"reason\":\"Approved\",\"pades\":true,\"hash_algo\":\"sha256\"}");

// Step 2 — produce a CMS signature with your own provider.
//   Sign `bytesToSign` using the token / smart-card / HSM / Windows-store key.
//   This is your own code (System.Security.Cryptography, a PKCS#11 provider, or a vendor SDK).
byte[] cms = SignWithMyProvider(bytesToSign);   // returns a CMS/PKCS#7 SignedData

// Step 3 — embed the CMS into the prepared document.
byte[] signed = Atick.Embed(prepared, cms);
File.WriteAllBytes("out.pdf", signed);
```

```{tip}
The CMS you build in step 2 must cover **`bytesToSign`** exactly and use the same `hash_algo` you
passed to `Atick.Prepare`. This is the standard eSign / detached-signature pattern: ATick owns the
PDF structure, your provider owns the private key.
```

If you have the key material in software (a `.pfx`/`.p12`/`.pem`), ATick can also build the CMS for
you with `Atick.CmsPfx`, then `Atick.Embed`:

```csharp
var (prepared, bytesToSign) = Atick.Prepare(pdf, "{\"cn\":\"Aniket\",\"pades\":true}");
byte[] cms    = Atick.CmsPfx(bytesToSign, pfx, "{\"password\":\"••••\",\"pades\":true}");
byte[] signed = Atick.Embed(prepared, cms);
```

## Common options

All signing calls (`SignPfx`, `Prepare` / `CmsPfx`, `SignField`) accept the same JSON keys.

| Key | Meaning |
|---|---|
| `"pades": true` | PAdES (`ETSI.CAdES.detached`); `false` → plain CMS (`adbe.pkcs7.detached`) |
| `"hash_algo": "sha256"` | `"sha256"`, `"sha384"`, `"sha512"` |
| `"timestamp": true` | add an RFC-3161 signature timestamp (B-T) |
| `"tsa_url": "…"`, `"tsa_auth": ["user","pass"]` | choose / authenticate the timestamp authority |
| `"ltv": true` | embed long-term validation (B-LT) |
| `"lta": true` | add a document timestamp (B-LTA) |
| `"certify": 1`, `"lock_fields": …` | certification & locking |
| `"verify": true`, `"verify_expiry"`, `"verify_crl"`, `"verify_ocsp"` | pre-sign expiry / CRL / OCSP / chain checks |
| `"field_name": "…"` | the signature field name (auto-uniquified — `Atick_1`, `Atick_2`, …) |
| `"mode": "single" \| "shared"` | one signature on many pages, or many fields sharing one value |

`SignPfx` additionally accepts `"open_password"` (decrypt an encrypted input), and
`"encrypt_password"` / `"owner_password"` (password-protect the output).

### Appearance options

The visible signature block is also configured through the same JSON: `cn`, `org`, `ou`,
`location`, `reason`, `text`, `date`, `dn`, `body`, `heading`, `show_mark`, `green_tick`,
`always_check`, `mark_color` (hex `"#E53935"`, name `"blue"`, or `[r,g,b]`), `mark_gradient`,
`mark_scale`, `text_color`, `bg_color`, `border`, `font_size`, `width`, `height`, `page`,
`rect` (`[x1,y1,x2,y2]`), and `placements` (`[[page,[x1,y1,x2,y2]], …]`).

```csharp
byte[] signed = Atick.SignPfx(pdf, pfx,
    "{\"password\":\"••••\",\"cn\":\"Aniket\",\"reason\":\"Approved\","
  + "\"show_mark\":true,\"green_tick\":true,\"mark_color\":\"#E53935\","
  + "\"page\":1,\"rect\":[36,36,236,96],\"pades\":true}");
```

## Multi-signatory (sign an already-signed PDF)

ATick signs as an **incremental update**: existing signatures keep their byte ranges and stay
valid. Just sign the already-signed PDF again — the field name is auto-uniquified so it never
collides.

```csharp
byte[] v1 = Atick.SignPfx(pdf, pfx, "{\"password\":\"••••\",\"cn\":\"Aniket\",\"pades\":true}");   // Atick_1
byte[] v2 = Atick.SignPfx(v1,  pfx, "{\"password\":\"••••\",\"cn\":\"Reviewer\",\"pades\":true}"); // Atick_2
```

The same holds for the deferred flow: run `Atick.Prepare` -> external CMS -> `Atick.Embed` on the
already-signed bytes to add another signature.
