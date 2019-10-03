/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Dispatch {

    using System;
    using System.Threading;
    using Core.Utilities;
    using Queue = System.Collections.Generic.List<System.Action>;
    
    [CoreDoc(80)]
    public abstract class IDispatch : IDisposable {

        #region --Op vars--
        protected Thread targetThread;
        protected Queue pending, executing;
        protected readonly object queueLock = new object();
        #endregion


        #region --Client API--

        /// <summary>
        /// Dispatch a delegate to be invoked
        /// </summary>
        /// <param name="action">The delegate to be invoked</param>
        /// <param name="repeating">Optional. Should delegate be invoked continuously?</param>
        [CoreDoc(81)]
        public virtual void Dispatch (Action action, bool repeating = false) {
            if (action == null) return;
            Action actionWrapper = action;
            if (repeating) actionWrapper = delegate () {
                action();
                lock (queueLock) pending.Add(actionWrapper);
            };
            if (Thread.CurrentThread.ManagedThreadId == targetThread.ManagedThreadId && !repeating) actionWrapper();
            else lock (queueLock) if (!pending.Contains(actionWrapper) || repeating) pending.Add(actionWrapper);
        }
        
        /// <summary>
        /// Release the dispatcher
        /// </summary>
        [CoreDoc(82)]
        public virtual void Release () {
            lock (queueLock) {pending.Clear(); pending = null;}
            Utilities.LogVerbose("Released dispatcher");
        }

        void IDisposable.Dispose () {
            Release();
        }
        #endregion


        #region --Callbacks--

        protected virtual void Update () {
            executing.Clear();
            lock (queueLock) {
                executing.AddRange(pending);
                pending.Clear();
            }
            executing.ForEach(e => e());
        }
        #endregion


        #region --Ctor--

        protected IDispatch () {
            pending = new Queue();
            executing = new Queue();
        }
        #endregion
    }
}