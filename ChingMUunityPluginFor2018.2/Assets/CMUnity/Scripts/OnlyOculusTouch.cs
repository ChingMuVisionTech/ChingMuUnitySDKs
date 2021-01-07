using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class OnlyOculusTouch : MonoBehaviour
{

    public enum OculusTouchType
    {
        LeftTouch,
        RightTouch
    }

    private string ServerIP;
    private int TrackerID;


    public OculusTouchType touchType;
   // private XRNode touchConctroller;

    LineRenderer lineRender;
    public Material lineM;
    public LayerMask Targetlayer;
    void SetLinerender(LineRenderer lineRender)
    {
        lineRender.receiveShadows = false;
        lineRender.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        lineRender.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        lineRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRender.material = lineM;
        lineRender.useWorldSpace = true;
        lineRender.alignment = LineAlignment.View;
#if (UNITY_5_4)
	    lineRender.SetWidth( 0.003f, 0.003f );
#else
        lineRender.startWidth = 0.003f;
        lineRender.endWidth = 0.003f;
#endif
        lineRender.enabled = true;
    }

    RaycastHit hitInfo;
    public Transform grabPointTrans;
    public Transform grabObjectParent;
    string axis;
    void RayGarb()
    {
       
        if (Input.GetAxis(axis) > 0.8f)
        {
            if (hitInfo.transform != null && grabPointTrans.childCount < 1)
            {
                hitInfo.transform.SetParent(grabPointTrans);
            }

        }
        else
        {
            if (grabPointTrans.childCount > 0)
            {
                grabPointTrans.GetChild(0).SetParent(grabObjectParent);
            }
        }
    }
    void RayCheckAndDraw()
    {

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + transform.forward * 8;

        bool IsHit = Physics.Linecast(startPos, endPos, out hitInfo, Targetlayer);
        if (IsHit)
        {
            lineRender.SetPosition(0, startPos);
            lineRender.SetPosition(1, hitInfo.point);
        }
        else
        {
            lineRender.SetPosition(0, startPos);
            lineRender.SetPosition(1, endPos);
        }
    }


    void Start()
    {
        ServerIP = Config.Instance.ServerIP;

        if (touchType == OculusTouchType.LeftTouch)
        {
            //touchConctroller = XRNode.LeftHand;
            TrackerID = Config.Instance.CMTrackPreset.IMUBodies[0];
            lineRender = transform.gameObject.AddComponent<LineRenderer>();
            SetLinerender(lineRender);
            axis = "OculusLeftTouch_PrimaryIndexTrigger";

        }
        else if (touchType == OculusTouchType.RightTouch)
        {
            //touchConctroller = XRNode.RightHand;
            TrackerID = Config.Instance.CMTrackPreset.IMUBodies[1];
            lineRender = transform.gameObject.AddComponent<LineRenderer>();
            SetLinerender(lineRender);
            axis = "OculusRightTouch_PrimaryIndexTrigger";
        }
    }


    Vector3 Pos = new Vector3();
    Quaternion Rot = new Quaternion();
    void FixedUpdate()
    {

        // 获取追踪体位置和旋转信息，第一个参数代表追踪系统的IP，第二个参数代表追踪体ID，第三个参数是oculus陀螺仪信息
        Pos = CMVrpn.CMPos(ServerIP, TrackerID);
        Rot = CMVrpn.CMQuat(ServerIP, TrackerID);

        transform.localPosition = Pos;
        transform.localRotation = Rot;

        RayCheckAndDraw();
        RayGarb();

    }
}
