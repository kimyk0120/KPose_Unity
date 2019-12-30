using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;


public class wc2 : MonoBehaviour
{
    private WebCamTexture web;
    public RawImage rawimage;
    private byte[] image_bytes;
    float Timer;
    private float FrameRate = 20;
    public byte[] GetImgBytes() { return image_bytes; }
    public void SetImageBytes(byte[] bytes) { this.image_bytes = bytes; }

    
    void Start()
    {
//        web = new WebCamTexture(1280,720,60);
//        GetComponent<MeshRenderer>().material.mainTexture = web;
//        web.Play();
//        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        web = new WebCamTexture(640,480,20);
        rawimage.texture = web;
//        rawimage.material.mainTexture = webcamTexture;
        web.Play();
    }

    // Update is called once per frame
    void Update()
    {
//        SaveImage();        
    }

    void LateUpdate()
    {
        Timer += Time.deltaTime;
        if (Timer > (1 / FrameRate))
        {
            Timer = 0;
            SaveImage();
        }
    }
    
    public void SaveImage(){
        Texture2D texture = new Texture2D(web.width,web.height, TextureFormat.ARGB32, false);
        texture.SetPixels(web.GetPixels());
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();
        
//        File.WriteAllBytes(Application.dataPath+"/images/test_file_"+Time.time+".png", bytes);
        //Debug.Log(Encoding.Default.GetString(bytes));
        
        this.SetImageBytes(bytes);
        
        
    }

    
    
}//.class

    
