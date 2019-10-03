/* 
*   NatCam Extended
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using Extended;

    public partial interface INatCam {

        #region --Events--
        event MetadataCallback<IMetadata> OnMetadata;
        #endregion

        #region --Properties--
        bool SupportsMetadata {get;}
        #endregion

        #region --Client API--
        void SavePhoto (byte[] png, SaveMode mode, SaveCallback callback);
        long MetadataTime (long timestamp);
        #endregion
    }
}