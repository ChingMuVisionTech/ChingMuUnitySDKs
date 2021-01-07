using System;
using UnityEngine;
using UnityEngine.XR;
using System.Runtime.InteropServices;

public static class CMUnity
{
    private const uint ENGINE_UNITY = 1;

    [DllImport("CMUnity")]
    private static extern void CMUnityStartExtern();

    [DllImport("CMUnity")]
    private static extern void CMUnityQuitExtern();

    [DllImport("CMUnity")]
    private static extern void CMUnityEnableLog(bool IsEnableTrackLog);

    [DllImport("CMUnity")]
    private static extern double CMAnalogExtern(string address, int channel, int frameCount);

    [DllImport("CMUnity")]
    private static extern bool CMButtonExtern(string address, int channel, int frameCount);

    [DllImport("CMUnity")]
    private static extern double CMTrackerExtern(string address, int channel, int component, int frameCount, bool lockUpRotation = false);

    [DllImport("CMUnity")]
    private static extern double CMHeadExtern(string address, int channel, int component, int frameCount, double[] R_oculus, double[] T_oculus, uint platform);

    [DllImport("CMUnity")]
    public static extern bool CMTrackerExternIsDetected(string address, int channel, int frameCount);

    [DllImport("CMUnity")]
    private static extern bool CMHumanExtern(string address, int channel, int frameCount, [In, Out] double[] attitude, [In, Out] int[] segmentIsDetected);
    [DllImport("CMUnity")]
    private static extern bool CMHumanHeadExtern(string address, int channel, int frameCount, double[] R_oculus, [In, Out] double[] rot, [In, Out] double[] pos, uint platform);

    [DllImport("CMUnity")]
    private static extern bool CMRetargetHumanExternTC(string address, int channel, int frameCount, ref int timecode, [In, Out] double[] position, [In, Out]double[] quaternion, [In, Out]int[] segmentIsDetected);

