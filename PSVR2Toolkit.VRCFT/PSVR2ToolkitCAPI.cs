using System;
using System.Runtime.InteropServices;

namespace PSVR2Toolkit
{
    #region Enums

    public enum VRControllerType : byte
    {
        Left = 0,
        Right = 1,
        Both = 2
    }

    public enum GazeCalibrationReportMode : ushort
    {
        None = 0,
        StartCalibration = 1,
        CollectCalibrationPoint = 2,
        DiscardCalibrationPoint = 3,
        ComputeAndApplyCalibration = 4,
        RetrieveCalibrationData = 5,
        StopCalibration = 6,
        SetEnabledEye = 7,
        HardwareCalibrationRetrieve = 8
    }

    public enum GazeCalibrationStatus : ushort
    {
        EyetrackingInactive = 0,
        EyetrackingActive = 1,
        CalibrationReady = 2,
        DSPBusy = 3,
        Computing = 4,
        ComputeSucceeded = 5,
        ComputeFailed = 6,
        SettingEye = 7
    }

    public enum GazeCalibrationResult : byte
    {
        Success = 0,
        Failure = 1,
        Discarded = 2,
        Waiting = 3
    }

    public enum hmd2_gaze_bool_t : uint
    {
        HMD2_GAZE_BOOL_FALSE = 0,
        HMD2_GAZE_BOOL_TRUE = 1
    }

    public enum hmd2_gaze_enabled_eye_t : byte
    {
        HMD2_GAZE_ENABLED_EYE_LEFT = 0,
        HMD2_GAZE_ENABLED_EYE_RIGHT = 1,
        HMD2_GAZE_ENABLED_EYE_BOTH = 2
    }

    public enum ScePadTriggerEffectMode : int
    {
        SCE_PAD_TRIGGER_EFFECT_MODE_OFF = 0,
        SCE_PAD_TRIGGER_EFFECT_MODE_FEEDBACK = 1,
        SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON = 2,
        SCE_PAD_TRIGGER_EFFECT_MODE_VIBRATION = 3,
        SCE_PAD_TRIGGER_EFFECT_MODE_MULTIPLE_POSITION_FEEDBACK = 4,
        SCE_PAD_TRIGGER_EFFECT_MODE_SLOPE_FEEDBACK = 5,
        SCE_PAD_TRIGGER_EFFECT_MODE_MULTIPLE_POSITION_VIBRATION = 6
    }

    #endregion

