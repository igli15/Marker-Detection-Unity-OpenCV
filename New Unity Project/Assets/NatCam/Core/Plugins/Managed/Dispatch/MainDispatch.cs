/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Dispatch {

    using System.Threading;
    using Core.Utilities;

    [CoreDoc(86)]
    public class MainDispatch : IDispatch {

        #region --Client API--

        /// <summary>
        /// Creates a dispatcher that will execute on the main thread
        /// </summary>
        [CoreDoc(87)]
        public MainDispatch () : base () {
            targetThread = Thread.CurrentThread;
            DispatchUtility.onPostRender += Update;
            Utilities.LogVerbose("Initialized main dispatcher");
        }

        public override void Release () {
            DispatchUtility.onPostRender -= Update;
            base.Release();
            executing.Clear(); executing = null;
        }
        #endregion
    }
}