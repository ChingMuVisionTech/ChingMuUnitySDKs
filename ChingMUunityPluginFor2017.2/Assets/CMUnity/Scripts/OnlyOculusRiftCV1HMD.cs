using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/*

在使用rift cv1的时候，如果没有sensor，头显就没有自带定位，所以不会给子物体VR camera上实时应用位置信息，但子物体VR Camera上有一个初始位置信息，这个位置信息来自于头显初始化时的位置信息，或者是在stemavr里进行房间设置时产生的位置信息，所以需要抵消掉这个位置，以下提供了两个办法：
另外如果没有sensor，但是头显自带的旋转依然存在，这个旋转会实时应用到子物体 VR Camera上，当头显的正方向与青瞳追踪的正方向不一致时，应该抵消这个旋转，通过API CMVrpn.CMQuatWithImu()，可以纠正头显的正方向与青瞳追踪正方向之间的偏差，所以头显自带的旋转可以用这个API处理；
*/
public class OnlyOculusRiftCV1HMD : MonoBehaviour
{
    
  
    Transform Eye = null;
    CMVrpn.DevicePose pose;
    Vector3 Pos;
    Quaternion Rot;


    void Start()
    {
       
        Eye = transform.Find("Eye").GetComponent<Transform>();

        //第一种办法，去掉VR Camera上Rift cv1 头显的初始位置信息
        InputTracking.disablePositionalTracking = true;
        Eye.GetChild(0).localPosition = new Vector3(0, 0, 0);
    }

    void FixedUpdate()
    {

        pose.position = InputTracking.GetLocalPosition(XRNode.Head);
        pose.orientation = InputTracking.GetLocalRotation(XRNode.Head);


        Pos = CMVrpn.CMPosWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);
        Rot = CMVrpn.CMQuatWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);

        Eye.position = new Vector3(Pos.x, Pos.y, Pos.z);
        Eye.rotation = Rot;


    }




    /*
   
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


        pose.position = InputTracking.GetLocalPosition(XRNode.Head);
        pose.orientation = InputTracking.GetLocalRotation(XRNode.Head);



        Pos = CMVrpn.CMPosWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);
        Rot = CMVrpn.CMQuatWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);


        Eye.rotation = Rot;
        //第二种办法，去掉VR Camera上Rift cv1 头显的初始位置信息;
        Eye.position = new Vector3(Pos.x - Eye.GetChild(0).localPosition.x, Pos.y - Eye.GetChild(0).localPosition.y, Pos.z - Eye.GetChild(0).localPosition.z);

    }
    */
}
