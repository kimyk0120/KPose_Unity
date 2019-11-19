using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
public class BoneController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField, Range(10, 120)] float FrameRate;
    [SerializeField] GameObject BoneRoot;
    public List<Transform> BoneList = new List<Transform>();
    Vector3[] points = new Vector3[17];
    Vector3[] DefaultNormalizeBone = new Vector3[12];
    Vector3[] NormalizeBone = new Vector3[12];
    Vector3[] LerpedNormalizeBone = new Vector3[12];

    Quaternion[] DefaultBoneRot = new Quaternion[17];
    Quaternion[] DefaultBoneLocalRot = new Quaternion[17];
    Vector3[] DefaultXAxis = new Vector3[17];
    Vector3[] DefaultYAxis = new Vector3[17];
    Vector3[] DefaultZAxis = new Vector3[17];
    
    public WebSocket_Control WebSocketControl;

    float Timer;
    
    //    int[, ] joints = new int[, ] {
// { 0, 1 }, = 엉덩이, 오른쪽 엉덩이 
// { 1, 2 }, = 오른쪽 엉덩이, 오른쪽 무릎
// { 2, 3 }, = 오른쪽 무릎, 오른쪽 발목
// { 0, 4 }, = 엉덩이, 왼쪽 엉덩이 
// { 4, 5 }, = 왼쪽 엉덩이 , 왼쪽 무릎
// { 5, 6 }, = 왼쪽 무릎, 왼쪽 발목
// { 0, 7 }, = 엉덩이, 척추
// { 7, 8 }, = 척추, 가슴
// { 8, 9 }, = 가슴, 목 / 코
// { 9, 10 }, = 목 / 코, 머리
// { 8, 11 }, = 가슴, 왼쪽 어깨
// { 11, 12 }, = 왼쪽 어깨, 왼쪽 팔꿈치
// { 12, 13 }, = 왼쪽 팔꿈치, 왼쪽 손목
// { 8, 14 }, = 가슴 , 오른쪽 어깨
// { 14, 15 }, = 오른쪽 어깨, 오른쪽 팔꿈치 
// { 15, 16 } = 오른쪽 팔꿈치 , 오른쪽 손목};

