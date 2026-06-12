<div align="center">

<img src="https://raw.githubusercontent.com/Aniketc068/ATick-DotNet/main/assets/atick_logo.png" alt="ATick" width="260"/>

# ATick for .NET

**Standalone PDF digital-signature library for .NET — PAdES / CMS signing with no external services.**

[![NuGet](https://img.shields.io/nuget/v/ATick?color=2ea44f&label=nuget)](https://www.nuget.org/packages/ATick/)
[![.NET](https://img.shields.io/badge/.NET-Framework%202.0%20to%20latest-512BD4)](https://dotnet.microsoft.com/)
[![PAdES](https://img.shields.io/badge/PAdES-B--B%20%7C%20B--T%20%7C%20B--LT%20%7C%20B--LTA-success)](#pades-levels)
[![Cross-platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-brightgreen)](#compatibility--one-package-everywhere)
[![License: AGPL v3](https://img.shields.io/badge/license-AGPL--3.0-blue)](LICENSE)
[![Also for Python](https://img.shields.io/badge/also%20for-Python-3776AB?logo=python&logoColor=white)](https://github.com/Aniketc068/ATick)
[![Also for Java](https://img.shields.io/badge/also%20for-Java-007396?logo=openjdk&logoColor=white)](https://github.com/Aniketc068/ATick-Java)

</div>

> **Also available for Python** (`pip install atick` — [ATick for Python](https://github.com/Aniketc068/ATick)) and
> **Java** (`io.github.aniketc068:atick` — [ATick for Java](https://github.com/Aniketc068/ATick-Java)).

---

ATick signs PDFs the way Adobe Acrobat and the EU DSS do — **PAdES baseline** signatures with
timestamps and long-term validation. It calls a bundled native engine through **P/Invoke**, so there
is **no external service** and **nothing to configure**. The matching engine for your OS/arch ships
**inside the package** and is loaded automatically. Add one NuGet package and you are done.

```csharp
using Aniketc068.ATick;

byte[] pdf = File.ReadAllBytes("doc.pdf");
byte[] pfx = File.ReadAllBytes("my.pfx");

byte[] signed = Atick.SignPfx(pdf, pfx,
    "{\"password\":\"••••\",\"cn\":\"Aniket Chaturvedi\",\"reason\":\"Approved\"," +
    "\"green_tick\":true,\"page\":1,\"rect\":[300,55,575,175]," +
    "\"pades\":true,\"timestamp\":true,\"ltv\":true}");   // PAdES-B-LT

File.WriteAllBytes("signed.pdf", signed);
```

---

## The green tick your readers trust

ATick draws a verified-signature appearance with a green tick. When the certificate is valid and
trusted, Adobe Reader / Acrobat shows **“Signed and all signatures are valid.”**

<div align="center">
<img src="https://raw.githubusercontent.com/Aniketc068/ATick-DotNet/main/assets/valid_signature_adobe.png" alt="Adobe — signed and all signatures are valid" width="560"/>
</div>

Adobe colours that same mark by the signature's real status — you don't draw these, Adobe does:

<table align="center">
<tr>
<td align="center"><img src="https://raw.githubusercontent.com/Aniketc068/ATick-DotNet/main/assets/signature_appearance.png" width="190"/><br/><b>Valid &amp; trusted</b><br/>green tick</td>
<td align="center"><img src="https://raw.githubusercontent.com/Aniketc068/ATick-DotNet/main/assets/sig_unknown.png" width="190"/><br/><b>Validity unknown</b><br/>yellow “?”</td>
<td align="center"><img src="https://raw.githubusercontent.com/Aniketc068/ATick-DotNet/main/assets/sig_notverified.png" width="190"/><br/><b>Not verified</b><br/>“?” not validated</td>
<td align="center"><img src="https://raw.githubusercontent.com/Aniketc068/ATick-DotNet/main/assets/sig_invalid.png" width="190"/><br/><b>Invalid</b><br/>red cross</td>
</tr>
</table>

The **green** tick appears only when the signature is valid *and* the certificate chains to a root
Adobe trusts.

---

## Why ATick

| | ATick for .NET |
|---|---|
| **Zero external services** | the crypto, PKCS#12/PEM, image decode, timestamp & LTV are all in the engine |
| **One package** | the native engine for every platform is bundled — `dotnet add package ATick` and run |
| **Full PAdES** | B-B, B-T, B-LT, B-LTA — recognised by Adobe Acrobat as *“PAdES Signature Level”* |
| **Deferred / remote keys** | two-step `Prepare` → external CMS → `Embed` for a token / HSM / smart-card / Windows store via your own provider |
| **Cross-platform** | Windows (64/32-bit), Linux (x64/ARM64/ARM), macOS (Intel + Apple Silicon) — .NET 6 and up |
| **Clear errors** | every failure is a normal `AtickException` you can catch |

---

## Features (A → Z)

| Feature | How |
|---|---|
| **Sign with a `.pfx` / `.p12` / `.pem`** | `Atick.SignPfx(pdf, pfx, options)` — PKCS#12 or PEM (key + certs), auto-detected |
| **PAdES levels** B-B / B-T / B-LT / B-LTA | `"pades":true` + `"timestamp":true` + `"ltv":true` + `"lta":true` |
| **Hash algorithm** | `"hash_algo":"sha256" \| "sha384" \| "sha512"` |
| **Timestamp authority** | built in — or your own with `"tsa_url":"…"` (and `"tsa_auth":["user","pass"]`) |
| **Long-term validation (LTV)** | `"ltv":true` embeds the chain + revocation (CRL/OCSP) |
| **Multi-page / custom coordinates** | `"placements":[[page,[x1,y1,x2,y2]], …]` |
| **Signature layout** | `"mode":"single"` (one signature on many pages) · `"mode":"shared"` (many fields, same value) |
| **Multi-signatory** | sign an already-signed PDF again — each signature is its own revision, all stay valid |
| **Certification (DocMDP)** | `"certify":1` (no changes) · `2` (form filling) · `3` (form filling + annotations) |
| **Field locking (FieldMDP)** | `"lock_fields":["*"]` or `["FieldA", …]` |
| **Pre-sign checks** | `"verify_expiry":true`, `"verify_crl":true`, `"verify_ocsp":true` (or `"verify":true`) |
| **Document metadata** | `Atick.SetMetadata(pdf, options)` |
| **Password protection** | `"encrypt_password"` (+ `"owner_password"`) for output; `"open_password"` for input; `Atick.Decrypt(pdf, pw)` |
| **Appearance** | options `cn, org, ou, location, reason, text, date, dn, body, heading, image` — auto-fit text, transparent logo |
| **The mark** | the `?` (Adobe greens it), an always-green tick, or nothing — see [The mark](#the-mark) |
| **CN on the left** (Adobe-style) | `"image":"cn"` |
| **Distinguished name** | `"dn":"CN=…, O=…, C=IN"` |
| **Custom-text-only appearance** | `"body":"*APPROVED*\nby *Aniket*"` — `\n` = line, `*x*` = bold |
| **Invisible signature** | `"placements":[]` |
| **Sign an already-signed PDF** | sign again (incremental) — existing signatures stay valid; use a fresh `"field_name"` |
| **Container only** | `Atick.PrepareFields(pdf, options)` |
| **Document timestamp** | `"lta":true` while signing; `Atick.AddDocTimestamp(pdf, options)` afterwards (PAdES-B-LTA) |
| **Fast signing** | revocation cache (ON by default) — `Atick.SetFastSigning(false)` to disable |
| **Deferred / eSign (2-step)** | `Atick.Prepare(pdf, options)` → external CMS → `Atick.Embed(prepared, cms)` |
| **Detached CMS** | `Atick.CmsPfx(data, pfx, options)` |

---

## Install

```bash
dotnet add package ATick
```

or in your `.csproj`:

```xml
<PackageReference Include="ATick" Version="1.0.3" />
```

The native engine for your platform comes with the package — nothing else to install.

---

## Compatibility — one package everywhere

- **Every .NET — .NET Framework 2.0 through the latest .NET.** The package multi-targets
  `net20`, `net35`, `net40`, `net48`, `netstandard2.0`, `net6.0` and `net8.0`, so it works on old
  .NET Framework (Windows 7 era) and on modern cross-platform .NET alike.
- **Every OS/arch** — the package bundles a native engine per platform and .NET loads the right one:

  | RID | Bundled |
  |---|---|
  | `win-x64` / `win-x86` | **Windows 7 and up**, 64 / 32-bit (one Windows-7-compatible engine covers Win 7 → 11) |
  | `win-arm64` | Windows on ARM64 |
  | `linux-x64` / `linux-arm64` / `linux-arm` | Linux x64 / ARM64 / ARM |
  | `osx-x64` / `osx-arm64` | macOS Intel / Apple Silicon |

On modern .NET the engine is resolved per-RID automatically; on .NET Framework MSBuild targets copy
the right-arch engine next to your app and ATick pre-loads it. So `ATick` is one cross-platform
dependency — just like `pip install atick` (Python) or `io.github.aniketc068:atick` (Java).

---

## The API

```csharp
Atick.SignPfx(pdf, pfx, optionsJson)          // sign with a .pfx / .p12 / .pem (auto-detected)
Atick.Prepare(pdf, optionsJson)               // deferred / eSign: returns (Prepared, BytesToSign)
Atick.CmsPfx(data, pfx, optionsJson)          // detached CMS over data
Atick.Embed(prepared, cms)                    // embed a detached CMS into a prepared PDF
Atick.PrepareFields(pdf, optionsJson)         // make an empty signature field (template)
Atick.SignField(pdf, pfx, optionsJson)        // sign an existing empty field
Atick.SetMetadata(pdf, optionsJson)           // Title / Author / Subject / Keywords / …
Atick.AddDocTimestamp(pdf, optionsJson)       // archive DocTimeStamp (PAdES-B-LTA)
Atick.SetFastSigning(true | false)            // revocation-cache toggle
Atick.Decrypt(pdf, password)                  // decrypt a password-protected PDF
Atick.Version()                               // engine version
```

All options are a JSON string. Any failure throws `AtickException`.

### Options (JSON)

`cn, org, ou, location, reason, text, date, dn, body, heading, show_mark, green_tick, always_check,
mark_color (hex / name / [r,g,b]), mark_gradient, mark_scale, text_color, bg_color, border, font_size,
width, height, page, rect, placements ([[page,[x1,y1,x2,y2]], …]), mode (single/shared), field_name,
pades, hash_algo (sha256/384/512), timestamp, tsa_url, tsa_auth, ltv, lta, certify, lock_fields,
verify, verify_expiry, verify_crl, verify_ocsp, open_password, encrypt_password, owner_password,
contents_size`.

---

## The mark

```csharp
"{… ,\"green_tick\":true}"      // the "?" mark — Adobe paints it GREEN for valid+trusted, RED if invalid
"{… ,\"always_check\":true}"    // the green-tick graphic as the base
"{… ,\"green_tick\":false}"     // no mark — a plain signature
```

Colour it: `"mark_color":"#E53935"`, `"blue"`, `[255,140,0]` — or a gradient
`"mark_gradient":["red","orange","yellow"]`.

---

## Deferred signing & Indian eSign (two-step)

When the private key lives elsewhere (a USB token / HSM / smart-card / Windows store via your own
provider, or an eSign ESP):

```csharp
// 1) prepare (no key): appearance + the exact bytes to sign
var (prepared, bytesToSign) = Atick.Prepare(pdf,
    "{\"cn\":\"DS TEST\",\"reason\":\"eSign\",\"placements\":[[1,[300,55,575,175]]],\"contents_size\":16384}");

// 2) your signer (token / HSM / eSign ESP) makes a DETACHED CMS over bytesToSign.
//    The eSign InputHash is just the SHA-256 of bytesToSign:
byte[] digest = System.Security.Cryptography.SHA256.HashData(bytesToSign);
//    ... sign with your provider, get back a detached CMS ...

// 3) embed
byte[] signed = Atick.Embed(prepared, cms);
```

---

## PAdES levels

```csharp
Atick.SignPfx(pdf, pfx, "{… ,\"pades\":true}")                                       // B-B
Atick.SignPfx(pdf, pfx, "{… ,\"pades\":true,\"timestamp\":true}")                    // B-T
Atick.SignPfx(pdf, pfx, "{… ,\"pades\":true,\"timestamp\":true,\"ltv\":true}")       // B-LT
Atick.SignPfx(pdf, pfx, "{… ,\"pades\":true,\"timestamp\":true,\"lta\":true}")       // B-LTA
```

---

## Examples

Self-contained console programs live in [`examples/`](examples/) — one per feature, mirroring the
Python and Java examples (sign, PAdES levels, appearance, mark colours, hashes, certify/lock,
multi-placement, encrypted, multi-revision, dates, deferred eSign, document timestamp, metadata,
field API, PEM, verify, sign-already-signed, fast signing, container).

```bash
cd examples/SignPfx
dotnet run
```

---

## Errors

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

---

## License

ATick is **dual-licensed** — free for personal & open use, paid if you sell:

- **Free under [GNU AGPL-3.0](LICENSE)** — personal projects, learning, internal use, and
  open-source projects (released publicly under AGPL-3.0).
- **Commercial license (paid)** — if you **build a product with ATick and sell it**, or use it in a
  **closed-source / commercial** product, you must buy a commercial license first. Contact
  **aniketc.pro@gmail.com** for a quote.

See [LICENSING.md](LICENSING.md) for details. © 2026 Aniket Chaturvedi.
