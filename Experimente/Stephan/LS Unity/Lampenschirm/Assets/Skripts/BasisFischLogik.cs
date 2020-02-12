using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasisFischLogik : MonoBehaviour
{

    private float acceleration = 5.2f;
    private float decceleration = 5.7f;

    private float min_speed = 0.8f;

    private bool is_schwarm=false;

    public bool do_debug=false;

    private float init_wait=1.5f;
    
    private Vector3 temp_velocity = new Vector3(0,0,0);
    private Vector3 goal = new Vector3(30,1,0);

    private bool is_bg=false;
    

    private float acceleraiotn_time=0.4f;

    private float acceleration_timer=0;


    private bool boost = false;
  
    private float run_time_left = Const.run_away_time;

    private float innate_top_speed_flee;
    private float innate_top_speed_base;
    private float innate_top_speed_close;

    private  int state = 0;
    private float deletion_time_after_death=Const.deletion_time;

    private bool has_target=false;

    private bool has_goal=false;

    private GameObject hunting_target;

    public int predatory_state=0; // 0 = normal entity 1 = predator 

    private Fisch_simple_rotation basis_daten;

    private Logik2D logik2D;

    private Color32 innate_color;

    public void set_as_predator()
    {
        predatory_state=1;
    }

    public void set_is_schwarm()
    {
        is_schwarm=true;
    }

    public void set_state_killed()
    {
        state=2;
    }

    public int get_state()
    {
        return state;
    }

    

    void Start()
    {
        basis_daten = this.GetComponent<Fisch_simple_rotation>();

        logik2D = GameObject.Find("/D2_Welt").GetComponent<Logik2D>();


        innate_top_speed_flee = Const.maxSpeed_flee+  Random.Range(-0.3f,0.3f);
        innate_top_speed_base = Const.maxSpeed+  Random.Range(-1.0f,1.0f);
        innate_top_speed_close = Const.maxSpeed_close_to_target+  Random.Range(-0.08f,0.08f);

        init_wait = init_wait+ Random.Range(-0.9f,0.8f);

        min_speed =(1f/3f) * innate_top_speed_base;

        innate_color = gameObject.GetComponent<SpriteRenderer>().color;

        if(predatory_state==1)
        {
            acceleration+=1.5f;
            innate_top_speed_base+=2;
        }
        
        Vector3 start_pos = transform.position ; // rand_pos();
        new_target();
        transform.position=start_pos;
        has_goal=true;

        //goal z psotion is 0:  all fishc msut be at 0or so.
        // bakcogrund bodne tiere mit animationen auf ebene -0.2f  oder so

        // fehler2? keine beegung. gefangen an falschem ort.

        temp_velocity = goal-transform.position;
        temp_velocity.Normalize();
        temp_velocity.Scale(new Vector3(0.2f,0.2f,0.2f));
        

        Vector3 dir = temp_velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        basis_daten.set_velocity(temp_velocity);

        if(is_bg)
        {
            innate_top_speed_base=innate_top_speed_base/2f;
            acceleration=acceleration/2f;
            decceleration=decceleration/2f;
        }

        if(is_schwarm)
        {
            if(transform.position.x>10000)
            {
                goal.x=10000 -400;
            }
            else
            {
                goal.x=10000 + 400;
            }
           
        }

        

    }

    // sehr hart eingrenzen, wo alles die welt ist, wo alles sichtbar ist.
    // testen: flache texture für metal ob das mit licht gut aussieht.
    // animation: fisch bewegt sich rechts links dann erscheint text aus dem zylinnder mit lichtquellen fix.
    // dann aufklapp animation und dahinter leigt dann das aquarium.
    // dazu vlt. erstmal 1 textur.d ann screenshot daovn und dann auf 4 aufspalten oder 2 wenne s zu seiten mit zähnen aufgeht.
    // schrift kann isch dabei auch um so 45 grad drehen


    // collision

    // populaiton control

    // drop from top or bottom as start

    //eater fisch

    // loop through sides

    // animaiton fill with water  birds attacking

    // a timer for all stuff

    // glas effekt


    // init animaiton metal  and lighting turn to glas then make world. risign mountain.

    // end animaiton world goes bad red sea and then restart.

    // use input to scatter or spawn fisch or create food.

    public void set_as_bg()
    {
        is_bg=true;
       
    }

    public void flee_affected(Vector3 interaction_position)
    {
        if(boost || is_bg)
        {
            return;
        }
        if(transform.position.x == interaction_position.x && transform.position.y == interaction_position.y)
        {
            // state = 2;
        }
        else if( (interaction_position-transform.position).magnitude < Const.affected_radius )
        {
            boost = true;
            run_time_left = Const.run_away_time;
            this.GetComponent<SpriteRenderer>().color=new Color32(200,20,30,255);
            has_goal=true;
            has_target=false;

            if(transform.position.x>=interaction_position.x)
            {
                goal.x+=Const.flee_add_x;
            }
            else
            {
                goal.x-=Const.flee_add_x;
            }
            goal.y=Random.Range(-9f,9f);

            if(is_schwarm)
            {
                goal.y=Random.Range(-50,50);
                if(transform.position.x>=interaction_position.x)
                {
                    goal.x=10000+Const.flee_add_x*3;
                }
                else
                {
                    goal.x=10000-Const.flee_add_x*3;
                }
            }


            // Vector3 new_pos = (transform.position-interaction_position);
            // new_pos.Normalize();

            // new_pos.Scale(new Vector3(Const.flee_distance,Const.flee_distance,Const.flee_distance));

            // new_pos = transform.position + new_pos;
            // if(new_pos.y>Const.wassergrenze_oben)
            // {
            //     new_pos.y=Const.wassergrenze_unten-(Const.safety_margin);
            // }
            // else if(new_pos.y<Const.wassergrenze_unten)
            // {
            //     new_pos.y=Const.wassergrenze_unten+(Const.safety_margin);
            // }

        }

    }
  


    private Vector3 rand_pos()
    {
        return new Vector3(
            Random.Range(Const.visible_main_area_left+(Const.safety_margin2), Const.visible_main_area_right-(Const.safety_margin2)),
            Random.Range(Const.wassergrenze_oben-(Const.safety_margin2), Const.wassergrenze_unten+(Const.safety_margin2)),
            transform.position.z);
    }
    // Update is called once per frame

    void Update()
    {
     
        if(state==0)
        {
            init_wait-=Time.deltaTime;
            if(init_wait<0)
            {
                state=1;
            }
        }
        else if(state == 1)
        {
            temp_velocity = basis_daten.get_velocity();
            // if(temp_velocity.magnitude==0)
            // {
            //     temp_velocity = new Vector3(1,0,0);
            // }

            

            if(!has_goal)
            {
                
                if(has_target)
                {
                    hunting_target.GetComponent<BasisFischLogik>().set_state_killed();
                }
                
                new_target();  
                 
            }

            // lock on target
            Vector3 temp_goal;
            if(has_target)
            {
                temp_goal=hunting_target.transform.position;
            }
            else
            {
                temp_goal=goal;
            }
            

            Vector3 dir_vec = temp_goal-transform.position;
            // float dist_to_target = dir_vec.magnitude;
            float mag_acc=0;
            dir_vec.Normalize();
            {
                
                mag_acc+=acceleration;
                acceleration_timer+= Time.deltaTime;
                if(acceleration_timer<acceleraiotn_time)
                {
                    mag_acc+=acceleration;
                }
                else if(acceleration_timer>=1.5*acceleraiotn_time)
                {
                    acceleration_timer=0;
                }
                else
                {
                    // mag_acc-=decceleration;
                }
               
                // else
                // {
                //     dir_vec = new Vector3(-dir_vec.x,-dir_vec.y,0);
                //     mag_acc+=decceleration;
                // }
                

                if(boost)
                {
                    mag_acc+=Const.acceleration_flee;

                    run_time_left -= Time.deltaTime;

                    if(run_time_left<=0)
                    {
                        run_time_left=Const.run_away_time;
                        boost=false;
                        this.GetComponent<SpriteRenderer>().color= innate_color;

                    }

                }
                
                // if(temp_velocity.magnitude> innate_top_speed_base || temp_velocity.magnitude> innate_top_speed_flee)
                // {
                //     mag_acc=Const.acceleration_close;
                // }
                mag_acc*=Time.deltaTime;
                dir_vec.Scale(new Vector3(mag_acc,mag_acc,0)); 

    
            }

            
            if(temp_velocity.magnitude+mag_acc <temp_velocity.magnitude  && temp_velocity.magnitude+mag_acc< min_speed)
            {
                temp_velocity.Normalize();
                temp_velocity.Scale(new Vector3(min_speed,min_speed,min_speed));
            }
            else
            {
                temp_velocity += dir_vec;
            }
            
            float speed = temp_velocity.magnitude;
            if(boost )
            {
                if( speed>=innate_top_speed_flee)
                {
                    temp_velocity.Normalize();
                    temp_velocity.Scale(new Vector3(innate_top_speed_flee,innate_top_speed_flee,innate_top_speed_flee));
                }
            }
            else if(temp_velocity.magnitude>=innate_top_speed_base)
            {
                temp_velocity.Normalize();
                temp_velocity.Scale(new Vector3(innate_top_speed_base,innate_top_speed_base,innate_top_speed_base));
            }
           
            float scale_value = temp_velocity.magnitude*Time.deltaTime;
            Vector3 delta_vel_vector = new Vector3(temp_velocity.x,temp_velocity.y,temp_velocity.z);
            delta_vel_vector.Normalize();
            delta_vel_vector.Scale(new Vector3(scale_value,scale_value,0));

            transform.position+= delta_vel_vector;

            // if(transform.position.y>=Const.wassergrenze_oben  )
            // {
            //     Vector3 pos_alt = transform.position;
            //     pos_alt.y=Const.wassergrenze_oben-Const.safety_margin2;
            //     transform.position= pos_alt;
            //     temp_velocity.y= -temp_velocity.y;
            // }
            // if( transform.position.y<=Const.wassergrenze_unten)
            // {
            //     Vector3 pos_alt = transform.position;
            //     pos_alt.y=Const.wassergrenze_unten+Const.safety_margin2;
            //     transform.position= pos_alt;
            //     temp_velocity.y= -temp_velocity.y;
            // }
      

            // test if goal reached
            // x koordiante wird getestet.
            if(Mathf.Abs((this.transform.position - temp_goal).magnitude) <= 2*Const.hitbox_radius)
            {
                has_goal=false;
            }
            // else if( (start_pos.x<goal.x && transform.position.x>=   goal.x)
            // ||  (start_pos.x>goal.x && transform.position.x<=   goal.x) )
            // {
            //     has_goal=false;
            // }



        }
        else if(state == 2)
        {
            // todo create death animaiton sprite and aslo delte it afterwards
            // Animator anim = this.GetComponent<Animator>();
            // if(anim.GetCurrentAnimatorStateInfo(0).IsName("Final"))
            // {
                
            // }
            this.GetComponent<Renderer>().enabled=false;
            float teleport=45;
            
            GameObject explosion_sprite =  (GameObject)Resources.Load("Prefab/exp1_0");
            Instantiate(explosion_sprite,transform.position,Quaternion.identity);
            if(Random.Range(0,2)==0)
            {
                transform.position=new Vector3(10000-teleport,Random.Range(-5f,5f),transform.position.z);
            }
            else
            {
                transform.position=new Vector3(10000+teleport,Random.Range(-5f,5f),transform.position.z);
            }


            
            state=3;

            
        }
        else if(state == 3)
        {
            deletion_time_after_death-=Time.deltaTime;
            if(deletion_time_after_death<=0)
            {
                deletion_time_after_death=Const.deletion_time;
                this.GetComponent<Renderer>().enabled=true;
                has_goal=false;
                has_target=false;
                state=1;
            }
        }

        basis_daten.set_velocity(temp_velocity);

    }

    void OnDestroy() {
        Debug.Log("toot");
    }

    private void new_target()
    {

        float mag = 0.2f;
        temp_velocity.Normalize();
        temp_velocity.Scale(new Vector3(mag,mag,mag));

        has_target=false;
        
        
        // goal;
        bool predatoring=true;
        Vector3 start_pos = transform.position;
        while(true)
        {
            if(predatory_state==1  && predatoring)
            {
                var target_return = logik2D.getRandomFischTarget();
                if(target_return.Item1 == false)
                {
                    predatoring=false;
                }
                else
                {
                    hunting_target=target_return.Item2;
                    has_target=true;
                    break;
                }
                
            }
            else
            {
                goal = rand_pos();
                
                if(goal.x!= start_pos.x )
                {
                    if(Mathf.Abs(start_pos.x-goal.x)>15f)
                    {
                        break;
                    }
                    
                }
            }
        }
        has_goal=true;

        
        
    }
}
