using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logik2D : MonoBehaviour
{
    private GameObject aquarium;

    private float camera_height;
    private float camera_width;

    private float border_up;
    private float border_down;
    private float border_left;
    private float border_right;

    private Vector3 cam_center;

    private Camera sprite_camera;

    private ArrayList kreise = new ArrayList();

    private GameObject prefab_kreis;



    void Start()
    {
        aquarium = GameObject.Find("/D2_Welt/Aquarium");

        sprite_camera = GameObject.Find("/D2_Welt/D2SpriteCamera").GetComponent<Camera>();

        camera_height = 2*sprite_camera.orthographicSize;
        camera_width  = camera_height*sprite_camera.aspect;
        border_up = sprite_camera.transform.position.y+(camera_height/2f);
        border_down = sprite_camera.transform.position.y-(camera_height/2f);
        border_left = sprite_camera.transform.position.x-(camera_width/2f);
        border_right = sprite_camera.transform.position.x+(camera_width/2f);

        cam_center = new Vector3(sprite_camera.transform.position.x,sprite_camera.transform.position.y,0);

        

        prefab_kreis = Resources.Load<GameObject>("Prefab/kreis");
        
    }

    public void setup_aqua()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < kreise.Count; i++)
        {
            GameObject sprite = (GameObject)kreise[i];
            Vector2 speed = sprite.GetComponent<Rigidbody2D>().velocity;
            
            if(speed.magnitude<4)
            {
                speed+= new Vector2(Random.Range(-3, 3),Random.Range(-1, 3));
                sprite.GetComponent<Rigidbody2D>().velocity=speed;
            }
        }


    }

    public void mehr(int x)
    {
        for (int i = 0; i < x; i++)
        {
            GameObject sprite = (GameObject)Instantiate(prefab_kreis,rand_pos() , Quaternion.identity);
            kreise.Add(sprite);
        }
    }



    private Vector3 rand_pos()
    {
        return new Vector3(
            Random.Range(border_left, border_right),
            Random.Range(border_up, border_down),
            cam_center.z);
    }
}
