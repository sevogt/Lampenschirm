using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logik : MonoBehaviour
{
    private Object base_ball;
    private Object base_fisch;

    private ArrayList bälle;
    // Start is called before the first frame update
    void Start()
    {
        bälle = new ArrayList();
        base_ball = Resources.Load<GameObject>("Prefab/BaseBall");
        base_fisch = Resources.Load<GameObject>("Prefab/FischA");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void reset_all()
    {

    }

    public void start_aqua()
    {
        GameObject fisch = (GameObject)Instantiate(base_fisch,new Vector3(0,0,0), Quaternion.identity);
        bälle.Add(fisch);           
    }

    public void start_ball()
    {
        StartCoroutine(make_balls());

        IEnumerator make_balls()
        {
            
            for (int k = 0; k < 20; k++)
            {
                if(test_coll())
                {
                    GameObject go = (GameObject)Instantiate(base_ball,new Vector3(0,0,0), Quaternion.identity);
                    bälle.Add(go);
                }
                else
                {
                    k--;
                }
                yield return new WaitForSeconds(0.3f);
                
            }
            
        }

        bool test_coll()
        {
            for (int i = 0; i < bälle.Count; i++)
            {
                if (Vector3.Distance(new Vector3(0,0,0), ((GameObject)bälle[i]).transform.position) < 0.07f) 
                {
                    return false;
                }
            }
            return true;
        }     
        
    }
}
