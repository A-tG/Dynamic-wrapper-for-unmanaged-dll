using System;
using System.Runtime.InteropServices;

namespace AtgDev.Utils.Native
{
    static class DllLoader
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        public static IntPtr Load(string dllToLoad)
        {
            return LoadLibrary(dllToLoad);
        }

        public static IntPtr GetProcedureAddress(IntPtr dllHandle, string procedureName)
        {
            return GetProcAddress(dllHandle, procedureName);
        }

        public static void Free(IntPtr dllHandle)
        {
            FreeLibrary(dllHandle);
        }
    }
}
