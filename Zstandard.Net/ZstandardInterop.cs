using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Zstandard.Net
{

    internal static class Kernel32
    {
        //https://docs.microsoft.com/en-us/windows/desktop/api/libloaderapi/nf-libloaderapi-loadlibraryexa

        [Flags]
        public enum LoadLibraryFlags : uint
        {
            DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
            LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
            LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
            LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
            LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
            LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,
            LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000,
            LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
            LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
            LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400,
            LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
        }

        [DllImport("kernel32")]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr module, string procName);

        [DllImport("kernel32")]
        public static extern int FreeLibrary(IntPtr module);
    }

    internal static class Libdl
    {
        private const string LibName = "libdl";

        public const int RTLD_NOW = 0x002;

        [DllImport(LibName)]
        public static extern IntPtr dlopen(string fileName, int flags);

        [DllImport(LibName)]
        public static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport(LibName)]
        public static extern int dlclose(IntPtr handle);

        [DllImport(LibName)]
        public static extern string dlerror();
    }


    internal static class ZstandardInterop
    {
        static ZstandardInterop()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var root = AppContext.BaseDirectory;//Path.GetDirectoryName(typeof(ZstandardInterop).Assembly.Location);
                var path = Environment.Is64BitProcess ? "win-x64" : "win-x86";
                var file = Path.Combine(root, path, "libzstd.dll");

                if (!File.Exists(file))
                {
                    throw new FileNotFoundException("Couldn't Load ZSTD lib", file);
                }

                Kernel32.LoadLibraryEx(file, IntPtr.Zero, Kernel32.LoadLibraryFlags.LOAD_LIBRARY_SEARCH_APPLICATION_DIR);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var root = AppContext.BaseDirectory;//Path.GetDirectoryName(typeof(ZstandardInterop).Assembly.Location);
                var path = File.Exists("/etc/alpine-release") ? "linuxalpine" : "linuxdebian";
                var file = $"{root}{path}/libzstd.so";

                if(!File.Exists(file))
                {
                    throw new FileNotFoundException("Couldn't Load ZSTD lib:"+ file, file);
                }

                Libdl.dlopen(file, Libdl.RTLD_NOW);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class Buffer
        {
            public IntPtr Data = IntPtr.Zero;
            public UIntPtr Size = UIntPtr.Zero;
            public UIntPtr Position = UIntPtr.Zero;
        }

        public static void ThrowIfError(UIntPtr code)
        {
            if (ZSTD_isError(code))
            {
                var errorPtr = ZSTD_getErrorName(code);
                var errorMsg = Marshal.PtrToStringAnsi(errorPtr);
                throw new IOException(errorMsg);
            }
        }

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------


        //[DllImport("kernel32", SetLastError = true)]
        //private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ZSTD_versionNumber();

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ZSTD_maxCLevel();

        //-----------------------------------------------------------------------------------------

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createCStream();

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_initCStream(IntPtr zcs, int compressionLevel);

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_freeCStream(IntPtr zcs);

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_CStreamInSize();

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_CStreamOutSize();

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_compressStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer);

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createCDict(IntPtr dictBuffer, UIntPtr dictSize, int compressionLevel);

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_freeCDict(IntPtr cdict);

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_initCStream_usingCDict(IntPtr zcs, IntPtr cdict);

        //-----------------------------------------------------------------------------------------

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createDStream();

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_initDStream(IntPtr zds);

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_freeDStream(IntPtr zds);

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_DStreamInSize();

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_DStreamOutSize();

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_decompressStream(IntPtr zds, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer);

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createDDict(IntPtr dictBuffer, UIntPtr dictSize);

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_freeDDict(IntPtr ddict);

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_initDStream_usingDDict(IntPtr zds, IntPtr ddict);

        //-----------------------------------------------------------------------------------------

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_flushStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer);

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_endStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer);

        //-----------------------------------------------------------------------------------------

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ZSTD_isError(UIntPtr code);

        [DllImport("libzstd", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ZSTD_getErrorName(UIntPtr code);
    }
}
