using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasisFischLogik : MonoBehaviour
{

    private float acceleration = 0.55f;
    private float decceleration = -0.15f;
    
    private Vector3 temp_velocity = new Vector3(0,0,0);
    private Vector3 goal = new Vector3(30,1,0);
    private Vector3 start_pos=new Vector3(0,0,0);

    private float acceleraiotn_time=0.5f;

    private float acceleration_timer=0;


    private bool boost = false;
    private bool close_to_target = false;

    private float run_time_left = Const.run_away_time;

    private float innate_top_speed_flee;
    private float innate_top_speed_base;
    private float innate_top_speed_close;

    public int state = 1;
    private float deletion_time_after_death=4;

    private bool has_target=false;

    private bool has_goal=false;

    private GameObject hunting_target;

    public int predatory_state=0; // 0 = normal entity 1 = predator 

    private Fisch_simple_rotation basis_daten;

    private Logik2D logik2D;

    public void set_as_predator()
    {
        predatory_state=1;
    }

    void Start()
    {
        basis_daten = this.GetComponent<Fisch_simple_rotation>();

        logik2D = GameObject.Find("/D2_Welt").GetComponent<Logik2D>();


        innate_top_speed_flee = Const.maxSpeed_flee+  Random.Range(-0.3f,0.3f);
        innate_top_speed_base = Const.maxSpeed+  Random.Range(-0.15f,0.15f);
        innate_top_speed_close = Const.maxSpeed_close_to_target+  Random.Range(-0.08f,0.08f);
        
        start_pos = rand_pos();
        new_target();
        transform.position=start_pos;
        has_goal=true;

        temp_velocity = goal-transform.position;
        temp_velocity.Normalize();
        temp_velocity.Scale(new Vector3(0.2f,0.2f,0.2f));
        

        Vector3 dir = temp_velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        basis_daten.set_velocity(temp_velocity);
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

    public void flee_affected(Vector3 interaction_position)
    {
        if(transform.position.x == interaction_position.x && transform.position.y == interaction_position.y)
        {
            Destroy(gameObject);
            // TODO fisch stirbt
        }
        else if( (interaction_position-transform.position).magnitude < Const.affected_radius )
        {
            boost = true;
            run_time_left = Const.run_away_time;

            Vector3 new_pos = (transform.position-interaction_position);
            new_pos.Normalize();

            new_pos.Scale(new Vector3(Const.flee_distance,Const.flee_distance,Const.flee_distance));

            new_pos = transform.position + new_pos;
            if(new_pos.y>Const.wassergrenze_oben)
            {
                new_pos.y=Const.wassergrenze_unten-(Const.safety_margin);
            }
            else if(new_pos.y<Const.wassergrenze_unten)
            {
                new_pos.y=Const.wassergrenze_unten+(Const.safety_margin);
            }

        }

    }
  


    private Vector3 rand_pos()
    {
        return new Vector3(
            Random.Range(Const.visible_main_area_left+(Const.safety_margin2), Const.visible_main_area_right-(Const.safety_margin2)),
            Random.Range(Const.wassergrenze_oben-(Const.safety_margin2), Const.world_limit_bottom_hard+(Const.safety_margin2)),
            0f);
    }
    // Update is called once per frame

    void Update()
    {
        if(state == 1)
        {
            temp_velocity = basis_daten.get_velocity();

            if(!has_goal)
            {
                if(has_target)
                {
                    start_pos = hunting_target.transform.position;
                    hunting_target.GetComponent<BasisFischLogik>().state=2;
                }
                else
                {
                    start_pos = goal;
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
            float dist_to_target = dir_vec.magnitude;
            dir_vec.Normalize();
            {
                float mag_acc=0;
                acceleration_timer+= Time.deltaTime;
                if(acceleration_timer<acceleraiotn_time)
                {
                    mag_acc+=acceleration;
                }
                else if(acceleration_timer>=2*acceleraiotn_time)
                {
                    acceleration_timer=0;
                }
                mag_acc+=decceleration;

                if(boost)
                {
                    mag_acc+=Const.acceleration_flee;

                    run_time_left -= Time.deltaTime;

                    if(run_time_left<=0)
                    {
                        run_time_left=Const.run_away_time;
                        boost=false;

                    }

                }
                
                if(temp_velocity.magnitude> innate_top_speed_base || temp_velocity.magnitude> innate_top_speed_flee)
                {
                    mag_acc=Const.acceleration_close;
                }
                mag_acc*=Time.deltaTime;
                dir_vec.Scale(new Vector3(mag_acc,mag_acc,0)); 
    
            }

            temp_velocity += dir_vec;

            transform.position+= temp_velocity;

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
            state=3;

            
        }
        else if(state == 3)
        {
            deletion_time_after_death-=Time.deltaTime;
            if(deletion_time_after_death<=0)
            {
                Destroy(gameObject);
            }
        }

        basis_daten.set_velocity(temp_velocity);

    }

    private void new_target()
    {

        float mag = 0.2f;
        temp_velocity.Normalize();
        temp_velocity.Scale(new Vector3(mag,mag,mag));

        has_target=false;
        
        // goal;
        bool predatoring=true;
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
            has_goal=true;
            
        }
        
    }
}
