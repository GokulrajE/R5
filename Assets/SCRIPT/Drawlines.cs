
using UnityEngine;
using System.Collections.Generic;
using static ChooseGame;
using System.IO;
using System;
using NeuroRehabLibrary;
using System.Collections;
using System.Linq;

public class Drawlines : MonoBehaviour
{
    public float speed;
    public float tilt;
    public Done_Boundary boundary;
    public static Drawlines instance;

    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    public float nextFire;

   
    public float fireStopClock;
    public float singleFireTime;

    float max_x_init;
    float min_x_init;
    float max_y_init;
    float min_y_init;
    float startWidth = 0.1f;
    float endWidth = 0.1f;
    double x_c;
    double y_c;

    double x_value;
    double y_value;
    double x_u;
    double y_u;
    public static float Xp;
    public static float Yp;
   
    public static LineRenderer lr;
    private LineRenderer hullRenderer; // For drawing the convex hull
    private LineRenderer rectRenderer; // For drawing the bounding box (rectangle)
   
    public static List<Vector3> paths_draw;
    public static List<Vector3> paths_pass;
    public static float PRomXmax;
    public static float PRomXmin;
    public static float PRomYmin;
    public static float PRomYmax;

    private string CalliPath;
    public static string start_time;
    public static string end_time;
    public static string datetime;
    public static string relativePath;
    public static string stopTime;
    public float[] Encoder = new float[6];
    public float[] ELValue = new float[6];
    private float romMinY = 67f;
    private float romMaxY = 17f;
    private float romMinX = 23f; //53
    private float romMaxX = -14f;//-24
    private bool isDrawing = true;
    private float drawDuration = 5f; // Time to draw the line
    private float timeElapsed = 0f;
    private List<Vector3> pathPoints = new List<Vector3>(); // Points for convex hull
    private float scaleFactor = 1f; // Scaling factor for the convex hull
    private float hullExpansion = 1f; // Adjust this value to make the hull larger


    private GameSession currentGameSession;

    public static class calliclass
    {
        public static string calli;

    }

