using System;
using System.Runtime.InteropServices;

namespace AtgDev.Utils.Native
{
    public abstract partial class DllWrapperBase
    {
        private IntPtr m_dllHandle;
        private string m_dllPath;

        /// <exception cref="DllNotFoundException">Thrown when cannot load DLL by path</exception>
        public DllWrapperBase(string dllPath)
        {
            m_dllHandle = DllLoader.Load(dllPath);
            m_dllPath = dllPath;
            if (m_dllHandle == IntPtr.Zero)
            {
                throw new DllNotFoundException($@"Cannot load dll ""{dllPath}""");
            }
        }

        /// <summary>
        ///     Get delegate from DLL's procedure.
        /// </summary>
        /// <typeparam name="T">Should match with DLL's procedure name</typeparam>
        /// <param name="del">Variable receiving the delegate</param>
        /// <remarks>Generic's name T should match with DLL's procedure name</remarks>
        protected bool TryGetReadyDelegate<T>(ref T del)
        {
            // dirty hack to avoid repeating writing procedure name in generic type and function parameter
            var procName = typeof(T).Name;
            return TryGetReadyDelegate<T>(ref del, procName);
        }

        /// <summary>
        ///     Get delegate from DLL's procedure
        /// </summary>
        /// <typeparam name="T">Delegate type's name</typeparam>
        /// <param name="del">Variable receiving the delegate</param>
        /// <param name="procName">DLL's procedure name</param>
        protected bool TryGetReadyDelegate<T>(ref T del, string procName)
        {
            bool isHandleReceived = TryGetMethodHandle(procName, out IntPtr methodHandle);
            if (isHandleReceived)
            {
                del = (T)(object)Marshal.GetDelegateForFunctionPointer(methodHandle, typeof(T));
            }
            return isHandleReceived;
        }

        /// <summary>
        ///     Get delegate from DLL's procedure
        /// </summary>
        /// <typeparam name="T">Should match with DLL's procedure name</typeparam>
        /// <remarks>Generic's name T should match with DLL's procedure name</remarks>
        /// <exception cref="EntryPointNotFoundException">Thrown when cannot load procedure by name</exception>
        protected T GetReadyDelegate<T>()
        {
            // dirty hack to avoid repeating writing procedure name in generic type and function parameter
            var procName = typeof(T).Name;
            return GetReadyDelegate<T>(procName);
        }

        /// <summary>
        ///     Get delegate from DLL's procedure
        /// </summary>
        /// <typeparam name="T">Delegate type's name</typeparam>
        /// <param name="procName">DLL's procedure name</param>
        /// <exception cref="EntryPointNotFoundException">Thrown when cannot load procedure by name</exception>
        protected T GetReadyDelegate<T>(string procName)
        {
            IntPtr methodHandle = GetMethodHandle(procName);
            return (T)(object)Marshal.GetDelegateForFunctionPointer(methodHandle, typeof(T));
        }

        /// <summary>
        ///     Get delegate from DLL's procedure
        /// </summary>
        /// <typeparam name="T">Should match with DLL's procedure name</typeparam>
        /// <param name="del">Variable receiving the delegate</param>
        /// <remarks>Generic's name T should match with DLL's procedure name</remarks>
        /// <exception cref="EntryPointNotFoundException">Thrown when cannot load procedure by name</exception>
        protected void GetReadyDelegate<T>(ref T del)
        {
            // dirty hack to avoid repeating writing procedure name in generic type and function parameter
            var procName = typeof(T).Name;
            del = GetReadyDelegate<T>(procName);
        }

        /// <summary>
        ///     Get delegate from DLL's procedure
        /// </summary>
        /// <typeparam name="T">Should match with DLL's procedure name</typeparam>
        /// <param name="del">Variable receiving the delegate</param>
        /// <param name="procName">DLL's procedure name</param>
        /// <exception cref="EntryPointNotFoundException">Thrown when cannot load procedure by name</exception>
        protected void GetReadyDelegate<T>(ref T del, string procName)
        {
            del = GetReadyDelegate<T>(procName);
        }

        private bool TryGetMethodHandle(string procName, out IntPtr methodHandle)
        {
            methodHandle = DllLoader.GetProcedureAddress(m_dllHandle, procName);
            return methodHandle != IntPtr.Zero;
        }

        private IntPtr GetMethodHandle(string procName)
        {
            bool isHandleReceived = TryGetMethodHandle(procName, out IntPtr methodHandle);
            if (!isHandleReceived)
            {
                ReleaseHandle();
                throw new EntryPointNotFoundException($@"Error getting address of interface ""{procName}"" in ""{m_dllPath}""");
            }
            return methodHandle;
        }

        private void ReleaseHandle()
        {
            if (m_dllHandle != IntPtr.Zero)
            {
                DllLoader.Free(m_dllHandle);
                m_dllHandle = IntPtr.Zero;
            }
        }

        ~DllWrapperBase()
        {
            ReleaseHandle();
        }
    }
}

