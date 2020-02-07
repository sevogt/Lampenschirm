using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const 
{
    public static readonly Vector3 target = new Vector3(0,0,0);
    public static readonly Vector3 sprite_centre = new Vector3(1000,0,0);
    public static readonly float initial_dist_to_centre = 1.93f + radius;
    public static readonly float initial_height_from_centre = -0.14f;
    public static readonly float radius = 0.35f;

    public static readonly int LEFT=1;
    public static readonly int RIGHT=-1; 

    public static readonly float hitbox_radius=0.7f;
 

    public static readonly float startTimer = 6;
    public static readonly float camMovementTime=2f;

    public static readonly float maxSpeed=4.1f;
    public static readonly float maxSpeed_close_to_target=0.2f;
    
    public static readonly float maxSpeed_flee=7.9f;

    public static readonly float acceleration_base=0.2f;
    public static readonly float acceleration_close=-0.2f;

    public static readonly Vector3 calibration_pos = new Vector3(10000,-50,-2);
    public static readonly Vector3 start_intro_pos = new Vector3(10000,200,-2);
    
    public static readonly float acceleration_flee=2.7f;
    public static readonly float flee_add_x=30f;

    public static readonly float deletion_time=20f;

    
    

    public static readonly float run_away_time = 3f; // sekunde

    public static readonly float affected_radius = 8.4f;

    public static readonly float flee_distance = 5f;

    public static readonly float wassergrenze_oben = 10.2f;
    public static readonly float wassergrenze_unten = -11f;
    public static readonly float safety_margin = 0.8f;
    public static readonly float safety_margin2 = 1.6f;

    public static readonly float margin_welt_seiten = 9f;

    public static readonly float visible_main_area_left = 9981f-margin_welt_seiten;
    public static readonly float visible_main_area_right = 10019f+margin_welt_seiten;

    public static readonly float visible_main_area_top = 10f;

    public static readonly float visible_main_area_bottom = -10f;

    public static readonly float world_limit_top_hard = 35f;
    public static readonly float world_limit_bottom_hard = -20f;

    public static readonly float world_limit_left_hard = 9945f;
    public static readonly float world_limit_right_hard= 10055f;

    public static readonly float camOrtoPos1=0;
    public static readonly float camOrtoPos2=15;

    public static readonly float buffer1 = 0.02f;

    public static readonly int höhe = 400;
    public static readonly float height_zylinder = 0.4f;
    public static readonly int weite = 2199;

    public static readonly float länge_texture=36.6f+2f;
    
}

