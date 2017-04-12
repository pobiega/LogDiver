using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace Logdiver.Util
{
    /// <summary>
    /// Author: Unknown.
    /// Used to turn a dispatcher (WPF thread owner) into an ISynchronizeInvoke object (Winforms thread owner).
    /// This is needed to have the timer events run in the same thread as the UI (so they can update the UI).
    /// </summary>
    internal class DispatcherWinFormsCompatAdapter : ISynchronizeInvoke
    {
        #region IAsyncResult implementation
        private class DispatcherAsyncResultAdapter : IAsyncResult
        {
            private DispatcherOperation m_op;
            private object m_state;

            public DispatcherAsyncResultAdapter(DispatcherOperation operation)
            {
                m_op = operation;
            }

            public DispatcherAsyncResultAdapter(DispatcherOperation operation, object state)
               : this(operation)
            {
                m_state = state;
            }

            public DispatcherOperation Operation
            {
                get { return m_op; }
            }

            #region IAsyncResult Members

            public object AsyncState
            {
                get { return m_state; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get { return null; }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get { return m_op.Status == DispatcherOperationStatus.Completed; }
            }

            #endregion
        }
        #endregion
        private Dispatcher m_disp;
        public DispatcherWinFormsCompatAdapter(Dispatcher dispatcher)
        {
            m_disp = dispatcher;
        }
        #region ISynchronizeInvoke Members

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            if (args != null && args.Length > 1)
            {
                object[] argsSansFirst = GetArgsAfterFirst(args);
                DispatcherOperation op = m_disp.BeginInvoke(DispatcherPriority.Normal, method, args[0], argsSansFirst);
                return new DispatcherAsyncResultAdapter(op);
            }
            if (args != null)
            {
                return new DispatcherAsyncResultAdapter(m_disp.BeginInvoke(DispatcherPriority.Normal, method, args[0]));
            }
            return new DispatcherAsyncResultAdapter(m_disp.BeginInvoke(DispatcherPriority.Normal, method));
        }

        private static object[] GetArgsAfterFirst(object[] args)
        {
            object[] result = new object[args.Length - 1];
            Array.Copy(args, 1, result, 0, args.Length - 1);
            return result;
        }

        public object EndInvoke(IAsyncResult result)
        {
            DispatcherAsyncResultAdapter res = result as DispatcherAsyncResultAdapter;
            if (res == null)
                throw new InvalidCastException();

            while (res.Operation.Status != DispatcherOperationStatus.Completed || res.Operation.Status == DispatcherOperationStatus.Aborted)
            {
                Thread.Sleep(50);
            }

            return res.Operation.Result;
        }

        public object Invoke(Delegate method, object[] args)
        {
            if (args != null && args.Length > 1)
            {
                object[] argsSansFirst = GetArgsAfterFirst(args);
                return m_disp.Invoke(DispatcherPriority.Normal, method, args[0], argsSansFirst);
            }
            if (args != null)
            {
                return m_disp.Invoke(DispatcherPriority.Normal, method, args[0]);
            }
            return m_disp.Invoke(DispatcherPriority.Normal, method);
        }

        public bool InvokeRequired
        {
            get { return m_disp.Thread != Thread.CurrentThread; }
        }

        #endregion
    }
}