    #region Structs

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct hmd2_gaze_vec2_t
    {
        public float x;
        public float y;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct hmd2_gaze_vec3_t
    {
        public float x;
        public float y;
        public float z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct hmd2_gaze_lens_config_t
    {
        public hmd2_gaze_vec3_t left;
        public hmd2_gaze_vec3_t right;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct hmd2_gaze_wearable_eye_t
    {
        public hmd2_gaze_bool_t is_gaze_origin_valid;
        public hmd2_gaze_vec3_t gaze_origin_mm;
        public hmd2_gaze_bool_t is_gaze_dir_valid;
        public hmd2_gaze_vec3_t gaze_dir_norm;
        public hmd2_gaze_bool_t is_pupil_dia_valid;
        public float pupil_dia_mm;
        public hmd2_gaze_bool_t is_pupil_pos_in_sensor_area_valid;
        public hmd2_gaze_vec2_t pupil_pos_in_sensor_area;
        public hmd2_gaze_bool_t is_pos_guide_valid;
        public hmd2_gaze_vec2_t pos_guide;
        public hmd2_gaze_bool_t is_blink_valid;
        public hmd2_gaze_bool_t blink;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct hmd2_gaze_wearable_data_t
    {
        public long timestamp;
        public uint frame_counter;
        public hmd2_gaze_wearable_eye_t left;
        public hmd2_gaze_wearable_eye_t right;
        public hmd2_gaze_bool_t is_gaze_origin_combined_valid;
        public hmd2_gaze_vec3_t gaze_origin_combined_mm;
        public hmd2_gaze_bool_t is_gaze_dir_combined_valid;
        public hmd2_gaze_vec3_t gaze_dir_combined_norm;
        public hmd2_gaze_bool_t is_convergence_distance_valid;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct hmd2_gaze_foveated_gaze_t
    {
        public long timestamp;
        public uint frame_counter;
        public uint tracking_state;
        public hmd2_gaze_vec3_t gaze_dir_left_norm;
        public hmd2_gaze_vec3_t gaze_dir_right_norm;
        public hmd2_gaze_vec3_t gaze_dir_combined_norm;
        public float convergence_distance_mm;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct hmd2_gaze_status_t
    {
        public byte magic0;
        public byte magic1;
        public ushort version;
        public uint size;
        public float exp_l;
        public float exp_r;
        public uint led_status;
        public uint exp_counter_l;
        public uint exp_counter_r;
        public uint led_counter;
        public int dsp_return_code;
        public hmd2_gaze_lens_config_t lens_config;
        public uint user_calibration_id;
        public hmd2_gaze_vec3_t fr_gaze_origin;
        public hmd2_gaze_enabled_eye_t enabled_eye;
        public byte motor_sequence;
        public byte motor_strength;
        // Padding: the C# compiler aligns this automatically based on Pack=8 rules.
        public hmd2_gaze_wearable_data_t wearable;
        public hmd2_gaze_foveated_gaze_t foveated;
    }

    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct ScePadTriggerEffectCommandData
    {
        // FeedbackParam
        [FieldOffset(0)] public byte feedbackPosition;
        [FieldOffset(1)] public byte feedbackStrength;

        // WeaponParam
        [FieldOffset(0)] public byte weaponStartPosition;
        [FieldOffset(1)] public byte weaponEndPosition;
        [FieldOffset(2)] public byte weaponStrength;

        // VibrationParam
        [FieldOffset(0)] public byte vibrationPosition;
        [FieldOffset(1)] public byte vibrationAmplitude;
        [FieldOffset(2)] public byte vibrationFrequency;

        // MultiplePositionFeedbackParam
        [FieldOffset(0)] public byte multiplePositionFeedbackStrength0;
        [FieldOffset(1)] public byte multiplePositionFeedbackStrength1;
        [FieldOffset(2)] public byte multiplePositionFeedbackStrength2;
        [FieldOffset(3)] public byte multiplePositionFeedbackStrength3;
        [FieldOffset(4)] public byte multiplePositionFeedbackStrength4;
        [FieldOffset(5)] public byte multiplePositionFeedbackStrength5;
        [FieldOffset(6)] public byte multiplePositionFeedbackStrength6;
        [FieldOffset(7)] public byte multiplePositionFeedbackStrength7;
        [FieldOffset(8)] public byte multiplePositionFeedbackStrength8;
        [FieldOffset(9)] public byte multiplePositionFeedbackStrength9;

        // SlopeFeedbackParam
        [FieldOffset(0)] public byte slopeStartPosition;
        [FieldOffset(1)] public byte slopeEndPosition;
        [FieldOffset(2)] public byte slopeStartStrength;
        [FieldOffset(3)] public byte slopeEndStrength;

        // MultiplePositionVibrationParam
        [FieldOffset(0)] public byte multiplePositionVibrationFrequency;
        [FieldOffset(1)] public byte multiplePositionVibrationAmplitude0;
        [FieldOffset(2)] public byte multiplePositionVibrationAmplitude1;
        [FieldOffset(3)] public byte multiplePositionVibrationAmplitude2;
        [FieldOffset(4)] public byte multiplePositionVibrationAmplitude3;
        [FieldOffset(5)] public byte multiplePositionVibrationAmplitude4;
        [FieldOffset(6)] public byte multiplePositionVibrationAmplitude5;
        [FieldOffset(7)] public byte multiplePositionVibrationAmplitude6;
        [FieldOffset(8)] public byte multiplePositionVibrationAmplitude7;
        [FieldOffset(9)] public byte multiplePositionVibrationAmplitude8;
        [FieldOffset(10)] public byte multiplePositionVibrationAmplitude9;

        // Helper methods to set arrays
        public void SetMultiplePositionFeedbackStrength(byte[] strength)
        {
            if (strength == null) throw new ArgumentNullException(nameof(strength));
            int len = Math.Min(strength.Length, 10);
            if (len > 0) multiplePositionFeedbackStrength0 = strength[0];
            if (len > 1) multiplePositionFeedbackStrength1 = strength[1];
            if (len > 2) multiplePositionFeedbackStrength2 = strength[2];
            if (len > 3) multiplePositionFeedbackStrength3 = strength[3];
            if (len > 4) multiplePositionFeedbackStrength4 = strength[4];
            if (len > 5) multiplePositionFeedbackStrength5 = strength[5];
            if (len > 6) multiplePositionFeedbackStrength6 = strength[6];
            if (len > 7) multiplePositionFeedbackStrength7 = strength[7];
            if (len > 8) multiplePositionFeedbackStrength8 = strength[8];
            if (len > 9) multiplePositionFeedbackStrength9 = strength[9];
        }

        public void SetMultiplePositionVibrationAmplitude(byte[] amplitude)
        {
            if (amplitude == null) throw new ArgumentNullException(nameof(amplitude));
            int len = Math.Min(amplitude.Length, 10);
            if (len > 0) multiplePositionVibrationAmplitude0 = amplitude[0];
            if (len > 1) multiplePositionVibrationAmplitude1 = amplitude[1];
            if (len > 2) multiplePositionVibrationAmplitude2 = amplitude[2];
            if (len > 3) multiplePositionVibrationAmplitude3 = amplitude[3];
            if (len > 4) multiplePositionVibrationAmplitude4 = amplitude[4];
            if (len > 5) multiplePositionVibrationAmplitude5 = amplitude[5];
            if (len > 6) multiplePositionVibrationAmplitude6 = amplitude[6];
            if (len > 7) multiplePositionVibrationAmplitude7 = amplitude[7];
            if (len > 8) multiplePositionVibrationAmplitude8 = amplitude[8];
            if (len > 9) multiplePositionVibrationAmplitude9 = amplitude[9];
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ScePadTriggerEffectCommand
    {
        public ScePadTriggerEffectMode mode;
        public uint padding;
        public ScePadTriggerEffectCommandData commandData;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TriggerEffectCommandPayload
    {
        public VRControllerType controllerType;
        public ScePadTriggerEffectCommand command;
    }

    [StructLayout(LayoutKind.Explicit, Size = 13)]
    public struct GazeCalibrationPacket
    {
        [FieldOffset(0)] public float x;
        [FieldOffset(4)] public float y;
        [FieldOffset(8)] public float z;
        [FieldOffset(12)] public GazeCalibrationResult result;
        [FieldOffset(12)] public hmd2_gaze_enabled_eye_t eyeEnabled;
    }

    [StructLayout(LayoutKind.Explicit, Size = 15)]
    public struct GazeCalibrationCommand
    {
        [FieldOffset(0)] public GazeCalibrationReportMode reportMode;
        [FieldOffset(0)] public GazeCalibrationStatus status;
        [FieldOffset(2)] public GazeCalibrationPacket payload;
    }

    #endregion

    public static class PSVR2ToolkitCAPI
    {
        #region Loader P/Invoke

        [DllImport("psvr2_toolkit_capi_loader", EntryPoint = "psvr2_toolkit_loader_get_module_handle", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr psvr2_toolkit_loader_get_module_handle();

        [DllImport("psvr2_toolkit_capi_loader", EntryPoint = "psvr2_toolkit_loader_get_module_path", CallingConvention = CallingConvention.Cdecl)]
        private static extern nuint psvr2_toolkit_loader_get_module_path(IntPtr buffer, nuint bufferSize);

        #endregion

        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int psvr2_toolkit_init_delegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void psvr2_toolkit_deinit_delegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool psvr2_toolkit_gaze_status_delegate(ref hmd2_gaze_status_t pGazeStatus, uint timeoutMs);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool psvr2_toolkit_gaze_image_delegate(ref IntPtr pGazeImage, uint timeoutMs);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void psvr2_toolkit_write_pcm_delegate(VRControllerType controllerType, IntPtr pcm);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void psvr2_toolkit_wait_for_pcm_delegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void psvr2_toolkit_set_trigger_effect_delegate(VRControllerType controllerType, ref ScePadTriggerEffectCommand command);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void psvr2_toolkit_set_hmd_rumble_delegate(byte rumbleHz);

        // Private
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate GazeCalibrationCommand psvr2_toolkit_private_send_gaze_set_command_delegate(GazeCalibrationCommand command);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate GazeCalibrationCommand psvr2_toolkit_private_send_gaze_get_command_delegate(GazeCalibrationCommand command);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void psvr2_toolkit_private_set_usb_connection_state_delegate(bool connected);

        #endregion

        #pragma warning disable CS8618
        #region Function Fields

        private static psvr2_toolkit_init_delegate _init;
        private static psvr2_toolkit_deinit_delegate _deinit;
        private static psvr2_toolkit_gaze_status_delegate _gaze_status;
        private static psvr2_toolkit_gaze_image_delegate _gaze_image;
        private static psvr2_toolkit_write_pcm_delegate _write_pcm;
        private static psvr2_toolkit_wait_for_pcm_delegate _wait_for_pcm;
        private static psvr2_toolkit_set_trigger_effect_delegate _set_trigger_effect;
        private static psvr2_toolkit_set_hmd_rumble_delegate _set_hmd_rumble;

        private static psvr2_toolkit_private_send_gaze_set_command_delegate _private_send_gaze_set_command;
        private static psvr2_toolkit_private_send_gaze_get_command_delegate _private_send_gaze_get_command;
        private static psvr2_toolkit_private_set_usb_connection_state_delegate _private_set_usb_connection_state;

        #endregion
        #pragma warning restore CS8618

        private static IntPtr _moduleHandle = IntPtr.Zero;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the module handle of the loaded CAPI.
        /// </summary>
        public static IntPtr ModuleHandle
        {
            get
            {
                EnsureLoaded();
                return _moduleHandle;
            }
        }

        /// <summary>
        /// Retrieves the path of the loaded CAPI module.
        /// </summary>
        public static string GetModulePath()
        {
            EnsureLoaded();
            try
            {
                // First call to get the size
                nuint requiredSize = psvr2_toolkit_loader_get_module_path(IntPtr.Zero, 0);
                if (requiredSize == 0) return string.Empty;

                IntPtr buffer = Marshal.AllocHGlobal((int)(requiredSize + 1));
                try
                {
                    nuint actualSize = psvr2_toolkit_loader_get_module_path(buffer, requiredSize + 1);
                    return Marshal.PtrToStringAnsi(buffer);
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
            catch
            {
                return "Unknown (Failed to query loader)";
            }
        }

        private static void EnsureLoaded()
        {
            if (_moduleHandle != IntPtr.Zero) return;

            lock (_lock)
            {
                if (_moduleHandle != IntPtr.Zero) return;

                try
                {
                    _moduleHandle = psvr2_toolkit_loader_get_module_handle();
                }
                catch (DllNotFoundException ex)
                {
                    throw new DllNotFoundException(
                        "Could not load the loader library 'psvr2_toolkit_capi_loader'. " +
                        "Make sure 'psvr2_toolkit_capi_loader.dll' is in your application directory or search path.", ex);
                }

                if (_moduleHandle == IntPtr.Zero)
                {
                    throw new InvalidOperationException(
                        "psvr2_toolkit_loader_get_module_handle returned NULL. " +
                        "Make sure psvr2tk_capi_path.txt contains the correct directory of 'psvr2_toolkit_capi.dll' " +
                        "and the dll exists in that directory.");
                }

                // Bind delegates
                _init = GetExport<psvr2_toolkit_init_delegate>("psvr2_toolkit_init");
                _deinit = GetExport<psvr2_toolkit_deinit_delegate>("psvr2_toolkit_deinit");
                _gaze_status = GetExport<psvr2_toolkit_gaze_status_delegate>("psvr2_toolkit_gaze_status");
                _gaze_image = GetExport<psvr2_toolkit_gaze_image_delegate>("psvr2_toolkit_gaze_image");
                _write_pcm = GetExport<psvr2_toolkit_write_pcm_delegate>("psvr2_toolkit_write_pcm");
                _wait_for_pcm = GetExport<psvr2_toolkit_wait_for_pcm_delegate>("psvr2_toolkit_wait_for_pcm");
                _set_trigger_effect = GetExport<psvr2_toolkit_set_trigger_effect_delegate>("psvr2_toolkit_set_trigger_effect");
                _set_hmd_rumble = GetExport<psvr2_toolkit_set_hmd_rumble_delegate>("psvr2_toolkit_set_hmd_rumble");

                // Private
                _private_send_gaze_set_command = GetExport<psvr2_toolkit_private_send_gaze_set_command_delegate>("psvr2_toolkit_private_send_gaze_set_command");
                _private_send_gaze_get_command = GetExport<psvr2_toolkit_private_send_gaze_get_command_delegate>("psvr2_toolkit_private_send_gaze_get_command");
                _private_set_usb_connection_state = GetExport<psvr2_toolkit_private_set_usb_connection_state_delegate>("psvr2_toolkit_private_set_usb_connection_state");
            }
        }

        private static T GetExport<T>(string name) where T : Delegate
        {
            IntPtr address = GetProcAddress(_moduleHandle, name);
            if (address == IntPtr.Zero)
            {
                throw new EntryPointNotFoundException($"Could not find entry point '{name}' in the loaded PSVR2 Toolkit CAPI library.");
            }
            return (T)Marshal.GetDelegateForFunctionPointer(address, typeof(T));
        }

        private static IntPtr GetProcAddress(IntPtr hModule, string procName)
        {
            // Use modern .NET NativeLibrary via reflection if available
            var nativeLibraryType = Type.GetType("System.Runtime.InteropServices.NativeLibrary, System.Runtime.InteropServices");
            if (nativeLibraryType != null)
            {
                var getExportMethod = nativeLibraryType.GetMethod("GetExport", new[] { typeof(IntPtr), typeof(string) });
                if (getExportMethod != null)
                {
                    try
                    {
                        return (IntPtr)getExportMethod.Invoke(null, new object[] { hModule, procName });
                    }
                    catch
                    {
                        // Fall back to platform specific imports
                    }
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Win32GetProcAddress(hModule, procName);
            }
            else
            {
                try
                {
                    return UnixDlsym(hModule, procName);
                }
                catch
                {
                    return UnixDlsym2(hModule, procName);
                }
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr Win32GetProcAddress(IntPtr hModule, string procName);

        [DllImport("libdl", EntryPoint = "dlsym", CharSet = CharSet.Ansi)]
        private static extern IntPtr UnixDlsym(IntPtr handle, string symbol);

        [DllImport("libdl.so.2", EntryPoint = "dlsym", CharSet = CharSet.Ansi)]
        private static extern IntPtr UnixDlsym2(IntPtr handle, string symbol);

        #region Public API Methods

        public static int Init()
        {
            EnsureLoaded();
            return _init();
        }

        public static void Deinit()
        {
            EnsureLoaded();
            _deinit();
        }

        public static bool GetGazeStatus(ref hmd2_gaze_status_t gazeStatus, uint timeoutMs)
        {
            EnsureLoaded();
            return _gaze_status(ref gazeStatus, timeoutMs);
        }

        public static unsafe bool GetGazeImage(ref byte[] gazeImage, uint timeoutMs)
        {
            if (gazeImage == null) throw new ArgumentNullException(nameof(gazeImage));
            if (gazeImage.Length < 0x200100) throw new ArgumentException("Array size must be at least 0x200100 bytes.", nameof(gazeImage));

            EnsureLoaded();
            IntPtr pGazeImage = IntPtr.Zero;
            bool success = _gaze_image(ref pGazeImage, timeoutMs);
            if (success && pGazeImage != IntPtr.Zero)
            {
                fixed (byte* p = gazeImage)
                {
                    Buffer.MemoryCopy((void*)pGazeImage, p, gazeImage.Length, 0x200100);
                }
            }
            return success;
        }

        public static unsafe bool GetGazeImage(ref IntPtr pGazeImage, uint timeoutMs)
        {
            EnsureLoaded();
            return _gaze_image(ref pGazeImage, timeoutMs);
        }

        public static unsafe void WritePcm(VRControllerType controllerType, byte[] pcm)
        {
            if (pcm == null) throw new ArgumentNullException(nameof(pcm));
            if (pcm.Length < 32) throw new ArgumentException("PCM array must be at least 32 bytes.", nameof(pcm));

            EnsureLoaded();
            fixed (byte* p = pcm)
            {
                _write_pcm(controllerType, (IntPtr)p);
            }
        }

        public static unsafe void WritePcm(VRControllerType controllerType, IntPtr pcm)
        {
            if (pcm == IntPtr.Zero) throw new ArgumentNullException(nameof(pcm));
            EnsureLoaded();
            _write_pcm(controllerType, pcm);
        }

        public static void WaitForPcm()
        {
            EnsureLoaded();
            _wait_for_pcm();
        }

        public static void SetTriggerEffect(VRControllerType controllerType, ref ScePadTriggerEffectCommand command)
        {
            EnsureLoaded();
            _set_trigger_effect(controllerType, ref command);
        }

        public static void SetHmdRumble(byte rumbleHz)
        {
            EnsureLoaded();
            _set_hmd_rumble(rumbleHz);
        }

        #endregion

        #region Public Private-API Methods

        public static GazeCalibrationCommand SendGazeSetCommand(GazeCalibrationCommand command)
        {
            EnsureLoaded();
            return _private_send_gaze_set_command(command);
        }

        public static GazeCalibrationCommand SendGazeGetCommand(GazeCalibrationCommand command)
        {
            EnsureLoaded();
            return _private_send_gaze_get_command(command);
        }

        public static void SetUsbConnectionState(bool connected)
        {
            EnsureLoaded();
            _private_set_usb_connection_state(connected);
        }

        #endregion
    }
}
