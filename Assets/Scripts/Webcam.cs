using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class Webcam : MonoBehaviour
{    
    static WebCamTexture webCamTexture;
    public RawImage rawImage;    
    int _CaptureCounter = 0;

    private byte[] image_bytes;

    public byte[] GetImgBytes() { return image_bytes; }
    public void SetImageBytes(byte[] bytes) { this.image_bytes = bytes; }

    // Start is called before the first frame update
    void Start()
    {
        if(webCamTexture == null)
            webCamTexture = new WebCamTexture();        
        rawImage.texture = webCamTexture;
        rawImage.material.mainTexture = webCamTexture;
        if(!webCamTexture.isPlaying) webCamTexture.Play();
        
    }

    void Update(){        
        SaveImage();        
    }

    public void SaveImage(){
        Texture2D texture = new Texture2D(rawImage.texture.width,
                        rawImage.texture.height, TextureFormat.ARGB32, false);
        texture.SetPixels(webCamTexture.GetPixels());
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();

        //Debug.Log(Encoding.Default.GetString(bytes));
        
        this.SetImageBytes(bytes);        

        //File.WriteAllBytes(Application.dataPath + "/images/testimg.png", bytes);
        
    }

       

}//.class
