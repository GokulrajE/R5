
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XCharts.Runtime;

public class summarySceneHandler : MonoBehaviour
{
    public BarChart barchart;
    public string title;
    private ConcurrentQueue<System.Action> _actionQueue = new ConcurrentQueue<System.Action>();

    public void Start()
    {

        //MarsComm.OnButtonReleased += onMarsButtonReleased;
        // Inialize the logger
        AppLogger.StartLogging(SceneManager.GetActiveScene().name);
        AppLogger.SetCurrentScene(SceneManager.GetActiveScene().name);
        AppLogger.LogInfo($"{SceneManager.GetActiveScene().name} scene started.");
        title = "summary";
        initializeChart();
    }

    void Update()
    {
        while (_actionQueue.TryDequeue(out var action))
        {
            action.Invoke(); // Execute the action
        }

    }

    // To load the data for a specific movement into the bar graph.
    
    ////To disconnect the Robot 
    //public void onMarsButtonReleased()
    //{
    //    AppLogger.LogInfo("Mars button released.");
    //    // Enqueue the disconnect and quit actions
    //    _actionQueue.Enqueue(() =>
    //    {
    //        AppLogger.LogInfo("Disconnected form Mars And Application closed succesfully");
    //        AppData.Quit();
    //        #if UNITY_EDITOR
    //                    UnityEditor.EditorApplication.isPlaying = false; // Stop play mode if in editor
    //        #endif
    //    });
    //}

    //To initialize the barchart with whole data of moveTime per day
    public void initializeChart()
    {
  
        SessionDataHandler.MovTimePerDay();
        // Get or add the BarChart component
        barchart = gameObject.GetComponent<BarChart>();
        if (barchart == null)
        {
            barchart = gameObject.AddComponent<BarChart>();
            barchart.Init();
        }

        // Set chart title and tooltip visibility
        barchart.EnsureChartComponent<Title>().show = true;
        barchart.EnsureChartComponent<Title>().text = title;

        barchart.EnsureChartComponent<Tooltip>().show = true;
        barchart.EnsureChartComponent<Legend>().show = true;

        // Ensure x and y axes are created
        var xAxis = barchart.EnsureChartComponent<XAxis>();
        var yAxis = barchart.EnsureChartComponent<YAxis>();
        xAxis.show = true;
        yAxis.show = true;
        xAxis.type = Axis.AxisType.Category; // Set x-axis type to Category
        yAxis.type = Axis.AxisType.Value; // Set y-axis type to Value
        yAxis.min = 0; // Make sure bars start from the y=0 line
        yAxis.max = SessionDataHandler.moveTimeData.Max(); // You can adjust the maximum value as needed

        // Set zoom properties
        var dataZoom = barchart.EnsureChartComponent<DataZoom>();
        dataZoom.enable = true;
        dataZoom.supportInside = true;
        dataZoom.supportSlider = true;
        dataZoom.start = 0;
        dataZoom.end = 100;
        AppLogger.LogInfo("chart initialized successfully");
        UpdateChartData();
    }

    //To update chart with data
    public void UpdateChartData()
    {
        if (barchart == null)
        {
            
            return;
        }
        barchart.RemoveData();
        barchart.EnsureChartComponent<Title>().text = title;
        barchart.AddSerie<Bar>();
        // Update the x-axis data
        var xAxis = barchart.GetChartComponent<XAxis>();
        xAxis.data.Clear();
        foreach (string date in SessionDataHandler.dateData)
        {
            xAxis.data.Add(date); // Add x-axis labels (dates)
        }

        // Update the y-axis data (movement time)
        var yAxis = barchart.GetChartComponent<YAxis>();
        yAxis.data.Clear();
      
        for (int i = 0; i < SessionDataHandler.dateData.Length; i++)
        {
            float yValue = SessionDataHandler.moveTimeData[i];
            barchart.AddData(0, yValue);
        }
        barchart.RefreshAllComponent();
        AppLogger.LogInfo("chart updated successfully");
    }
    //private void OnDestroy()
    //{
    //    MarsComm.OnButtonReleased -= onMarsButtonReleased;
    //}
    public void quit()
    {
       
        AppData.Quit();
    }
    private void OnApplicationQuit()
    {

      AppData.Quit();
    }
}
