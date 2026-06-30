using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;
using VRCFaceTracking;
using VRCFaceTracking.Core.Library;
using VRCFT_Vector2 = VRCFaceTracking.Core.Types.Vector2;

namespace PSVR2Toolkit.VRCFT {
    public unsafe class Psvr2TrackingModule : ExtTrackingModule {

        private bool m_eyeAvailable = false;

        private const int k_noiseFilterSamples = 15;
        private LowPassFilter? m_leftEyeOpenLowPass;
        private LowPassFilter? m_rightEyeOpenLowPass;
        private hmd2_gaze_status_t m_gazeStatus;

        public override (bool SupportsEye, bool SupportsExpression) Supported => (true, false);

        public override (bool eyeSuccess, bool expressionSuccess) Initialize(bool eyeAvailable, bool expressionAvailable) {
            ModuleInformation.Name = "PlayStation VR2 Module";

            var stream = GetType().Assembly.GetManifestResourceStream("PSVR2Toolkit.VRCFT.Resources.Logo256.png");
            if ( stream != null ) {
                ModuleInformation.StaticImages = new List<Stream> { stream };
            }

            m_leftEyeOpenLowPass = new LowPassFilter(k_noiseFilterSamples);
            m_rightEyeOpenLowPass = new LowPassFilter(k_noiseFilterSamples);

            try {
                int initResult = PSVR2ToolkitCAPI.Init();
                if (initResult == 0) {
                    m_eyeAvailable = eyeAvailable;
                } else {
                    Logger.LogWarning($"Failed to initialise PSVR2 Toolkit CAPI. Return code = {initResult}. Eye tracking unavailable.");
                    return (false, false);
                }
            } catch (Exception ex) {
                Logger.LogWarning($"Failed to load/initialise PSVR2 Toolkit CAPI library: {ex.Message}. Eye tracking unavailable.");
                return (false, false);
            }
            
            return (m_eyeAvailable, false);
        }

        public override void Teardown() {
            try {
                PSVR2ToolkitCAPI.Deinit();
            } catch (Exception ex) {
                Logger.LogWarning($"Error during PSVR2 Toolkit CAPI Teardown: {ex.Message}");
            }
        }

        public override void Update() {
            if ( Status == ModuleState.Active ) {
                if ( PSVR2ToolkitCAPI.GetGazeStatus(ref m_gazeStatus, 50) ) {

                    if ( m_gazeStatus.wearable.left.is_blink_valid == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE ) {
                        float leftOpenness = m_gazeStatus.wearable.left.blink == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE ? 0.0f : 1.0f;
                        if ( m_leftEyeOpenLowPass != null ) {
                            leftOpenness = m_leftEyeOpenLowPass.FilterValue(leftOpenness);
                        }
                        UnifiedTracking.Data.Eye.Left.Openness = leftOpenness;
                    }
                    if ( m_gazeStatus.wearable.right.is_blink_valid == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE ) {
                        float rightOpenness = m_gazeStatus.wearable.right.blink == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE ? 0.0f : 1.0f;
                        if ( m_rightEyeOpenLowPass != null ) {
                            rightOpenness = m_rightEyeOpenLowPass.FilterValue(rightOpenness);
                        }
                        UnifiedTracking.Data.Eye.Right.Openness = rightOpenness;
                    }

                    if ( m_gazeStatus.wearable.left.is_gaze_dir_valid == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE ) {
                        float x = m_gazeStatus.wearable.left.gaze_dir_norm.x;
                        float y = m_gazeStatus.wearable.left.gaze_dir_norm.y;
                        float z = m_gazeStatus.wearable.left.gaze_dir_norm.z;

                        float yaw = -x / z;
                        float pitch = y / z;
                        
                        UnifiedTracking.Data.Eye.Left.Gaze = new VRCFT_Vector2(yaw, pitch);
                    }
                    if ( m_gazeStatus.wearable.right.is_gaze_dir_valid == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE ) {
                        float x = m_gazeStatus.wearable.right.gaze_dir_norm.x;
                        float y = m_gazeStatus.wearable.right.gaze_dir_norm.y;
                        float z = m_gazeStatus.wearable.right.gaze_dir_norm.z;

                        float yaw = -x / z;
                        float pitch = y / z;
                        
                        UnifiedTracking.Data.Eye.Right.Gaze = new VRCFT_Vector2(yaw, pitch);
                    }

                    // pupil dilation
                    if ( m_gazeStatus.wearable.left.is_pupil_dia_valid == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE ) {
                        UnifiedTracking.Data.Eye.Left.PupilDiameter_MM = m_gazeStatus.wearable.left.pupil_dia_mm;
                    }
                    if ( m_gazeStatus.wearable.right.is_pupil_dia_valid == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE ) {
                        UnifiedTracking.Data.Eye.Right.PupilDiameter_MM = m_gazeStatus.wearable.right.pupil_dia_mm;
                    }

                    // Force the normalization values of Dilation to fit avg. pupil values.
                    UnifiedTracking.Data.Eye._minDilation = 0;
                    UnifiedTracking.Data.Eye._maxDilation = 10;
                }
            }
        }
    }
}
