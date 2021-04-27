using System;
using System.Collections.Generic;
using System.Text;

namespace AtgDev.Utils.Native
{
    partial class DllWrapperBase : IDisposable
    {
        private bool m_isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!m_isDisposed)
            {
                ReleaseHandle();
                m_isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
