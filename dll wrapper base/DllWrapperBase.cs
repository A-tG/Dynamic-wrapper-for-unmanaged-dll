using System;
using System.Runtime.InteropServices;

namespace AtgDev.Utils.Native
{
    abstract class DllWrapperBase
    {
        // To add a procedure from the DLL declare in a same manner:
        //
        //      private delegate int32 ProcedureNameFromDLL(IntPtr someParam1, in int32 someParam2);
        //      private ProcedureNameFromDLL fieldName;
        //      public int32 functionName(IntPtr someParam1, in int32 someParam2)
        //      {
        //              // do something with parameters here if needed
        //              return fieldName(someParam1, in someParam2);
        //      }
        //
        // And initialize "fieldName" somewhere (for example in constructor):
        //
        //      fieldName = GetReadyDelegate<ProcedureNameFromDLL>();

        private IntPtr m_dllHandle;

        /// <exception cref="DllNotFoundException">Thrown when cannot load DLL by path</exception>
        public DllWrapperBase(string dllPath)
        {
            m_dllHandle = DllLoader.Load(dllPath);
            if (m_dllHandle == IntPtr.Zero)
            {
                throw new DllNotFoundException($"Cannot load dll: {dllPath}");
            }
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

        private IntPtr GetMethodHandle(string procName)
        {
            IntPtr methodHandle = DllLoader.GetProcedureAddress(m_dllHandle, procName);
            if (methodHandle == IntPtr.Zero)
            {
                ReleaseHandle();
                throw new EntryPointNotFoundException($"Error getting address of dll's interface: {procName}");
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

