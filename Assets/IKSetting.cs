using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using WebSocketSharp.Server;

//    허리 0
//    오른발 1 ~ 3
//    4-6 왼발
//    7 척추
//    8 가슴
//    9 목 
//    10 헤드
//    11 ~ 13 왼팔
//    14 ~ 16 오른팔
//
// 0 엉덩이 (꼬리뼈)
// 1 오른쪽 엉덩이 (오른발 기지)
// 2 오른쪽 무릎
// 3 오른쪽 발목
// 4 左尻 (왼발 관절)
// 5 왼쪽 무릎
// 6 왼쪽 발목
// 7 척추
// 8 가슴
// 9 목 / 코
// 10 머리
// 11 왼쪽 어깨
// 12 왼쪽 팔꿈치
// 13 왼쪽 손목
// 14 우견
// 15 오른쪽 팔꿈치
// 16 오른쪽 손목

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
// { 0, 9 }, = 엉덩이, 목 / 코
// { 9, 10 }, = 목 / 코, 머리
// { 9, 11 }, = 목 / 코 , 왼쪽 어깨
// { 11, 12 }, = 왼쪽 어깨, 왼쪽 팔꿈치
// { 12, 13 }, = 왼쪽 팔꿈치, 왼쪽 손목
// { 9, 14 }, = 목 / 코, 오른쪽 어깨 
// { 14, 15 }, = 오른쪽 어깨, 오른쪽 팔꿈치 
// { 15, 16 } }; = 오른쪽 팔꿈치 , 오른쪽 손목

public class IKSetting : MonoBehaviour
{
    [SerializeField, Range(10, 120)] float FrameRate;
    public List<Transform> BoneList = new List<Transform>();
    [SerializeField] string Data_Path;
    [SerializeField] string File_Name;
    [SerializeField] int Data_Size;
    GameObject FullbodyIK;
    Vector3[] points = new Vector3[17];
    Vector3[] NormalizeBone = new Vector3[12];
    float[] BoneDistance = new float[12];
    float Timer;
    int[, ] joints = new int[, ] { { 0, 1 }, { 1, 2 }, { 2, 3 }, { 0, 4 }, { 4, 5 }, { 5, 6 }, { 0, 7 }, { 7, 8 }, { 8, 9 }, { 9, 10 }, { 8, 11 }, { 11, 12 }, { 12, 13 }, { 8, 14 }, { 14, 15 }, { 15, 16 } };
    int[, ] BoneJoint = new int[, ] { { 0, 2 }, { 2, 3 }, { 0, 5 }, { 5, 6 }, { 0, 9 }, { 9, 10 }, { 9, 11 }, { 11, 12 }, { 12, 13 }, { 9, 14 }, { 14, 15 }, { 15, 16 } };
    int[, ] NormalizeJoint = new int[, ] { { 0, 1 }, { 1, 2 }, { 0, 3 }, { 3, 4 }, { 0, 5 }, { 5, 6 }, { 5, 7 }, { 7, 8 }, { 8, 9 }, { 5, 10 }, { 10, 11 }, { 11, 12 } };
    int NowFrame = 0;
    public WebSocket_Control WebSocketControl;
    
    void Start()
    {
        PointUpdate();
//        PointUpdate_from_msg();
    }
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > (1 / FrameRate))
        {
            Timer = 0;
//            PointUpdate();
            PointUpdate_from_msg();
        }
        if (!FullbodyIK)
        {
            IKFind();
        }
        else
        {
            IKSet();
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
            
            for (int i = 1; i < 17; i++)
            {
                x[i] = x[i] - x[0];
//                y[i] = y[i] - 389.17077f;
                z[i] = z[i] - z[0];
                points[i] = new Vector3(-x[i], y[i], -z[i]);
            }
            points[0] = new Vector3(0, y[0], 0);

            for (int i = 0; i < 12; i++)
            {
                NormalizeBone[i] = (points[BoneJoint[i, 1]] - points[BoneJoint[i, 0]]).normalized;
            }
        }//.if
    }//.PointUpdate_from_msg

    void PointUpdate()
    {
        StreamReader fi = null;
        if (NowFrame < Data_Size)
        {
//            Debug.Log("test : " +NowFrame.ToString());
//            fi = new StreamReader(Application.dataPath + Data_Path + File_Name + NowFrame.ToString() + ".txt");
            fi = new StreamReader(Application.dataPath + Data_Path + File_Name + ".txt");
            NowFrame++;
            string all = fi.ReadToEnd();
            string[] axis = all.Split(']');
            
            // 17 points
            float[] x = axis[0].Replace("[", "").Replace(Environment.NewLine, "").Split(' ').Where(s => s != "").Select(f => float.Parse(f)).ToArray();
            float[] y = axis[2].Replace("[", "").Replace(Environment.NewLine, "").Split(' ').Where(s => s != "").Select(f => float.Parse(f)).ToArray();
            float[] z = axis[1].Replace("[", "").Replace(Environment.NewLine, "").Split(' ').Where(s => s != "").Select(f => float.Parse(f)).ToArray();

            for (int i = 0; i < 17; i++)
            {
                points[i] = new Vector3(-x[i], y[i], -z[i]);
            }
            
//            for (int i = 1; i < 17; i++)
//            {
//                x[i] = x[i] - x[0];
//                y[i] = y[i] - y[0];
//                z[i] = z[i] - z[0];
//                points[i] = new Vector3(-x[i], y[i], -z[i]);
//            }
//            points[0] = new Vector3(0, y[0], 0);
            

            for (int i = 0; i < 12; i++)
            {
                NormalizeBone[i] = (points[BoneJoint[i, 1]] - points[BoneJoint[i, 0]]).normalized;
//                Debug.Log(NormalizeBone[i]);
            }
        }
    }//.PointUpdate
    
    void IKFind()
    {
        FullbodyIK = GameObject.Find("FullBodyIK");
        if (FullbodyIK)
        {
            for (int i = 0; i < Enum.GetNames(typeof(OpenPoseRef)).Length; i++)
            {
                Transform obj = GameObject.Find(Enum.GetName(typeof(OpenPoseRef), i)).transform;
                if (obj)
                {
                    BoneList.Add(obj);
                }
            }
            for (int i = 0; i < Enum.GetNames(typeof(NormalizeBoneRef)).Length; i++)
            {
                BoneDistance[i] = Vector3.Distance(BoneList[NormalizeJoint[i, 0]].position, BoneList[NormalizeJoint[i, 1]].position);
            }
        }
    }

