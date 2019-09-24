using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Use plugin namespace
using HybridWebSocket;

public class WebSocket_Control : MonoBehaviour {

    public string socket_adress;
    private WebSocket ws;

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

}//.class