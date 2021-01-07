using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

/*
    在使用RiftS时，RiftS头显自带的位置与旋转信息会应用到 子物体VR Camera上；为了与青瞳定位融合，所以需要抵消它自带的位置与旋转：
    对于旋转来说，当头显的正方向与青瞳追踪的正方向不一致时，应该抵消这个旋转，通过API CMUnity.CMQuatWithImu()，可以纠正头显的正方向与青瞳追踪正方向之间的偏差，所以头显自带的旋转可以用这个API处理；
    对于位置来说，通过API CMUnity.CMPosWithImu(),并不能抵消掉这个位置，此时有两个办法去抵消这个位置，以下提供了两个办法：
 
     */
public class OnlyRiftSHmd : MonoBehaviour
{

   
  
   Transform Eye = null;
   CMUnity.DevicePose pose;
   Vector3 Pos;
   Quaternion Rot;

   Transform VrCameraRig;


   void Start()
   {
      
       Eye = transform.Find("Eye").GetComponent<Transform>();
       VrCameraRig = transform;

       //第一种办法，去掉VR Camera上  RiftS头显的自带位置信息
        InputTracking.disablePositionalTracking = true;
        Eye.GetChild(0).localPosition = new Vector3(0, 0, 0);


    }

   void FixedUpdate()
   {


       pose.position = InputTracking.GetLocalPosition(VRNode.Head);
       pose.orientation = InputTracking.GetLocalRotation(VRNode.Head);


      
       Pos = CMUnity.CMPosWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);
       Rot = CMUnity.CMQuatWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);


       VrCameraRig.position = Pos;
       VrCameraRig.rotation = Rot;


   }



    /*
  
    Transform Eye = null;
    CMUnity.DevicePose pose;
    Vector3 Pos;
    Quaternion Rot;


    Transform VrCameraRig;
  

    void Start()
    {
       
        Eye = transform.Find("Eye").GetComponent<Transform>();
        VrCameraRig =  transform;


    }

    void FixedUpdate()
    {


        pose.position = InputTracking.GetLocalPosition(VRNode.Head);
        pose.orientation = InputTracking.GetLocalRotation(VRNode.Head);



        //第二种办法，去掉VR Camera上  RiftS头显的自带位置信息
        Pos = CMUnity.CMPosWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);
        Rot = CMUnity.CMQuatWithImu(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.IMUBodies[0], pose);
       

        VrCameraRig.position = new Vector3(Pos.x - Eye.GetChild(0).localPosition.x, Pos.y - Eye.GetChild(0).localPosition.y, Pos.z - Eye.GetChild(0).localPosition.z); ;
        VrCameraRig.rotation = Rot;



    }
    
    */



}
