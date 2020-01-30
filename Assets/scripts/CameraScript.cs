using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Texture2D body;

    void Start()
    {
        /*Application.targetFrameRate = 2;
        Time.fixedDeltaTime = 2f;*/
        /*Time.timeScale = 0.001f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;*/
    }

    void Update()
    {
        
    }

    public void OnPostRender()
    {
        Debug.Log("OnPostRender");
        Graphics.DrawTexture(
            new Rect(0, 0, Camera.main.pixelWidth, Camera.main.pixelHeight), body);
    } 
}
