using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.XR;
using UnityEngine.VR;

public class IMUTracker : MonoBehaviour {

   
    Transform Eye = null;
    CMUnity.DevicePose pose;
    Vector3 Pos;
    Quaternion Rot;

    void Start()
    {
        
        Eye = transform.Find("Eye").GetComponent<Transform>();
    }

    void FixedUpdate()
    {


        pose.position = InputTracking.GetLocalPosition(VRNode.CenterEye);
        pose.orientation = InputTracking.GetLocalRotation(VRNode.CenterEye);
   
        Pos = CMUnity.CMPosWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);
        Rot = CMUnity.CMQuatWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);

        Eye.position = Pos;
        Eye.rotation = Rot;
    }
}
