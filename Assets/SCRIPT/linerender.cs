//using UnityEngine;
//using System.Collections.Generic;

//public class LineDrawer : MonoBehaviour
//{
//    public RectTransform whiteImage;  // Reference to the WhiteImage
//    private LineRenderer lineRenderer;
//    private List<Vector3> points = new List<Vector3>();
//    private float realBoardWidth = 40f;  // 3.33 feet in inches
//    private float realBoardHeight = 24f;  // 2 feet in inches

//    void Start()
//    {
//        // Get the LineRenderer component
//        lineRenderer = GetComponent<LineRenderer>();
//        lineRenderer.positionCount = 0;
//        lineRenderer.startWidth = 0.01f;  // Adjust for line thickness
//        lineRenderer.endWidth = 0.01f;
//        lineRenderer.useWorldSpace = false;

//        // Start generating random sensor data every 1 second
//        InvokeRepeating("GenerateRandomSensorData", 0f, 1f);  // Generate data every 1 second
//    }

//    // Function to generate random sensor data and draw the line
//    void GenerateRandomSensorData()
//    {
//        // Generate random sensor data within the real board size
//        float randomSensorX = Random.Range(0f, realBoardWidth);  // Random X within board width
//        float randomSensorY = Random.Range(0f, realBoardHeight); // Random Y within board height

//        // Call the DrawLine method to add the random data point
//        DrawLine(randomSensorX, randomSensorY);
//        Debug.Log(randomSensorX + ", " + randomSensorY);
//    }

//    public void DrawLine(float sensorX, float sensorY)
//    {
//        // Normalize sensor data to image size
//        float canvasWidth = whiteImage.rect.width;
//        float canvasHeight = whiteImage.rect.height;

//        float normalizedX = (sensorX / realBoardWidth) * canvasWidth;
//        float normalizedY = (sensorY / realBoardHeight) * canvasHeight;

//        // Clamp to canvas boundaries
//        normalizedX = Mathf.Clamp(normalizedX, 0, canvasWidth);
//        normalizedY = Mathf.Clamp(normalizedY, 0, canvasHeight);

//        // Add point to line
//        points.Add(new Vector3(normalizedX, normalizedY, 0));

//        // Update LineRenderer
//        lineRenderer.positionCount = points.Count;
//        lineRenderer.SetPositions(points.ToArray());
//    }
//}
using UnityEngine;
using System.Collections.Generic;

public class LineRendererTest : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();
    private RectTransform drawArea; // Reference to the RectTransform of the drawArea
    private float realBoardWidth = 86f;  // Real-world width (inches or cm)
    private float realBoardHeight = 80f; // Real-world height (inches or cm)

    void Start()
    {

        // Get the LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is missing on the GameObject!");
            return;
        }

        // Get the RectTransform of the drawArea
        drawArea = GetComponent<RectTransform>();
        if (drawArea == null)
        {
            Debug.LogError("RectTransform is missing on the drawArea GameObject!");
            return;
        }

        // LineRenderer setup
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 5f;
        lineRenderer.endWidth = 5f;
        lineRenderer.useWorldSpace = false;

        // Generate random points
        //InvokeRepeating("GenerateRandomPoint", 0f, 0.5f);
    }
    private void Update()
    {
        // Normalize the sensor data to the drawArea size
        float normalizedX = (R5comm.x ) * drawArea.rect.width;
        float normalizedY = (R5comm.y ) * drawArea.rect.height;

        // Clamp the values to stay within the drawArea
        normalizedX = Mathf.Clamp(normalizedX, 0, drawArea.rect.width);
        normalizedY = Mathf.Clamp(normalizedY, 0, drawArea.rect.height);

        // Convert to local coordinates relative to drawArea
        Vector3 localPoint = new Vector3(normalizedX - drawArea.rect.width / 2,
                                         normalizedY - drawArea.rect.height / 2, 0f);

        points.Add(localPoint);

        // Update the LineRenderer
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());

        Debug.Log($"New Point Added: {localPoint}");
    }

    void GenerateRandomPoint()
    {
        // Generate random sensor data within the real-world board dimensions
        float randomX = Random.Range(0f, realBoardWidth);
        float randomY = Random.Range(0f, realBoardHeight);

        // Normalize the sensor data to the drawArea size
        float normalizedX = (randomX / realBoardWidth) * drawArea.rect.width;
        float normalizedY = (randomY / realBoardHeight) * drawArea.rect.height;

        // Clamp the values to stay within the drawArea
        normalizedX = Mathf.Clamp(normalizedX, 0, drawArea.rect.width);
        normalizedY = Mathf.Clamp(normalizedY, 0, drawArea.rect.height);

        // Convert to local coordinates relative to drawArea
        Vector3 localPoint = new Vector3(normalizedX - drawArea.rect.width / 2,
                                         normalizedY - drawArea.rect.height / 2, 0f);

        points.Add(localPoint);

        // Update the LineRenderer
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());

        Debug.Log($"New Point Added: {localPoint}");
    }
}

//using UnityEngine;
//using System.Collections.Generic;

//public class LineRendererOnSeparateCanvas : MonoBehaviour
//{
//    public RectTransform lineRendererCanvas; // Reference to the new Canvas RectTransform
//    public RectTransform drawArea;          // White rectangle for drawing area
//    public LineRenderer lineRenderer;       // LineRenderer component

//    private List<Vector3> points = new List<Vector3>();

//    private float canvasWidth;
//    private float canvasHeight;

//    void Start()
//    {
//        // Get the size of the new canvas
//        canvasWidth = lineRendererCanvas.rect.width;
//        canvasHeight = lineRendererCanvas.rect.height;
//        lineRenderer = GetComponent<LineRenderer>();
//        if (lineRenderer == null)
//        {
//            Debug.LogError("LineRenderer is missing on the drawArea GameObject!");
//            return;
//        }

//        // Initialize the LineRenderer
//        lineRenderer.positionCount = 0;
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Space)) // Press space to add random points
//        {
//            GenerateRandomPoint();
//        }
//    }

//    void GenerateRandomPoint()
//    {
//        // Generate random positions within the canvas dimensions
//        float randomX = Random.Range(0f, canvasWidth);
//        float randomY = Random.Range(0f, canvasHeight);

//        // Convert to world space based on the lineRendererCanvas
//        Vector3 localPoint = new Vector3(randomX, randomY, 0f);
//        Vector3 worldPoint = lineRendererCanvas.TransformPoint(localPoint);

//        // Add the point to the list
//        points.Add(worldPoint);

//        // Update the LineRenderer
//        lineRenderer.positionCount = points.Count;
//        lineRenderer.SetPositions(points.ToArray());

//        Debug.Log($"Point Added: {worldPoint}");
//    }
//}


