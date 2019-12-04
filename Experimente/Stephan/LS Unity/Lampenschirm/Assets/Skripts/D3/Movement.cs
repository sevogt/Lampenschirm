using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Camera camera1;
    private Camera camera2;
    private Camera camera3;

    void Start()
    {
        camera1 = GameObject.Find("/D3_Welt/Camera1").GetComponent<Camera>();
        camera2 = GameObject.Find("/D3_Welt/Camera2").GetComponent<Camera>();
        camera3 = GameObject.Find("/D3_Welt/Camera3").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown ("s"))
        { 
            camera1.transform.position = new Vector3(camera1.transform.position.x-0.05f,-0.2f,0);
         
        }
        if(Input.GetKeyDown ("w"))
        {
            camera1.transform.position = new Vector3(camera1.transform.position.x+0.05f,-0.2f,0);
        }
        if(Input.GetKeyDown ("y"))
        {
            camera1.fieldOfView+=1;
        }
        if(Input.GetKeyDown ("x"))
        {
            camera1.fieldOfView-=1;
        }
        camera1.transform.LookAt(Const.target);
       
    }
}
