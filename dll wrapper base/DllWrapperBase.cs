using System;
using System.Runtime.InteropServices;

namespace Atg.Utils.Native
{
    abstract class DllWrapperBase
    {
        // To add a procedure from the DLL declare in a same manner:
        //
        //      private delegate RETURN_TYPE SAME_NAME_AS_DLL_PROCEDURE(PROC_PARAMETERS);
        //      private SAME_NAME_AS_DLL_PROCEDURE fieldName;
        //      public RETURN_TYPE functionName(parameters)
        //      {
        //              //convert parameters to PROC_PARAMETERS if needed
        //              return fieldName(PROC_PARAMETERS);
        //      }
        //
        // And initialize "fieldName" somewhere (for example in constructor):
        //
        //      fieldName = GetReadyDelegate<SAME_NAME_AS_DLL_PROCEDURE>();

        private IntPtr m_dllHandle;

        /// <exception cref="DllNotFoundException">Thrown when cannot load DLL by path</exception>
        public DllWrapperBase(string dllPath)
        {
            m_dllHandle = DllLoader.LoadLibrary(dllPath);
            if (m_dllHandle == IntPtr.Zero)
            {
                throw new DllNotFoundException($"Cannot load remote API dll: {dllPath}");
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
            IntPtr methodHandle = DllLoader.GetProcAddress(m_dllHandle, procName);
            if (methodHandle == IntPtr.Zero)
            {
                ReleaseHandle();
                throw new EntryPointNotFoundException($"Error getting address of dll's interface: {procName}");
            }
            return (T)(object)Marshal.GetDelegateForFunctionPointer(methodHandle, typeof(T));
        }

        private void ReleaseHandle()
        {
            if (m_dllHandle != IntPtr.Zero)
            {
                DllLoader.FreeLibrary(m_dllHandle);
                m_dllHandle = IntPtr.Zero;
            }
        }

        ~DllWrapperBase()
        {
            ReleaseHandle();
        }
    }
}

