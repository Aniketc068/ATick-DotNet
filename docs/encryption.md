# Encryption

ATick for .NET reads and writes password-protected PDFs through the same `Atick.SignPfx` entry point,
plus a dedicated `Atick.Decrypt` helper. All passwords are passed as keys inside the
options JSON string.

```csharp
using Aniketc068.ATick;
using System.IO;
```

| Option key        | Applies to    | Meaning                                                              |
| ----------------- | ------------- | -------------------------------------------------------------------- |
| `open_password`   | Input PDF     | Password used to open an already-encrypted PDF before signing it.    |
| `encrypt_password`| Output PDF    | User password — required to open the signed PDF that ATick produces. |
| `owner_password`  | Output PDF    | Owner/permissions password for the signed output (optional).         |

## Password-protect the output

Add `encrypt_password` to encrypt the signed PDF that ATick writes. Supply `owner_password`
as well to set a separate owner/permissions password; if you omit it, the owner password
defaults to the user password.

```csharp
using Aniketc068.ATick;
using System.IO;

byte[] pdf = File.ReadAllBytes("contract.pdf");
byte[] pfx = File.ReadAllBytes("signer.pfx");

byte[] signed = Atick.SignPfx(pdf, pfx,
    "{\"password\":\"••••\",\"encrypt_password\":\"open-me\",\"owner_password\":\"owner\"}");

File.WriteAllBytes("contract-signed.pdf", signed);
```

```{admonition} The signature stays valid
:class: note
The output is AES-128 encrypted. The signature's `/Contents` is exempt from encryption,
so the signed byte range still verifies in any compliant PDF reader.
```

## Sign an encrypted input

If the input PDF is already password-protected, pass `open_password` so ATick can open it
before signing. The decrypted document is signed and then written back out (encrypt the
output again with `encrypt_password` if you want the result to stay protected).

```csharp
using Aniketc068.ATick;
using System.IO;

byte[] pdf = File.ReadAllBytes("locked.pdf");
byte[] pfx = File.ReadAllBytes("signer.pfx");

byte[] signed = Atick.SignPfx(pdf, pfx,
    "{\"password\":\"••••\",\"open_password\":\"the-input-password\"}");

File.WriteAllBytes("locked-signed.pdf", signed);
```

```{tip}
You can combine the keys: open an encrypted input with `open_password` and re-encrypt the
signed output in one call by also passing `encrypt_password` (and optionally `owner_password`).
```

## Decrypt a PDF

Use `Atick.Decrypt` to strip the password protection from a PDF and obtain its plaintext bytes.

```csharp
using Aniketc068.ATick;
using System.IO;

byte[] encrypted = File.ReadAllBytes("locked.pdf");

byte[] plain = Atick.Decrypt(encrypted, "the-password");

File.WriteAllBytes("unlocked.pdf", plain);
```

## Handling failures

Both `Atick.SignPfx` and `Atick.Decrypt` throw `AtickException` on failure — for
example, when a password is wrong or the input PDF is not actually encrypted.

```csharp
using Aniketc068.ATick;
using System.IO;

try
{
    byte[] plain = Atick.Decrypt(File.ReadAllBytes("locked.pdf"), "wrong-pw");
    File.WriteAllBytes("unlocked.pdf", plain);
}
catch (AtickException e)
{
    Console.Error.WriteLine("Could not decrypt PDF: " + e.Message);
}
```
