using System.Collections;
using UnityEngine;

namespace Infinadeck
{
    public class Interpreter : MonoBehaviour
    {
        public float SlowLoopWaitTime = 0.5f;
        public bool Connected { get; private set; }
        public SpeedVector2 InfIntGetFloorSpeeds { get; private set; }
        public double InfIntGetFloorSpeedMagnitude { get; private set; }
        public double InfIntGetFloorSpeedAngle { get; private set; }
        public bool InfIntGetTreadmillRunState { get; private set; }
        public Ring InfIntGetRingValues { get; private set; }
        public bool InfIntGetTreadmillPauseState { get; private set; }
        public bool InfIntGetVirtualRingEnabled { get; private set; }
        public QuaternionVector4 InfIntGetReferenceDeviceAngleDifference { get; private set; }

        public string InfIntGetTreadmillInfoID { get; private set; }
        public string InfIntGetTreadmillInfoModel_Number { get; private set; }
        public string InfIntGetTreadmillInfoDLL_Version { get; private set; }

        private bool running;

        private int loopdelay = 0;

        private Coroutine slowCo = null;

        public string errorInfo;

        private void Awake()
        {
            this.enabled = false;
        }

        void OnEnable()
        {
            slowCo = StartCoroutine(SlowLoop());
        }

        void OnDisable()
        {
            if (running)
            {
                StopCoroutine(slowCo);
                running = false;
            }
        }

        private void OnApplicationQuit()
        {
            if (Connected) { Sdk.StopTreadmill(); }
        }

        // Update is called once per frame
        void Update()
        {
            if (Connected)
            {
                FastLoop();
                StandardLoop();
            }
        }

        private IEnumerator SlowLoop() //Expensive to run
        {
            running = true;
            while (true)
            {
                if (Sdk.CheckRuntimeOpen())
                {
                    if (!Sdk.CheckConnection())
                    {
                        InitError e = InitError.None;
                        Sdk.InitConnection(ref e);
                        Connected = Sdk.CheckConnection();
                        errorInfo = "";
                        if (!Connected) ///If connection attempt failed, report why
                        {
                            if (e == InitError.None) { errorInfo = "ERROR: INIT CONNECTION FAILED WITH NO ERROR"; }
                            else if (e == InitError.Unknown) { errorInfo = "ERROR: UNKNOWN ERROR"; }
                            else if (e == InitError.NoServer) { errorInfo = "ERROR: NO SERVER FOUND"; }
                            else if (e == InitError.UpdateRequired) { errorInfo = "ERROR: GAME API UPDATE REQUIRED"; }
                            else if (e == InitError.InterfaceVerificationFailed) { errorInfo = "ERROR: FAILED TO VERIFY INTERFACE"; }
                            else if (e == InitError.ControllerVerificationFailed) { errorInfo = "ERROR: FAILED TO VERIFY CONTROLLER"; }
                            else if (e == InitError.FailedInitialization) { errorInfo = "ERROR: FAILED TO INITIALIZE"; }
                            else if (e == InitError.FailedHostResolution) { errorInfo = "ERROR: FAILED TO RESOLVE HOST"; }
                            else if (e == InitError.FailedServerConnection) { errorInfo = "ERROR: FAILED TO CONNECT TO SERVER"; }
                            else if (e == InitError.FailedServerSend) { errorInfo = "ERROR: FAILED TO SEND PACKET TO SERVER"; }
                            else if (e == InitError.RuntimeOutOfDate) { errorInfo = "ERROR: RUNTIME OUT OF DATE"; }
                            else { errorInfo = "ERROR: UNDOCUMENTED ERROR"; }
                        }
                    }
                    else { Connected = true; }
                }
                else { Connected = false; }

                yield return new WaitForSeconds(SlowLoopWaitTime);
            }
        }

        private void StandardLoop() //Low frequency information, delay allowed
        {
            switch (loopdelay)
            {
                case 0: InfIntGetFloorSpeedMagnitude = Sdk.GetFloorSpeedMagnitude(); break;
                case 1: InfIntGetFloorSpeedAngle = Sdk.GetFloorSpeedAngle(); break;
                case 2: InfIntGetRingValues = Sdk.GetRingValues(); break;
                case 3: InfIntGetTreadmillPauseState = Sdk.GetTreadmillPauseState(); break;
                case 4: InfIntGetVirtualRingEnabled = Sdk.GetVirtualRingEnabled(); break;
                case 5: InfIntGetReferenceDeviceAngleDifference = Sdk.GetReferenceDeviceAngleDifference(); break;
                case 6: InfIntGetTreadmillInfoID = Sdk.GetTreadmillInfo().id; break;
                case 7: InfIntGetTreadmillInfoModel_Number = Sdk.GetTreadmillInfo().model_number; break;
                case 8: InfIntGetTreadmillInfoDLL_Version = Sdk.GetTreadmillInfo().dll_version; break;
                default: loopdelay = -1; break;
            }
            loopdelay++;
        }

        private void FastLoop() //High frequency information, cheap to run, no delay allowed
        {
            InfIntGetFloorSpeeds = Sdk.GetFloorSpeeds();
            InfIntGetTreadmillRunState = Sdk.GetTreadmillRunState();
        }

        public void InfIntSetManualSpeeds(double x, double y)
        {
            if (Connected) { Sdk.SetManualSpeeds(x, y); }
        }
        public void InfIntRequestTreadmillRunState(bool run)
        {
            if (Connected) { Sdk.RequestTreadmillRunState(run); }
        }
        public void InfIntSetTreadmillPause(bool pause)
        {
            if (Connected) { Sdk.SetTreadmillPause(pause); }
        }
        public void InfIntSetVirtualRing(bool pause)
        {
            if (Connected) { Sdk.SetTreadmillPause(pause); }
        }
        public void InfIntStopTreadmill()
        {
            if (Connected) { Sdk.StopTreadmill(); }
        }
        public void InfIntStartTreadmillManualControl()
        {
            if (Connected) { Sdk.StartTreadmillManualControl(); }
        }
        public void InfIntStartTreadmillUserControl()
        {
            if (Connected) { Sdk.StartTreadmillUserControl(); }
        }
    }
}
