using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Webcam : MonoBehaviour
{
    static WebCamTexture backCam;
    public RawImage rawImage;
    
    // Start is called before the first frame update
    void Start()
    {
        if(backCam == null)
            backCam = new WebCamTexture();        
        rawImage.texture = backCam;
        rawImage.material.mainTexture = backCam;
        if(!backCam.isPlaying) backCam.Play();    
    }
}
