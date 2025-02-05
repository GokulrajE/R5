using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homeposition : MonoBehaviour
{
    //public GameObject Circle;
    public GameObject start;
 
   

    public void Update()
    {
       

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(" Space Pressed");
            //transform.position = HomePoint;
            //Circle.GetComponent<SpriteRenderer>().enabled = true;
            start.GetComponent<SpriteRenderer>().enabled = true; 
        }
        if (Input.GetKeyDown(KeyCode.W))
        
        {
            //Circle.GetComponent<SpriteRenderer>().enabled = false;
            start.GetComponent<SpriteRenderer>().enabled= false;
        }
    }
}
