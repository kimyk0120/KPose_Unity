using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiScript : MonoBehaviour
{
    
    
    public void ColorChg(string object_nm)
    {
        GameObject a = GameObject.Find(object_nm);
        Debug.Log(a.gameObject.name);
        
        a.GetComponent<Image>().color = Color.grey;
        
    }
    
    public void ColorChg_white()
    {
        GameObject[] gos =GameObject.FindGameObjectsWithTag("Button");
        for (int i = 0; i < gos.Length; i++)
        {
            GameObject a = GameObject.Find(gos[i].gameObject.name);
            a.GetComponent<Image>().color = Color.white; 
        }
    }

    public void QuitGo()
    {
        Debug.Log("Quit");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}
