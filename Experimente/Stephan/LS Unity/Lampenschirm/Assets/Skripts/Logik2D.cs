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

    private Camera sprite_camera;

    private ArrayList fische = new ArrayList();

    private GameObject prefab_kreis;
    private GameObject prefab_wasserbg;
    private GameObject prefab_vogel;

    private float timeLeft =Const.startTimer;

    private byte cameraState=1;
    private bool camAnimation=false;

    private float camTimeLeft=0;

    private float cam_to_pos=20;



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

        prefab_kreis = Resources.Load<GameObject>("Prefab/kreis");
        
        prefab_wasserbg = Resources.Load<GameObject>("SpriteData/wasserbg");

        prefab_vogel = Resources.Load<GameObject>("SpriteData/ezgif.com-gif-maker_0");

        // for (int i = 0; i < camera_width; i++)
        // {
        //     for (int k = 0; k < camera_height; k++)
        //     {
        //         GameObject sprite = (GameObject)Instantiate(prefab_wasserbg,new Vector3(border_left+(i),border_down+(k),0) , Quaternion.identity);
            
        //     }
        // }


        
    }


    private GameObject anifi;

    private float timer1=2;
    private float distance_light=4;

    public void setup_aqua()
    {
        // animation metal
        // dann glass
        // dann fülle becken und berge

        anifi = (GameObject)Instantiate(prefab_vogel,new Vector3(10000,0,0) , Quaternion.identity);
        
        BasisFischLogik bfl = anifi.GetComponent<BasisFischLogik>();
        
        bfl.init_new(1);
        bfl.enabled=false;

    }

    private bool camera_mover_enabled=false;
    private bool init_animation = true;

    private int go=1;

    // Update is called once per frame
    void Update()
    {
        if(init_animation)
        {
            
            
            if(timer1<=0)
            {
                GameObject light = GameObject.Find("/Light");
                Vector3 move_vec = new Vector3(0,0,-0.772f) - light.transform.position;
                move_vec.Normalize();
                move_vec.Scale(new Vector3(Time.deltaTime/3f,Time.deltaTime/3f,Time.deltaTime/3f));

                Vector3 be_vecss= new Vector3(-0.001f,0,0);
                
                if(light.transform.position.z<-0.772f)
                {
                    if(go==1)
                    {
                        Vector3 be_vec= new Vector3(-0.1f,0,0);
                        anifi.transform.position+=be_vec;
                        light.transform.position+=be_vecss;
                        if(anifi.transform.position.x<Const.world_limit_left)
                        {
                            go=2;
                        }
                    }
                    else if(go==2)
                    {
                        Vector3 be_vec= new Vector3(0.1f,0,0);
                        anifi.transform.position+=be_vec;
                        light.transform.position-=be_vecss;
                        if(anifi.transform.position.x>Const.world_limit_right)
                        {
                            go=1;
                        }
                    }



                }
                else
                {
                    light.transform.position+=move_vec;
                }
            }
            else
            {
                timer1 -= Time.deltaTime;
            }
        }
        else
        {
            if(camera_mover_enabled)
            {
                if ( timeLeft < 0 )
                {
                    timeLeft=Const.startTimer;
                    camAnimation=true;
                    camTimeLeft=Const.camMovementTime;

                    if(cameraState==1)
                    {
                        cameraState=2;
                        cam_to_pos=Const.camOrtoPos2;
                    }
                    else if(cameraState ==2)
                    {
                        cameraState=1;
                        cam_to_pos=Const.camOrtoPos1;
                    }

                }
                timeLeft -= Time.deltaTime;
                if(camAnimation)
                {
                    Vector3 newPos = sprite_camera.transform.position;
                    newPos.y+= ((cam_to_pos-sprite_camera.transform.position.y)/(camTimeLeft/Time.deltaTime));
                    sprite_camera.transform.position=newPos;
                    camTimeLeft -= Time.deltaTime;
                    if(camTimeLeft<=0)
                    {
                        newPos.y=cam_to_pos;
                        sprite_camera.transform.position=newPos;
                        camAnimation=false;
                    }
                    
                }
            }
        }


        
        
        

        

    }

    

    public void mehr(int x)
    {

        
        GameObject sprite = (GameObject)Instantiate(prefab_vogel,rand_pos() , Quaternion.identity);
        BasisFischLogik bfl = sprite.GetComponent<BasisFischLogik>();
        bfl.init_new(1);

        fische.Add(sprite);
    }



    private Vector3 rand_pos()
    {
        return new Vector3(
            Random.Range(border_left, border_right),
            Random.Range(border_up, border_down),
            0);
    }
}
