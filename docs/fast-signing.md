# Fast signing

Fast signing is **ON by default**. When LTV is enabled, the first signature fetches the
certificate's CRL/OCSP over the network; ATick then keeps that revocation in an in-memory cache, so
every later signature **with the same certificate** reuses it instead of fetching again. This is a
large speed-up for batch and multi-signature runs (≈ 6× in practice).

```csharp
using Aniketc068.ATick;

Atick.SetFastSigning(true);    // default — reuse cached revocation for the same certificate
Atick.SetFastSigning(false);   // always fetch fresh (also clears the cache)
```

## Signing with LTV

Pass options as a JSON string. The first signature with a given certificate populates the cache;
the rest reuse it.

```csharp
using Aniketc068.ATick;
using System.IO;

byte[] pdf = File.ReadAllBytes("in.pdf");
byte[] pfx = File.ReadAllBytes("my.pfx");

string options = "{\"password\":\"secret\",\"ltv\":true}";

byte[] signed = Atick.SignPfx(pdf, pfx, options);
File.WriteAllBytes("out.pdf", signed);
```

## Batch signing

Because the cache is keyed per certificate, signing many PDFs with the **same** `.pfx` fetches
revocation once and reuses it for the rest of the run.

```csharp
using Aniketc068.ATick;
using System.IO;

byte[] pfx = File.ReadAllBytes("my.pfx");
string options = "{\"password\":\"secret\",\"ltv\":true}";

string[] inputs = { "a.pdf", "b.pdf", "c.pdf", "d.pdf" };

Atick.SetFastSigning(true);   // default; shown here for clarity

foreach (string name in inputs)
{
    byte[] pdf = File.ReadAllBytes(name);
    try
    {
        byte[] signed = Atick.SignPfx(pdf, pfx, options);   // first call fetches, rest reuse cache
        File.WriteAllBytes("signed-" + name, signed);
    }
    catch (AtickException ex)
    {
        Console.Error.WriteLine("Failed to sign " + name + ": " + ex.Message);
    }
}
```

## Disabling the cache

To force a fresh CRL/OCSP fetch on every signature, turn fast signing off before signing.

```csharp
using Aniketc068.ATick;

Atick.SetFastSigning(false);   // always fetch fresh, also clears the cache
```

```{tip}
Leave fast signing on for batch runs. Turn it off only when you need each signature to reflect the
very latest revocation state.
```

## Behaviour at a glance

| Setting | First signature | Later signatures (same certificate) | Timestamps |
| --- | --- | --- | --- |
| `SetFastSigning(true)` (default) | Fetch CRL/OCSP, cache it | Reuse cached revocation | Always fresh |
| `SetFastSigning(false)` | Fetch CRL/OCSP | Fetch CRL/OCSP again | Always fresh |

## Notes

- The cache lives in **process memory** only and is gone when the process ends.
- It is keyed per request, so a **different / removed certificate** simply misses and is fetched
  fresh — there is no risk of reusing the wrong certificate's revocation.
- **Timestamps are never cached** — each signature must carry its own unique RFC-3161 token, so the
  timestamp authority is always contacted per signature.
- Any failure (bad password, network error, malformed PDF) throws `AtickException`; wrap calls
  in a `try`/`catch` as shown in the batch loop above.
