/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

//#define CORE_DOC // Internal. Do not use
//#define EXTENDED_DOC // Internal. Do not use
//#define PROFESSIONAL_DOC // Internal. Do not use // Remember to enable OPENCV_API

#if CORE_DOC || EXTENDED_DOC || PROFESSIONAL_DOC
    #define DOC_GEN
#endif

using System;

namespace NatCamU.Core.Utilities {

    #if DOC_GEN
    using Calligraphy;
    using Platforms;
    using CD = NatCamCoreDocs;
    using ED = Extended.Utilities.NatCamExtendedDocs;
    using PD = Professional.Utilities.NatCamProfessionalDocs;
    #endif

    #if DOC_GEN
    public class NatCamDocAttribute : CADescriptionAttribute {
        public NatCamDocAttribute (int id) : base (Map.summaries[id]) {}
        public NatCamDocAttribute (int sid, int id) : base (Map.descriptions[id], Map.summaries[sid]) {}
    }
    
    public class NatCamCodeAttribute : CACodeExampleAttribute {
        public NatCamCodeAttribute (int id) : base (Map.codes[id]) {}
    }
    
    public class NatCamRefAttribute : CASeeAlsoAttribute {
        public NatCamRefAttribute (params int[] ids) : base (Collect(ids)) {}
        private static Type[] Collect (int[] references) {
            Type[] result = new Type[references.Length];
            for (int i = 0; i < references.Length; i++) result[i] = Map.references[i];
            return result;
        }
    }
    #endif

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class NoDocAttribute : Attribute {
        public NoDocAttribute (int id) {}
        public NoDocAttribute (int id, int sid) {}
    }
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, Inherited = false, AllowMultiple = true)]
    public class NoCodeAttribute : Attribute {
        public NoCodeAttribute (int id) {}
    }
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, Inherited = false, AllowMultiple = false)]
    public class NoRefAttribute : Attribute {
        public NoRefAttribute (params int[] id) {}
    }

    public class CoreDocAttribute :
    #if CORE_DOC
    NatCamDocAttribute
    #else
    NoDocAttribute
    #endif
    {
        public CoreDocAttribute (int id) : base (id) {}
        public CoreDocAttribute (int sid, int id) : base (sid, id) {}
    }

    public class CoreCodeAttribute :
    #if CORE_DOC
    NatCamCodeAttribute
    #else
    NoCodeAttribute
    #endif
    {
        public CoreCodeAttribute (int id) : base(id) {}
    }

    public class CoreRefAttribute :
    #if CORE_DOC
    NatCamRefAttribute
    #else
    NoRefAttribute
    #endif
    {
        public CoreRefAttribute (params int[] ids) : base(ids) {}
    }

    public class ExtDocAttribute :
    #if EXTENDED_DOC
    NatCamDocAttribute
    #else
    NoDocAttribute
    #endif
    {
        public ExtDocAttribute (int id) : base (id) {}
        public ExtDocAttribute (int sid, int id) : base (sid, id) {}
    }

    public class ExtCodeAttribute :
    #if EXTENDED_DOC
    NatCamCodeAttribute
    #else
    NoCodeAttribute
    #endif
    {
        public ExtCodeAttribute (int id) : base(id) {}
    }

    public class ExtRefAttribute :
    #if EXTENDED_DOC
    NatCamRefAttribute
    #else
    NoRefAttribute
    #endif
    {
        public ExtRefAttribute (params int[] ids) : base(ids) {}
    }

    public class ProDocAttribute :
    #if PROFESSIONAL_DOC
    NatCamDocAttribute
    #else
    NoDocAttribute
    #endif
    {
        public ProDocAttribute (int id) : base (id) {}
        public ProDocAttribute (int sid, int id) : base (sid, id) {}
    } 

    public class ProCodeAttribute :
    #if PROFESSIONAL_DOC
    NatCamCodeAttribute
    #else
    NoCodeAttribute
    #endif
    {
        public ProCodeAttribute (int id) : base(id) {}
    }

    public class ProRefAttribute :
    #if PROFESSIONAL_DOC
    NatCamRefAttribute
    #else
    NoRefAttribute
    #endif
    {
        public ProRefAttribute (params int[] ids) : base(ids) {}
    }
    
    #if DOC_GEN
    class Map {
        public static readonly string[]
        descriptions = {
            //--Core Spec--
            CD.NatCamDeviceSetFocusDescription,
            CD.FocusModeSoftFocusDescription,
            CD.FocusModeMacroFocusDescription,
            CD.NatCamBehaviourSwitchCameraDescription,
            //--Extended Spec--
            ED.NatCamMetadataDecodeDescription,
            //--Professional Spec--
            PD.NatCamPreviewFrameDescription,
            PD.NatCamPreviewBufferDescription,
            //Forgot :'(
            CD.NatCamDeviceExposureBiasDescription,
            CD.NatCamCapturePhotoDescription,
        },
        summaries = {
            //--Core Spec--
            CD.NatCamDescription,
            CD.NatCamOnPreviewStartSummary,
            CD.NatCamOnPreviewUpdateSummary,
            CD.NatCamImplementationSummary,
            CD.NatCamPreviewSummary,
            CD.NatCamCameraSummary,
            CD.NatCamIsPlayingSummary,
            CD.NatCamPlaySummary,
            CD.NatCamPauseSummary,
            CD.NatCamReleaseSummary,
            CD.NatCamCapturePhotoSummary,
            CD.NatCamDeviceCameraFacingSummary,
            CD.NatCamDeviceCameraPreviewResolutionSummary,
            CD.NatCamDeviceCameraPhotoResolutionSummary,
            CD.NatCamDeviceCameraIsFlashSupportedSummary,
            CD.NatCamDeviceCameraIsTorchSupportedSummary,
            CD.NatCamDeviceCameraIsZoomSupportedSummary,
            CD.NatCamDeviceCameraHorizontalFOVSummary,
            CD.NatCamDeviceCameraVerticalFOVSummary,
            CD.NatCamDeviceCameraMinExposureBiasSummary,
            CD.NatCamDeviceCameraMaxExposureBiasSummary,
            CD.NatCamDeviceFocusModeSummary,
            CD.NatCamDeviceExposureModeSummary,
            CD.NatCamDeviceExposureBiasSummary,
            CD.NatCamDeviceFlashModeSummary,
            CD.NatCamDeviceTorchModeSummary,
            CD.NatCamDeviceZoomRatioSummary,
            CD.NatCamDeviceSetFocusSummary,
            CD.NatCamDeviceSetFramerateFloatSummary,
            CD.NatCamDeviceSetFramerateFrameratePresetSummary,
            CD.NatCamDeviceSetPreviewResolutionIntSummary,
            CD.NatCamDeviceSetPreviewResolutionResolutionPresetSummary,
            CD.NatCamDeviceSetPhotoResolutionIntSummary,
            CD.NatCamDeviceSetPhotoResolutionResolutionPresetSummary,
            CD.NatCamDeviceFrontCameraSummary,
            CD.NatCamDeviceRearCameraSummary,
            CD.NatCamDeviceCamerasSummary,
            CD.PreviewCallbackDescription,
            CD.PhotoCallbackDescription,
            CD.ExposureModeDescription,
            CD.ExposureModeAutoExposeSummary,
            CD.ExposureModeLockedSummary,
            CD.FacingDescription,
            CD.FacingRearSummary,
            CD.FacingFrontSummary,
            CD.FlashModeDescription,
            CD.FlashModeAutoSummary,
            CD.FlashModeOnSummary,
            CD.FlashModeOffSummary,
            CD.FocusModeDescription,
            CD.FocusModeOffSummary,
            CD.FocusModeTapToFocusSummary,
            CD.FocusModeAutoFocusSummary,
            CD.FocusModeSoftFocusSummary,
            CD.FocusModeMacroFocusSummary,
            CD.FrameratePresetDescription,
            CD.FrameratePresetDefaultSummary,
            CD.FrameratePresetSmoothSummary,
            CD.FrameratePresetSlowMotionSummary,
            CD.FrameratePresetHighestSummary,
            CD.FrameratePresetLowestSummary,
            CD.ResolutionPresetDescription,
            CD.ResolutionPresetHDSummary,
            CD.ResolutionPresetFullHDSummary,
            CD.ResolutionPresetHighestResolutionSummary,
            CD.ResolutionPresetMediumResolutionSummary,
            CD.ResolutionPresetLowestResolutionSummary,
            CD.SwitchDescription,
            CD.SwitchOnSummary,
            CD.SwitchOffSummary,
            CD.NatCamBehaviourDescription,
            CD.NatCamBehaviourPreviewSummary,
            CD.NatCamBehaviourFacingSummary,
            CD.NatCamBehaviourPreviewResolutionSummary,
            CD.NatCamBehaviourPhotoResolutionSummary, 
            CD.NatCamBehaviourFramerateSummary,
            CD.NatCamBehaviourVerboseSummary,
            CD.NatCamBehaviourOnPreviewStartSummary,
            CD.NatCamBehaviourOnPreviewUpdateSummary,
            CD.NatCamBehaviourSwitchCameraSummary,
            CD.INatCamDispatchDescription,
            CD.INatCamDispatchDispatchSummary,
            CD.INatCamDispatchReleaseSummary,
            CD.NatCamConcurrentDispatchDescription,
            CD.NatCamConcurrentDispatchCtorSummary,
            CD.NatCamConcurrentDispatchReleaseSummary,
            CD.NatCamMainDispatchDescription,
            CD.NatCamMainDispatchCtorSummary,
            CD.NatCamRenderDispatchDescription,
            CD.NatCamRenderDispatchCtorSummary,
            CD.NatCamRenderDispatchDispatchSummary,
            CD.NatCamAndroidDescription,
            CD.NatCamiOSDescription,
            CD.NatCamLegacyDescription,
            CD.INatCamMobileHasPermissionsSummary,
            CD.NatCamLegacyPreviewTextureSummary,
            CD.NatCamDeviceDescription,
            //--Extended Spec--
            ED.NatCamOnBarcodeDetectSummary,
            ED.NatCamOnFaceDetectSummary,
            ED.NatCamSupportsMetadataSummary,
            ED.NatCamRequestBarcodeSummary,
            ED.NatCamSavePhotoSummary,
            ED.BarcodeCallbackDescription,
            ED.FaceCallbackDescription,
            ED.SaveCallbackDescription,
            ED.BarcodeFormatDescription,
            ED.BarcodeFormatQRSummary,
            ED.BarcodeFormatEAN13Summary,
            ED.BarcodeFormatEAN8Summary,
            ED.BarcodeFormatDataMatrixSummary,
            ED.BarcodeFormatPDF417Summary,
            ED.BarcodeFormatCode128Summary,
            ED.BarcodeFormatCode93Summary,
            ED.BarcodeFormatCode39Summary,
            ED.BarcodeFormatITFSummary,
            ED.BarcodeFormatAllSummary,
            ED.ResetModeDescription,
            ED.ResetModeTweenSummary,
            ED.ResetModeSnapSummary,
            ED.SaveModeDescription,
            ED.SaveModeSaveToAppDocumentsSummary,
            ED.SaveModeSaveToPhotoGallerySummary,
            ED.SaveModeSaveToPhotoAlbumSummary,
            ED.SaveOrientationDescription,
            ED.SaveOrientationRotation0Summary,
            ED.SaveOrientationRotation90Summary,
            ED.SaveOrientationRotation180Summary,
            ED.SaveOrientationRotation270Summary,
            ED.ScaleModeDescription,
            ED.ScaleModeFixedWidthVariableHeightSummary,
            ED.ScaleModeFixedHeightVariableWidthSummary,
            ED.ScaleModeFillViewSummary,
            ED.ScaleModeFillScreenSummary,
            ED.ScaleModeNoneSummary,
            ED.TransformModeDescription,
            ED.TransformModeUVTransformSummary,
            ED.TransformModeVertexTransformSummary,
            ED.ZoomModeDescription,
            ED.ZoomModeDigitalZoomSummary,
            ED.ZoomModeOpticalZoomSummary,
            ED.BarcodeDescription,
            ED.BarcodeDataSummary,
            ED.BarcodeFormatSummary,
            ED.BarcodeTimestampSummary,
            ED.BarcodeCtorSummary,
            ED.BarcodeToStringSummary,
            ED.FaceDescription,
            ED.FaceFaceIDSummary,
            ED.FacePositionSummary,
            ED.FaceSizeSummary,
            ED.FaceRollAngleSummary,
            ED.FaceYawAngleSummary,
            ED.FaceTimestampSummary,
            ED.FaceCtorSummary,
            ED.FaceToStringSummary,
            ED.BarcodeRequestDescription,
            ED.BarcodeRequestCallbackSummary,
            ED.BarcodeRequestFormatSummary,
            ED.BarcodeRequestDetectOnceSummary,
            ED.BarcodeRequestCtorSummary,
            ED.NatCamMetadataDescription,
            ED.NatCamMetadataDecodeSummary,
            ED.NatCamPreviewDescription,
            ED.NatCamPreviewTrackingEnabledSummary,
            ED.NatCamPreviewTransformModeSummary,
            ED.NatCamPreviewScaleModeSummary,
            ED.NatCamPreviewTrackFocusGesturesSummary,
            ED.NatCamPreviewTrackZoomGesturesSummary,
            ED.NatCamPreviewZoomModeSummary,
            ED.NatCamPreviewMaxZoomRatio,
            ED.NatCamPreviewZoomRatioSummary,
            //--Professional Spec--
            PD.NatCamPreviewFrameSummary,
            PD.NatCamPreviewMatrixSummary,
            PD.NatCamIsRecordingSummary,
            PD.NatCamPreviewBufferSummary,
            PD.NatCamStartRecordingSummary,
            PD.NatCamStopRecordingSummary,
            PD.NatCamUtilitiesSummary,
            PD.NatCamUtilitiesSaveVideoToGallerySummary,
            //--Extras--
            CD.ScaleModeDescription,
            CD.ScaleModeFixedWidthVariableHeightSummary,
            CD.ScaleModeFixedHeightVariableWidthSummary,
            CD.ScaleModeFillViewSummary,
            CD.ScaleModeFillScreenSummary,
            CD.ScaleModeNoneSummary,
            CD.ZoomModeDescription,
            CD.ZoomModeDigitalZoomSummary,
            CD.ZoomModeOpticalZoomSummary,
            ED.IMetadataRectSummary,
            ED.IMetadataSupplementSummary,
            ED.FaceRotationSummary,
        },
        codes = {
            //--Core Spec--
            CD.NatCamCameraDeviceCameraExample,
            CD.NatCamCameraIntExample,
            CD.NatCamCapturePhotoDelegateExample,
            CD.NatCamCapturePhotoLambdaExample,
            CD.NatCamDeviceCamerasExample,
            CD.NatCamDeviceSetPreviewResolutionResolutionPresetExample,
            CD.NatCamDeviceSetPreviewResolutionIntExample,
            CD.NatCamDeviceSetFramerateFrameratePresetExample,
            CD.NatCamDeviceSetFramerateFloatExample,
            CD.NatCamDeviceSetFocusExample,
            CD.NatCamDeviceFocusModeExample,
            CD.NatCamDeviceExposureModeExample,
            CD.NatCamDeviceExposureBiasExample,
            CD.NatCamImplementationHasPermissionsExample,
            CD.NatCamBehaviourSwitchCameraNoParamExample,
            CD.NatCamBehaviourSwitchCameraIntExample,
            CD.NatCamBehaviourSwitchCameraDeviceCameraExample,
            CD.NatCamBehaviourOnPreviewUpdateExample,
            //--Extended Spec--
            ED.BarcodeToStringExample,
            ED.FaceToStringExample,
            ED.NatCamRequestBarcodeExample,
            ED.NatCamSavePhotoDefaultsExample,
            ED.NatCamMetadataDecodeExample,
            ED.NatCamSavePhotoFullExample,
            //--Professional Spec--
            PD.NatCamPreviewFrameExample,
            PD.NatCamPreviewMatrixExample,
            PD.NatCamPreviewBufferExample,
            PD.NatCamStartRecordingExample,
            PD.NatCamUtilitiesSaveVideoToGalleryExample
        };
        public static readonly Type[] references = {
            //--Core Spec--
            typeof(NatCamAndroid),
            typeof(NatCamiOS),
            typeof(NatCamLegacy),
        };
    }
    #endif
}
