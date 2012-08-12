#region

using System;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace Waveface
{
    #region WorkItem

    public sealed class WorkItem
    {
        private WaitCallback m_callback;
        private ExecutionContext m_ctx;
        private object m_state;

        internal WorkItem(WaitCallback wc, object state, ExecutionContext ctx)
        {
            m_callback = wc;
            m_state = state;
            m_ctx = ctx;
        }

        internal WaitCallback Callback
        {
            get { return m_callback; }
        }

        internal object State
        {
            get { return m_state; }
        }

        internal ExecutionContext Context
        {
            get { return m_ctx; }
        }
    }

    #endregion

    #region WorkItemStatus

    public enum WorkItemStatus
    {
        Completed,
        Queued,
        Executing,
        Aborted
    }

    #endregion

    #region AbortableThreadPool

    public static class AbortableThreadPool
    {
        private static LinkedList<WorkItem> m_callbacks =
            new LinkedList<WorkItem>();

        private static Dictionary<WorkItem, Thread> m_threads =
            new Dictionary<WorkItem, Thread>();

        public static WorkItem QueueUserWorkItem(
            WaitCallback callback, object state)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            WorkItem _item = new WorkItem(callback, state, ExecutionContext.Capture());

            lock (m_callbacks) 
                m_callbacks.AddLast(_item);

            ThreadPool.QueueUserWorkItem(HandleItem);
            return _item;
        }

        private static void HandleItem(object ignored)
        {
            WorkItem _item = null;

            try
            {
                lock (m_callbacks)
                {
                    if (m_callbacks.Count > 0)
                    {
                        _item = m_callbacks.First.Value;
                        m_callbacks.RemoveFirst();
                    }

                    if (_item == null)
                        return;

                    m_threads.Add(_item, Thread.CurrentThread);
                }

                ExecutionContext.Run(_item.Context,
                                     delegate { _item.Callback(_item.State); }, null);
            }
            finally
            {
                lock (m_callbacks)
                {
                    if (_item != null) m_threads.Remove(_item);
                }
            }
        }

        public static WorkItemStatus Cancel(WorkItem item, bool allowAbort)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            lock (m_callbacks)
            {
                LinkedListNode<WorkItem> _node = m_callbacks.Find(item);

                if (_node != null)
                {
                    m_callbacks.Remove(_node);
                    return WorkItemStatus.Queued;
                }
                else if (m_threads.ContainsKey(item))
                {
                    if (allowAbort)
                    {
                        m_threads[item].Abort();
                        m_threads.Remove(item);
                        return WorkItemStatus.Aborted;
                    }
                    else
                        return WorkItemStatus.Executing;
                }
                else
                    return WorkItemStatus.Completed;
            }
        }
    }

    #endregion
}