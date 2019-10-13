using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using LitJson;

// Use plugin namespace
using HybridWebSocket;

public class WebSocket_Control : MonoBehaviour {

    // ws://echo.websocket.org
    public string socket_adress = "ws://192.168.1.103:4649/Chat";
    private WebSocket ws;
    public Webcam wc;

    // Use this for initialization
    void Start () {
        

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
        };

        // Add OnClose event listener
        ws.OnClose += (WebSocketCloseCode code) =>
        {
            Debug.Log("WS closed with code: " + code.ToString());            
        };

        // Connect to the server
        ws.Connect();
    }
	
    public void StartBtnOn(){
        Debug.Log("StartBtnOn");        
        ws.Send(Encoding.UTF8.GetBytes("Hello from Unity 3D!"));
        StartCoroutine(ImageSend);
    }

    public void JoinBtnOn(){
        Debug.Log("JoinBtnOn");
    }
    
    private void weOnMessage(){
        
        ws.OnMessage += (byte[] msg) =>
        {
            Debug.Log("WS received message: " + Encoding.UTF8.GetString(msg));
            //ws.Close();
        };        
    }


    IEnumerator ImageSend
    {
        get
        {
            Debug.Log("while Image Send");
            while (true)
            {
                
                byte[] b = wc.GetImgBytes();
                //ws.Send(Encoding.UTF8.GetBytes("Image Send"));
                
                ReqJson item = new ReqJson("image", Encoding.Default.GetString(b));
                JsonData jsonData = JsonMapper.ToJson(item);
                File.WriteAllText(Application.dataPath + "/Resources/itemData.json", jsonData.ToString());
                
                ws.Send(Encoding.UTF8.GetBytes(jsonData.ToString()));
                yield return new WaitForSeconds(0.01f);
            }
        }//.ImageSend
    }
}//.class


public class ReqJson
{
    public string Image;
    public string Name;

    public ReqJson(string name , string image) {
        Name = name;
        Image = image;        
    }
}