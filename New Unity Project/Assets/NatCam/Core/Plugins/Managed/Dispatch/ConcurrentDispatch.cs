/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Dispatch {

    using System.Threading;
    using Core.Utilities;

    [CoreDoc(83)]
    public sealed class ConcurrentDispatch : IDispatch {

        #region --Op vars--
        private EventWaitHandle waitHandle = new AutoResetEvent(false);
        private bool running;
        private readonly object runFence = new object();
        #endregion


        #region --Ctor--

        /// <summary>
        /// Creates a dispatcher that will execute on a worker thread
        /// </summary>
        [CoreDoc(84)]
        public ConcurrentDispatch () : base () {
            lock (runFence) running = true;
            targetThread = new Thread(Update);
            targetThread.Start();
            DispatchUtility.onPostRender += Notify;
            Utilities.LogVerbose("Initialized concurrent dispatcher");
        }
        #endregion


        #region --Client API--

        /// <summary>
        /// Release the dispatcher and free its worker thread
        /// </summary>
        [CoreDoc(85)]
        public override void Release () {
            DispatchUtility.onPostRender -= Notify;
            lock (runFence) running = false;
            waitHandle.Set();
            targetThread.Join();
            base.Release();
        }
        #endregion


        #region --Callbacks--

        protected override void Update () {
            for (;;) {
                waitHandle.WaitOne();
                base.Update();
                lock (runFence) if (!running) break;
            }
            executing.Clear(); executing = null;
        }

        private void Notify () {
            waitHandle.Set();
        }
        #endregion
    }
}