using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class IMUTracker : MonoBehaviour {

   
    Transform Eye = null;
    CMVrpn.DevicePose pose;
    Vector3 Pos;
    Quaternion Rot;

    void Start()
    {
        
        Eye = transform.Find("Eye").GetComponent<Transform>();
    }

    void FixedUpdate()
    {


        pose.position = InputTracking.GetLocalPosition(XRNode.CenterEye);
        pose.orientation = InputTracking.GetLocalRotation(XRNode.CenterEye);
   
        Pos = CMVrpn.CMPosWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);
        Rot = CMVrpn.CMQuatWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);

        Eye.position = Pos;
        Eye.rotation = Rot;
    }
}
