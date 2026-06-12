---
sd_hide_title: true
---

# ATick for .NET

```{raw} html
<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "SoftwareApplication",
  "name": "ATick for .NET",
  "alternateName": "ATick C# PDF digital signature library",
  "description": "Standalone .NET library for PDF digital signatures — PAdES and CMS signing with a PFX/PEM file, deferred / remote-key (eSign / HSM / token) signing, RFC-3161 timestamps, long-term validation (LTV), and a green-tick verified-signature appearance that Adobe shows as valid. One NuGet package, cross-platform.",
  "applicationCategory": "DeveloperApplication",
  "operatingSystem": "Windows, Linux, macOS",
  "programmingLanguage": "C#",
  "softwareVersion": "1.0.3",
  "offers": { "@type": "Offer", "price": "0", "priceCurrency": "USD" },
  "license": "https://www.gnu.org/licenses/agpl-3.0.html",
  "author": { "@type": "Person", "name": "Aniket Chaturvedi" },
  "url": "https://atick-dotnet.readthedocs.io/",
  "codeRepository": "https://github.com/Aniketc068/ATick-DotNet",
  "downloadUrl": "https://www.nuget.org/packages/ATick/",
  "keywords": "PDF digital signature .NET, sign PDF C#, PAdES, CAdES, eSign, LTV, RFC-3161 timestamp, Adobe valid signature, green tick"
}
</script>
```

```{image} _static/green_tick.png
:class: hero-mark
:width: 78px
:align: center
```

```{rst-class} hero-title
Sign PDFs with confidence
```

```{rst-class} hero-sub
ATick for .NET is the standalone PDF digital-signature library for C# — PAdES & CMS signing,
deferred / remote-key signing and a green-tick appearance Adobe shows as valid, in **one NuGet
package**.
```

::::{div} hero-buttons
:::{button-ref} quickstart
:color: primary
:class: hero-btn
Get started  →
:::
:::{button-ref} api
:color: primary
:outline:
:class: hero-btn
API reference
:::
::::

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

## Everything you need to sign PDFs

::::{grid} 1 2 2 3
:gutter: 3

:::{grid-item-card} {octicon}`key;1.3em;sd-text-success` Sign anywhere
PFX/P12 **or PEM** files directly, and tokens / HSMs / the Windows store via the deferred flow and
your own provider — one consistent API.
+++
[Signing »](signing.md)
:::

:::{grid-item-card} {octicon}`shield-check;1.3em;sd-text-success` Full PAdES
B-B, B-T, **B-LT**, **B-LTA** with RFC-3161 timestamps and long-term validation — recognised by
Adobe Acrobat as *“PAdES Signature Level”*.
+++
[PAdES levels »](pades.md)
:::

:::{grid-item-card} {octicon}`globe;1.3em;sd-text-success` Deferred / eSign
Two-step `Prepare` → external CMS → `Embed` for remote keys (eSign ESP, HSM, token) — the InputHash
is just the SHA-256 of the bytes-to-sign.
+++
[Deferred / eSign »](esign.md)
:::

:::{grid-item-card} {octicon}`paintbrush;1.3em;sd-text-success` Rich appearance
Logo or CN-on-the-left, the validity mark (`?` / green tick), distinguished name, custom text,
invisible signatures, any date format.
+++
[Appearance »](appearance.md)
:::

:::{grid-item-card} {octicon}`lock;1.3em;sd-text-success` Trust & control
Certification (DocMDP), field-locking (FieldMDP), pre-sign expiry / CRL / OCSP checks, password
protection and metadata.
+++
[Certification »](certification.md)
:::

:::{grid-item-card} {octicon}`rocket;1.3em;sd-text-success` Built for scale
A revocation cache speeds up batch signing, multi-signatory documents stay valid, and every error is
a clean .NET exception.
+++
[Fast signing »](fast-signing.md)
:::

::::

---

## The green tick your readers trust

ATick draws a verified-signature appearance with a green tick. When the certificate is valid and
trusted, Adobe Reader / Acrobat shows **“Signed and all signatures are valid.”**

```{image} _static/valid_signature_adobe.png
:alt: Adobe Reader — signed and all signatures are valid, with the ATick green tick
:width: 600px
:align: center
:class: shadow-img
```

---

## Why ATick

```{list-table}
:header-rows: 1
:widths: 32 68

* - 
  - ATick for .NET
* - **External services**
  - none — the crypto, PKCS#12/PEM, image decode, timestamping and LTV are all built into the engine
* - **Install**
  - one NuGet package `ATick` (the native engine is included)
* - **Build step**
  - none on your side — P/Invoke to the bundled engine, no C compiler
* - **Platforms**
  - Windows (64/32-bit), Linux (x64/ARM64/ARM) and macOS — one cross-platform package
* - **Errors**
  - every failure is an `AtickException` you can catch
```

```{toctree}
:hidden:

getting-started
guide
reference
```
