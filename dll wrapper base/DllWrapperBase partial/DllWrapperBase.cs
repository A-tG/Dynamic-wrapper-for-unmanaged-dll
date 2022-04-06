﻿using System;
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

        /// <inheritdoc cref="GetReadyDelegate{T}(ref T, string)"/>
        protected bool TryGetReadyDelegate<T>(ref T del)
        {
            // dirty hack to avoid repeating writing procedure name in generic type and function parameter
            var procName = typeof(T).Name;
            return TryGetReadyDelegate<T>(ref del, procName);
        }

        /// <inheritdoc cref="GetReadyDelegate{T}(ref T, string)"/>
        protected bool TryGetReadyDelegate<T>(ref T del, string procName)
        {
            bool isHandleReceived = TryGetMethodHandle(procName, out IntPtr methodHandle);
            if (isHandleReceived)
            {
                del = GetReadyDelegate<T>(methodHandle);
            }
            return isHandleReceived;
        }

        /// <inheritdoc cref="GetReadyDelegate{T}(ref T, string)"/>
        protected T GetReadyDelegate<T>()
        {
            // dirty hack to avoid repeating writing procedure name in generic type and function parameter
            var procName = typeof(T).Name;
            return GetReadyDelegate<T>(procName);
        }

        /// <inheritdoc cref="GetReadyDelegate{T}(ref T, string)"/>
        protected T GetReadyDelegate<T>(string procName)
        {
            IntPtr methodHandle = GetMethodHandle(procName);
            return GetReadyDelegate<T>(methodHandle);
        }

        /// <inheritdoc cref="GetReadyDelegate{T}(ref T, string)"/>
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
        /// <inheritdoc cref="GetMethodHandle(string)" path="/exception"/>
        /// <inheritdoc cref="Marshal.GetDelegateForFunctionPointer(IntPtr, Type)" path="/exception"/>
        protected void GetReadyDelegate<T>(ref T del, string procName)
        {
            del = GetReadyDelegate<T>(procName);
        }

        private T GetReadyDelegate<T>(IntPtr handle)
        {
#if NET5_0_OR_GREATER || NETCOREAPP1_0_OR_GREATER || NET452_OR_GREATER || NETSTANDARD1_2_OR_GREATER
            return Marshal.GetDelegateForFunctionPointer<T>(handle);
#else
            return (T)(object)Marshal.GetDelegateForFunctionPointer(handle, typeof(T));
#endif
        }

        private bool TryGetMethodHandle(string procName, out IntPtr methodHandle)
        {
            methodHandle = DllLoader.GetProcedureAddress(m_dllHandle, procName);
            return methodHandle != IntPtr.Zero;
        }


        /// <exception cref="EntryPointNotFoundException">Thrown when cannot load procedure by name</exception>
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

