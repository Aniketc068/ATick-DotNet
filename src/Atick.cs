// ATick for .NET — a complete PDF digital-signature library.
// Calls the compiled ATick engine through P/Invoke (no C glue on your side). The matching native
// engine for the running OS/arch ships with the package and is loaded automatically. Every failure
// is a normal .NET AtickException.
//
// Targets .NET 2.0 through the latest .NET: on modern .NET the engine is resolved per-RID
// (runtimes/<rid>/native), on .NET Framework it is pre-loaded from the application directory.
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
#if NET6_0_OR_GREATER
using System.Reflection;
#endif

namespace Aniketc068.ATick
{
    /// <summary>Thrown by any ATick operation that fails.</summary>
    public sealed class AtickException : Exception
    {
        public AtickException(string message) : base(message) { }
    }

    /// <summary>The result of <see cref="Atick.Prepare"/> — the prepared PDF and the exact bytes to sign.</summary>
    public sealed class DeferredSignature
    {
        public byte[] Prepared { get; private set; }
        public byte[] BytesToSign { get; private set; }
        public DeferredSignature(byte[] prepared, byte[] bytesToSign) { Prepared = prepared; BytesToSign = bytesToSign; }
        /// <summary>Allows <c>var (prepared, bytesToSign) = Atick.Prepare(...);</c> on C# 7+.</summary>
        public void Deconstruct(out byte[] prepared, out byte[] bytesToSign) { prepared = Prepared; bytesToSign = BytesToSign; }
    }

    /// <summary>ATick — sign PDFs with a PFX/P12/PEM, deferred/remote keys, timestamps and LTV.</summary>
    public static class Atick
    {
        private const string Lib = "atick";
        private static readonly byte[] Empty = new byte[0];

        static Atick()
        {
#if NET6_0_OR_GREATER
            NativeLibrary.SetDllImportResolver(typeof(Atick).Assembly, Resolve);
#elif NETFRAMEWORK
            PreloadWindows();
#endif
            // FIXED branding: this is the .NET binding -> "ATick_net" everywhere branding is fixed.
            try { atick_set_brand(Utf8("net")); } catch { /* engine loads lazily on the first real call */ }
        }

#if NET6_0_OR_GREATER
        // modern .NET: resolve runtimes/<rid>/native/<file> for the running platform
        private static IntPtr Resolve(string name, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (name != Lib) return IntPtr.Zero;
            string rid = Rid();
            string file = EngineFile();
            string asmDir = Path.GetDirectoryName(assembly.Location);
            if (string.IsNullOrEmpty(asmDir)) asmDir = AppContext.BaseDirectory;
            string[] candidates =
            {
                Path.Combine(AppContext.BaseDirectory, "runtimes", rid, "native", file),
                Path.Combine(asmDir, "runtimes", rid, "native", file),
                Path.Combine(AppContext.BaseDirectory, file),
                Path.Combine(asmDir, file),
            };
            foreach (var candidate in candidates)
                if (File.Exists(candidate) && NativeLibrary.TryLoad(candidate, out var handle))
                    return handle;
            return IntPtr.Zero; // fall back to the default OS search
        }

        private static string Rid()
        {
            string os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win"
                      : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx" : "linux";
            string arch;
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X64: arch = "x64"; break;
                case Architecture.X86: arch = "x86"; break;
                case Architecture.Arm64: arch = "arm64"; break;
                case Architecture.Arm: arch = "arm"; break;
                default: arch = "x64"; break;
            }
            return os + "-" + arch;
        }

        private static string EngineFile()
            => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "atick.dll"
             : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "libatick.dylib" : "libatick.so";
#endif

#if NETFRAMEWORK
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibraryW(string lpFileName);

