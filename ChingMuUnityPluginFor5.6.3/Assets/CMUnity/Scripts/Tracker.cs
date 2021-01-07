using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour {

    Vector3 Pos = new Vector3();
    Quaternion quat = new Quaternion();
    void FixedUpdate () {

        // 获取追踪体位置和旋转信息，第一个参数代表追踪系统的IP，第二个参数代表追踪体ID
        //Debug.Log(Config.Instance.CMTrackPreset.Humans[0]);

        Pos = CMUnity.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[0]);
        quat = CMUnity.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[0]);
        
        transform.position = Pos;
        transform.rotation = quat;
    }

}
