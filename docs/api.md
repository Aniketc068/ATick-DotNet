# API reference

All operations are static methods on `Aniketc068.ATick.Atick`. Every method takes raw `byte[]`
for PDFs and certificates, and an options JSON string where applicable. On any failure a method
throws `AtickException` — a sealed class extending `Exception`. The error text is available from
`e.Message`.

```csharp
using Aniketc068.ATick;
```

## Signing

```csharp
static byte[] SignPfx(byte[] pdf, byte[] pfx, string optionsJson)
```

Sign `pdf` with a `.pfx`/`.p12`/`.pem` credential (the format is auto-detected). For a PEM file pass
the password as the empty string `""` inside the options. Returns the signed PDF bytes.

- **pdf** — the PDF to sign.
- **pfx** — the credential bytes (`.pfx`, `.p12`, or `.pem`).
- **optionsJson** — the options JSON (see the [Options](#options-json) table). Pass the credential
  password as the `password` key; use `""` for PEM.
- **returns** — the signed PDF as `byte[]`.

```csharp
using Aniketc068.ATick;

byte[] pdf = File.ReadAllBytes("in.pdf");
byte[] pfx = File.ReadAllBytes("signer.pfx");

string options = """
    {
      "password": "secret",
      "cn": "Aniket Chaturvedi",
      "reason": "Approval",
      "page": 1,
      "rect": [40, 40, 240, 140],
      "pades": true,
      "timestamp": true,
      "tsa_url": "http://timestamp.example/tsa"
    }
    """;

try
{
    byte[] signed = Atick.SignPfx(pdf, pfx, options);
    File.WriteAllBytes("signed.pdf", signed);
}
catch (AtickException e)
{
    Console.Error.WriteLine("signing failed: " + e.Message);
}
```

```csharp
static byte[] SignField(byte[] pdf, byte[] pfx, string optionsJson)
```

Sign an existing empty signature field. Use the `field_name` option to select the field. Returns the
signed PDF bytes.

- **pdf** — a PDF containing an empty signature field (see [`PrepareFields`](#field-templates)).
- **pfx** — the credential bytes.
- **optionsJson** — must include `field_name`; same credential and signing keys as `SignPfx`.
- **returns** — the signed PDF as `byte[]`.

## Deferred / remote-key signing

These three methods cover the deferred (eSign / HSM / remote-key) flow: prepare the PDF, sign the
returned bytes elsewhere, then embed the resulting CMS.

```csharp
static (byte[] Prepared, byte[] BytesToSign) Prepare(byte[] pdf, string optionsJson)
```

Step 1 of deferred signing. Adds an empty signature field, the appearance, and the signature
container, then returns the exact bytes that must be signed. Returns a value tuple of two elements:

- `Prepared` — the **prepared PDF** (`byte[]`).
- `BytesToSign` — the **bytes to sign** (`byte[]`); hash and sign these with the remote key.

- **pdf** — the PDF to prepare.
- **optionsJson** — appearance and signing options (see the [Options](#options-json) table).
- **returns** — `(byte[] Prepared, byte[] BytesToSign)`.

```csharp
static byte[] CmsPfx(byte[] data, byte[] pfx, string optionsJson)
```

Produce a detached PKCS#7 / CMS signature over `data` using a PFX. Useful for producing the CMS that
[`Embed`](#embed) expects when the signing credential is a local PFX.

- **data** — the bytes to sign (typically `BytesToSign` from `Prepare`).
- **pfx** — the credential bytes.
- **optionsJson** — `password`, `hash_algo`, `pades`, `timestamp`, `tsa_url`, `tsa_auth`, `ltv`.
- **returns** — the detached CMS as `byte[]`.

(embed)=
```csharp
static byte[] Embed(byte[] prepared, byte[] cms)
```

Embed a detached CMS / PKCS#7 into a prepared PDF. Returns the signed PDF bytes.

- **prepared** — the prepared PDF (`Prepared` from `Prepare`).
- **cms** — the detached CMS (from `CmsPfx`, an eSign reply, or an HSM).
- **returns** — the signed PDF as `byte[]`.

```csharp
using Aniketc068.ATick;

var (prepared, bytesToSign) = Atick.Prepare(pdf, options);

byte[] cms    = Atick.CmsPfx(bytesToSign, pfx, "{\"password\":\"secret\"}");
byte[] signed = Atick.Embed(prepared, cms);
```

(field-templates)=
## Field templates

```csharp
static byte[] PrepareFields(byte[] pdf, string optionsJson)
```

Create an empty signature field as a template: the appearance is drawn, but the signature is left
empty so it can be signed later with [`SignField`](#signing). Returns the PDF bytes.

- **pdf** — the PDF to add the field to.
- **optionsJson** — appearance options plus `field_name`, `page`, `rect` / `placements`.
- **returns** — the PDF with an empty field as `byte[]`.

## Long-term validation & timestamps

```csharp
static byte[] AddDocTimestamp(byte[] pdf, string optionsJson)
```

Add an archive DocTimeStamp (and the DSS validation material) to an already-signed PDF, producing a
PAdES-B-LTA document. Returns the timestamped PDF bytes.

- **pdf** — an already-signed PDF.
- **optionsJson** — `tsa_url`, `tsa_auth`, `ltv`, `contents_size`.
- **returns** — the timestamped PDF as `byte[]`.

## Documents & utilities

```csharp
static byte[] SetMetadata(byte[] pdf, string optionsJson)
```

Set the document information (`/Info`) metadata on a PDF. Returns the updated PDF bytes.

- **pdf** — the PDF to update.
- **optionsJson** — `title`, `author`, `subject`, `keywords`, `application`, `created`, `modified`
  (see the [Metadata options](#metadata-options) table).
- **returns** — the updated PDF as `byte[]`.

```csharp
static byte[] Decrypt(byte[] pdf, string password)
```

Decrypt a password-protected PDF. Returns the plaintext PDF bytes.

- **pdf** — the encrypted PDF.
- **password** — the open (user) password.
- **returns** — the decrypted PDF as `byte[]`.

```csharp
static void SetFastSigning(bool on)
```

Enable or disable the in-memory revocation cache (used to speed up repeated CRL/OCSP lookups).
Passing `false` disables it.

- **on** — `true` to enable the cache, `false` to disable it.

```csharp
static string Version()
```

Return the engine version string.

- **returns** — the version as a `string`.

```csharp
Console.WriteLine("ATick " + Atick.Version());
```

(options-json)=
## Options JSON

The `optionsJson` argument is a JSON object string. All keys are optional unless a method note says
otherwise. Keys are grouped below by purpose.

### Identity & appearance text

| Key | Type | Meaning |
| --- | --- | --- |
| `cn` | string | Common name shown in the appearance. |
| `org` | string | Organisation line. |
| `ou` | string | Organisational unit line. |
| `location` | string | Signing location, also written to the signature. |
| `reason` | string | Reason for signing, also written to the signature. |
| `text` | string | Free text shown in the appearance. |
| `date` | string | Date string shown in the appearance. |
| `dn` | string | Full distinguished name line. |
| `body` | string | Custom-text-only appearance body (`\n` = new line, `*x*` = bold). |
| `heading` | string | Heading line above the signature details. |

### Verified mark

| Key | Type | Meaning |
| --- | --- | --- |
| `show_mark` | bool | Draw the verified mark. |
| `green_tick` | bool | Use the "?" verified mark. |
| `always_check` | bool | Always draw the verified/checked mark. |
| `mark_color` | string hex / name / `[r,g,b]` | Colour of the mark. |
| `mark_gradient` | array of colours | Gradient fill for the mark. |
| `mark_scale` | number | Scale factor for the mark size. |

### Layout & styling

| Key | Type | Meaning |
| --- | --- | --- |
| `text_color` | string hex / name / `[r,g,b]` | Text colour. |
| `bg_color` | string hex / name / `[r,g,b]` | Background colour of the appearance. |
| `border` | bool | Draw a border around the appearance. |
| `font_size` | number | Font size of the appearance text. |
| `width` | number | Appearance width. |
| `height` | number | Appearance height. |

### Placement

| Key | Type | Meaning |
| --- | --- | --- |
| `page` | int | Page number for the signature (1-based). |
| `rect` | `[x1, y1, x2, y2]` | Rectangle of the appearance on `page`. |
| `placements` | `[[page, [x1, y1, x2, y2]], ...]` | Multiple appearance placements (one signature, several pages). |
| `mode` | `"single"` \| `"shared"` | Whether placements share one signature (`"single"`) or are separate. |
| `field_name` | string | Name of the signature field. |

### Cryptography & PAdES

| Key | Type | Meaning |
| --- | --- | --- |
| `pades` | bool | Produce a PAdES signature. |
| `hash_algo` | `"sha256"` \| `"sha384"` \| `"sha512"` | Digest algorithm. |
| `timestamp` | bool | Add an RFC-3161 signature timestamp. |
| `tsa_url` | string | Timestamp authority URL. |
| `tsa_auth` | `["user", "pass"]` | Basic-auth credentials for the TSA. |
| `ltv` | bool | Add long-term validation material (DSS). |
| `lta` | bool | Add an archive DocTimeStamp (PAdES-B-LTA). |
| `contents_size` | int | Size of the signature `/Contents` placeholder (default `16384`). |

### Certification & locking

| Key | Type | Meaning |
| --- | --- | --- |
| `certify` | int | Certification level: `1` = no changes, `2` = form filling, `3` = form filling + annotations. |
| `lock_fields` | `["*"]` or names | Fields to lock after signing (`["*"]` = all). |

### Verification

| Key | Type | Meaning |
| --- | --- | --- |
| `verify` | bool | Verify the certificate before signing. |
| `verify_expiry` | bool | Check certificate validity dates. |
| `verify_crl` | bool | Check the CRL. |
| `verify_ocsp` | bool | Check OCSP. |

### Document security

| Key | Type | Meaning |
| --- | --- | --- |
| `open_password` | string | User/open password for the output PDF. |
| `encrypt_password` | string | Password used to encrypt the output PDF. |
| `owner_password` | string | Owner/permissions password for the output PDF. |

(metadata-options)=
## Metadata options

These keys apply to [`SetMetadata`](#documents-utilities).

| Key | Type | Meaning |
| --- | --- | --- |
| `title` | string | Document title. |
| `author` | string | Document author. |
| `subject` | string | Document subject. |
| `keywords` | string | Document keywords. |
| `application` | string | Creating/producing application. |
| `created` | string | Creation date. |
| `modified` | string | Modification date. |

## Exceptions

```csharp
public sealed class AtickException : Exception
```

Thrown by every `Atick` operation on failure — bad password, malformed PDF, network error, invalid
options, and so on. The error text is available from `Message`.

```csharp
try
{
    byte[] signed = Atick.SignPfx(pdf, pfx, options);
}
catch (AtickException e)
{
    Console.Error.WriteLine("ATick error: " + e.Message);
}
```
