using UnityEngine;
using System.Collections;
using UnityEngine.XR;

public class IMUTracker2 : MonoBehaviour {


    CMVrpn.DevicePose pose;
    Transform leftEye=null;
    Transform rightEye=null;

    void Start ()
	{
		 
        leftEye = transform.Find("CMLeftEye").GetComponent<Transform>();
        rightEye = transform.Find("CMRightEye").GetComponent<Transform>();
    }

	void FixedUpdate () 
	{

        pose.position = InputTracking.GetLocalPosition(XRNode.CenterEye);
        pose.orientation = InputTracking.GetLocalRotation(XRNode.CenterEye);
        // 获取追踪体位置和旋转信息，第一个参数代表追踪系统的IP，第二个参数代表追踪体ID，第三个参数是oculus陀螺仪信息
        Vector3 trackerPos = CMVrpn.CMPosWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);
        Quaternion trackerRot = CMVrpn.CMQuatWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);

        Quaternion actualRotation = trackerRot * pose.orientation;
        leftEye.position = actualRotation * new Vector3(-0.032f, 0, 0) + trackerPos;
        rightEye.position = actualRotation * new Vector3(0.032f, 0, 0) + trackerPos;
        leftEye.rotation = rightEye.rotation = trackerRot;
    }
}
