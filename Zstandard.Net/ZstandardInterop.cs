using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Zstandard.Net
{
    internal static class ZstandardInterop
    {
        static ZstandardInterop()
        {
            var root = Path.GetDirectoryName(typeof(ZstandardInterop).Assembly.Location);
            var path = Environment.Is64BitProcess ? "x64" : "x86";
            var file = Path.Combine(root, path, "libzstd.dll");
            LoadLibraryEx(file, IntPtr.Zero, LoadLibraryFlags.LOAD_LIBRARY_SEARCH_APPLICATION_DIR);
        }

        [StructLayout(LayoutKind.Sequential)]
        public class Buffer
        {
            public IntPtr Data = IntPtr.Zero;
            public UIntPtr Size = UIntPtr.Zero;
            public UIntPtr Position = UIntPtr.Zero;
        }

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

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

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        public static uint GetVersionNumber()
        {
            return ZSTD_versionNumber();
        }

        public static int GetMaxCompressionLevel()
        {
            return ZSTD_maxCLevel();
        }

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_versionNumber", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint ZSTD_versionNumber();

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_maxCLevel", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ZSTD_maxCLevel();

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        public static IntPtr CreateCompressionStream()
        {
            return ZSTD_createCStream();
        }

        public static void InitCompressionStream(IntPtr zcs, int compressionLevel)
        {
            var result = ZSTD_initCStream(zcs, compressionLevel);
        }

        public static void FreeCompressionStream(IntPtr zcs)
        {
            var result = ZSTD_freeCStream(zcs);
        }

        public static uint GetCompressionStreamInputSize()
        {
            return ZSTD_CStreamInSize().ToUInt32();
        }

        public static uint GetCompressionStreamOutputSize()
        {
            return ZSTD_CStreamOutSize().ToUInt32();
        }

        public static void WriteToCompressionStream(IntPtr zcs, Buffer outputBuffer, Buffer inputBuffer)
        {
            ThrowIfError(ZSTD_compressStream(zcs, outputBuffer, inputBuffer));
        }

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_createCStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ZSTD_createCStream();

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_initCStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD_initCStream(IntPtr zcs, int compressionLevel);

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_freeCStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD_freeCStream(IntPtr zcs);

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_CStreamInSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD_CStreamInSize();

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_CStreamOutSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD_CStreamOutSize();

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_compressStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD_compressStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer);

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        public static IntPtr CreateDecompressionStream()
        {
            return ZSTD_createDStream();
        }

        public static void InitDecompressionStream(IntPtr zcs)
        {
            var result = ZSTD_initDStream(zcs);
        }

        public static void FreeDecompressionStream(IntPtr zcs)
        {
            var result = ZSTD_freeDStream(zcs);
        }

        public static uint GetDecompressionStreamInputSize()
        {
            return ZSTD_DStreamInSize().ToUInt32();
        }

        public static uint GetDecompressionStreamOutputSize()
        {
            return ZSTD_DStreamOutSize().ToUInt32();
        }

        public static void ReadFromDecompressionStream(IntPtr zcs, Buffer outputBuffer, Buffer inputBuffer)
        {
            ThrowIfError(ZSTD_decompressStream(zcs, outputBuffer, inputBuffer));
        }

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_createDStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ZSTD_createDStream();

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_initDStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD_initDStream(IntPtr zcs);

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_freeDStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD_freeDStream(IntPtr zcs);

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_DStreamInSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD_DStreamInSize();

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_DStreamOutSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD_DStreamOutSize();

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_decompressStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD_decompressStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer);

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        public static void FlushStream(IntPtr zcs, Buffer outputBuffer)
        {
            ThrowIfError(ZSTD_flushStream(zcs, outputBuffer));
        }

        public static void EndStream(IntPtr zcs, Buffer outputBuffer)
        {
            ThrowIfError(ZSTD_endStream(zcs, outputBuffer));
        }

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_flushStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD_flushStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer);

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_endStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD_endStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer);

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        private static void ThrowIfError(UIntPtr code)
        {
            if (ZSTD_isError(code))
            {
                throw new IOException(GetErrorName(code));
            }
        }

        private static string GetErrorName(UIntPtr code)
        {
            var error = ZSTD_getErrorName(code);
            return Marshal.PtrToStringAnsi(error);
        }

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_getErrorName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ZSTD_getErrorName(UIntPtr code);

        [DllImport("libzstd.dll", EntryPoint = "ZSTD_isError", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ZSTD_isError(UIntPtr code);
    }
}
