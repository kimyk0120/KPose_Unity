﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using WebSocketSharp.Server;

public class IKSetCowboy : MonoBehaviour
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
    
    // Start is called before the first frame update
    void Start()
    {
        PointUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > (1 / FrameRate))
        {
            Timer = 0;
//            PointUpdate();
//            PointUpdate_from_msg();    
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
        //FullbodyIK = transform.Find("FullBodyIK").gameObject;
        
        if (FullbodyIK)
        {
            for (int i = 0; i < Enum.GetNames(typeof(OpenPoseRef)).Length; i++)
            {
                
                Transform obj = GameObject.Find(Enum.GetName(typeof(OpenPoseRef), i)).transform;
                //Transform obj = GameObject.FindWithTag(Enum.GetName(typeof(OpenPoseRef), i)+"-chan").transform;
                
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
    
    void IKSet()
    {
            
            
        BoneList[0].position = Vector3.Lerp(BoneList[0].position, points[0] * 0.001f + Vector3.up * 0.8f, 0.1f);
        FullbodyIK.transform.position = Vector3.Lerp(FullbodyIK.transform.position, points[0] * 0.001f, 0.01f);
        Vector3 hipRot = (NormalizeBone[0] + NormalizeBone[2] + NormalizeBone[4]).normalized;
        FullbodyIK.transform.forward = Vector3.Lerp(FullbodyIK.transform.forward, new Vector3(hipRot.x, 0, hipRot.z), 0.3f);
        
        
        for (int i = 0; i < 12; i++)
        {
            
            Debug.Log(BoneList[i]);
            BoneList[i].position = Vector3.Lerp(
                BoneList[i].position,
                points[i].normalized + BoneDistance[i] * NormalizeBone[i], 0.05f
            );
            
            //DrawLine(BoneList[NormalizeJoint[i, 0]].position + Vector3.right, BoneList[NormalizeJoint[i, 1]].position + Vector3.right, Color.red);
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
    
}//.class
