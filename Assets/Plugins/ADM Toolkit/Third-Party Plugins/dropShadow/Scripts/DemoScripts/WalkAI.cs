using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAI : MonoBehaviour 
{

    public float speed;
    public int dirInt = 1;

    public bool turned = false;
    private int reset = 0;
	
	// Update is called once per frame
	void Update () 
    {

        Vector3 Pos = transform.position;
        Pos.x += dirInt;
        transform.position = Vector3.MoveTowards(transform.position, Pos ,speed * Time.deltaTime);

//        print(transform.right);

        if (turned)
        {
            reset ++;

            if (reset > 5)
            {
                turned = false;
                reset = 0;
            }
        }

	}

    void OnCollisionEnter2D(Collision2D coll) 
    {
        if (coll.gameObject.tag == "Block" && !turned)
        {
            dirInt *= -1;
            transform.localScale = new Vector3((float)dirInt,1f,1f);

            turned = true;
        }
        
    }
}
