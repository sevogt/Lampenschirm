using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasisFischLogik : MonoBehaviour
{

    private Vector3 acceleration = new Vector3(1,0,0);

    private float innate_normal_acceleration = 2f;
    private Vector3 velocity = new Vector3(0,0,0);
    private Vector3 goal = new Vector3(30,1,0);
    private Vector3 start_pos=new Vector3(0,0,0);
    private bool is_flipped = false;
    private bool do_flip=true;

    public int init_left = 2;

    private bool boost = false;
    private bool close_to_target = false;

    private float run_time_left = Const.run_away_time;

    private float innate_top_speed_flee;
    private float innate_top_speed_base;
    private float innate_top_speed_close;

    private int state = 1;
    


    void Start()
    {


        innate_top_speed_flee = Const.maxSpeed_flee+  Random.Range(-0.3f,0.3f);
        innate_top_speed_base = Const.maxSpeed+  Random.Range(-0.2f,0.2f);
        innate_top_speed_close = Const.maxSpeed_close_to_target+  Random.Range(-0.08f,0.08f);


        if(init_left == 2)
        {
            // GetComponent<SpriteRenderer>().flipX=true;
            
        }
        else if (init_left == 0)
        {
            do_flip=false;
        }
        
        start_pos = rand_pos();
        new_target();
        transform.position=start_pos;

        velocity = goal-transform.position;
        velocity.Normalize();
        velocity.Scale(new Vector3(0.2f,0.2f,0.2f));
        

        Vector3 dir = velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void init_new(int dir)
    {
        init_left=dir;
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
            Random.Range(Const.world_limit_left+(Const.safety_margin2), Const.world_limit_right-(Const.safety_margin2)),
            Random.Range(Const.wassergrenze_oben-(Const.safety_margin2), Const.world_limit_bottom_hard+(Const.safety_margin2)),
            0f);
    }
    // Update is called once per frame

    float xxxxxxxx =4;
    void Update()
    {
        if(state == 1)
        {

            xxxxxxxx -= Time.deltaTime;
        
            if(xxxxxxxx<=0)
            {
                Animator anim = this.GetComponent<Animator>();
                // GameObject sprite = (GameObject)Instantiate(Resources.Load<GameObject>("SpriteData/exp1_0"),transform.position , Quaternion.identity);
                if(anim.GetInteger("State")==1)
                {
                    anim.SetInteger("State",2);
                    state = 2;
                }
                
            }


            Vector3 dir_vec = goal-transform.position;
            float dist_to_target = dir_vec.magnitude;
            dir_vec.Normalize();
            {
                float mag_acc=acceleration.magnitude;
                acceleration.Set(dir_vec.x,dir_vec.y,0);
                Vector3 acc_copy = new Vector3(dir_vec.x,dir_vec.y,0);  

                if(dist_to_target<2.8)
                {
                    if(velocity.magnitude>innate_top_speed_close)
                    {
                        acceleration.Scale(new Vector3(Const.acceleration_close,Const.acceleration_close,Const.acceleration_close));
                    }
                    else
                    {
                        acceleration.Scale(new Vector3(Const.acceleration_base,Const.acceleration_base,Const.acceleration_base));
                    }
                }
                else
                {
                    acceleration.Scale(new Vector3(Const.acceleration_base,Const.acceleration_base,Const.acceleration_base));
                }

                if(boost)
                {
                    acc_copy.Scale(new Vector3(Const.acceleration_flee,Const.acceleration_flee,Const.acceleration_flee));
                    acceleration += acc_copy;

                    run_time_left -= Time.deltaTime;

                    if(run_time_left<=0)
                    {
                        run_time_left=Const.run_away_time;
                        boost=false;

                    }

                }

                if(velocity.magnitude> innate_top_speed_base || velocity.magnitude> innate_top_speed_flee)
                {
                    acceleration.Normalize();
                    acceleration.Scale(new Vector3(Const.acceleration_close,Const.acceleration_close,Const.acceleration_close));
                }
    
            }

            
            
            velocity =  velocity + (acceleration*Time.deltaTime);

            Vector3 dir = velocity;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            transform.position+= velocity;

            // test if goal reached
            // x koordiante wird getestet.
            if( (start_pos.x<goal.x && transform.position.x>=   goal.x)
            ||  (start_pos.x>goal.x && transform.position.x<=   goal.x) )
            {
                start_pos = goal;
                new_target();
                Vector3 dir_vec2= goal-transform.position;
                dir_vec2.Normalize();
                
                float mag = velocity.magnitude*(1f/3f);
                velocity.Normalize();
                velocity.Scale(new Vector3(mag,mag,mag));

            }


            if(velocity.x >0 && is_flipped && do_flip)
            {
                is_flipped=false;
                GetComponent<SpriteRenderer>().flipY=!GetComponent<SpriteRenderer>().flipY;

            }
            else if (velocity.x <0 && !is_flipped  && do_flip)
            {
                is_flipped=true;
                GetComponent<SpriteRenderer>().flipY=!GetComponent<SpriteRenderer>().flipY;
            }

        }
        else if(state == 2)
        {
            Animator anim = this.GetComponent<Animator>();
            if(anim.GetCurrentAnimatorStateInfo(0).IsName("Final"))
            {
                Destroy(gameObject);
            }

            
        }

    }

    private void new_target()
    {
        
        // goal;
        while(true)
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
}