    void Start()
    {
        //max_x_init = -591;
        //min_x_init = 91;
        //max_y_init = -575;
        //min_y_init = 75;
       

        paths_draw = new List<Vector3>();
        paths_pass = new List<Vector3>();
        lr = GetComponent<LineRenderer>();
        lr.startWidth = startWidth;
        lr.endWidth = endWidth;
        hullRenderer = new GameObject("HullRenderer").AddComponent<LineRenderer>(); // Convex hull LineRenderer
        rectRenderer = new GameObject("BoundingBoxRenderer").AddComponent<LineRenderer>();
        // Configure path renderer (black line)
        //SetupLineRenderer(lr, Color.black, 0.1f);

        // Configure hull renderer (transparent blue)
        SetupLineRenderer(hullRenderer, new Color(0f, 1f, 0f, 1f), 0.2f);
        SetupLineRenderer(rectRenderer, Color.red, 0.1f);
        x_c = (max_x_init + min_x_init) / 2;
        y_c = (max_y_init + min_y_init) / 2;


        string welcompath = Staticvlass.FolderPath;
        string callifile = Path.Combine(welcompath, "calibration");
        if (!Directory.Exists(callifile))
        {
            Directory.CreateDirectory(callifile);
        }
        CalliPath = Path.Combine(callifile, "calibration_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".csv");
        calliclass.calli = CalliPath;
        string fullFilePath = calliclass.calli;

        // Define the part of the path you want to store
        string partOfPath = @"Application.dataPath";

        // Use Path class to get the relative path
        relativePath = Path.GetRelativePath(partOfPath, fullFilePath);



        start_time = DateTime.Now.ToString("HH:mm:ss.fff");
        datetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff");

        // Ensure the file is ready for writing
        PrepareCSVFile(CalliPath, "Time,Encoder1,Encoder2\n");

        // CalliData();

        StartNewGameSession();
      

    }
    void SetupLineRenderer(LineRenderer lr, Color color, float width)
    {
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = width;
        lr.endWidth = width;
        lr.startColor = color;
        lr.endColor = color;
        lr.useWorldSpace = true;
    }


    void OnDisable()
    {
        EndCurrentGameSession();


    }
    void StartNewGameSession()
    {
        currentGameSession = new GameSession
        {
            GameName = "CALIBRATION",
            Assessment = 1 // Example assessment value, adjust as needed
        };

        SessionManager.Instance.StartGameSession(currentGameSession);
        Debug.Log($"Started new game session with session number: {currentGameSession.SessionNumber}");

        SetSessionDetails();
    }



    private void SetSessionDetails()
    {
        string device = "R5"; // Set the device name
        string assistMode = "Null"; // Set the assist mode
        string assistModeParameters = "Null"; // Set the assist mode parameters
        string deviceSetupLocation = "Null"; // Set the device setup location
        string gameParameter = "Null"; // Set the game parameter
        string trialDataFileLocation = relativePath;
        SessionManager.Instance.moveTime("0", currentGameSession);
        SessionManager.Instance.SetDevice(device, currentGameSession);
        SessionManager.Instance.SetAssistMode(assistMode, assistModeParameters, currentGameSession);
        SessionManager.Instance.SetDeviceSetupLocation(deviceSetupLocation, currentGameSession);
        SessionManager.Instance.SetGameParameter(gameParameter, currentGameSession);
        SessionManager.Instance.SetTrialDataFileLocation(trialDataFileLocation, currentGameSession);

    }

    void EndCurrentGameSession()
    {
        if (currentGameSession != null)
        {
           
            SessionManager.Instance.EndGameSession(currentGameSession);
        }
    }

    
    private void PrepareCSVFile(string path, string header)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, header);
        }
    }
   


    void Update()
    {
        Time.timeScale = 1;
        SensorValue();
        // Debug.Log(AppData.plutoData.motorCurrent1 == 0 ? "pREESSED" : "nOtPressed");
        end_time = DateTime.Now.ToString("HH:mm:ss.fff");
        if (isDrawing)
        {
            drawLine();
        }


        //StartCoroutine(DrawLineOverTime());
        if (Input.GetKey(KeyCode.UpArrow))
            hullExpansion += 0.01f;
           
        if (Input.GetKey(KeyCode.DownArrow))
            hullExpansion = Mathf.Max(0.01f, hullExpansion - 0.01f);
           

        if (!isDrawing) { 
            DrawExpandedHull();
        }



    }


    IEnumerator DrawLineOverTime()
    {
        while (timeElapsed < drawDuration)
        {
            GenerateRandomPositions();
            timeElapsed += Time.deltaTime;
            yield return null;
        }
            isDrawing = false;  
            DrawExpandedHull();

    }

    void GenerateRandomPositions()
    {
        if (isDrawing)
        {
           float xp = UnityEngine.Random.Range(romMinX, romMaxX); // Generate random X within the range
           float yp = UnityEngine.Random.Range(romMinY, romMaxY); // Generate random Y within the range
            float x_u = (float)-(((xp - 15) / 86) * 8.6 * 2);
            float y_u = -(((yp - 40) / 80) * 8f);

            // Add new points to the main path and for the convex hull
            Vector3 newPoint = new Vector3(x_u, y_u, 0.0f);
            paths_draw.Add(newPoint);
            pathPoints.Add(newPoint); // Store points for convex hull calculation
          
            // Update the line
            lr.positionCount = paths_draw.Count;
            lr.SetPositions(paths_draw.ToArray());
        }
    }

    void DrawExpandedHull()
    {
        List<Vector3> hullPoints = ComputeConvexHull__(pathPoints);

        // Expand each hull point outward
        Vector3 center = GetCentroid(hullPoints);
        List<Vector3> expandedHull = new List<Vector3>();

        foreach (var point in hullPoints)
        {
            Vector3 direction = (point - center).normalized;
            expandedHull.Add(point + direction * hullExpansion);
        }

        // Draw the hull
        hullRenderer.positionCount = expandedHull.Count + 1;
        for (int i = 0; i < expandedHull.Count; i++)
        {
            hullRenderer.SetPosition(i, expandedHull[i]);
        }
        hullRenderer.SetPosition(expandedHull.Count, expandedHull[0]); // Close the hull
        DrawBoundingBox(expandedHull);
        ComputeExtendedHullBounds(expandedHull);
    }
    void ComputeExtendedHullBounds(List<Vector3> hullPoints)
    {
        List<Vector3> rawHullPoints = new List<Vector3>();

        foreach (Vector3 p in hullPoints)
        {
            float rawXp = -((p.x / (8.6f * 2)) * 86) + 15;
            float rawYp = -((p.y / 8f) * 80) + 40;

            rawHullPoints.Add(new Vector3(rawXp, rawYp, 0));
        }

        // Get min & max for Xp, Yp
        PRomXmin = rawHullPoints.Min(p => p.x);
        PRomXmax = rawHullPoints.Max(p => p.x);
        PRomYmin = rawHullPoints.Min(p => p.y);
        PRomYmax = rawHullPoints.Max(p => p.y);

        Debug.Log($"Extended Hull Raw Bounds -> MinX: {PRomXmin}, MaxX: {PRomXmax}, MinY: {PRomYmin}, MaxY: {PRomYmax}");
       

    }
   

    void DrawBoundingBox(List<Vector3> points)
    {
        float minX = points.Min(p => p.x);
        float maxX = points.Max(p => p.x);
        float minY = points.Min(p => p.y);
        float maxY = points.Max(p => p.y);
       
        Debug.Log($"Bounding Box: MinX={minX}, MaxX={maxX}, MinY={minY}, MaxY={maxY}");

        // Define rectangle corners
        Vector3 topLeft = new Vector3(minX, maxY, 0);
        Vector3 topRight = new Vector3(maxX, maxY, 0);
        Vector3 bottomRight = new Vector3(maxX, minY, 0);
        Vector3 bottomLeft = new Vector3(minX, minY, 0);

        Vector3[] rectPoints = { topLeft, topRight, bottomRight, bottomLeft, topLeft };

        // Draw the rectangle
        rectRenderer.positionCount = rectPoints.Length;
        rectRenderer.SetPositions(rectPoints);
    }

    // Compute convex hull using a simple algorithm (replace with Graham scan or QuickHull if needed)
    List<Vector3> ComputeConvexHull__(List<Vector3> points)
    {
        if (points.Count < 3) return points;

        points = points.OrderBy(p => p.x).ThenBy(p => p.y).ToList();
        List<Vector3> hull = new List<Vector3>();

        foreach (var point in points)
        {
            while (hull.Count >= 2 && Cross(hull[hull.Count - 2], hull[hull.Count - 1], point) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(point);
        }

        int lowerHullStart = hull.Count + 1;
        for (int i = points.Count - 2; i >= 0; i--)
        {
            while (hull.Count >= lowerHullStart && Cross(hull[hull.Count - 2], hull[hull.Count - 1], points[i]) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(points[i]);
        }

        hull.RemoveAt(hull.Count - 1);
        return hull;
    }

    float Cross(Vector3 O, Vector3 A, Vector3 B)
    {
        return (A.x - O.x) * (B.y - O.y) - (A.y - O.y) * (B.x - O.x);
    }

    Vector3 GetCentroid(List<Vector3> points)
    {
        Vector3 sum = Vector3.zero;
        foreach (var p in points)
            sum += p;
        return sum / points.Count;
    }
    /// <summary>
    /// /new
    /// </summary>
   
    private void SensorValue()
    {

        ELValue[0] = R5comm.x;
        ELValue[1] = R5comm.y;

        Encoder = new float[] { R5comm.Xp, R5comm.Yp };

        string data = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss.fff},{Encoder[0]},{Encoder[1]}\n";
        File.AppendAllText(calliclass.calli, data);

    }
  

    void drawLine()
    {

        x_value = R5comm.Xp;
        y_value = R5comm.Yp;
        //Xp = (float.Parse(JediSerialPayload.data[0].ToString()));
        //Yp = (float.Parse(JediSerialPayload.data[1].ToString()));

        //x_value = Xp;
        //y_value = Yp;

        //Adjust the coordinate transformation unity draw area size size x is 8.6 cm and y is 4
        x_u = -(((x_value - 15) / 86) * 8.6 * 2); // Adjust the x transformation
        y_u = -(((y_value - 40) / 80) * 8f);  // Adjust the y transformation


        Debug.Log("x: pos" + x_value + "y:" + y_value);

        // Create the draw and pass vectors
        Vector3 to_draw_values = new Vector3((float)x_u, (float)y_u, 0.0f);
        Vector3 to_pass_values = new Vector3((float)x_value, (float)y_value, 0.0f);

        // Add the vectors to the lists
        paths_draw.Add(to_draw_values);
        paths_pass.Add(to_pass_values);
        pathPoints.Add(to_draw_values); // Store points for hull calculatio
        // Update the LineRenderer
        lr.positionCount = paths_draw.Count;
        lr.SetPositions(paths_draw.ToArray());
        lr.useWorldSpace = true;
       

    }
    public void FinishDrawHUll()
    {
        // Stop drawing and draw the expanded hull
        isDrawing = false;
        DrawExpandedHull();
    }
    
    //    void drawLine()
    //    {
    //        x_value = R5comm.Xp;
    //        y_value = R5comm.Yp;

    //        // Adjust coordinate transformation
    //        x_u = -(((x_value - 15) / 86) * 8.6f * 2); // Adjust x transformation
    //        y_u = -(((y_value - 40) / 80) * 8f);       // Adjust y transformation

    //        Debug.Log("x: pos " + x_value + " y: " + y_value);

    //        // Create smoothed interpolation for the draw path
    //        Vector3 to_draw_values = new Vector3((float)x_u, (float)y_u, 0.0f);
    //        Vector3 to_pass_values = new Vector3((float)x_value, (float)y_value, 0.0f);

    //        // Ensure a smooth transition by interpolating
    //        if (paths_draw.Count > 0)
    //        {
    //            Vector3 lastPoint = paths_draw[paths_draw.Count - 1];

    //            // Add interpolated points for smoother movement
    //            float smoothnessFactor = 10; // Number of interpolated points
    //            for (int i = 1; i <= smoothnessFactor; i++)
    //            {
    //                float t = i / smoothnessFactor;
    //                Vector3 interpolatedPoint = Vector3.Lerp(lastPoint, to_draw_values, t);
    //                paths_draw.Add(interpolatedPoint);
    //            }
    //        }

    //        // Add the latest point
    //        paths_draw.Add(to_draw_values);
    //        paths_pass.Add(to_pass_values);

    //        // Update LineRenderer with the smoothed paths
    //        lr.positionCount = paths_draw.Count;
    //        lr.SetPositions(paths_draw.ToArray());
    //        lr.useWorldSpace = true;

    //        // Adjust line rendering properties for smoother drawing
    //        lr.widthMultiplier = 0.1f; // Adjust thickness
    //        lr.numCapVertices = 5; // Makes line ends rounded for smoother effect
    //    }


}