//    int[, ] BoneJoint = new int[, ] {
// { 0, 2 }, = 엉덩이, 오른쪽 무릎
// { 2, 3 }, = 오른쪽 무릎, 오른쪽 발목
// { 0, 5 }, = 엉덩이, 왼쪽 무릎
// { 5, 6 }, = 왼쪽 무릎, 왼쪽 발목
// { 0, 7 }, = 엉덩이, 척추
// { 7, 8  }, = 척추, 가슴
// { 8, 9 }, = 가슴, 목 / 코
// { 9, 10 }, = 목 / 코, 머리
// { 9, 12 }, = 목 / 코, 왼쪽 팔꿈치
// { 12, 13 }, =  왼쪽 팔꿈치, 왼쪽 손목
// { 9, 15 } }; = 목 / 코, 오른쪽 팔꿈치 
// { 15, 16 } }; = 오른쪽 팔꿈치 , 오른쪽 손목

    
    int[,] joints = new int[,] { { 0, 1 }, { 1, 2 }, { 2, 3 }, { 0, 4 }, { 4, 5 },
                                 { 5, 6 }, { 0, 7 }, { 7, 8 }, { 8, 9 }, { 9, 10},
                                 { 8, 11 }, { 11, 12 }, { 12, 13 }, { 8, 14 }, { 14, 15 },
                                 { 15, 16 } };
    int[,] BoneJoint = new int[,] { { 0, 2 }, { 2, 3 }, { 0, 5 }, { 5, 6 }, { 0, 7 },
                                    { 7, 8 }, { 8, 9 }, { 9, 10 }, { 9, 12 }, { 12, 13 },
                                    { 9, 15 }, { 15, 16 } };
    int NowFrame = 0;
    void Start()
    {
        GetBones();
        PointUpdate();
    }

    void Update()
    {
        PointUpdateByTime();
        SetBoneRot();
    }
    void GetBones()
    {
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.Hips));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.LeftFoot));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.RightUpperLeg));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.RightFoot));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.Spine));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.Chest));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.Neck));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.Head));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.RightUpperArm));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.RightHand));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
        BoneList.Add(animator.GetBoneTransform(HumanBodyBones.LeftHand));

        for (int i = 0; i < 17; i++)
        {
            var rootT = animator.GetBoneTransform(HumanBodyBones.Hips).root; // root : Returns the topmost transform in the hierarchy
            DefaultBoneRot[i] = BoneList[i].rotation;
            DefaultBoneLocalRot[i] = BoneList[i].localRotation;
            DefaultXAxis[i] = new Vector3(
                    Vector3.Dot(BoneList[i].right, rootT.right),
                    Vector3.Dot(BoneList[i].up, rootT.right),
                    Vector3.Dot(BoneList[i].forward, rootT.right)
                    );
            DefaultYAxis[i] = new Vector3(
                    Vector3.Dot(BoneList[i].right, rootT.up),
                    Vector3.Dot(BoneList[i].up, rootT.up),
                    Vector3.Dot(BoneList[i].forward, rootT.up)
                    );
            DefaultZAxis[i] = new Vector3(
                    Vector3.Dot(BoneList[i].right, rootT.forward),
                    Vector3.Dot(BoneList[i].up, rootT.forward),
                    Vector3.Dot(BoneList[i].forward, rootT.forward)
                    );
        }
        for (int i = 0; i < 12; i++)
        {
            DefaultNormalizeBone[i] = (BoneList[BoneJoint[i, 1]].position - BoneList[BoneJoint[i, 0]].position).normalized;
        }
    }
    void PointUpdate()
    {
        if (NowFrame < 600)
        {
            StreamReader fi = new StreamReader(Application.dataPath + "/datas/" + "3d_data0.txt");
            NowFrame++;
            string all = fi.ReadToEnd();
            string[] axis = all.Split(']');
            float[] x = axis[0].Replace("[", "").Replace(Environment.NewLine, "").Split(' ').Where(s => s != "").Select(f => float.Parse(f)).ToArray();
            float[] y = axis[2].Replace("[", "").Replace(Environment.NewLine, "").Split(' ').Where(s => s != "").Select(f => float.Parse(f)).ToArray();
            float[] z = axis[1].Replace("[", "").Replace(Environment.NewLine, "").Split(' ').Where(s => s != "").Select(f => float.Parse(f)).ToArray();
            for (int i = 0; i < 17; i++)
            {
                points[i] = new Vector3(x[i], y[i], -z[i]);
            }
            for (int i = 0; i < 12; i++)
            {
                NormalizeBone[i] = (points[BoneJoint[i, 1]] - points[BoneJoint[i, 0]]).normalized;
            }
        }
    }
    
    private string oldMsg = "";
    private string newMsg = "";
    void PointUpdate_from_msg()
    {
        var msg = WebSocketControl.GetResturnMsg();
        if (msg == null || msg.Equals("")) msg = "";
        if (oldMsg == null || oldMsg.Equals("")) oldMsg = msg;
        if (newMsg == null || newMsg.Equals("")) newMsg = msg;
        
        newMsg = msg;
        
        if (oldMsg.Equals(newMsg)){
            msg = "";
            return;
        }
        else{
            oldMsg = newMsg;
        }

        if (msg!=null&&!msg.Equals(""))
        {
            Debug.Log("WebSocketControl.returnMsg : " + WebSocketControl.GetResturnMsg());
            string[] axis = msg.Split(']');
            float[] x = axis[0].Replace("[", "").Replace(Environment.NewLine, "").Split(' ').Where(s => s != "").Select(f => float.Parse(f)).ToArray();
            float[] y = axis[2].Replace("[", "").Replace(Environment.NewLine, "").Split(' ').Where(s => s != "").Select(f => float.Parse(f)).ToArray();
            float[] z = axis[1].Replace("[", "").Replace(Environment.NewLine, "").Split(' ').Where(s => s != "").Select(f => float.Parse(f)).ToArray();
            
            for (int i = 0; i < 17; i++)
            {
                points[i] = new Vector3(-x[i], y[i], z[i]);
            }
            for (int i = 0; i < 12; i++)
            {
                NormalizeBone[i] = (points[BoneJoint[i, 1]] - points[BoneJoint[i, 0]]).normalized;
            }
        }//.if
    }//.PointUpdate_from_msg
    
    void PointUpdateByTime()
    {
        Timer += Time.deltaTime;
        if (Timer > (1 / FrameRate))
        {
            Timer = 0;
            PointUpdate_from_msg();
        }
    }
    Quaternion GetBoneRot(int jointNum)
    {
        Quaternion target = Quaternion.FromToRotation(DefaultNormalizeBone[jointNum], LerpedNormalizeBone[jointNum]);
        return target;
    }
    void SetBoneRot()
    {
        for (int i = 0; i < 12; i++)
        {
            LerpedNormalizeBone[i] = Vector3.Slerp(LerpedNormalizeBone[i], NormalizeBone[i], 1.0f);
        }
        if (Math.Abs(points[0].x) < 1000 && Math.Abs(points[0].y) < 1000 && Math.Abs(points[0].z) < 1000)
        {
            BoneList[0].position = Vector3.Lerp(BoneList[0].position, points[0] * 0.001f + Vector3.up * 0.8f, 1.0f);
            Vector3 hipRot = (NormalizeBone[0] + NormalizeBone[2] + NormalizeBone[4]).normalized;
            BoneRoot.transform.forward = Vector3.Lerp(BoneRoot.transform.forward, new Vector3(hipRot.x, 0, hipRot.z), 1.0f);
        }
        int j = 0;
        for (int i = 1; i < 17; i++)
        {
            if (i != 3 && i != 6 && i != 13 && i != 16)
            {
                float angle;
                Vector3 axis;
                GetBoneRot(j).ToAngleAxis(out angle, out axis);

                Vector3 axisInLocalCoordinate = axis.x * DefaultXAxis[i] + axis.y * DefaultYAxis[i] + axis.z * DefaultZAxis[i];

                Quaternion modifiedRotation = Quaternion.AngleAxis(angle, axisInLocalCoordinate);

                BoneList[i].localRotation = Quaternion.Lerp(BoneList[i].localRotation, DefaultBoneLocalRot[i] * modifiedRotation, 1.0f);
                j++;
            }
        }
        for (int i = 0; i < 16; i++)
        {
            DrawLine(points[joints[i, 0]] * 0.001f + new Vector3(-1, 0.8f, 0), points[joints[i, 1]] * 0.001f + new Vector3(-1, 0.8f, 0), Color.blue);
            DrawRay(points[joints[i, 0]] * 0.001f + new Vector3(-1, 0.8f, 0), BoneList[i].right * 0.1f, Color.magenta);
            DrawRay(points[joints[i, 0]] * 0.001f + new Vector3(-1, 0.8f, 0), BoneList[i].up * 0.1f, Color.green);
            DrawRay(points[joints[i, 0]] * 0.001f + new Vector3(-1, 0.8f, 0), BoneList[i].forward * 0.1f, Color.cyan);
        }
        for (int i = 0; i < 12; i++)
        {
            DrawRay(points[BoneJoint[i, 0]] * 0.001f + new Vector3(1, 0.8f, 0), NormalizeBone[i] * 0.1f, Color.green);
        }
    }
    void DrawLine(Vector3 s, Vector3 e, Color c)
    {
        Debug.DrawLine(s, e, c);
    }
    void DrawRay(Vector3 s, Vector3 d, Color c)
    {
        Debug.DrawRay(s, d, c);
    }
}
enum PointsNum
{
    Hips, RightUpperLeg, RightLowerLeg, RightFoot, LeftUpperLeg, LeftLowerLeg, LeftFoot, Spine, Chest, Neck, Head, LeftUpperArm, LeftLowerArm, LeftHand, RightUpperArm, RightLowerArm, RightHand
}