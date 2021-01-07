﻿using UnityEngine;
using System.Collections;
//using UnityEngine.XR;
using UnityEngine.VR;
public class humanHead : MonoBehaviour
{
    
    Transform leftEye;
    Transform rightEye;
    private Vector3 trackerPos;
    private Quaternion trackerRot;
    private Vector3 humanPos;
    private  Quaternion[] segmentRot = new Quaternion[23];
    private bool[] segmentIsDetected = new bool[23];
    private Quaternion actualRotation;


    void Start()
    {

        leftEye = transform.Find("CMLeftEye").GetComponent<Transform>();
        rightEye = transform.Find("CMRightEye").GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        CMUnity.DevicePose pose; 
        pose.position = InputTracking.GetLocalPosition(VRNode.CenterEye);
        pose.orientation = InputTracking.GetLocalRotation(VRNode.CenterEye);

        trackerPos = CMUnity.CMPosWithImu(Config.Instance.ServerIP, 100 + 24 * (Config.Instance.CMTrackPreset.Humans[0] + 1) - 1, pose);
        trackerRot = CMUnity.CMQuatWithImu(Config.Instance.ServerIP, 100 + 24 * (Config.Instance.CMTrackPreset.Humans[0] + 1) - 1, pose);


        actualRotation = trackerRot * pose.orientation;
        leftEye.position = actualRotation * new Vector3(-0.032f, 0, 0) + trackerPos;
        rightEye.position = actualRotation * new Vector3(0.032f, 0, 0) + trackerPos;
        leftEye.rotation = rightEye.rotation = trackerRot;
    }
}
