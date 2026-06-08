using System;
using System.Runtime.InteropServices;

/**
 * ------------------------------------------------------------
 * InfinadeckAPI.dll InterOp for use with C# Applications.
 * https://github.com/Infinadeck/InfinadeckSDK
 * Created by George Burger @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

namespace Infinadeck
{
    public enum InitError
    {
        None,
        Unknown,
        NoServer,
        UpdateRequired,
        InterfaceVerificationFailed,
        ControllerVerificationFailed,
        FailedInitialization,
        FailedHostResolution,
        FailedServerConnection,
        FailedServerSend,
        RuntimeOutOfDate,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SpeedVector2
    {
        public double v0;
        public double v1;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Ring
    {
        public double x;
        public double y;
        public double z;
        public double r;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TreadmillInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string model_number;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dll_version;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PositionVector3
    {
        public double x;
        public double y;
        public double z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct QuaternionVector4
    {
        public double w;
        public double x;
        public double y;
        public double z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserPositionRotation
    {
        public PositionVector3 position;
        public QuaternionVector4 quaternion;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DiagnosticInfo
    {
        SpeedVector2 service_distance;
        SpeedVector2 total_distance;
        double service_hours;
        double total_hours;
    }

    internal static class NativeMethods
    {
        [DllImport("InfinadeckAPI", EntryPoint = "GetFloorSpeeds", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SpeedVector2 GetFloorSpeeds();

        [DllImport("InfinadeckAPI", EntryPoint = "GetFloorSpeedsNormalized", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SpeedVector2 GetFloorSpeedsNormalized();

        [DllImport("InfinadeckAPI", EntryPoint = "GetFloorSpeedMagnitude", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetFloorSpeedMagnitude();

        [DllImport("InfinadeckAPI", EntryPoint = "GetFloorSpeedAngle", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetFloorSpeedAngle();

        [DllImport("InfinadeckAPI", EntryPoint = "SetManualSpeeds", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetManualSpeeds(double x, double y);

        [DllImport("InfinadeckAPI", EntryPoint = "StartTreadmillUserControl", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StartTreadmillUserControl();

        [DllImport("InfinadeckAPI", EntryPoint = "CheckConnection", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool CheckConnection();

        [DllImport("InfinadeckAPI", EntryPoint = "IsRuntimeOpen", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsRuntimeOpen();

        [DllImport("InfinadeckAPI", EntryPoint = "GetRing", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Ring GetRing();

        [DllImport("InfinadeckAPI", EntryPoint = "SetTreadmillRunState", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetTreadmillRunState(bool state);

        [DllImport("InfinadeckAPI", EntryPoint = "StartTreadmillManualControl", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StartTreadmillManualControl();

        [DllImport("InfinadeckAPI", EntryPoint = "StopTreadmill", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StopTreadmill();

        [DllImport("InfinadeckAPI", EntryPoint = "GetTreadmillRunState", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetTreadmillRunState(bool get_lock);

        [DllImport("InfinadeckAPI", EntryPoint = "GetTreadmillSerialNumber", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetTreadmillSerialNumber(char[] buffer, int length);

        [DllImport("InfinadeckAPI", EntryPoint = "GetTreadmillInfo", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetTreadmillInfo(out TreadmillInfo info);

        [DllImport("InfinadeckAPI", EntryPoint = "GetUserPositionRotation", CallingConvention = CallingConvention.Cdecl)]
        internal static extern UserPositionRotation GetUserPositionRotation();

        [DllImport("InfinadeckAPI", EntryPoint = "GetDiagnostics", CallingConvention = CallingConvention.Cdecl)]
        internal static extern DiagnosticInfo GetDiagnostics();

        [DllImport("InfinadeckAPI", EntryPoint = "SetTreadmillPause", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetTreadmillPause(bool pause);

        [DllImport("InfinadeckAPI", EntryPoint = "GetTreadmillPauseState", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetTreadmillPauseState();

        [DllImport("InfinadeckAPI", EntryPoint = "SetVirtualRing", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetVirtualRing(bool enable);

        [DllImport("InfinadeckAPI", EntryPoint = "GetVirtualRingEnabled", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetVirtualRingEnabled();

        [DllImport("InfinadeckAPI", EntryPoint = "GetReferenceDeviceAngleDifference", CallingConvention = CallingConvention.Cdecl)]
        internal static extern QuaternionVector4 GetReferenceDeviceAngleDifference();

        [DllImport("InfinadeckAPI", EntryPoint = "InitInternal", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint InitInternal(ref InitError inError);

        [DllImport("InfinadeckAPI", EntryPoint = "DeInitInternal", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint DeInitInternal(ref InitError inError);

        [DllImport("InfinadeckAPI", EntryPoint = "GetLastInitErrorDescription", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetLastInitErrorDescription(char[] buffer, int buffer_size = 128);
    }

    /// <summary>
    /// Managed entry point to the Infinadeck native API. All members are static.
    /// (Formerly the <c>Infinadeck.Infinadeck</c> class.)
    /// </summary>
    public static class Sdk
    {
        /**
        * Used to prevent calling CheckConnection when InitConnection returns an error.
        * Once InitConnection returns no errors, CheckConnection should be safe to call.
        */
        private static bool preventEarlyFunctionCalls = false;

        /**
        * Loads internal functionality. Should be called during application
        * initialization
        */
        public static void InitConnection(ref InitError inError)
        {
            NativeMethods.InitInternal(ref inError);
            if (inError == InitError.None) { preventEarlyFunctionCalls = false; }
            else { preventEarlyFunctionCalls = true; }
            return;
        }

        /**
        * Unloads internal functionality. API functions should not be called after
        * this. Should be called on application exit.
        */
        public static void DeInitConnection(ref InitError inError)
        {
            NativeMethods.DeInitInternal(ref inError);
        }

        /**
        * Check if connection to treadmill service has been established.
        */
        public static bool CheckConnection()
        {
            if (preventEarlyFunctionCalls) return false;
            return NativeMethods.CheckConnection();
        }

        /**
        * Check if connected to an instance of the Runtime.
        */
        public static bool CheckRuntimeOpen()
        {
            return NativeMethods.IsRuntimeOpen();
        }

        /**
        * Returns the x and y floor speeds of the treadmill.
        */
        public static SpeedVector2 GetFloorSpeeds()
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            return NativeMethods.GetFloorSpeeds();
        }

        /**
        * Returns the polar magnitude of the speed of the treadmill.
        */
        public static double GetFloorSpeedMagnitude()
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            return NativeMethods.GetFloorSpeedMagnitude();
        }

        /**
        * Returns the polar direction of the speed of the treadmill.
        */
        public static double GetFloorSpeedAngle()
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            return NativeMethods.GetFloorSpeedAngle();
        }

        /**
        * Sets manual floor speed of the treadmill.
        */
        public static void SetManualSpeeds(double x, double y)
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            NativeMethods.SetManualSpeeds(x, y);
        }

        /**
        * Returns true if the treadmill is running, and false if the treadmill is
        * stopped.
        */
        public static bool GetTreadmillRunState()
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            return NativeMethods.GetTreadmillRunState(true);
        }

        /**
        * Requests a change in the treadmill's run state.
        */
        public static void RequestTreadmillRunState(bool run)
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            NativeMethods.SetTreadmillRunState(run);
        }

        /**
        * Check if the treadmill is in "Calibration" mode.
        *
        * NOTE: Not currently implemented
        */
        public static bool GetCalibrating()
        {
            return false;
        }

        /**
        * Returns the x,y,z coordinates of the ring, which corresponds to the center
        * of the treadmill in VR space. Also retrieves the radius of the ring.
        */
        public static Ring GetRingValues()
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            return NativeMethods.GetRing();
        }

        /**
        * Fills a TreadmillInfo struct with information about currently connected
        * treadmill.
        *
        * NOTE: Not currently implemented
        */
        internal static TreadmillInfo GetTreadmillInfo()
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            TreadmillInfo info_payload;
            NativeMethods.GetTreadmillInfo(out info_payload);
            return info_payload;
        }

        /**
        * Puts the treadmill into a "paused" state, where it will not move, but will
        * remain "enabled"
        */
        public static void SetTreadmillPause(bool pause)
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            NativeMethods.SetTreadmillPause(pause);
        }

        /**
        * Checks if the treadmill is in a "paused" state.
        *
        */
        public static bool GetTreadmillPauseState()
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            return NativeMethods.GetTreadmillPauseState();
        }

        /**
        * Enables or disables the virtual ring in the user's virtual display.
        *
        */
        public static void SetVirtualRing(bool pause)
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            NativeMethods.SetVirtualRing(pause);
        }

        /**
        * Checks if the virtual ring should be displayed to the user.
        *
        */
        public static bool GetVirtualRingEnabled()
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            return NativeMethods.GetVirtualRingEnabled();
        }

        /**
        * Get the angle of the reference device relative to the treadmill's orientation.
        *
        */
        public static QuaternionVector4 GetReferenceDeviceAngleDifference()
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            return NativeMethods.GetReferenceDeviceAngleDifference();
        }

        //Deprecated Functions
        /**
        * Start or Stop the treadmill.
        *
        */
        public static void SetTreadmillRunState(bool run)
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            NativeMethods.SetTreadmillRunState(run);
        }

        /**
        * Start the treadmill in manual control mode.
        *
        */
        public static void StartTreadmillManualControl()
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            NativeMethods.StartTreadmillManualControl();
        }

        /**
        * Start the treadmill using tracking controls.
        *
        */
        public static void StartTreadmillUserControl()
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            NativeMethods.StartTreadmillUserControl();
        }

        /**
        * Stop the treadmill.
        *
        */
        public static void StopTreadmill()
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            NativeMethods.StopTreadmill();
        }

        /**
        * Get the position and rotation of the user from the treadmill.
        *
        */
        public static UserPositionRotation GetUserPositionRotation()
        {
            return NativeMethods.GetUserPositionRotation();
        }

        /**
        * Get diagnostic information from the treadmill.
        *
        */
        public static DiagnosticInfo GetDiagnostics()
        {
            return NativeMethods.GetDiagnostics();
        }

        public static String GetLastInitErrorDescription()
        {
            InitError e = InitError.None;
            if (!CheckConnection()) InitConnection(ref e);
            char[] buffer = new char[128];
            NativeMethods.GetLastInitErrorDescription(buffer, 128);
            return new string(buffer);
        }
    }
}
