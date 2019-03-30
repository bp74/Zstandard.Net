using System;
using System.IO;
using System.Runtime.InteropServices;
#if NETSTANDARD2_0
using NativeLibraryLoader;
#endif

namespace Zstandard.Net
{
    internal static class ZstandardInterop
    {
#if NETSTANDARD2_0
        private static T Call<T>(string name) => ZStandard.LoadFunction<T>(name);

        private static NativeLibrary ZStandard
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && RuntimeInformation.OSArchitecture == Architecture.X86) return new NativeLibrary(@"build\x86\libzstd.dll");
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && RuntimeInformation.OSArchitecture == Architecture.X64) return new NativeLibrary(@"build\x64\libzstd.dll");
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return new NativeLibrary(@"build\"); //TODO: Edit to relative file location.
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return new NativeLibrary(@"build\"); //TODO: Edit to relative file location.
                else throw new PlatformNotSupportedException();
            }
        }
#else
        static ZstandardInterop()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var root = Path.GetDirectoryName(typeof(ZstandardInterop).Assembly.Location);
                var path = Environment.Is64BitProcess ? "x64" : "x86";
                var file = Path.Combine(root, path, "libzstd.dll");
                LoadLibraryEx(file, IntPtr.Zero, LoadLibraryFlags.LOAD_LIBRARY_SEARCH_APPLICATION_DIR);
            }
        }
#endif

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

#if NETSTANDARD2_0
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate uint ZSTDversionNumber();
        public static uint ZSTD_versionNumber() => Call<ZSTDversionNumber>("ZSTD_versionNumber")();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int ZSTDmaxCLevel();
        public static int ZSTD_maxCLevel() => Call<ZSTDmaxCLevel>("ZSTD_maxCLevel")();

        //-----------------------------------------------------------------------------------------

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr ZSTDcreateCStream();
        public static IntPtr ZSTD_createCStream() => Call<ZSTDcreateCStream>("ZSTD_createCStream")();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDinitCStream(IntPtr zcs, int compressionLevel);
        public static UIntPtr ZSTD_initCStream(IntPtr zcs, int compressionLevel) => Call<ZSTDinitCStream>("ZSTD_initCStream")(zcs, compressionLevel);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDfreeCStream(IntPtr zcs);
        public static UIntPtr ZSTD_freeCStream(IntPtr zcs) => Call<ZSTDfreeCStream>("ZSTD_freeCStream")(zcs);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDCStreamInSize();
        public static UIntPtr ZSTD_CStreamInSize() => Call<ZSTDCStreamInSize>("ZSTD_CStreamInSize")();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDCStreamOutSize();
        public static UIntPtr ZSTD_CStreamOutSize() => Call<ZSTDCStreamOutSize>("ZSTD_CStreamOutSize")();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDcompressStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer);
        public static UIntPtr ZSTD_compressStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer) => Call<ZSTDcompressStream>("ZSTD_compressStream")(zcs, outputBuffer, inputBuffer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr ZSTDcreateCDict(IntPtr dictBuffer, UIntPtr dictSize, int compressionLevel);
        public static IntPtr ZSTD_createCDict(IntPtr dictBuffer, UIntPtr dictSize, int compressionLevel) => Call<ZSTDcreateCDict>("ZSTD_createCDict")(dictBuffer, dictSize, compressionLevel);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDfreeCDict(IntPtr cdict);
        public static UIntPtr ZSTD_freeCDict(IntPtr cdict) => Call<ZSTDfreeCDict>("ZSTD_freeCDict")(cdict);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDinitCStream_usingCDict(IntPtr zcs, IntPtr cdict);
        public static UIntPtr ZSTD_initCStream_usingCDict(IntPtr zcs, IntPtr cdict) => Call<ZSTDinitCStream_usingCDict>("ZSTD_initCStream_usingCDict")(zcs, cdict);

        //-----------------------------------------------------------------------------------------

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr ZSTDcreateDStream();
        public static IntPtr ZSTD_createDStream() => Call<ZSTDcreateDStream>("ZSTD_createDStream")();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDinitDStream(IntPtr zds);
        public static UIntPtr ZSTD_initDStream(IntPtr zds) => Call<ZSTDinitDStream>("ZSTD_initDStream")(zds);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDfreeDStream(IntPtr zds);
        public static UIntPtr ZSTD_freeDStream(IntPtr zds) => Call<ZSTDfreeDStream>("ZSTD_freeDStream")(zds);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDDStreamInSize();
        public static UIntPtr ZSTD_DStreamInSize() => Call<ZSTDDStreamInSize>("ZSTD_DStreamInSize")();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDDStreamOutSize();
        public static UIntPtr ZSTD_DStreamOutSize() => Call<ZSTDDStreamOutSize>("ZSTD_DStreamOutSize")();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDdecompressStream(IntPtr zds, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer);
        public static UIntPtr ZSTD_decompressStream(IntPtr zds, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer) => Call<ZSTDdecompressStream>("ZSTD_decompressStream")(zds, outputBuffer, inputBuffer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr ZSTDcreateDDict(IntPtr dictBuffer, UIntPtr dictSize);
        public static IntPtr ZSTD_createDDict(IntPtr dictBuffer, UIntPtr dictSize) => Call<ZSTDcreateDDict>("ZSTD_createDDict")(dictBuffer, dictSize);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDfreeDDict(IntPtr ddict);
        public static UIntPtr ZSTD_freeDDict(IntPtr ddict) => Call<ZSTDfreeDDict>("ZSTD_freeDDict")(ddict);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDinitDStream_usingDDict(IntPtr zds, IntPtr ddict);
        public static UIntPtr ZSTD_initDStream_usingDDict(IntPtr zds, IntPtr ddict) => Call<ZSTDinitDStream_usingDDict>("ZSTD_initDStream_usingDDict")(zds, ddict);

        //-----------------------------------------------------------------------------------------

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDflushStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer);
        public static UIntPtr ZSTD_flushStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer) => Call<ZSTDflushStream>("ZSTD_flushStream")(zcs, outputBuffer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr ZSTDendStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer);
        public static UIntPtr ZSTD_endStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer) => Call<ZSTDendStream>("ZSTD_endStream")(zcs, outputBuffer);

        //-----------------------------------------------------------------------------------------

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool ZSTDisError(UIntPtr code);
        public static bool ZSTD_isError(UIntPtr code) => Call<ZSTDisError>("ZSTD_isError")(code);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr ZSTDgetErrorName(UIntPtr code);
        public static IntPtr ZSTD_getErrorName(UIntPtr code) => Call<ZSTDgetErrorName>("ZSTD_getErrorName")(code);

#else
        [Flags]
        private enum LoadLibraryFlags : uint
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

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

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
#endif
    }
}
