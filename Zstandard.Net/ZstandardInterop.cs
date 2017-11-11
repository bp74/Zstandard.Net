using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Zstandard.Net
{
    internal static class ZstandardInterop
    {
        private const String Lib64 = "libzstd64.dll";
        private const String Lib32 = "libzstd32.dll";

        [StructLayout(LayoutKind.Sequential)]
        public class Buffer
        {
            public IntPtr Data = IntPtr.Zero;
            public UIntPtr Size = UIntPtr.Zero;
            public UIntPtr Position = UIntPtr.Zero;
        }

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        public static uint GetVersionNumber()
        {
            return Environment.Is64BitProcess ? ZSTD64_versionNumber() : ZSTD32_versionNumber();
        }

        public static int GetMaxCompressionLevel()
        {
            return Environment.Is64BitProcess ? ZSTD64_maxCLevel() : ZSTD32_maxCLevel();
        }

        [DllImport(Lib64, EntryPoint = "ZSTD_versionNumber", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint ZSTD64_versionNumber();

        [DllImport(Lib32, EntryPoint = "ZSTD_versionNumber", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint ZSTD32_versionNumber();

        [DllImport(Lib64, EntryPoint = "ZSTD_maxCLevel", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ZSTD64_maxCLevel();

        [DllImport(Lib32, EntryPoint = "ZSTD_maxCLevel", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ZSTD32_maxCLevel();

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        public static IntPtr CreateCompressionStream()
        {
            return Environment.Is64BitProcess ? ZSTD64_createCStream() : ZSTD32_createCStream();
        }

        public static void InitCompressionStream(IntPtr zcs, int compressionLevel)
        {
            var result = Environment.Is64BitProcess ? ZSTD64_initCStream(zcs, compressionLevel) : ZSTD32_initCStream(zcs, compressionLevel);
        }

        public static void FreeCompressionStream(IntPtr zcs)
        {
            var result = Environment.Is64BitProcess ? ZSTD64_freeCStream(zcs) : ZSTD32_freeCStream(zcs);
        }

        public static uint GetCompressionStreamInputSize()
        {
            return Environment.Is64BitProcess ? ZSTD64_CStreamInSize().ToUInt32() : ZSTD32_CStreamInSize().ToUInt32();
        }

        public static uint GetCompressionStreamOutputSize()
        {
            return Environment.Is64BitProcess ? ZSTD64_CStreamOutSize().ToUInt32() : ZSTD32_CStreamOutSize().ToUInt32();
        }

        public static void WriteToCompressionStream(IntPtr zcs, Buffer outputBuffer, Buffer inputBuffer)
        {
            ThrowIfError(Environment.Is64BitProcess ? ZSTD64_compressStream(zcs, outputBuffer, inputBuffer) : ZSTD32_compressStream(zcs, outputBuffer, inputBuffer));
        }

        [DllImport(Lib64, EntryPoint = "ZSTD_createCStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ZSTD64_createCStream();

        [DllImport(Lib32, EntryPoint = "ZSTD_createCStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ZSTD32_createCStream();

        [DllImport(Lib64, EntryPoint = "ZSTD_initCStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD64_initCStream(IntPtr zcs, int compressionLevel);

        [DllImport(Lib32, EntryPoint = "ZSTD_initCStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD32_initCStream(IntPtr zcs, int compressionLevel);

        [DllImport(Lib64, EntryPoint = "ZSTD_freeCStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD64_freeCStream(IntPtr zcs);

        [DllImport(Lib32, EntryPoint = "ZSTD_freeCStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD32_freeCStream(IntPtr zcs);

        [DllImport(Lib64, EntryPoint = "ZSTD_CStreamInSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD64_CStreamInSize();

        [DllImport(Lib32, EntryPoint = "ZSTD_CStreamInSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD32_CStreamInSize();

        [DllImport(Lib64, EntryPoint = "ZSTD_CStreamOutSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD64_CStreamOutSize();

        [DllImport(Lib32, EntryPoint = "ZSTD_CStreamOutSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD32_CStreamOutSize();

        [DllImport(Lib64, EntryPoint = "ZSTD_compressStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD64_compressStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer);

        [DllImport(Lib32, EntryPoint = "ZSTD_compressStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD32_compressStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer);

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        public static IntPtr CreateDecompressionStream()
        {
            return Environment.Is64BitProcess ? ZSTD64_createDStream() : ZSTD32_createDStream();
        }

        public static void InitDecompressionStream(IntPtr zcs)
        {
            var result = Environment.Is64BitProcess ? ZSTD64_initDStream(zcs) : ZSTD32_initDStream(zcs);
        }

        public static void FreeDecompressionStream(IntPtr zcs)
        {
            var result = Environment.Is64BitProcess ? ZSTD64_freeDStream(zcs) : ZSTD32_freeDStream(zcs);
        }

        public static uint GetDecompressionStreamInputSize()
        {
            return Environment.Is64BitProcess ? ZSTD64_DStreamInSize().ToUInt32() : ZSTD32_DStreamInSize().ToUInt32();
        }

        public static uint GetDecompressionStreamOutputSize()
        {
            return Environment.Is64BitProcess ? ZSTD64_DStreamOutSize().ToUInt32() : ZSTD32_DStreamOutSize().ToUInt32();
        }

        public static void ReadFromDecompressionStream(IntPtr zcs, Buffer outputBuffer, Buffer inputBuffer)
        {
            ThrowIfError(Environment.Is64BitProcess ? ZSTD64_decompressStream(zcs, outputBuffer, inputBuffer) : ZSTD32_decompressStream(zcs, outputBuffer, inputBuffer));
        }

        [DllImport(Lib64, EntryPoint = "ZSTD_createDStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ZSTD64_createDStream();

        [DllImport(Lib32, EntryPoint = "ZSTD_createDStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ZSTD32_createDStream();

        [DllImport(Lib64, EntryPoint = "ZSTD_initDStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD64_initDStream(IntPtr zcs);

        [DllImport(Lib32, EntryPoint = "ZSTD_initDStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD32_initDStream(IntPtr zcs);

        [DllImport(Lib64, EntryPoint = "ZSTD_freeDStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD64_freeDStream(IntPtr zcs);

        [DllImport(Lib32, EntryPoint = "ZSTD_freeDStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD32_freeDStream(IntPtr zcs);

        [DllImport(Lib64, EntryPoint = "ZSTD_DStreamInSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD64_DStreamInSize();

        [DllImport(Lib32, EntryPoint = "ZSTD_DStreamInSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD32_DStreamInSize();

        [DllImport(Lib64, EntryPoint = "ZSTD_DStreamOutSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD64_DStreamOutSize();

        [DllImport(Lib32, EntryPoint = "ZSTD_DStreamOutSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD32_DStreamOutSize();

        [DllImport(Lib64, EntryPoint = "ZSTD_decompressStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD64_decompressStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer);

        [DllImport(Lib32, EntryPoint = "ZSTD_decompressStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD32_decompressStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer);

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        public static void FlushStream(IntPtr zcs, Buffer outputBuffer)
        {
            ThrowIfError(Environment.Is64BitProcess ? ZSTD64_flushStream(zcs, outputBuffer) : ZSTD32_flushStream(zcs, outputBuffer));
        }

        public static void EndStream(IntPtr zcs, Buffer outputBuffer)
        {
            ThrowIfError(Environment.Is64BitProcess ? ZSTD64_endStream(zcs, outputBuffer) : ZSTD32_endStream(zcs, outputBuffer));
        }

        [DllImport(Lib64, EntryPoint = "ZSTD_flushStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD64_flushStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer);

        [DllImport(Lib32, EntryPoint = "ZSTD_flushStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD32_flushStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer);

        [DllImport(Lib64, EntryPoint = "ZSTD_endStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD64_endStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer);

        [DllImport(Lib32, EntryPoint = "ZSTD_endStream", CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr ZSTD32_endStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer);

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        private static void ThrowIfError(UIntPtr code)
        {
            if (Environment.Is64BitProcess ? ZSTD64_isError(code) : ZSTD32_isError(code))
            {
                throw new IOException(GetErrorName(code));
            }
        }

        private static string GetErrorName(UIntPtr code)
        {
            var error = Environment.Is64BitProcess ? ZSTD64_getErrorName(code) : ZSTD32_getErrorName(code);
            return Marshal.PtrToStringAnsi(error);
        }

        [DllImport(Lib64, EntryPoint = "ZSTD_getErrorName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ZSTD64_getErrorName(UIntPtr code);

        [DllImport(Lib32, EntryPoint = "ZSTD_getErrorName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ZSTD32_getErrorName(UIntPtr code);

        [DllImport(Lib64, EntryPoint = "ZSTD_isError", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ZSTD64_isError(UIntPtr code);

        [DllImport(Lib32, EntryPoint = "ZSTD_isError", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ZSTD32_isError(UIntPtr code);
    }
}