        // .NET Framework is Windows-only: pre-load the right-arch engine so DllImport("atick") binds to it.
        private static void PreloadWindows()
        {
            try
            {
                string arch = IntPtr.Size == 8 ? "win-x64" : "win-x86";
                string baseDir = AppDomain.CurrentDomain.BaseDirectory ?? "";
                string ridPath = Path.Combine(Path.Combine(Path.Combine(baseDir, "runtimes"), arch), "native");
                string[] candidates =
                {
                    Path.Combine(baseDir, "atick.dll"),
                    Path.Combine(ridPath, "atick.dll"),
                };
                foreach (var c in candidates)
                    if (File.Exists(c)) { LoadLibraryW(c); return; }
            }
            catch { /* fall back to the default DLL search (atick.dll on PATH) */ }
        }
#endif

        // ---- helpers ----
        private static byte[] Utf8(string s) => Encoding.UTF8.GetBytes((s ?? "") + "\0");
        private static UIntPtr Sz(int n) => new UIntPtr((uint)n);
        private static int Len(UIntPtr n) => (int)n.ToUInt64();

        private static byte[] Take(int rc, IntPtr ptr, UIntPtr len)
        {
            int n = Len(len);
            byte[] buf = (ptr != IntPtr.Zero && n > 0) ? new byte[n] : Empty;
            if (buf.Length > 0) Marshal.Copy(ptr, buf, 0, n);
            if (ptr != IntPtr.Zero) atick_free(ptr, len);
            if (rc != 0) throw new AtickException(Encoding.UTF8.GetString(buf));
            return buf;
        }

        // ---- public API (mirrors the Python and Java libraries) ----

        /// <summary>The engine version string.</summary>
        public static string Version() { return Marshal.PtrToStringAnsi(atick_version()) ?? ""; }

        /// <summary>Sign a PDF with a PFX/P12 (or PEM). The options JSON carries the password, appearance and flags.</summary>
        public static byte[] SignPfx(byte[] pdf, byte[] pfx, string optionsJson)
        {
            IntPtr o; UIntPtr n;
            int rc = atick_sign_pfx(pdf, Sz(pdf.Length), pfx, Sz(pfx.Length), Utf8(optionsJson), out o, out n);
            return Take(rc, o, n);
        }

        /// <summary>Embed a detached CMS/PKCS#7 into a prepared PDF.</summary>
        public static byte[] Embed(byte[] prepared, byte[] cms)
        {
            IntPtr o; UIntPtr n;
            int rc = atick_embed(prepared, Sz(prepared.Length), cms, Sz(cms.Length), out o, out n);
            return Take(rc, o, n);
        }

        /// <summary>Produce a detached CMS over <paramref name="data"/> signed with a PFX.</summary>
        public static byte[] CmsPfx(byte[] data, byte[] pfx, string optionsJson)
        {
            IntPtr o; UIntPtr n;
            int rc = atick_cms_pfx(data, Sz(data.Length), pfx, Sz(pfx.Length), Utf8(optionsJson), out o, out n);
            return Take(rc, o, n);
        }

        /// <summary>Decrypt a password-protected PDF.</summary>
        public static byte[] Decrypt(byte[] pdf, string password)
        {
            IntPtr o; UIntPtr n;
            int rc = atick_decrypt(pdf, Sz(pdf.Length), Utf8(password), out o, out n);
            return Take(rc, o, n);
        }

        /// <summary>Prepare a PDF for deferred / remote (eSign / HSM / token) signing.
        /// Sign the returned BytesToSign externally and call <see cref="Embed"/>.</summary>
        public static DeferredSignature Prepare(byte[] pdf, string optionsJson)
        {
            IntPtr o, od; UIntPtr n, odn;
            int rc = atick_prepare(pdf, Sz(pdf.Length), Utf8(optionsJson), out o, out n, out od, out odn);
            int dn = Len(odn);
            byte[] data = (od != IntPtr.Zero && dn > 0) ? new byte[dn] : Empty;
            if (data.Length > 0) Marshal.Copy(od, data, 0, dn);
            if (od != IntPtr.Zero) atick_free(od, odn);
            byte[] prepared = Take(rc, o, n); // throws on error (message is in o)
            return new DeferredSignature(prepared, data);
        }

        /// <summary>Prepare an empty signing field (template) — appearance drawn, signature left empty.</summary>
        public static byte[] PrepareFields(byte[] pdf, string optionsJson)
        {
            IntPtr o; UIntPtr n;
            int rc = atick_prepare_fields(pdf, Sz(pdf.Length), Utf8(optionsJson), out o, out n);
            return Take(rc, o, n);
        }

