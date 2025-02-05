
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using Michsky.UI.ModernUIPack;
//using Unity.VisualScripting;

//public class FlappyCalibrate : MonoBehaviour
//{
//    public static LineRenderer lr;
//    public static List<Vector3> paths_draw;
//    public static List<Vector3> paths_pass;
    

//    public static Rigidbody2D rb2d;
//    public float speed = 0.001f;
//    public float tilt;
//    public Text ScoreText;

//    float startWidth = 1.0f;
//    float endWidth = 1.0f;
//    public Color startColor = Color.green;
//    public Color endColor = Color.green;

//    public float max_y;
//    public float min_y;

//    List<Vector3> paths;
//    float l1 = 333;
//    float l2 = 381;

//    //float maxy_init = -100;
//    //float miny_init = 200;


//    float maxy_init = -290;
//    float miny_init = 32;


//    //float maxy_init = -293;
//    //float miny_init = 32;

//    Color c2;

//    // Start is called before the first frame update
//    void Start()
//    {
//        rb2d = GetComponent<Rigidbody2D>();

//        paths_draw = new List<Vector3>();
//        paths_pass = new List<Vector3>();



       


//        c2 = new Color(1, 1, 1, 0);

//        lr = GetComponent<LineRenderer>();
//        //lr.SetWidth(1.0f, 1.0f);

//        lr.startWidth = startWidth;
//        lr.endWidth = endWidth;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        // if (Input.GetMouseButtonDown(0))
//        // {
//        //     Vector3 mousePos = Input.mousePosition;
//        //     Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition)+" :xx");
//        //     // Debug.Log(mousePos.y+" yy");
//        // }
//    }

//    void FixedUpdate ()
//	{		
//        // float moveHorizontal = Input.GetAxis ("Horizontal");
//		float moveVertical = Input.GetAxis ("Vertical");

//		// Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
//        Vector3 movement = new Vector3 (0.0f, 0.0f, moveVertical);
//		GetComponent<Rigidbody2D>().velocity = movement * speed;

//        //double y_value = ((Mathf.Cos(3.14f/180*AppData.plutoData.enc1) * (333 * Mathf.Cos(3.14f / 180 * AppData.plutoData.enc2) + 381 * Mathf.Cos(3.14f / 180 * AppData.plutoData.enc2 + 3.14f / 180 * AppData.plutoData.enc3))));
//        float l1 = 333;
//        float l2 = 381;
//        float x2, y2;
//        //float theta1 = R5comm.theta1;
//        //float theta2 = R5comm.theta2;


//        //Debug.Log(R5comm.theta1 + "        " + R5comm.theta2);

//        float thetaa = theta1 * Mathf.Deg2Rad;
//        float thetab = theta2 * Mathf.Deg2Rad;

//        float y1 = -(Mathf.Cos((thetaa)) * l1 + Mathf.Cos((thetaa + thetab)) * l2);
//        float x1 = -(Mathf.Sin((thetaa)) * l1 + Mathf.Sin((thetaa + thetab)) * l2);

//        //x2 = ((x1) / (333 + 381) * 7.5f);
//        //y2 = ((y1 + 350) / (400 * 2)) * 7.8f;
//        ///

//        //x2 = ((x1) / (333 + 381) * 5f);
//        //y2 = ((y1 + 350) / (400 * 2)) * 7f;


//       x2 = ((x1) / (333 + 381) * 5f);
//        y2 = ((y1 + 350) / (400 * 2)) * 3.4f;

//        //x2 = ((x1) / (333 + 381) );
//        //y2 = ((y1 + 350) / (400 * 2)) ;

//       Debug.Log("xaxis" + "   " + theta1 + "    " + thetaa + "   " + x1 + "    " + x2 + "    " + "yaxis" + theta2 + "    " + thetab + "   " + y1 + "    " + y2);



//        //transform.position = new Vector2(x2, y2);
//        //transform.Translate(transform.position);


//        //float y_value = -Mathf.Clamp(Angle2ScreenZ(y2), -6, 3);

//        float y_value = y2;

//        //float y_value = Angle2ScreenZ(y2);
//       // Debug.Log(y_value);


//        //double result_value = (-(y_value/400)*7.0);

//       // double result_value =y_value;

//        float y_u = ((y_value / (maxy_init - miny_init)) * -680f+0.7f);

//         Debug.Log("PlayerPosition: "+ y_u);
//        //ScoreText.text = AppData.plutoData.enc1.ToString();

//  //      GetComponent<Rigidbody2D>().position = new Vector3
//  //(
//  //    0.0f,
//  //    (float)y_u + 1.0f,
//  //          0.0f
//  //);

//        //GetComponent<Rigidbody2D>().transform.rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody2D>().velocity.x * -tilt);


//        //Vector3 xxx = GetComponent<Rigidbody2D>().position;
//        //// Debug.Log("PlayerPosition: "+xxx);

//        Vector3 to_draw_values = new Vector3(0.0f, (float)y_u, 0.0f);
//        Vector3 to_pass = new Vector3 (0.0f, (float)y_value,0.0f);
        
//        paths_draw.Add(to_draw_values);
//        paths_pass.Add(to_pass);

//        lr.positionCount = paths_draw.Count;
//        lr.SetPositions (paths_draw.ToArray());
//        //lr.SetColors(Color.green,Color.green);

//        lr.startColor = startColor;
//        lr.endColor = endColor;
//        lr.useWorldSpace = true;
		
//	}


//    //public void onclick_recalibrate()
//    //{
//    //    SceneManager.LoadScene("FlappyRecalibrate");
//    //}


//    //public void onlick_game()
//    //{
//    //    SceneManager.LoadScene("FlappyGame");
//    //    max_y = paths.Max(v => v.y);
//    //    min_y = paths.Min(v => v.y);
//    //}
//}


