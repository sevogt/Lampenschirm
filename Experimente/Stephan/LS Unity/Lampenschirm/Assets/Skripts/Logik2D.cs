using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Logik2D : MonoBehaviour, IDetektorListener
{
    private GameObject aquarium;

    private class text_rotations
    {
        public GameObject gameObject;
        public float theta_scale;

        public int state=0;


    };

    private float intro_welt_z_part = 0;

    private float camera_height;
    private float camera_width;

    private float border_up;
    private float border_down;
    private float border_left;
    private float border_right;

    private Camera sprite_camera;
    private GameObject licht_maske;

    private ArrayList fische = new ArrayList();
    private ArrayList fische_bg = new ArrayList();

    private GameObject prefab_vogel;


    private float timeLeft =Const.startTimer;

    private byte cameraState=1;
    private bool camAnimation=false;

    private float camTimeLeft=0;

    private float cam_to_pos=20;

    private State state = State.init_wait;  //init_wait

    private float init_wait_time = 4f;
    
    private enum State
    {
        init_animation = 1,
        intro_mitte_animation = 2,
        opening_animation = 3,

        intro_1=5,

        init_wait=4,
        normal_play = 8
    };

    private enum AnimationState
    {
        s01 = 1,
        s02 = 2,
        s03 = 3,
        s04 = 4,
        s05 = 5,
        s06 = 6,
        s07 = 7,
        s08 = 8,
        s09 = 9,
        s10 = 10,
        s11 = 11,
        s12 = 12
    };

    private (float, float) last_detection_coords = (-1f, -1f);

    public float item1=-1;
    public float item2=-1;
    public void recv_last_detection((float, float) last) {
        last_detection_coords = last;
        //Debug.Log("Last Detect: x: "+last.Item1.ToString()+" y: "+last.Item2.ToString());
    }



    void Start()
    {
        aquarium = GameObject.Find("/D2_Welt/Aquarium");

        sprite_camera = GameObject.Find("/D2_Welt/D2SpriteCamera").GetComponent<Camera>();
        Camera introCamera = GameObject.Find("/IntroCamera").GetComponent<Camera>();

    

        camera_height = 2*introCamera.orthographicSize;
        camera_width  = camera_height*introCamera.aspect;
        border_up = introCamera.transform.position.y+(camera_height/2f);
        border_down = introCamera.transform.position.y-(camera_height/2f);
        border_left = introCamera.transform.position.x-(camera_width/2f);
        border_right = introCamera.transform.position.x+(camera_width/2f);

        
        GameObject cam_sprites = GameObject.Find("/D2_Welt/D2SpriteCamera");
        cam_sprites.transform.position=Const.calibration_pos;

        licht_maske = GameObject.Find("/licht_maske");

        init_all_fisch();

        GameObject.Find("/Detektor").GetComponent<Detektor>().register_listener(this);

        {
            AnimationState intro_animation_state = AnimationState.s01;

            GameObject light = GameObject.Find("/intro1/intro_text_light");
            GameObject text  = GameObject.Find("/intro1/intro_text_1");

            

            float pre_start_timer=3;

            float time=9;
            float goal_x=51;

            float light_move_speed= goal_x/ time;

            Vector3 light_dir_vec = new Vector3(1,0,0);

            Func<float, bool> closure = delegate(float deltaTime)
            {         
                if(intro_animation_state == AnimationState.s01)
                {
                    if(pre_start_timer>0)
                    {
                        pre_start_timer-=deltaTime;
                        intro_animation_state = AnimationState.s02;
                    }
                }
                else if(intro_animation_state == AnimationState.s02)
                {
                    float speed_scale_value = light_move_speed*deltaTime;
                    Vector3 tempv = new Vector3(light_dir_vec.x,light_dir_vec.y,light_dir_vec.z);
                    tempv.Scale(new Vector3(speed_scale_value,speed_scale_value,speed_scale_value));

                    light.transform.localPosition +=tempv;
                    if(light.transform.localPosition.x>=goal_x)
                    {
                        Destroy(light);
                        Destroy(text);
                    
                        return true;
                    }
                }
                return false;         
            };
            intro_1= new ComplexAnimation(closure);

           
        }

            
        {

            float pre_start_timer = 2;
            float intro_animation_timer = 8;
            float infinity_move_half = 5;
            float ball_timer = 2;
            
            var anifi = GameObject.Find("/intro_fisch0");
            anifi.GetComponent<Fisch_simple_rotation>().set_normal_orientation_left();
           
        
            AnimationState intro_animation_state = AnimationState.s01;


            float fisch_travel_dist= 13;
            float fisch_travel_time_half= 5f;
            float fisch_travel_speed= fisch_travel_dist / fisch_travel_time_half;
            Vector3 fisch_travel_dir = new Vector3(1,0,0);
            int fisch_travel_flip_state =0;

            GameObject light = new GameObject("Light");
            light.transform.position = new Vector3(10000,100,1);
            Light light_light = light.AddComponent<Light>();
            light_light.type=LightType.Spot;
            light_light.color = new Color32(92,78,161,255); 
            light_light.intensity=7;
            light_light.spotAngle=66;
            light_light.range=30;

            float light_goal_z=-7f;
            Vector3 light_goal = new Vector3(10000f,100,light_goal_z);
            Vector3 light_dir_vec = (light_goal-light.transform.position);
            
            float light_move_speed1 = light_dir_vec.magnitude / intro_animation_timer;
            float ellipse_weite = 8;
            float ellipse_höhe = 5;

            float ball_Start_pos_z=1;
            float theta1 = 180*Mathf.Deg2Rad; 
            ArrayList balls = new ArrayList();
            float final_ball_pos_z = -1;
            float ball_move_speed= Mathf.Abs(final_ball_pos_z-ball_Start_pos_z) / ball_timer;
            bool flip=false;
            float theta2 = 0*Mathf.Deg2Rad; 

            float theta0=0;

            light_dir_vec.Normalize();

            


            Func<float, bool> closure = delegate(float deltaTime)
            {
                
                if(intro_animation_state == AnimationState.s01)
                {
                    // ein vogel: und licht erscheint. dann vogel nach rechts und links und dann weg.
                    
                    if(pre_start_timer>0)
                    {
                        anifi.transform.position=new Vector3(10000,100,-1);
                        pre_start_timer-=deltaTime;
                        intro_animation_state = AnimationState.s02;
                    }
                }
                else if(intro_animation_state == AnimationState.s02)
                {
                    
                    float speed_scale_value = light_move_speed1*deltaTime;
                    
                    Vector3 tempv = new Vector3(light_dir_vec.x,light_dir_vec.y,light_dir_vec.z);
                    tempv.Scale(new Vector3(speed_scale_value,speed_scale_value,speed_scale_value));

                    

                    if(light.transform.position.z>light_goal_z)
                    {
                        light.transform.position+=tempv;
                    }
                    else if(light.transform.position.z<=light_goal_z)
                    {
                        light.transform.position = light_goal;
                        intro_animation_state = AnimationState.s03;
                    }
                }
                else if(intro_animation_state == AnimationState.s03)
                {
                    
                    float sp_fisch_travel = fisch_travel_speed*deltaTime;

                    if(fisch_travel_flip_state==0 || fisch_travel_flip_state ==2)
                    {
                        if(fisch_travel_flip_state == 0 && anifi.transform.position.x >= 10000 + fisch_travel_dist)
                        {
                            fisch_travel_flip_state=1;
                        }
                        if(fisch_travel_flip_state == 2 && anifi.transform.position.x >= 10000 )
                        {
                            fisch_travel_flip_state=3;

                            
                        }
                        
                    }
                    else
                    {
                        sp_fisch_travel*=-1;
                        if(anifi.transform.position.x <= 10000 - fisch_travel_dist)
                        {
                            fisch_travel_flip_state=2;
                        }
                    }
                    Vector3 tempv = new Vector3(fisch_travel_dir.x,fisch_travel_dir.y,fisch_travel_dir.z);
                    tempv.Scale(new Vector3(sp_fisch_travel,sp_fisch_travel,sp_fisch_travel));

                    anifi.GetComponent<Fisch_simple_rotation>().set_velocity(tempv);

                    anifi.transform.position+=tempv;
                    light.transform.position+=tempv;
                    
                    


                    
                    if(fisch_travel_flip_state==3)
                    {
                        anifi.transform.position=new Vector3(10000,100,-1.5f);
                        light.transform.position=new Vector3(10000,100,light_goal_z);
                        intro_animation_state = AnimationState.s04;
                    }

                    
                }
                else if(intro_animation_state == AnimationState.s04)
                {
                    // bird moves in an infinity path
                    bool flag_final=false;
                    bool do_flip=false;
                    if(!flip)
                    {
                        if(theta1 > -180 * Mathf.Deg2Rad)
                        {
                            theta1-= ((360f *Mathf.Deg2Rad)/infinity_move_half ) * deltaTime;
                        }
                        else 
                        {
                            // intro_animation_state = AnimationState.s04;
                            do_flip=true;
                            theta1 = -180 * Mathf.Deg2Rad;
                        }
                        theta0=theta1;
                    }
                    else
                    {
                        if(theta2 <359f * Mathf.Deg2Rad)
                        {
                            theta2+= ((360f *Mathf.Deg2Rad)/infinity_move_half ) * deltaTime;
                        }
                        else 
                        {
                            // intro_animation_state = AnimationState.s04;
                            theta2 = 360f * Mathf.Deg2Rad;
                            flag_final=true;
                        }
                        theta0=theta2;
                    }

                    
                    float x = ellipse_weite * Mathf.Cos(theta0);
                    float y = ellipse_höhe * Mathf.Sin(theta0);

                    Vector3 point_on_ellipse;

                    if(!flip)
                    {
                        point_on_ellipse = new Vector3(10000+ellipse_weite+x,100+y,-1.5f);
                    }
                    else
                    {
                        point_on_ellipse = new Vector3(10000-ellipse_weite+x,100+y,-1.5f);
                    }

                    
                    anifi.GetComponent<Fisch_simple_rotation>().set_velocity(point_on_ellipse-anifi.transform.position);
                    anifi.transform.position = point_on_ellipse;

                    light.transform.position=new Vector3(point_on_ellipse.x,point_on_ellipse.y,light_goal_z);
                    

                    if(do_flip)
                    {
                        flip=true;
                    }

                    point_on_ellipse = new Vector3(point_on_ellipse.x,point_on_ellipse.y,ball_Start_pos_z);

                    // set bird and balls
                    
                    

                    if(!flag_final)
                    {
                        GameObject ball = Instantiate(GameObject.Find("/Sphere"));
                        ball.name="woball";
                        ball.transform.position = point_on_ellipse;
                        balls.Add(ball);
                    }


                    bool flag_all_done=true;

                    for (int i = 0; i < balls.Count; i++)
                    {
                        GameObject my_ball = (GameObject)balls[i];
                        if(my_ball.transform.position.z>final_ball_pos_z)
                        {
                            flag_all_done=false;
                            float speed_scale_value = ball_move_speed*deltaTime;
                            
                            Vector3 tempv = new Vector3(light_dir_vec.x,light_dir_vec.y,light_dir_vec.z);
                            tempv.Scale(new Vector3(speed_scale_value,speed_scale_value,speed_scale_value));

                            my_ball.transform.position+=tempv;
                        }
                    }
                    
                    if(flag_all_done && flag_final)
                    {
                        Destroy (anifi);
                        intro_animation_state = AnimationState.s05;
                    }

                }
                else if(intro_animation_state == AnimationState.s05)
                {
                    bool flag_all_done=true;

                    for (int i = 0; i < balls.Count; i++)
                    {
                        GameObject my_ball = (GameObject)balls[i];
                        if(my_ball.transform.position.z<ball_Start_pos_z)
                        {
                            flag_all_done=false;
                            float speed_scale_value = -ball_move_speed*deltaTime;
                            light_dir_vec.Normalize();
                            Vector3  tempv=new Vector3(light_dir_vec.x,light_dir_vec.y,light_dir_vec.z);
                            tempv.Scale(new Vector3(speed_scale_value,speed_scale_value,speed_scale_value));
                            my_ball.transform.position+=tempv;
                        }
                    }
                    if(flag_all_done)
                    {
                        for (int i = balls.Count-1; i >=0; i--)
                        {
                            GameObject my_ball = (GameObject)balls[i];
                            Destroy(my_ball);
                        }
                        intro_animation_state = AnimationState.s06;
                        
                    }
                }
                else if(intro_animation_state == AnimationState.s06)
                {
                    float speed_scale_value = light_move_speed1*deltaTime;
                    
                    Vector3 tempv = new Vector3(light_dir_vec.x,light_dir_vec.y,light_dir_vec.z);
                    tempv.Scale(new Vector3(speed_scale_value,speed_scale_value,speed_scale_value));


                    if(light.transform.position.z < 0)
                    {
                        light.transform.position-=tempv;
                    }
                    else if(light.transform.position.z >=0)
                    {
                        return true; // ende animation
                    }
                    
                }
                
                return false;
            };

            intro_start = new ComplexAnimation(closure);
        }

        {
            AnimationState mitte_animation_state = AnimationState.s01;

            Camera orto = GameObject.Find("/IntroCamera").GetComponent<Camera>();

            float screenAspect = (float) Screen.width / (float) Screen.height;
            float camHalfHeight = orto.orthographicSize;
            float camHalfWidth = screenAspect * camHalfHeight;

            GameObject intro_fisch_l=null;
            GameObject intro_fisch_r=null;
            GameObject intro_licht_l=null;
            GameObject intro_licht_r=null;
            GameObject intro_licht_m=null;

            
            ArrayList text_left_active  = new ArrayList();
            ArrayList text_right_active = new ArrayList();

            GameObject text_basis=GameObject.Find("/intro3/text");

  

            float start_scale=0.2f;
            float first_big_scale=2f;
            float time_scaling_to_big=3;
            float time_scale_big_to_normal=2;
            float scale_speed_stb=(first_big_scale-start_scale)/time_scaling_to_big;
            float scale_speed_btn=(time_scaling_to_big-1)/time_scale_big_to_normal;

            float second_big_scale=2.2f;
            float time_to_scale_big_2=2;
            float time_to_scale_normal=2f;
            float scale_speed_ntvb=(second_big_scale-1)/time_to_scale_big_2;
            float scale_speed_vbtn=(second_big_scale-1)/time_to_scale_normal;

            float wave_start_befor_fraction=3f/4f;

            text_rotations prepareGO_text(String text, Color32 color)
            {
                text_rotations stru =new text_rotations();
                
                GameObject text_letter = Instantiate(text_basis);
                text_letter.transform.position = new Vector3(10000,100,1);
                text_letter.tag="destroy_for_opening";
                var tm1 = text_letter.GetComponent<TextMeshPro>();
                tm1.color=color;
                tm1.text=text;

                stru.gameObject=text_letter;
                stru.theta_scale=start_scale;
                stru.state=-1;

                return stru;
                
            }
            
            text_left_active.Add( (prepareGO_text( "W",new Color32(221,146,243,255))    ) );
            text_left_active.Add( (prepareGO_text( "A",new Color32(221,146,243,255))    ) );
            text_left_active.Add( (prepareGO_text( "T",new Color32(221,146,243,255))    ) );
            text_left_active.Add( (prepareGO_text( "E",new Color32(221,146,243,255))    ) );
            text_left_active.Add( (prepareGO_text( "R",new Color32(221,146,243,255))    ) );

            text_right_active.Add( (prepareGO_text( "W",new Color32(221,146,243,255)) ) );
            text_right_active.Add( (prepareGO_text( "O",new Color32(221,146,243,255)) ) );
            text_right_active.Add( (prepareGO_text( "R",new Color32(221,146,243,255)) ) );
            text_right_active.Add( (prepareGO_text( "L",new Color32(221,146,243,255)) ) );
            text_right_active.Add( (prepareGO_text( "D",new Color32(221,146,243,255)) ) );

            float text_margin = 0.2f;

            float text_letter_size=2;

            // set text letters left
            float width_sum=0;
            for (int i = text_left_active.Count-1; i >= 0; i--)
            {
                GameObject go = ((text_rotations)text_left_active[i]).gameObject;
                // Bounds textBounds = go.GetComponent<Renderer>().bounds;
                float width = text_letter_size; //textBounds.max.x-textBounds.min.x;
                
                go.transform.position= new Vector3(10000-width_sum-text_margin-(width/2f),100,1);
                width_sum+=width+text_margin;

            }

            width_sum=0;
            for (int i = 0; i < text_right_active.Count; i++)
            {
                GameObject go = ((text_rotations)text_right_active[i]).gameObject;
                // Bounds textBounds = go.GetComponent<Renderer>().bounds;
                float width = text_letter_size; //textBounds.max.x-textBounds.min.x;
                
                go.transform.position= new Vector3(10000+width_sum+text_margin+(width/2f),100,1);
                width_sum+=width+text_margin;

            }


            intro_fisch_l = GameObject.Find("/intro_fisch_l"); 
            intro_fisch_l.tag="destroy_for_opening";

            intro_fisch_r = GameObject.Find("/intro_fisch_r");  
            intro_fisch_r.tag="destroy_for_opening";

            intro_licht_l = GameObject.Find("/intro_licht_l");  
            intro_licht_l.tag="destroy_for_opening";

            intro_licht_r = GameObject.Find("/intro_licht_r");  
            intro_licht_r.tag="destroy_for_opening";

            intro_licht_m = GameObject.Find("/intro_licht_m");  
            intro_licht_m.tag="destroy_for_opening";



            float timer_fisch_appear=4;
            float timer_licht_appear=6;

            Vector3 appear_dir = new Vector3(0,0,-1);
            float dist_to_start_pos_fisch=1.6f;
            float dist_to_start_pos_licht=7f;

            float speed_fishc_appear=dist_to_start_pos_fisch/timer_fisch_appear;
            float speed_licht_appear=timer_licht_appear/dist_to_start_pos_licht;


            int anzahl_kreisungen=5;
            float time_for_1_kreis=3.2f;
            float winkel_speed = (360f/time_for_1_kreis) * Mathf.Deg2Rad;
            int fisch_offset_spawn=5;
            float theta=0;

            float time_fisch_move_side = 9;
            float speed_fisch_move_side = (camHalfWidth*0.96f)/time_fisch_move_side;
            float fisch_side_goal_x=speed_fisch_move_side*time_fisch_move_side;

            
            Vector3 move_dir = new Vector3(1,0,0);

            float subtext_time = 2;
            float subtext_dist= 2.2f;
            float final_pos_z=-1.2f;

            float subtext_light_time = 6;
            float subtext_light_dist=6;

            float subtext_y_offset=5;

            float subtext_t_speed=subtext_dist/subtext_time;
            float subtext_l_speed=subtext_light_dist/subtext_light_time;

            GameObject subtext =  Instantiate(text_basis);
            subtext.transform.position = new Vector3(10000,88,1);
            subtext.tag="destroy_for_opening";
            var tm2 = subtext.GetComponent<TextMeshPro>();
            tm2.color=new Color32(125,255,33,255);
            tm2.text="Aquarium Master";

            Color32 start_color = new Color32(187,86,32,255);
            Color32 ende_color = new Color32(56,86,32,255);

            float time_for_change=0.03f;
            float time_for_change_counter=0.0f;
            bool flip_color_change=false;



            void set_dynamic_fisch_light_color(ref Color32 set_color,  float deltaTime)
            {
                time_for_change_counter+=deltaTime;
                if(time_for_change_counter >= time_for_change)
                {
                    time_for_change_counter=0;
                    if(!flip_color_change)
                    {
                        if(set_color.r<start_color.r)
                        {
                            set_color.r+=1;
                        }
                        else
                        {
                            
                            flip_color_change=true;
                        }
                        
                    }
                    else
                    {
                        if(set_color.r>ende_color.r)
                        {
                            set_color.r-=1;
                        }
                        else
                        {
                            flip_color_change=false;
                        }
                    }
                }
                
            }

            Func<float, bool> closure = delegate(float deltaTime)
            {
                // dann 2 farben lichter, mit farbübergang und 2 fische, schwimmen nach links rehcts und text erscheint.
                // text klappt sich auf. 2 teile von mitte asugehend. bis es flahc sit. 
                // dann drittes licht in mitte für sub text und voll asubreiten über alles.
                // fishce warten an den enden mit lich

                if(mitte_animation_state == AnimationState.s01)
                {
                    intro_fisch_l.transform.position=new Vector3(10000-fisch_offset_spawn,100,-1);
                    intro_fisch_r.transform.position=new Vector3(10000+fisch_offset_spawn,100,-1);

                    intro_fisch_l.GetComponent<Fisch_simple_rotation>().set_velocity(new Vector3(1,0,0));
                    intro_fisch_r.GetComponent<Fisch_simple_rotation>().set_velocity(new Vector3(-1,0,0));

                    
                    intro_licht_l.transform.position=new Vector3(10000-fisch_offset_spawn,100,1);
                    intro_licht_r.transform.position=new Vector3(10000+fisch_offset_spawn,100,1);

                    mitte_animation_state=AnimationState.s02;
                }
                else if(mitte_animation_state == AnimationState.s02)
                {
                    // fishce und lciht erscheinen
                    float speed_scale_value_fisch = speed_fishc_appear*deltaTime;
                    float speed_scale_value_licht = speed_licht_appear*deltaTime;
                    Vector3 tempv=new Vector3(appear_dir.x,appear_dir.y,appear_dir.z);
                    tempv.Scale(new Vector3(0,0,speed_scale_value_fisch));

                    int flag=0;

                    if(intro_fisch_l.transform.position.z> intro_welt_z_part-dist_to_start_pos_fisch)
                    {
                        intro_fisch_l.transform.position+=tempv;
                        intro_fisch_r.transform.position+=tempv;
                    }
                    else
                    {
                        flag+=1;
                    }

                    if(intro_licht_l.transform.position.z> intro_welt_z_part-dist_to_start_pos_licht)
                    {
                        tempv.Normalize();
                        tempv.Scale(new Vector3(0,0,speed_scale_value_licht));

                        intro_licht_l.transform.position+=tempv;
                        intro_licht_r.transform.position+=tempv;
                    }
                    else
                    {
                        flag+=1;
                    }

                    if(flag==2)
                    {
                        mitte_animation_state=AnimationState.s03;
                    }

                    
                }
                else if(mitte_animation_state == AnimationState.s03)
                {
                    // fishce und lciht machen mehrere kreisebewegungen 8 stück schnell in 6 sekunden ende auf mitte
                    time_for_change_counter+=deltaTime;
                    Color32 set_color =  intro_licht_l.GetComponent<Light>().color;
                    
                    set_dynamic_fisch_light_color(ref set_color,deltaTime); 
                    
                    intro_licht_l.GetComponent<Light>().color=set_color;
                    intro_licht_r.GetComponent<Light>().color=set_color;

                    theta+=winkel_speed*deltaTime;


                    float x = fisch_offset_spawn * Mathf.Cos(theta);
                    float y = fisch_offset_spawn * Mathf.Sin(theta);

                    Vector3 new_pos_l=new Vector3(10000-x,100-y,intro_welt_z_part-dist_to_start_pos_fisch);
                    Vector3 new_pos_r=new Vector3(10000+x,100+y,intro_welt_z_part-dist_to_start_pos_fisch);

                    intro_fisch_l.GetComponent<Fisch_simple_rotation>().set_velocity(new_pos_l-intro_fisch_l.transform.position);
                    intro_fisch_r.GetComponent<Fisch_simple_rotation>().set_velocity(new_pos_r-intro_fisch_r.transform.position);

                    intro_fisch_l.transform.position=new_pos_l;
                    intro_fisch_r.transform.position=new_pos_r;

                    

                    intro_licht_l.transform.position=new Vector3(new_pos_l.x,new_pos_l.y,intro_licht_l.transform.position.z);
                    intro_licht_r.transform.position=new Vector3(new_pos_r.x,new_pos_r.y,intro_licht_l.transform.position.z);
                        

                    if(theta >=360*Mathf.Deg2Rad)
                    {
                        anzahl_kreisungen--;
                        theta=0;
                    }
                   

                    if(anzahl_kreisungen==0)
                    {

                        Vector3 pos1= GameObject.Find("intro3/Point Light1").transform.position;
                        pos1.y=100;
                        GameObject.Find("intro3/Point Light1").transform.position=pos1;

                        Vector3 pos2= GameObject.Find("intro3/Point Light2").transform.position;
                        pos2.y=100;
                        GameObject.Find("intro3/Point Light2").transform.position=pos2;

                        Vector3 pos3= GameObject.Find("intro3/Point Light3").transform.position;
                        pos3.y=100;
                        GameObject.Find("intro3/Point Light3").transform.position=pos3;

                        Vector3 pos4= GameObject.Find("intro3/Point Light4").transform.position;
                        pos4.y=100;
                        GameObject.Find("intro3/Point Light4").transform.position=pos4;

                        Vector3 pos5= GameObject.Find("intro3/Point Light5").transform.position;
                        pos5.y=100;
                        GameObject.Find("intro3/Point Light5").transform.position=pos5;

                        intro_fisch_l.GetComponent<Fisch_simple_rotation>().set_velocity(new Vector3(1,0,0));
                        intro_fisch_r.GetComponent<Fisch_simple_rotation>().set_velocity(new Vector3(-1,0,0));


                        intro_fisch_l.transform.position=new Vector3(10000-fisch_offset_spawn,100,intro_welt_z_part-dist_to_start_pos_fisch);
                        intro_fisch_r.transform.position=new Vector3(10000+fisch_offset_spawn,100,intro_welt_z_part-dist_to_start_pos_fisch);

                        intro_licht_l.transform.position=new Vector3(10000-fisch_offset_spawn,100,intro_licht_l.transform.position.z);
                        intro_licht_r.transform.position=new Vector3(10000+fisch_offset_spawn,100,intro_licht_r.transform.position.z);
                        mitte_animation_state=AnimationState.s04;
                    }

                }
                else if(mitte_animation_state == AnimationState.s04)
                {

                    // todo text appears  then afertwards inense glow
                    // text ist einzelbucshtaben dei rotieren und dann raus kommen.  4 sekunden rotieren und vlt wasser aniamiton wo sie raus kommen.
                    // oder halooder partikel

                    Color32 set_color =  intro_licht_l.GetComponent<Light>().color;
                    set_dynamic_fisch_light_color(ref set_color,deltaTime); 
                    intro_licht_l.GetComponent<Light>().color=set_color;
                    intro_licht_r.GetComponent<Light>().color=set_color;

                    float speed_scale_value_fisch = speed_fisch_move_side*deltaTime;
                    Vector3 tempv=new Vector3(move_dir.x,move_dir.y,0);
                    tempv.Scale(new Vector3(speed_scale_value_fisch,0,0));

                    // Bounds textBounds = MyTextMesh.renderer.bounds;

                    int adder=0;

                    for (int i = 0; i < text_left_active.Count; i++)
                    {
                        text_rotations content = (text_rotations)text_left_active[i];
                        GameObject go = content.gameObject;

                        if(content.state ==-1 && intro_fisch_r.transform.position.x <= go.transform.position.x)
                        {
                            go.transform.position= new Vector3(go.transform.position.x,go.transform.position.y,-1.2f);
                            content.state=0;
                        }
                        else if(content.state==0)
                        {
                            float scale_change =scale_speed_stb*deltaTime;
                            content.theta_scale+=scale_change;
                            if(content.theta_scale >=first_big_scale)
                            {
                                content.theta_scale=first_big_scale;
                                content.state=1;
                            }
                            go.GetComponent<RectTransform>().localScale=new Vector3(content.theta_scale,content.theta_scale,1);
                                
                        }
                        else if(content.state==1)
                        {
                            float scale_change =scale_speed_btn*deltaTime;
                            content.theta_scale-=scale_change;
                            if(content.theta_scale <=1)
                            {
                                content.theta_scale=1;
                                content.state=2;
                            }
                            go.GetComponent<RectTransform>().localScale=new Vector3(content.theta_scale,content.theta_scale,1);
                                
                        }

                        if(content.state==2)
                        {
                            adder+=1;
                        }
                    }

                    for (int i = 0; i< text_right_active.Count; i++)
                    {
                        text_rotations content = (text_rotations)text_right_active[i];
                        GameObject go = content.gameObject;

                        if(content.state==-1 && intro_fisch_l.transform.position.x >=go.transform.position.x )
                        {
                            go.transform.position= new Vector3(go.transform.position.x,go.transform.position.y,-1.2f);
                            content.state=0;
                        }
                        else if(content.state==0)
                        {
                            float scale_change =scale_speed_stb*deltaTime;
                            content.theta_scale+=scale_change;
                            if(content.theta_scale >=first_big_scale)
                            {
                                content.theta_scale=first_big_scale;
                                
                                content.state=1;
                            }
                            go.GetComponent<RectTransform>().localScale=new Vector3(content.theta_scale,content.theta_scale,1);
                                
                        }
                        else if(content.state==1)
                        {
                            float scale_change =scale_speed_btn*deltaTime;
                            content.theta_scale-=scale_change;
                            if(content.theta_scale <=1)
                            {
                                content.theta_scale=1;
                                
                                content.state=2;
                            }
                            go.GetComponent<RectTransform>().localScale=new Vector3(content.theta_scale,content.theta_scale,1);
                                
                        }

                        if(content.state==2)
                        {
                            adder+=1;
                        }
                    }



                    if(intro_fisch_l.transform.position.x < 10000+fisch_side_goal_x)
                    {
                        intro_fisch_l.transform.position+=tempv;
                        intro_fisch_r.transform.position-=tempv;

                        intro_licht_l.transform.position+=tempv;
                        intro_licht_r.transform.position-=tempv;
                    }
                    else if( adder == (text_right_active.Count+text_left_active.Count) && intro_fisch_l.transform.position.x >= 10000+fisch_side_goal_x)
                    {

                        intro_fisch_l.GetComponent<Fisch_simple_rotation>().set_velocity(new Vector3(-1,0,0));
                        intro_fisch_r.GetComponent<Fisch_simple_rotation>().set_velocity(new Vector3(1,0,0));

                        intro_fisch_l.transform.position=new Vector3(10000+fisch_side_goal_x,100,intro_welt_z_part-dist_to_start_pos_fisch);
                        intro_fisch_r.transform.position=new Vector3(10000-fisch_side_goal_x,100,intro_welt_z_part-dist_to_start_pos_fisch);

                        intro_licht_l.transform.position=new Vector3(10000+fisch_side_goal_x,100,intro_licht_l.transform.position.z);
                        intro_licht_r.transform.position=new Vector3(10000-fisch_side_goal_x,100,intro_licht_r.transform.position.z);
               
                        mitte_animation_state=AnimationState.s05;
                    }

                }
                else if(mitte_animation_state == AnimationState.s05)
                {
                    int adder=0;

                    for (int i = 0; i <text_left_active.Count; i++)
                    {
                        text_rotations content = (text_rotations)text_left_active[i];
                        GameObject go = content.gameObject;

                        if( content.state==2 && i==0)
                        {
                            content.state=3;
                        }
                        else if(content.state==2 )
                        {
                            text_rotations content2 = (text_rotations)text_left_active[i-1];
                            GameObject go2 = content2.gameObject;
                            if(content2.theta_scale>second_big_scale*wave_start_befor_fraction)
                            {
                                content.state=3;
                            }
                        }
                        
                        if(content.state==3)
                        {
                            float scale_change = scale_speed_ntvb*deltaTime;
                            content.theta_scale+=scale_change;
                            if(content.theta_scale >=second_big_scale)
                            {
                                content.theta_scale=second_big_scale;
                                
                                content.state=4;
                            }
                            go.GetComponent<RectTransform>().localScale=new Vector3(content.theta_scale,content.theta_scale,1);
                                
                        }
                        else if(content.state==4)
                        {
                            float scale_change =scale_speed_vbtn*deltaTime;
                            content.theta_scale-=scale_change;
                            if(content.theta_scale <=1)
                            {
                                content.theta_scale=1;
                                content.state=5;
                            }
                            go.GetComponent<RectTransform>().localScale=new Vector3(content.theta_scale,content.theta_scale,1);
                                
                        }

                        if(content.state==5)
                        {
                            // set final rota and final position
                            adder+=1;
                        }
                    }

                    for (int i = 0; i < text_right_active.Count; i++)
                    {
                        text_rotations content = (text_rotations)text_right_active[i];
                        GameObject go = content.gameObject;

                        if( content.state==2 && i==0)
                        {
                            text_rotations content2 = (text_rotations)text_left_active[text_left_active.Count-1];
                            GameObject go2 = content2.gameObject;
                            if(content2.theta_scale>second_big_scale*wave_start_befor_fraction)
                            {
                                content.state=3;
                            }
                        }
                        else if(content.state==2 )
                        {
                            text_rotations content2 = (text_rotations)text_right_active[i-1];
                            GameObject go2 = content2.gameObject;
                            if(content2.theta_scale>second_big_scale*wave_start_befor_fraction)
                            {
                                content.state=3;
                            }
                               
                        }
                        
                        if(content.state==3)
                        {
                            float scale_change = scale_speed_ntvb*deltaTime;
                            content.theta_scale+=scale_change;
                            if(content.theta_scale >=second_big_scale)
                            {
                                content.theta_scale=second_big_scale;
                                content.state=4;
                            }
                            go.GetComponent<RectTransform>().localScale=new Vector3(content.theta_scale,content.theta_scale,1);
                                
                        }
                        else if(content.state==4)
                        {
                            float scale_change =scale_speed_vbtn*deltaTime;
                            content.theta_scale-=scale_change;
                            if(content.theta_scale <=1)
                            {
                                content.theta_scale=1;
                                content.state=5;
                            }
                            go.GetComponent<RectTransform>().localScale=new Vector3(content.theta_scale,content.theta_scale,1);
                                
                        }

                        if(content.state==5)
                        {
                            adder+=1;
                        }
                    }

                    if(adder == (text_right_active.Count+text_left_active.Count) )
                    {
                        subtext.transform.position=new Vector3(10000,100-subtext_y_offset,1f);
                        intro_licht_m.transform.position=new Vector3(10000,100-subtext_y_offset,1);
                        mitte_animation_state=AnimationState.s06;
                    }

                    
                }
                else if(mitte_animation_state == AnimationState.s06)
                {
                    // subtext erscheint und licht kommt riesen licht
                    // licht stärke und angle will increase over time
                    // dann endewenn fertig

                    Vector3 temp = new Vector3(appear_dir.x,appear_dir.y,appear_dir.z);
                    
                    temp.Scale(new Vector3(0,0,(subtext_t_speed*deltaTime)));

                    if(subtext.transform.position.z >= final_pos_z)
                    {
                        subtext.transform.position+=temp;
                    }

                    Vector3 temp2 = new Vector3(appear_dir.x,appear_dir.y,appear_dir.z);
                    temp2.Scale(new Vector3(0,0,(subtext_l_speed*deltaTime)));

                    if(intro_licht_m.transform.position.z >= -subtext_light_dist)
                    {
                        intro_licht_m.transform.position+=temp;
                    }
                    else
                    {
                        return true;
                    }


                }

                return false;
            };
            intro_mitte = new ComplexAnimation(closure);
        }



        {
            AnimationState opening_animation_state = AnimationState.s01;

            
            

            Camera orto = GameObject.Find("/IntroCamera").GetComponent<Camera>();

            float screenAspect = (float) Screen.width / (float) Screen.height;
            float camHalfHeight = orto.orthographicSize;
            float camHalfWidth = screenAspect * camHalfHeight;

            int zacken_länge=150;
            int zacken_höhe=100;

            //Debug.Log(camHalfWidth);


            Texture2D image= null;
            Texture2D ta = null;
            Texture2D tb= null;

            GameObject a = null;
            GameObject b= null;

            GameObject intro1Ende = GameObject.Find("/Intro1Ende");

            float moving_time = 12;
            Vector3 moving_dir = new Vector3(camHalfWidth,0,0);

            float wall_move_speed=moving_dir.magnitude / moving_time;
            moving_dir.Normalize();
            
            Func<float, bool> closure = delegate(float deltaTime)
            {

                if(opening_animation_state == AnimationState.s01 )
                {
                    

                    image = RTImage(orto);
                    ta = Instantiate(image);
                    tb = Instantiate(image);

                    foreach(GameObject fooObj in GameObject.FindGameObjectsWithTag("destroy_for_opening"))
                    {
                        Destroy(fooObj);
                    }
                    opening_animation_state=AnimationState.s02;
                }
                else if(opening_animation_state == AnimationState.s02 )
                {

                    a = Instantiate(intro1Ende,new Vector3(10000,0,-10),Quaternion.identity);
                    b = Instantiate(intro1Ende,new Vector3(10000,0,-10),Quaternion.identity);

                    a.GetComponent<Renderer>().material = Instantiate(a.GetComponent<Renderer>().material);
                    b.GetComponent<Renderer>().material = Instantiate(b.GetComponent<Renderer>().material);

                    Color color_transparent = new Color(0,0,0,0f);
                    Color color_black = new Color(0,0,0,1f);

                    // 150   100

                    for (int i = 0; i < ta.width/2; i++)
                    {
                        for (int k = 0; k < ta.height; k++)
                        {
                            ta.SetPixel((ta.width/2)+i,k,color_transparent);
                            tb.SetPixel(i,k,color_transparent);
                        }
                    }

                    // hier jetzt die zacken einfügen.
                    for (int i = ta.width/2; i < (ta.width/2)+zacken_länge; i++)
                    {
                        for (int k = 0; k < 12; k+=2)
                        {
                            for (int s = 0; s < zacken_höhe; s++)
                            {
                                ta.SetPixel(i,((k*ta.height/12)+s),image.GetPixel(i,((k*ta.height/12)+s)));
                            }
                        }
                    }

                    for (int i = tb.width/2 -zacken_länge; i < (tb.width/2); i++)
                    {
                        for (int k = 1; k < 12; k+=2)
                        {
                            for (int s = 0; s < zacken_höhe; s++)
                            {
                                tb.SetPixel(i,((k*tb.height/12)+s),image.GetPixel(i,((k*tb.height/12)+s)));
                            }
                        }
                    }

                    ta.Apply();
                    tb.Apply();

                    a.GetComponent<Renderer>().material.mainTexture = ta;
                    b.GetComponent<Renderer>().material.mainTexture = tb;

                    Camera sprite_camera = GameObject.Find("/D2_Welt/D2SpriteCamera").GetComponent<Camera>();
                    sprite_camera.transform.position=new Vector3(10000,0,-100);


                    opening_animation_state=AnimationState.s03;

                }
                else if(opening_animation_state == AnimationState.s03 )
                {
                    if(b.transform.position.x< 10000 + camHalfWidth+2 )
                    {
                        float speed_scale_value = wall_move_speed*deltaTime;
                        Vector3 tempv=new Vector3(moving_dir.x,moving_dir.y,moving_dir.z);
                        tempv.Scale(new Vector3(speed_scale_value,speed_scale_value,speed_scale_value));
                        a.transform.position-=tempv;
                        b.transform.position+=tempv;
                    }
                    else
                    {
                        Destroy(a);
                        Destroy(b);
                        Destroy(orto);
                        return true;                   
                    }                
                }

                return false;
            };
            

            intro_opening= new ComplexAnimation(closure);
        }

  
    }

    private ComplexAnimation intro_1;
    private ComplexAnimation intro_start;
    private ComplexAnimation intro_mitte;
    private ComplexAnimation intro_opening;
    


    interface ComplexAnimationI
    {
        bool play(float time);
    }

    class ComplexAnimation : ComplexAnimationI
    {
        
        private Func<float, bool> closure;
        public ComplexAnimation(Func<float, bool> closur)
        {
            this.closure=closur;
        }
        public bool play(float time)
        {
            
            return closure.Invoke(time);
        }
    }


    public void setup_aqua()
    {
        // animation metal
        // dann glass explosion
        // dann fülle becken und berge

        //doer sandstorm or particle storm fills the zylinder.
        // baicsly jstu a texcutre with trnsaprency and particles moving itno palce.

    }

    private bool camera_mover_enabled=false;

    private bool init_wait()
    {
        init_wait_time-=Time.deltaTime;
        if(init_wait_time<0)
        {
            GameObject cam_sprites = GameObject.Find("/D2_Welt/D2SpriteCamera");
            cam_sprites.transform.position=Const.start_intro_pos;
            GameObject real_cam = GameObject.Find("/D2_Welt/Camera4");
            real_cam.GetComponent<Camera>().backgroundColor = new Color32(0,0,0,255);
            return true;
        }
        return false;
    }

    private bool intro_1_m()
    {
       return intro_1.play(Time.deltaTime);
    }

    

    private bool init_animation()
    {
       return intro_start.play(Time.deltaTime);
    }
    private bool intro_mitte_animation()
    {
       return intro_mitte.play(Time.deltaTime);
    }
    private bool opening_animation()
    {
        return intro_opening.play(Time.deltaTime);
    }
    private bool normal_play()
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
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            

            case State.init_wait:
                if (init_wait())
                {
                    state = State.intro_1;  // intro_1
                }
                break;

            case State.intro_1:
                if (intro_1_m())
                {
                    state = State.init_animation; // init_animation
                }
                break;

            case State.init_animation:
                if (init_animation())
                {
                    state = State.intro_mitte_animation;
                }
                break;
                
            case State.intro_mitte_animation:
            if (intro_mitte_animation())
                {
                    state = State.opening_animation;
                }
                
                break;
            case State.opening_animation:
            if (opening_animation())
                {
                    state = State.normal_play;
                }
                
                break;
            case State.normal_play:
            if (normal_play())
                {
                    state = State.normal_play;
                }
                
                break;
        }

        population_control();


        

    }

    Texture2D RTImage(Camera camera)
    {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        // Render the camera's view.
        camera.Render();
        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();

        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;
        return image;
    }

    public void intro_skip()
    {
        state = State.normal_play;
        sprite_camera.transform.position=new Vector3(sprite_camera.transform.position.x,0,sprite_camera.transform.position.z);

    }

    

    public void licht(bool an)
    {

        
        if(an)
        {
            licht_maske.SetActive(true);
        }
        else
        {
            licht_maske.SetActive(false);
        }
    }



    private Vector3 rand_pos()
    {
        return new Vector3(
            UnityEngine.Random.Range(border_left, border_right),
            UnityEngine.Random.Range(border_up, border_down),
            0);
    }

    public Tuple<bool, GameObject> getRandomFischTarget()
    {
        if(fische.Count==0)
        {
            return Tuple.Create(false,(GameObject)null);
        }
        else
        {
            while(true)
            {
                int rand_indice = UnityEngine.Random.Range(0,fische.Count-1);
                GameObject fishc = (GameObject)fische[rand_indice];
                if(fishc.GetComponent<BasisFischLogik>().get_state()!=1)
                {
                    continue;
                }
        
                return Tuple.Create(true,fishc);
            }
            
        }
        
    }

    private int max_fisch_count=30;
    private int max_fisch_bg_count=35;



    private ArrayList fisch_prefabs;
    private GameObject predator;

    private void init_all_fisch()
    {
        fisch_prefabs= new ArrayList();
        fisch_prefabs.Add( Resources.Load("Prefab/FischGelb"));
        fisch_prefabs.Add( Resources.Load("Prefab/FischPink1"));
        fisch_prefabs.Add( Resources.Load("Prefab/ClownPink"));
        fisch_prefabs.Add( Resources.Load("Prefab/FischStreifenPG"));
        fisch_prefabs.Add( Resources.Load("Prefab/FischStreifenBR"));
        fisch_prefabs.Add( Resources.Load("Prefab/FischBlau"));
        fisch_prefabs.Add( Resources.Load("Prefab/FischOrange"));
        fisch_prefabs.Add( Resources.Load("Prefab/Ariel"));
        fisch_prefabs.Add( Resources.Load("Prefab/FischPink2"));
        // predator = Resources.Load("Prefab/PredatorFisch");


        
        for (int i = 0; i < max_fisch_bg_count; i++)
        {
            GameObject fisch_prefab = (GameObject)fisch_prefabs[UnityEngine.Random.Range(0,fisch_prefabs.Count)];
            int rand = UnityEngine.Random.Range(0,2);
            int offset=60;
            if(rand==0)
            {
                offset*=-1;
            }
            Vector3 position = new Vector3(10000+offset,0,7);
            GameObject new_fisch = Instantiate(fisch_prefab,position,Quaternion.identity);
            // new_fisch.GetComponent<SpriteRenderer>().color=new Color32(118,118,118,255);
            new_fisch.GetComponent<BasisFischLogik>().set_as_bg();
            new_fisch.transform.localScale *= UnityEngine.Random.Range(0.3f,0.6f);
            fische_bg.Add(new_fisch);
        }
    }

    private void population_control()
    {
        if(fische.Count<max_fisch_count)
        {
            int diff = max_fisch_count-fische.Count;
            for (int i = 0; i < diff; i++)
            {
                GameObject fisch_prefab = (GameObject)fisch_prefabs[UnityEngine.Random.Range(0,fisch_prefabs.Count)];
                int rand = UnityEngine.Random.Range(0,2);
                int offset=60;
                if(rand==0)
                {
                    offset*=-1;
                }
                Vector3 position = new Vector3(10000+offset,0,0);
                GameObject new_fisch = Instantiate(fisch_prefab,position,Quaternion.identity);
                new_fisch.transform.localScale *= UnityEngine.Random.Range(0.5f,1.4f);
                fische.Add(new_fisch);
            }
        }

        // last_detection_coords.Item1=item1;
        // last_detection_coords.Item2=item2;
        
        if(last_detection_coords.Item1>=0)
        {
            float breite = border_right-border_left;
            float höhe = border_up-border_down;

            Vector3 apoint = new Vector3(last_detection_coords.Item1*breite+border_left,((1-last_detection_coords.Item2)*höhe)+-10f,0);
                

            var pointde = GameObject.Find("/pointde");
            // pointde.transform.position=new Vector3(apoint.x,apoint.y,-1);
            
            for (int i = 0; i < fische.Count; i++)
            {
                GameObject fisch = (GameObject)fische[i];
                // Debug.Log(apoint);
                fisch.GetComponent<BasisFischLogik>().flee_affected(apoint);
            }
        }
        
    }

}
