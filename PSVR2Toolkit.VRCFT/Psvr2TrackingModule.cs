using System.Collections.Generic;
using System.IO;
using VRCFaceTracking;
using VRCFaceTracking.Core.Library;
using PSVR2Toolkit.CAPI;
using System.Threading.Tasks;
using VRCFT_Vector2 = VRCFaceTracking.Core.Types.Vector2;
using Microsoft.Extensions.Logging;

namespace PSVR2Toolkit.VRCFT {
    public unsafe class Psvr2TrackingModule : ExtTrackingModule {

        private bool m_eyeAvailable = false;

        private const int k_noiseFilterSamples = 15;
        private LowPassFilter? m_leftEyeOpenLowPass;
        private LowPassFilter? m_rightEyeOpenLowPass;

        public override (bool SupportsEye, bool SupportsExpression) Supported => (true, false);

        public override (bool eyeSuccess, bool expressionSuccess) Initialize(bool eyeAvailable, bool expressionAvailable) {
            ModuleInformation.Name = "PlayStation VR2 Module";

            var stream = GetType().Assembly.GetManifestResourceStream("PSVR2Toolkit.VRCFT.Resources.Logo256.png");
            if ( stream != null ) {
                ModuleInformation.StaticImages = new List<Stream> { stream };
            }

            m_leftEyeOpenLowPass = new LowPassFilter(k_noiseFilterSamples);
            m_rightEyeOpenLowPass = new LowPassFilter(k_noiseFilterSamples);

            IpcClient.Instance().SetLogger(Logger);
            if (IpcClient.Instance().Start()) {
                m_eyeAvailable = eyeAvailable;
            }
            else
            {
                Logger.LogWarning("Failed to initialise PSVR2 Toolkit IPC Client. Eye tracking unavailable.");
                return (false, false);
            }
            
            return (m_eyeAvailable, false);
        }

        public override void Teardown() {
            IpcClient.Instance().Stop();
        }

        public override void Update() {
            if ( Status == ModuleState.Active ) {
                var eyeTrackingData = IpcClient.Instance().RequestEyeTrackingData();

                if ( eyeTrackingData.leftEye.isBlinkValid ) {
                    float leftOpenness = eyeTrackingData.leftEye.isOpenEnabled ? eyeTrackingData.leftEye.open : (eyeTrackingData.leftEye.blink ? 0 : 1);
                    if ( m_leftEyeOpenLowPass != null ) {
                        leftOpenness = m_leftEyeOpenLowPass.FilterValue(leftOpenness);
                    }
                    UnifiedTracking.Data.Eye.Left.Openness = leftOpenness;
                }
                if ( eyeTrackingData.rightEye.isBlinkValid ) {
                    float rightOpenness = eyeTrackingData.rightEye.isOpenEnabled ? eyeTrackingData.rightEye.open : (eyeTrackingData.rightEye.blink ? 0 : 1);
                    if ( m_rightEyeOpenLowPass != null ) {
                        rightOpenness = m_rightEyeOpenLowPass.FilterValue(rightOpenness);
                    }
                    UnifiedTracking.Data.Eye.Right.Openness = rightOpenness;
                }

                if ( eyeTrackingData.leftEye.isGazeDirValid ) {
                    UnifiedTracking.Data.Eye.Left.Gaze = new VRCFT_Vector2(eyeTrackingData.leftEye.gazeDirNorm.x, eyeTrackingData.leftEye.gazeDirNorm.y).FlipXCoordinates();
                }
                if ( eyeTrackingData.rightEye.isGazeDirValid ) {
                    UnifiedTracking.Data.Eye.Right.Gaze = new VRCFT_Vector2(eyeTrackingData.rightEye.gazeDirNorm.x, eyeTrackingData.rightEye.gazeDirNorm.y).FlipXCoordinates();
                }

                // pupil dilation
                if ( eyeTrackingData.leftEye.isPupilDiaValid ) {
                    UnifiedTracking.Data.Eye.Left.PupilDiameter_MM = eyeTrackingData.leftEye.pupilDiaMm;
                }
                if ( eyeTrackingData.rightEye.isPupilDiaValid ) {
                    UnifiedTracking.Data.Eye.Right.PupilDiameter_MM = eyeTrackingData.rightEye.pupilDiaMm;
                }

                // Force the normalization values of Dilation to fit avg. pupil values.
                UnifiedTracking.Data.Eye._minDilation = 0;
                UnifiedTracking.Data.Eye._maxDilation = 10;
            }
        }
    }
}
