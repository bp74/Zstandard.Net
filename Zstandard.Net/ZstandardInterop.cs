﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Zstandard.Net
{
    internal static class ZstandardInterop
    {
        static ZstandardInterop()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var root = Path.GetDirectoryName(typeof(ZstandardInterop).Assembly.Location);
                var path = Environment.Is64BitProcess ? "x64" : "x86";
                var file = Path.Combine(root, path, "zstd.dll");
                LoadLibraryEx(file, IntPtr.Zero, LoadLibraryFlags.LOAD_LIBRARY_SEARCH_APPLICATION_DIR);
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

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ZSTD_versionNumber();

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ZSTD_maxCLevel();

        //-----------------------------------------------------------------------------------------

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createCStream();

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_initCStream(IntPtr zcs, int compressionLevel);

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_freeCStream(IntPtr zcs);

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_CStreamInSize();

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_CStreamOutSize();

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_compressStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer);

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createCDict(IntPtr dictBuffer, UIntPtr dictSize, int compressionLevel);

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_freeCDict(IntPtr cdict);

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_initCStream_usingCDict(IntPtr zcs, IntPtr cdict);

        //-----------------------------------------------------------------------------------------

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createDStream();

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_initDStream(IntPtr zds);

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_freeDStream(IntPtr zds);

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_DStreamInSize();

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_DStreamOutSize();

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_decompressStream(IntPtr zds, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer, [MarshalAs(UnmanagedType.LPStruct)] Buffer inputBuffer);

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ZSTD_createDDict(IntPtr dictBuffer, UIntPtr dictSize);

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_freeDDict(IntPtr ddict);

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_initDStream_usingDDict(IntPtr zds, IntPtr ddict);

        //-----------------------------------------------------------------------------------------

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_flushStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer);

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr ZSTD_endStream(IntPtr zcs, [MarshalAs(UnmanagedType.LPStruct)] Buffer outputBuffer);
        
        //-----------------------------------------------------------------------------------------

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ZSTD_isError(UIntPtr code);

        [DllImport("zstd", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ZSTD_getErrorName(UIntPtr code);
    }
}