        /// <summary>Sign an existing empty field (e.g. from <see cref="PrepareFields"/>) with a PFX/P12/PEM.</summary>
        public static byte[] SignField(byte[] pdf, byte[] pfx, string optionsJson)
        {
            IntPtr o; UIntPtr n;
            int rc = atick_sign_field(pdf, Sz(pdf.Length), pfx, Sz(pfx.Length), Utf8(optionsJson), out o, out n);
            return Take(rc, o, n);
        }

        /// <summary>Set document metadata (Title/Author/Subject/Keywords/Creator/CreationDate/ModDate via JSON).</summary>
        public static byte[] SetMetadata(byte[] pdf, string optionsJson)
        {
            IntPtr o; UIntPtr n;
            int rc = atick_set_metadata(pdf, Sz(pdf.Length), Utf8(optionsJson), out o, out n);
            return Take(rc, o, n);
        }

        /// <summary>Add a standalone archive DocTimeStamp (+ DSS) to an already-signed PDF (PAdES-B-LTA).</summary>
        public static byte[] AddDocTimestamp(byte[] pdf, string optionsJson)
        {
            IntPtr o; UIntPtr n;
            int rc = atick_add_doctimestamp(pdf, Sz(pdf.Length), Utf8(optionsJson), out o, out n);
            return Take(rc, o, n);
        }

        /// <summary>Enable/disable fast signing (reuse fetched revocation across many documents in one run).</summary>
        public static void SetFastSigning(bool on) { atick_set_fast_signing(on ? 1 : 0); }

        // ---- P/Invoke declarations (the C ABI). return 0 = ok (out holds result), else out holds a UTF-8 error ----
        [DllImport(Lib)] private static extern IntPtr atick_version();
        [DllImport(Lib)] private static extern void atick_free(IntPtr ptr, UIntPtr len);
        [DllImport(Lib)] private static extern void atick_set_brand(byte[] tag);
        [DllImport(Lib)] private static extern void atick_set_fast_signing(int on);
        [DllImport(Lib)] private static extern int atick_sign_pfx(byte[] pdf, UIntPtr pdfLen, byte[] pfx, UIntPtr pfxLen, byte[] opt, out IntPtr outPtr, out UIntPtr outLen);
        [DllImport(Lib)] private static extern int atick_embed(byte[] prep, UIntPtr prepLen, byte[] cms, UIntPtr cmsLen, out IntPtr outPtr, out UIntPtr outLen);
        [DllImport(Lib)] private static extern int atick_cms_pfx(byte[] data, UIntPtr dataLen, byte[] pfx, UIntPtr pfxLen, byte[] opt, out IntPtr outPtr, out UIntPtr outLen);
        [DllImport(Lib)] private static extern int atick_decrypt(byte[] pdf, UIntPtr pdfLen, byte[] pw, out IntPtr outPtr, out UIntPtr outLen);
        [DllImport(Lib)] private static extern int atick_prepare(byte[] pdf, UIntPtr pdfLen, byte[] opt, out IntPtr outPtr, out UIntPtr outLen, out IntPtr outData, out UIntPtr outDataLen);
        [DllImport(Lib)] private static extern int atick_add_doctimestamp(byte[] pdf, UIntPtr pdfLen, byte[] opt, out IntPtr outPtr, out UIntPtr outLen);
        [DllImport(Lib)] private static extern int atick_set_metadata(byte[] pdf, UIntPtr pdfLen, byte[] opt, out IntPtr outPtr, out UIntPtr outLen);
        [DllImport(Lib)] private static extern int atick_prepare_fields(byte[] pdf, UIntPtr pdfLen, byte[] opt, out IntPtr outPtr, out UIntPtr outLen);
        [DllImport(Lib)] private static extern int atick_sign_field(byte[] pdf, UIntPtr pdfLen, byte[] pfx, UIntPtr pfxLen, byte[] opt, out IntPtr outPtr, out UIntPtr outLen);
    }
}
