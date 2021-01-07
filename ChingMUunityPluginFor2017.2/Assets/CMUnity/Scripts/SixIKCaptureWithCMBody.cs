using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixIKCaptureWithCMBody : MonoBehaviour
{
    Animator animator;
    [Range(-1f, 1f)]
    public float RealHuamnMassOffset = 0;
    public Transform CharacterHipTrans;
    public Transform CharacterHeadTrans;
    public Transform CharacterLeftFootTrans;
    public Transform CharacterRightFootTrans;
    Quaternion saveCharacterLeftFootRot;
    Quaternion saveCharacterRightFootRot;
    float characterHipHeight = 0;
    void Start()
    {
        animator = transform.GetComponent<Animator>();
        saveCharacterLeftFootRot = CharacterLeftFootTrans.localRotation;
        saveCharacterRightFootRot = CharacterRightFootTrans.localRotation;

        characterHipHeight = CharacterHipTrans.position.y;
    }

    void ScaleHuman(float characterHipHeight, float humanHipHeight)
    {
        float SclaeFactor = humanHipHeight / characterHipHeight;
        transform.localScale = new Vector3(SclaeFactor, SclaeFactor, SclaeFactor);
    }

    bool ScaleCharacter(string server, int hipBodyId)
    {
        float humanHipHeight = CMVrpn.CMPos(server, hipBodyId).y;
        if (humanHipHeight > 0.6f)
        {
            ScaleHuman(characterHipHeight, humanHipHeight);
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsSacle = false;
    void Update()
    {
        if (!IsSacle)
        {
            IsSacle = ScaleCharacter(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[1]);
        }
    }

    /*
    debug:
    public Transform hipTrans;
    public Transform leftHandTrans;
    public Transform rightHandTrans;
    public Transform leftFootTrans;
    public Transform rightFootTrans;
    private void FixedUpdate()
    {
        //设置BodyMass;
        Vector3 HipPos_w = CMVrpn.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[1]);
        Quaternion HipRotation_w = CMVrpn.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[1]);
        hipTrans.position = HipPos_w;

        //设置LeftHand;
        Vector3 LeftHandPos_w = CMVrpn.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[2]);
        Quaternion LeftHandQua_w = CMVrpn.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[2]);
        leftHandTrans.position = LeftHandPos_w;


        //设置RightHand;
        Vector3 rightHandPos_w = CMVrpn.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[3]);
        Quaternion rightHandQua_w = CMVrpn.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[3]);
        rightHandTrans.position = rightHandPos_w;


        //设置LeftFoot;
        Vector3 leftFootPos_w = CMVrpn.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[4]);
        Quaternion leftFootQua_w = CMVrpn.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[4]);
        leftFootTrans.position = leftFootPos_w;

        //设置RightFoot;
        Vector3 rightFootPos_w = CMVrpn.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[5]);
        Quaternion rightFootQua_w = CMVrpn.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[5]);
        rightFootTrans.position = rightFootPos_w;
    }
    */

    private void FixedUpdate()
    {


    }
    private void OnAnimatorIK(int layerIndex)
    {

        if (IsSacle)
        {

            //设置BodyMass;
            Vector3 HipPos_w = CMVrpn.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[1]);
            Quaternion HipRotation_w = CMVrpn.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[1]);
            animator.bodyPosition = new Vector3(HipPos_w.x, HipPos_w.y - RealHuamnMassOffset, HipPos_w.z);
            animator.bodyRotation = HipRotation_w;


            //设置head;
            Vector3 HeadPos_w = CMVrpn.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[0]);
            Quaternion HeadRotation_w = CMVrpn.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[0]);
            Quaternion newq = Quaternion.Inverse(HipRotation_w) * HeadRotation_w;
            animator.SetBoneLocalRotation(HumanBodyBones.Head, newq);


            //设置LeftHand;
            Vector3 LeftHandPos_w = CMVrpn.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[2]);
            Quaternion LeftHandQua_w = CMVrpn.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[2]);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandPos_w);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);



            //设置RightHand;
            Vector3 rightHandPos_w = CMVrpn.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[3]);
            Quaternion rightHandQua_w = CMVrpn.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[3]);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPos_w);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);




            //设置LeftFoot;
            Vector3 Lpos = CMVrpn.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[4]);
            Vector3 leftFootPos_w = new Vector3(Lpos.x, Lpos.y - RealHuamnMassOffset, Lpos.z);
            Quaternion leftFootQua_w = CMVrpn.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[4]);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPos_w);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            //animator.SetBoneLocalRotation(HumanBodyBones.LeftFoot, saveCharacterLeftFootRot);



            //设置RightFoot;
            Vector3 Rpos = CMVrpn.CMPos(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[5]);
            Vector3 rightFootPos_w = new Vector3(Rpos.x, Rpos.y - RealHuamnMassOffset, Rpos.z);
            Quaternion rightFootQua_w = CMVrpn.CMQuat(Config.Instance.ServerIP, Config.Instance.CMTrackPreset.Bodies[5]);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPos_w);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            //animator.SetBoneLocalRotation(HumanBodyBones.RightFoot, saveCharacterRightFootRot);

        }


    }

}
