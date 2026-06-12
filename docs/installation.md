# Installation

ATick for .NET is one NuGet package. The matching native engine for your OS/arch ships inside it and
is loaded automatically — there is no C compiler or build step on your side.

## Requirements

- **Any .NET** — .NET Framework 2.0 / 3.5 / 4.x (Windows 7 era), .NET Standard 2.0, or modern
  .NET 5/6/7/8 and up. The package multi-targets them all.
- Any supported OS/arch — Windows 7+ (x86/x64/ARM64), Linux (x64/ARM64/ARM), macOS (Intel/Apple Silicon).

## Install

```bash
dotnet add package ATick
```

`.csproj`:

```xml
<PackageReference Include="ATick" Version="1.0.3" />
```

Package Manager Console:

```powershell
Install-Package ATick
```

## One package, every platform

The package bundles a native engine per platform and .NET loads the right one at runtime, so the same
dependency works everywhere.

| RID | Bundled |
|---|---|
| `win-x64` / `win-x86` | Windows 7 → 11, 64 / 32-bit (Windows-7-compatible engine) |
| `win-arm64` | Windows on ARM64 |
| `linux-x64` | Linux x64 |
| `linux-arm64` / `linux-arm` | Linux ARM64 / ARM |
| `osx-x64` / `osx-arm64` | macOS Intel / Apple Silicon |

## Verify the install

```csharp
using Aniketc068.ATick;

Console.WriteLine(Atick.Version());   // prints the engine version, e.g. 1.0.3
```
