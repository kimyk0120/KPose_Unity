using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;


public class wc2 : MonoBehaviour
{
    private WebCamTexture web;
    
    private byte[] image_bytes;

    public byte[] GetImgBytes() { return image_bytes; }
    public void SetImageBytes(byte[] bytes) { this.image_bytes = bytes; }

    
    void Start()
    {
        web = new WebCamTexture(1280,720,60);
        GetComponent<MeshRenderer>().material.mainTexture = web;
        web.Play();
    }

    // Update is called once per frame
    void Update()
    {
        SaveImage();        
    }
    
    
    public void SaveImage(){
        Texture2D texture = new Texture2D(web.width,web.height, TextureFormat.ARGB32, false);
        texture.SetPixels(web.GetPixels());
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();

        //Debug.Log(Encoding.Default.GetString(bytes));
        
        this.SetImageBytes(bytes);        
    }
    
}//.class

    
