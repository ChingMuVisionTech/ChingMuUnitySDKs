using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CMFullBodyCapWithHMD : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform CharacterRootTrans;
    public Transform CharacterHeadTrans;
    public Transform OVRCameraRig;

    CMVrpn.DevicePose pose;
    float offset_y = 0.1f;
    void Start()
    {
       // CharacterRootTrans.gameObject.AddComponent<humanbody>();
        InputTracking.disablePositionalTracking = true;
        OVRCameraRig.GetChild(1).GetChild(0).localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        pose.position = InputTracking.GetLocalPosition(XRNode.Head);
        pose.orientation = InputTracking.GetLocalRotation(XRNode.Head);
  
        Quaternion ChingMU_hmd_Rot = CMVrpn.CMQuatWithImu(Config.Instance.ServerIP, 100 + 24 * (Config.Instance.CMTrackPreset.Humans[0] + 1) - 1, pose);
        OVRCameraRig.rotation = ChingMU_hmd_Rot;
        OVRCameraRig.position = CharacterHeadTrans.position + new Vector3(0, offset_y, 0);
    }
}