    public struct timeval
    {
        public int tv_sec;
        public int tv_usec;
    }
    public struct VrpnHierarchy
    {
        public timeval msg_time;
        public int sensor;
        public int parent;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 127)]
        public string name;
    }
    public delegate void UpdateHierarchyCallback(IntPtr CallBackFun_agrs, VrpnHierarchy CurHierarchy);
    [DllImport("CMUnity", CallingConvention = CallingConvention.StdCall)]
    public static extern bool CMPluginRegisterUpdateHierarchy(string address, IntPtr CallBackFun_agrs, UpdateHierarchyCallback CallBackGetHierarchy);

    private static bool IsCMTrackThreadRuning = false;
    /// <summary>
    /// VR设备的姿态
    /// </summary>
    public struct DevicePose
    {
        public Vector3 position;
        public Quaternion orientation;


    }
    /// <summary>
    /// 开启追踪线程
    /// when you want to start CMU Tracker Thread,you can call this function;
    /// </summary>
    /// <returns></returns>
    public static void CMUnityStart()
    {
        CMUnityStartExtern();
    }


    /// <summary>
    /// 关闭追踪线程
    /// when you want to stop CMU Tracker Thread,you can call this function in OnDestroy ;
    /// </summary>
    /// <returns></returns>
    public static void CMUnityQuit()
    {
        CMUnityQuitExtern();
    }

    /// <summary>
    /// 是否打印追踪日志
    /// Enable CMUnity to log;
    /// </summary>
    /// <returns></returns>
    public static void CMUnityEnableTrackLog(bool IsEnableTrackLog)
    {
        CMUnityEnableLog(IsEnableTrackLog);
    }

    /// <summary>
    /// 获取被追踪的手柄上的摇杆数据。
    /// Track log
    /// </summary>
    /// <param name="address">ServerIP，for example:"MCServer@192.168.3.178" or "MCServer@SH1DT010"</param>
    /// <param name="channel">ID of Body</param>
    /// <returns></returns>
    public static double CMAnalog(string address, int channel)
    {
        return CMAnalogExtern(address, channel, Time.frameCount);
    }

    public static bool CMButton(string address, int channel)
    {
        return CMButtonExtern(address, channel, Time.frameCount);
    }

    /// <summary>
    /// 刚体追踪数据的空间位置Position
    /// Get the Position of tracker Body
    /// </summary>
    /// <param name="address">ServerIP，for example:"MCServer@192.168.3.178" or "MCServer@SH1DT010"</param>
    /// <param name="channel">ID of Body</param>
    /// <returns></returns> 
    public static Vector3 CMPos(string address, int channel)
    {
        return new Vector3(
        (float)CMTrackerExtern(address, channel, 0, Time.frameCount) / 1000f,
        (float)CMTrackerExtern(address, channel, 2, Time.frameCount) / 1000f,
        (float)CMTrackerExtern(address, channel, 1, Time.frameCount) / 1000f);
    }

    /// <summary>
    /// 刚体追踪数据的旋转值Quaternion
    /// Get the Rotation of tracker Body
    /// </summary>
    /// <param name="address">ServerIP，for example:"MCServer@192.168.3.178" or "MCServer@SH1DT010"</param>
    /// <param name="channel">ID of Body</param>
    /// <param name="lockYRotation">if value is true,the Y axis of rotation  will be lock </param>
    /// <returns></returns>
    public static Quaternion CMQuat(string address, int channel, bool lockYRotation = false)
    {
        return new Quaternion(
        (float)CMTrackerExtern(address, channel, 3, Time.frameCount, lockYRotation),
        (float)CMTrackerExtern(address, channel, 5, Time.frameCount, lockYRotation),
        (float)CMTrackerExtern(address, channel, 4, Time.frameCount, lockYRotation),
        -(float)CMTrackerExtern(address, channel, 6, Time.frameCount, lockYRotation));
    }

    /// <summary>
    /// 获取刚体追踪数据和头显陀螺仪数据融合后的空间位置Position
    /// Get the position  data that Merge tracker Body data with IMU of HMD 
    /// </summary>
    /// <param name="address">ServerIP，for example:"MCServer@192.168.3.178" or "MCServer@SH1DT010"</param>
    /// <param name="channel">ID of Body</param>
    /// <param name="oculusRT">the RT  of IMU</param>
    /// <returns></returns>
    public static Vector3 CMPosWithImu(string address, int channel, DevicePose oculusRT)//OVRPose oculusRT
    {

        double[] oculusQuat = new double[4];
        double[] oculusPos = new double[3];
        oculusQuat[0] = oculusRT.orientation.x;
        oculusQuat[1] = oculusRT.orientation.y;
        oculusQuat[2] = oculusRT.orientation.z;
        oculusQuat[3] = oculusRT.orientation.w;
        oculusPos[0] = oculusRT.position.x;
        oculusPos[1] = oculusRT.position.y;
        oculusPos[2] = oculusRT.position.z;
        return new Vector3(
            (float)CMHeadExtern(address, channel, 0, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY) / 1000f,
            (float)CMHeadExtern(address, channel, 2, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY) / 1000f,
            (float)CMHeadExtern(address, channel, 1, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY) / 1000f);

    }


    /// <summary>
    /// 获取刚体追踪数据和头显陀螺仪数据融合后的四元素旋转值
    /// Get the rotation  data that Merge tracker Body data with IMU of HMD 
    /// </summary>
    /// <param name="address">ServerIP，for example:"MCServer@192.168.3.178" or "MCServer@SH1DT010"</param>
    /// <param name="channel">ID of Body</param>
    /// <param name="oculusRT">the RT  of IMU</param>
    /// <returns>Quaternion of that merge tracker quaternion and quaternion of IMU</returns>
    public static Quaternion CMQuatWithImu(string address, int channel, DevicePose oculusRT)
    {

        double[] oculusQuat = new double[4];
        double[] oculusPos = new double[3];
        oculusQuat[0] = oculusRT.orientation.x;
        oculusQuat[1] = oculusRT.orientation.y;
        oculusQuat[2] = oculusRT.orientation.z;
        oculusQuat[3] = oculusRT.orientation.w;
        oculusPos[0] = oculusRT.position.x;
        oculusPos[1] = oculusRT.position.y;
        oculusPos[2] = oculusRT.position.z;
        return new Quaternion(
            (float)CMHeadExtern(address, channel, 3, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY),
            (float)CMHeadExtern(address, channel, 5, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY),
            (float)CMHeadExtern(address, channel, 4, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY),
            -(float)CMHeadExtern(address, channel, 6, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY));

    }

    /// <summary>
    /// 获取刚体追踪数据和头显陀螺仪数据融合后的空间位置Position
    /// Get the position  data that Merge tracker Body data with IMU of HMD 
    /// </summary>
    /// <param name="address">ServerIP，for example:"MCServer@192.168.3.178" or "MCServer@SH1DT010"</param>
    /// <param name="channel">ID of Body</param>
    /// <param name="touchRT">the RT  of Touch IMU</param>
    /// <returns></returns>
    public static Vector3 CMPosOfTouch(string address, int channel, Quaternion touchRT)
    {

        double[] oculusQuat = new double[4];
        double[] oculusPos = new double[3];
        oculusQuat[0] = touchRT.x;
        oculusQuat[1] = touchRT.y;
        oculusQuat[2] = touchRT.z;
        oculusQuat[3] = touchRT.w;
        oculusPos[0] = 0;
        oculusPos[1] = 0;
        oculusPos[2] = 0;
        return new Vector3(
            (float)CMHeadExtern(address, channel, 0, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY) / 1000f,
            (float)CMHeadExtern(address, channel, 2, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY) / 1000f,
            (float)CMHeadExtern(address, channel, 1, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY) / 1000f);

    }


    /// <summary>
    /// 获取刚体追踪数据和头显陀螺仪数据融合后的四元素旋转值
    /// Get the rotation  data that Merge tracker Body data with IMU of HMD 
    /// </summary>
    /// <param name="address">ServerIP，for example:"MCServer@192.168.3.178" or "MCServer@SH1DT010"</param>
    /// <param name="channel">ID of Body</param>
    /// <param name="touchRT">the RT  of Touch IMU</param>
    /// <returns>Quaternion of that merge tracker quaternion and quaternion of IMU</returns>
    public static Quaternion CMQuatOfTouch(string address, int channel, Quaternion touchRT)
    {

        double[] oculusQuat = new double[4];
        double[] oculusPos = new double[3];
        oculusQuat[0] = touchRT.x;
        oculusQuat[1] = touchRT.y;
        oculusQuat[2] = touchRT.z;
        oculusQuat[3] = touchRT.w;
        oculusPos[0] = 0;
        oculusPos[1] = 0;
        oculusPos[2] = 0;
        return new Quaternion(
            (float)CMHeadExtern(address, channel, 3, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY),
            (float)CMHeadExtern(address, channel, 5, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY),
            (float)CMHeadExtern(address, channel, 4, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY),
            -(float)CMHeadExtern(address, channel, 6, Time.frameCount, oculusQuat, oculusPos, ENGINE_UNITY));

    }


    /// <summary>
    /// 获取全身动捕数据接口，包括23个关节相对旋转值，和根骨骼Hip的世界坐标
    /// Get Human Fullbody Tracker data ,including of 21joints localRotation and root joint world Position;
    /// </summary>
    /// <param name="address">ServerIP，for example:"MCServer@192.168.3.178" or "MCServer@SH1DT010"</param>
    /// <param name="channel">ID of human</param>
    /// <param name="pos">root joint world Position</param>
    /// <param name="rot">23joints localRotation</param>
    /// <param name="segmentIsDetected"></param>
    /// <returns></returns>
    public static bool CMHumanAttitude(string address, int channel, out Vector3 pos, [In, Out] Quaternion[] rot, [In, Out]bool[] segmentIsDetected)
    {


        double[] attitude = new double[95];
        int[] _isDetected = new int[23];
        bool isDetected = CMHumanExtern(address, channel, Time.frameCount, attitude, _isDetected);
        pos = new Vector3((float)attitude[0], (float)attitude[2], (float)attitude[1]) / 1000f;
        for (int i = 0; i < 23; i++)
        {
            if (_isDetected[i] == 1)
            {
                rot[i] = new Quaternion((float)attitude[i * 4 + 3], (float)attitude[i * 4 + 5], (float)attitude[i * 4 + 4], -(float)attitude[i * 4 + 6]);
                segmentIsDetected[i] = true;
            }
            else
            {
                rot[i] = Quaternion.identity;
                segmentIsDetected[i] = false;
            }
        }
        return isDetected;

    }

    public static bool CMRetargetHuman(string address, int channel, [In, Out] Vector3[] pos, [In, Out] Quaternion[] boneRot, [In, Out] bool[] segmentIsDetected)
    {
        int vrpntimecode = 0;
        double[] bonePosition = new double[3 * 150];//150
        double[] boneAttitude = new double[4 * 150];//150
        int[] isBoneDetected = new int[150];//150
        if (pos == null || boneRot == null)
            return false;


        int segmentNum = address.Contains(":3884") ? 150 : 23;//150
        //   Debug.Log("   "+ segmentNum);
        bool isHumanDetected = CMRetargetHumanExternTC(address, channel, Time.frameCount, ref vrpntimecode, bonePosition, boneAttitude, isBoneDetected);
        if (isHumanDetected)
        {
            //set rotation
            for (int i = 0; i < segmentNum; ++i)
            {
                if (isBoneDetected[i] == 1)
                {

                    //set pos
                    pos[i].x = (float)bonePosition[3 * i + 0] / 1000;
                    pos[i].y = (float)bonePosition[3 * i + 2] / 1000;
                    pos[i].z = (float)bonePosition[3 * i + 1] / 1000;


                    //Maya Skeleton
                    boneRot[i] = new Quaternion((float)boneAttitude[i * 4 + 0], (float)boneAttitude[i * 4 + 2], (float)boneAttitude[i * 4 + 1], -(float)boneAttitude[i * 4 + 3]);
                    //  Debug.Log(pos[i] + "  " + boneRot[i]);
                    segmentIsDetected[i] = true;
                }
                else
                {
                    pos[i] = Vector3.zero;
                    boneRot[i] = Quaternion.identity;
                    segmentIsDetected[i] = false;
                }
            }
        }

        //// set timecode
        //if (isHumanDetected && vrpntimecode.ttc.valid)
        //{
        //    timecode.Hours = vrpntimecode.ttc.hours;
        //    timecode.Minutes = vrpntimecode.ttc.minutes;
        //    timecode.Seconds = vrpntimecode.ttc.seconds;
        //    timecode.Frames = vrpntimecode.ttc.frames;
        //    timecode.bDropFrameFormat = false;
        //}
        //else
        //{
        //    timecode.Hours = 0;
        //    timecode.Minutes = 0;
        //    timecode.Seconds = 0;
        //    timecode.Frames = 0;
        //    timecode.bDropFrameFormat = false;
        //}

        return isHumanDetected;
    }
    /// <summary>
    /// 获取全身动捕专用头显追踪数据接口,包括位置和旋转
    /// When you want to creat a game with human Fullbody Tracker,and you want the Tracked Human see the game scene, too. This API for getting the rotation and position data that Merge tracker Body data with IMU of HMD 
    /// </summary>
    /// <param name="address">ServerIP:"MCServer@192.168.3.178"</param>
    /// <param name="channel">human ID</param>
    /// <param name="oculusRT">the RT  of IMU</param>
    /// <param name="rot">out rotation  </param>
    /// <param name="pos">out position</param>
    /// <returns>the body of head is detected or not </returns>
    public static bool CMHumanHead(string address, int channel, DevicePose oculusRT, out Quaternion rot, out Vector3 pos)
    {
        double[] oculusQuat = new double[4];
        oculusQuat[0] = oculusRT.orientation.x;
        oculusQuat[1] = oculusRT.orientation.y;
        oculusQuat[2] = oculusRT.orientation.z;
        oculusQuat[3] = oculusRT.orientation.w;

        double[] _rot = new double[4];
        double[] _pos = new double[3];

        bool isDetected = CMHumanHeadExtern(address, channel, Time.frameCount, oculusQuat, _rot, _pos, ENGINE_UNITY);
        rot = new Quaternion((float)_rot[0], (float)_rot[2], (float)_rot[1], -(float)_rot[3]);
        pos = new Vector3((float)_pos[0], (float)_pos[2], (float)_pos[1]) / 1000f;
        return isDetected;

    }

    /// <summary>
    /// 查询游戏客户端跟CMTracker Server的网络延时
    /// show the network delay between Client and CMTracker Server
    /// </summary>
    /// <param name="address">ServerIP，for example:"MCServer@192.168.3.178" or "MCServer@SH1DT010"</param>
    /// <param name="channel">ID of body</param>
    /// <returns>time Delay(unit: ms )</returns>
    public static double CMTrackerDelay(string address, int channel)
    {
        return CMTrackerExtern(address, channel, 7, Time.frameCount);
    }

    /// <summary>
    /// 检测某个刚体是否正在追踪，特别注意：使用CMTrackerIsDetected（）接口检测是否正在追踪时，代码写在 CMPos（）、CMQuat（）、CMPosWithImu（）和CMQuatWithImu（）等接口调用之后
    /// Check the body has being tracked or not.Atention! When use CMTrackerIsDetected（）to check the body has being tracked or not. "CMTrackerIsDetected（）"code should be byte behind "CMPos（）、CMQuat（）、CMPosWithImu（）和CMQuatWithImu（）"
    /// </summary>
    /// <param name="address">ServerIP，for example:"MCServer@192.168.3.178" or "MCServer@SH1DT010"</param>
    /// <param name="channel">ID of body</param>
    /// <returns>true means is Detected</returns>
    public static bool CMTrackerIsDetected(string address, int channel)
    {

        return CMTrackerExternIsDetected(address, channel, Time.frameCount);

    }
}