//    private float test = 0.0f;
    void IKSet()
    {
//        Debug.Log(Math.Abs(points[0].x));
//        Debug.Log(Math.Abs(points[0].y));
//        Debug.Log(Math.Abs(points[0].z));
        if (Math.Abs(points[0].x) < 1000 && Math.Abs(points[0].y) < 1000 && Math.Abs(points[0].z) < 1000)
        {
            
            //NormalizeBone[0] = 엉덩이.position - 오른쪽 무릎.position
            //NormalizeBone[2] = 엉덩이.position - 왼쪽 무릎.position
            //NormalizeBone[4] = 엉덩이.position - 목 / 코.position
            
            BoneList[0].position = Vector3.Lerp(BoneList[0].position, points[0] * 0.001f + Vector3.up * 0.8f, 0.1f);
            FullbodyIK.transform.position = Vector3.Lerp(FullbodyIK.transform.position, points[0] * 0.001f, 0.01f);
            Vector3 hipRot = (NormalizeBone[0] + NormalizeBone[2] + NormalizeBone[4]).normalized;
            
//            Debug.Log(hipRot.x);
//            Debug.Log(hipRot.y);
//            Debug.Log(hipRot.z);
            
            FullbodyIK.transform.forward = Vector3.Lerp(FullbodyIK.transform.forward, new Vector3(hipRot.x, 0, hipRot.z), 0.3f);
            
        }
        for (int i = 0; i < 12; i++)
        {
//            BoneList[NormalizeJoint[i, 1]].position = Vector3.Lerp(
//                BoneList[NormalizeJoint[i, 1]].position,
//                BoneList[NormalizeJoint[i, 0]].position + BoneDistance[i] * NormalizeBone[i], 0.05f
//            );
            
            BoneList[NormalizeJoint[i, 1]].position = Vector3.Lerp(
                BoneList[NormalizeJoint[i, 1]].position,
                BoneList[NormalizeJoint[i, 0]].position + BoneDistance[i] * NormalizeBone[i], 1.0f
            );
            
            DrawLine(BoneList[NormalizeJoint[i, 0]].position + Vector3.right, BoneList[NormalizeJoint[i, 1]].position + Vector3.right, Color.red);
        }
        for (int i = 0; i < joints.Length / 2; i++)
        {
            DrawLine(points[joints[i, 0]] * 0.001f + new Vector3(-1, 0.8f, 0), points[joints[i, 1]] * 0.001f + new Vector3(-1, 0.8f, 0), Color.blue);
        }
    }
    void DrawLine(Vector3 s, Vector3 e, Color c)
    {
        Debug.DrawLine(s, e, c);
    }
}
enum OpenPoseRef
{
    Hips,
    RightKnee,
    RightFoot,
    LeftKnee,
    LeftFoot,
    Neck,
    Head,
    LeftArm,
    LeftElbow,
    LeftWrist,
    RightArm,
    RightElbow,
    RightWrist,
};
enum NormalizeBoneRef
{
    Hip2LeftKnee,
    LeftKnee2LeftFoot,
    Hip2RightKnee,
    RightKnee2RightFoot,
    Hip2Neck,
    Neck2Head,
    Neck2RightArm,
    RightArm2RightElbow,
    RightElbow2RightWrist,
    Neck2LeftArm,
    LeftArm2LeftElbow,
    LeftElbow2LeftWrist
};