using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl_try : MonoBehaviour
{
    public static PlayerControl_try instance;

	float max_x_;
	float min_x_;
	float max_y_;
	float min_y_;

	public float speed;
	public float tilt;
	public Done_Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	
	// private float nextFire;
	public float nextFire;

	public float[] angXRange = { -40, 40 };
	public float[] angZRange  = { -40, -120 };

	public float fireStopClock;

	public float singleFireTime;

    double x_c;
    double y_c;

    private Rigidbody rb;

    void Awake()
    {
      instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        max_x_ = ChooseGame.instance.max_x;
		min_x_ = ChooseGame.instance.min_x;
		max_y_ = ChooseGame.instance.max_y;
		min_y_ = ChooseGame.instance.min_y;

        
        x_c = (max_x_+min_x_)/2;
        y_c = (max_y_+min_y_)/2;

        // rb.transform.position = new Vector3(0f,0f,0f);

        // Debug.Log(rb.transform.position);


    }
	
	void Update ()
	{
        // lr.positionCount = 0;
        // clear_lr = DrawRecalibrate.instance.clear_lr;
        // Debug.Log(clear_lr+" :clear_lr");

        // if (clear_lr == true)
        // {
        //     lr.positionCount = 0;
        // }
	}

	
	void FixedUpdate ()
	{
        //double x_value = (-(333 * Mathf.Sin(3.14f / 180 * AppData.plutoData.enc2) + 381 * Mathf.Sin(3.14f / 180 * AppData.plutoData.enc2 + 3.14f / 180 * AppData.plutoData.enc3)));
        //double y_value = ((Mathf.Sin(3.14f/180*AppData.plutoData.enc1) * (333 * Mathf.Cos(3.14f / 180 * AppData.plutoData.enc2) + 381 * Mathf.Cos(3.14f / 180 * AppData.plutoData.enc2 + 3.14f / 180 * AppData.plutoData.enc3))));

       double x_value = R5comm.x;
       double y_value = R5comm.y;



        //double x_u = -((Drawlines.x_value-x_c)*14)/(max_x_-min_x_);


        //double y_u = -(((Drawlines.y_value-y_c)*7)/(max_y_-min_y_))+1;

        double x_u = -((x_value-x_c)*14)/(max_x_-min_x_);


        double y_u = -(((y_value-y_c)*7)/(max_y_-min_y_))+1;

        Vector3 to_draw_values = new Vector3((float)x_u,(float)y_u,0.0f);

        // Debug.Log(to_draw_values);

       
	}


	
}
