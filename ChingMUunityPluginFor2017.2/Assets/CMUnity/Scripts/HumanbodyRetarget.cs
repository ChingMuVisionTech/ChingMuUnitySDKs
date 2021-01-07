using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class HumanbodyRetarget : MonoBehaviour {

    Vector3[] JointWorldPos = new Vector3[150]; //150；
    Quaternion[] JointWorldRot = new Quaternion[150];//150
    bool[] isBoneDetected = new bool[150];//150
    List<Transform> HuamnJointTrans;

    CMUnity.VrpnHierarchy CurCharacterHierResult;
    CMUnity.VrpnHierarchy CurCharacterHier;
    CMUnity.UpdateHierarchyCallback GetCurHierCallback;
    IntPtr callback_args;


    Dictionary<string, Transform> UnityCharAllTransNodeAndNameMap;
    List<string> ChingMUAllRetargetNodeNames;
    List<Transform> CharAllTransNode = new List<Transform>();



    //角色xiaobeixiu_v05 在client里面的ID为0，连本地模拟server，用本地离线数据测试；
    //角色Jigen_Tyan_Casual_clothes_03__zhuan_V04 在client里面的ID为2，连本地模拟server，用本地离线数据测试；
    //角色a_wukong_default_01_rig_hi_sklt.v001_Tpose在client里面的ID为2，连远程实时server，用本地离线数据测试；
    //角色b_moji_default_01_rig_hi_sklt.v001_Tpose_test 在client里面的ID为3，连远程实时server，用本地离线数据测试；
    private int PerStartFindIndex = -1;
    
    [Header("ChingMUTrackerSeting")]
    [Tooltip("ID is Tracker Client manger list Order index")]
    public int ObjectID_InCMTrackSence = 0;
    public string ServerID = string.Empty; //"MCServer@192.168.3.163:3884"
    bool IsRegisterCallBack_Finished = false;
    void Start()
    {
       
        HuamnJointTrans = new List<Transform>();
        GetRetargetDataMapTransHierarchy(transform);
        UnityCharAllTransNodeAndNameMap = new Dictionary<string, Transform>();
        foreach (Transform var in HuamnJointTrans)
        {
            CharAllTransNode.Add(null);
            UnityCharAllTransNodeAndNameMap.Add(var.gameObject.name, var);
        }

        CurCharacterHier = new CMUnity.VrpnHierarchy();
        CurCharacterHierResult = new CMUnity.VrpnHierarchy();

        callback_args = Marshal.AllocHGlobal(Marshal.SizeOf(CurCharacterHier));
        GetCurHierCallback = GetClientThisHumanHierarchy;
        ChingMUAllRetargetNodeNames = new List<string>();
        StartCoroutine(AsyRegisterCallBack_GetVrpnDataOrder());
    }

    bool RegisterCallback_IsFinished()
    {
        bool IsFinished = CMUnity.CMPluginRegisterUpdateHierarchy(ServerID, callback_args, GetCurHierCallback);
        return !IsFinished;
    }
    IEnumerator AsyRegisterCallBack_GetVrpnDataOrder()
    {

        yield return new WaitWhile(RegisterCallback_IsFinished);
        IsRegisterCallBack_Finished = true;
    }

    void GetClientThisHumanHierarchy(IntPtr CallBackFun_agrs, CMUnity.VrpnHierarchy CurHierarchy)
    {
        CurCharacterHierResult = CurHierarchy;
        //Debug.Log("InClient Current node, name,id,ParentID; "+ CurCharacterHierResult.name +"    " +CurCharacterHierResult.sensor+"    "+CurCharacterHierResult.parent);
        int startIndex =(ObjectID_InCMTrackSence * 150 + 100);//150
        int endIndex = startIndex + 150;//150
        if ((startIndex <= CurCharacterHierResult.sensor) && (CurCharacterHierResult.sensor < endIndex))
        {

            if (UnityCharAllTransNodeAndNameMap.ContainsKey(CurCharacterHierResult.name))
            {
                int ChingMUClent_boneId = (CurCharacterHierResult.sensor - 100) % 150;
                CharAllTransNode[ChingMUClent_boneId] = UnityCharAllTransNodeAndNameMap[CurCharacterHierResult.name];
            }
        }
       
    }

    private void OnDestroy()
    {
        Marshal.FreeHGlobal(callback_args);
    }
    void FixedUpdate()
    {
        /*
         当追踪client里的角色列表里的角色层级发生变动，亦或者是管理列表中的对象被删除或者有新对象添加时，或者在第一次连接追踪client时，每在追踪client里面都会触发一个事件，
         这个事件就是，遍历追踪client场景里的所有对象（刚体，与huamn中的每个网格对象，以及骨骼节点对象），当遍历每个对象时就会调用事先注册的回调函数，把当前遍历的节点信息通过回调函数返回
         */
        //用重定向数据驱动
        bool IsTrackedHuman = CMUnity.CMRetargetHuman(ServerID, ObjectID_InCMTrackSence, JointWorldPos, JointWorldRot, isBoneDetected);
        if (IsTrackedHuman && IsRegisterCallBack_Finished)
        {
            for (int i = 0; i < CharAllTransNode.Count; i++)
            {
                if (CharAllTransNode[i] != null)
                {
                    CharAllTransNode[i].localRotation = JointWorldRot[i];
                    CharAllTransNode[i].localPosition = JointWorldPos[i];
                }
            }
        }

    }

    void GetRetargetDataMapTransHierarchy(Transform CurBoneJointTrans)//第一帧深度递归获取对应骨骼节点的transform;
    {

        HuamnJointTrans.Add(CurBoneJointTrans);
        for (int i = 0; i < CurBoneJointTrans.childCount; i++)
        {

            GetRetargetDataMapTransHierarchy(CurBoneJointTrans.GetChild(i));
        }
    }

}