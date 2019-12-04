using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D2_Welt : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject aquarium;
    private Camera camera1;
    private Camera camera2;
    private Camera camera3;

    void Awake() 
    {
        Time.timeScale = 0;
        aquarium = GameObject.Find("/D2_Welt/Aquarium");
        camera1 = GameObject.Find("/D2_Welt/Camera1").GetComponent<Camera>();
        camera2 = GameObject.Find("/D2_Welt/Camera2").GetComponent<Camera>();
        camera3 = GameObject.Find("/D2_Welt/Camera3").GetComponent<Camera>();

        // init
        aquarium.transform.position = Const.target;

        foreach(Display dis in Display.displays)
        {
            dis.Activate();
        }

        setup_camera(camera1,0,0,17);
        setup_camera(camera2,120,0,17);
        setup_camera(camera3,240,0,17);
    }

    private void setup_camera(Camera camera, float rot_around, float up, float fov)
    {
        camera.transform.position=new Vector3(Const.initial_dist_to_centre,Const.initial_height_from_centre,0);
        camera.transform.RotateAround(Vector3.zero, Vector3.up, rot_around);
        
        camera.clearFlags= CameraClearFlags.Color;
        camera.backgroundColor = Color.black;
        camera.fieldOfView=fov;
        camera.nearClipPlane=0.3f;
        camera.farClipPlane = 200;
        camera.transform.LookAt(Const.target);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
