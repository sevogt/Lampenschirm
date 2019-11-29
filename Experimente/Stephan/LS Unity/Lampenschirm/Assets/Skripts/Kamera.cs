using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamera : MonoBehaviour
{
    // Start is called before the first frame update


    private float dist_to_centre = 2.15f;
    private Vector3 target = new Vector3(0,0,0);
    Camera[] myCams = new Camera[4];
    void Start()
    {
        myCams[0] = GameObject.Find("GUIKamera").GetComponent<Camera>();

        //Find  All other Cameras
        myCams[1] = GameObject.Find("Camera1").GetComponent<Camera>();
        myCams[2] = GameObject.Find("Camera2").GetComponent<Camera>();
        myCams[3] = GameObject.Find("Camera3").GetComponent<Camera>();

        myCams[0].targetDisplay = 0;
        myCams[1].targetDisplay = 1;
        myCams[2].targetDisplay = 2;
        myCams[3].targetDisplay = 3;

        foreach(Display dis in Display.displays)
        {
            dis.Activate();
        }

        
        setup_camera(myCams[1],0,0);
        setup_camera(myCams[2],120,0);
        setup_camera(myCams[3],240,0);

         
    }

    // up für schieflage der beamer, also weil das bild nach oben gerichtet ist
    private void setup_camera(Camera camera, float rot_around, float up)
    {
        camera.transform.position=new Vector3(dist_to_centre,-0.2f,0);
        camera.transform.RotateAround(Vector3.zero, Vector3.up, rot_around);
        
        camera.clearFlags= CameraClearFlags.Color;
        camera.backgroundColor = Color.black;
        camera.fieldOfView=17;
        camera.nearClipPlane=0.3f;
        camera.farClipPlane = 200;
        camera.transform.LookAt(target);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown ("s"))
        { 
            myCams[1].transform.position = new Vector3(myCams[1].transform.position.x-0.05f,-0.2f,0);
         
        }
        if(Input.GetKeyDown ("w"))
        {
            myCams[1].transform.position = new Vector3(myCams[1].transform.position.x+0.05f,-0.2f,0);
        }
        if(Input.GetKeyDown ("y"))
        {
            myCams[1].fieldOfView+=1;
        }
        if(Input.GetKeyDown ("x"))
        {
            myCams[1].fieldOfView-=1;
        }
        myCams[1].transform.LookAt(target);
        myCams[2].transform.LookAt(target);
        myCams[3].transform.LookAt(target);
       
    }
}
