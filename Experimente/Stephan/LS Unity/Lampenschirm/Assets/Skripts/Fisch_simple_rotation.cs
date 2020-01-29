using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fisch_simple_rotation : MonoBehaviour
{
    // Start is called before the first frame update
    private int init_left = 2;
    private bool is_flipped = false;
    private bool do_flip=true;

    private Vector3 velocity = new Vector3(1,0,0);

    void Start()
    {
        if (init_left == 0)
        {
            do_flip=false;
        }
    }

    public void set_normal_orientation_left()
    {
        init_left=1;
    }
    public void set_normal_orientation_right()
    {
        init_left=0;
    }

    public void set_velocity(Vector3 vel)
    {
        velocity=vel;
    }


    public Vector3 get_velocity()
    {
        return velocity;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 dir = velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

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
}
