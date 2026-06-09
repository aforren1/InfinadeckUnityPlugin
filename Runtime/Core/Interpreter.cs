using System.Collections;
using UnityEngine;

namespace Infinadeck
{
    public class Interpreter : MonoBehaviour
    {
        public float SlowLoopWaitTime = 0.5f;
        public bool Connected { get; private set; }
        public SpeedVector2 FloorSpeeds { get; private set; }
        public double FloorSpeedMagnitude { get; private set; }
        public double FloorSpeedAngle { get; private set; }
        public bool TreadmillRunState { get; private set; }
        public Ring RingValues { get; private set; }
        public bool TreadmillPauseState { get; private set; }
        public bool VirtualRingEnabled { get; private set; }
        public QuaternionVector4 ReferenceDeviceAngleDifference { get; private set; }

        public string TreadmillInfoID { get; private set; }
        public string TreadmillInfoModelNumber { get; private set; }
        public string TreadmillInfoDllVersion { get; private set; }

        private bool running;

        private int loopDelay = 0;

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
            switch (loopDelay)
            {
                case 0: FloorSpeedMagnitude = Sdk.GetFloorSpeedMagnitude(); break;
                case 1: FloorSpeedAngle = Sdk.GetFloorSpeedAngle(); break;
                case 2: RingValues = Sdk.GetRingValues(); break;
                case 3: TreadmillPauseState = Sdk.GetTreadmillPauseState(); break;
                case 4: VirtualRingEnabled = Sdk.GetVirtualRingEnabled(); break;
                case 5: ReferenceDeviceAngleDifference = Sdk.GetReferenceDeviceAngleDifference(); break;
                case 6:
                    TreadmillInfo tInfo = Sdk.GetTreadmillInfo();
                    TreadmillInfoID = tInfo.id;
                    TreadmillInfoModelNumber = tInfo.model_number;
                    TreadmillInfoDllVersion = tInfo.dll_version;
                    break;
                default: loopDelay = -1; break;
            }
            loopDelay++;
        }

        private void FastLoop() //High frequency information, cheap to run, no delay allowed
        {
            FloorSpeeds = Sdk.GetFloorSpeeds();
            TreadmillRunState = Sdk.GetTreadmillRunState();
        }

        public void SetManualSpeeds(double x, double y)
        {
            if (Connected) { Sdk.SetManualSpeeds(x, y); }
        }
        public void RequestTreadmillRunState(bool run)
        {
            if (Connected) { Sdk.RequestTreadmillRunState(run); }
        }
        public void SetTreadmillPause(bool pause)
        {
            if (Connected) { Sdk.SetTreadmillPause(pause); }
        }
        public void SetVirtualRing(bool enable)
        {
            if (Connected) { Sdk.SetVirtualRing(enable); }
        }
        public void StopTreadmill()
        {
            if (Connected) { Sdk.StopTreadmill(); }
        }
        public void StartTreadmillManualControl()
        {
            if (Connected) { Sdk.StartTreadmillManualControl(); }
        }
        public void StartTreadmillUserControl()
        {
            if (Connected) { Sdk.StartTreadmillUserControl(); }
        }
    }
}
