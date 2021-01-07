using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR;


public class TouchTracker : MonoBehaviour
{

    public enum TouchType
    {
        LeftTouch,
        RightTouch
    }
    private string ServerIP;
    private int TrackerID;

    public TouchType touchType;
    private VRNode  touchConctroller;

    void Start()
    {
        ServerIP = Config.Instance.ServerIP;        

        if (touchType == TouchType.LeftTouch)
        {
            touchConctroller= VRNode.LeftHand;
            TrackerID = Config.Instance.CMTrackPreset.IMUBodies[0];
        }
        else
        {
            touchConctroller = VRNode.RightHand;
            TrackerID = Config.Instance.CMTrackPreset.IMUBodies[1];
        }
    }  

   
    private Quaternion CMPreQuat;
    private Quaternion TouchPreQuat;
    //private bool IsCMTrackerDetected=false;


    Vector3 Pos=new Vector3();
    Quaternion Rot=new Quaternion();
    void FixedUpdate()
    {
        TouchPreQuat = InputTracking.GetLocalRotation(touchConctroller);
        //TouchPreQuat = OVRInput.GetLocalControllerRotation(touchConctroller);
        //print(TouchPreQuat.eulerAngles);
        // 获取追踪体位置和旋转信息，第一个参数代表追踪系统的IP，第二个参数代表追踪体ID，第三个参数是oculus陀螺仪信息
        Pos = CMVrpn.CMPosOfTouch(Config.Instance.ServerIP, TrackerID, TouchPreQuat);
        Rot = CMVrpn.CMQuatOfTouch(Config.Instance.ServerIP, TrackerID, TouchPreQuat);

        transform.position = Pos;
        transform.rotation = Rot;

    }

}
