using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using LitJson;

// Use plugin namespace
using HybridWebSocket;
using UnityEngine.UI;

public class WebSocket_Control : MonoBehaviour
{

    // ws://echo.websocket.org
//    public string socket_adress = "ws://192.168.1.103:4649/Chat";
    public string socket_adress;
    private WebSocket ws;
    public wc2 wc;
    private string returnMsg;
    private UiScript uiScript;
    private int cnntStat;
    public GameObject player;
    
    void Awake ()
    {
        uiScript = new UiScript();
        cnntStat = 0;
    }
    
    void Update()
    {
        if (cnntStat == 1)
        {
            StopBtnOn();
        }

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (ws!=null&&ws.GetState().ToString()=="Open")
        {
            GameObject inpurpanel = GameObject.FindGameObjectWithTag("inputPanel");
            if(inpurpanel!=null&&inpurpanel.activeSelf==true) inpurpanel.SetActive(false);
            if(player.activeSelf==false) player.SetActive(true);
        }
        else
        {
            if(player!=null&&player.activeSelf==true) player.SetActive(false);
        }
    }

    public string GetResturnMsg()
    {
        return this.returnMsg;
    }

    public void WSConnect()
    {
        // Create WebSocket instance
        ws = WebSocketFactory.CreateInstance(socket_adress);
        
        // Add OnOpen event listener
        ws.OnOpen += () =>
        {
            Debug.Log("WS connected!");
            Debug.Log("WS state: " + ws.GetState().ToString());            
            //ws.Send(Encoding.UTF8.GetBytes("Hello from Unity 3D!"));
        };                

        // Add OnMessage event listener
        weOnMessage();

        // Add OnError event listener
        ws.OnError += (string errMsg) =>
        {
            Debug.Log("WS error: " + errMsg);
            //ws.Close();
        };
        
        
        // Add OnClose event listener
        ws.OnClose += (WebSocketCloseCode code) =>
        {
            Debug.Log("WS closed with code: " + code.ToString());
            cnntStat = 1;
        };
        
        
        // Connect to the server
        ws.Connect();
    }


    public void StartBtnOn(){
        Debug.Log("StartBtnOn");        
//        ws.Send(Encoding.UTF8.GetBytes("Hello from Unity 3D!"));
//        ImageSendOne();
        try
        {
            StartCoroutine(ImageSend);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            cnntStat = 1;
        }
    }

    public void JoinBtnOn(){
        Debug.Log("JoinBtnOn");
        
//        string filePath = Application.dataPath + "/images/full_body_img2.jpg";
//        Texture2D tex = null;
//        byte[] fileData;
 
//        if (File.Exists(filePath))     {
//            Debug.Log("File.Exists");
//            fileData = File.ReadAllBytes(filePath);
//            tex = new Texture2D(2, 2);
//            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
//            byte[] b = tex.EncodeToPNG();
//            ws.Send(b);
//        }
    }

    public void StopBtnOn()
    {
//        Debug.Log("StopBtnOn");
        StopCoroutine(ImageSend);
        uiScript.ColorChg_white();
        cnntStat = 0;
        ws?.Close();
    }
    
    private void weOnMessage(){
        
        ws.OnMessage += (byte[] msg) =>
        {
            //Debug.Log("WS received message: " + Encoding.UTF8.GetString(msg));
            //ws.Close();
            returnMsg = Encoding.UTF8.GetString(msg);
        };        
    }

//    void ImageSend()
//    {
//        
//    }


    IEnumerator ImageSend
    {
        get
        {
            while (true)
            {
                byte[] b = wc.GetImgBytes();
                
//                ReqJson item = new ReqJson("image", Encoding.UTF8.GetString(b));
//                JsonData jsonData = JsonMapper.ToJson(item);
//                File.WriteAllText(Application.dataPath + "/Resources/itemData.json", jsonData.ToString());
//                ws.Send(Encoding.UTF8.GetBytes(jsonData.ToString()));
//                System.Convert.ToBase64String(b);
                try
                {
                    //Debug.Log("send");
                    ws.Send(b);
                }
                catch (Exception e)
                {
                    cnntStat = 1;
                    throw;
                }
                yield return new WaitForSeconds(0.35f);
            }
        }//.ImageSend
    }
    
    
    void ImageSendOne(){
        byte[] b = wc.GetImgBytes();
        ws.Send(b);
        Debug.Log("send one");
    }
    
    public void GetAddress(Text addressText)
    {
        var inputtext = addressText.GetComponent<Text>().text;
        if (inputtext == null || inputtext.Equals("")) return;
        Debug.Log(inputtext);
        this.socket_adress = inputtext;
        try
        {
            WSConnect();
        }
        catch (Exception e)
        {
            Debug.Log("Connect Failed..");
            var messageObj = GameObject.FindGameObjectWithTag("MessageText");
            messageObj.GetComponent<Text>().text = "Connection Failed.."; 
            System.Console.WriteLine(e);
            throw;
        }
    }

}//.class

    


public class ReqJson
{
    public string Name;
    public string Image;
    
    public ReqJson(string name , string image) {
        Name = name;
        Image = image;        
    }
}


