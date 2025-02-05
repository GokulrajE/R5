using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyColumnPool : MonoBehaviour
{
    enum AssessStates
    {
        DAY = 1,
        EVE = 2,
        NIGHT = 3
    };

    public int _state; 
    public static FlappyColumnPool instance;
    public int columnPoolSize = 5;
    private GameObject[] columns;
    public GameObject[] columnPrefab;
    public GameObject[] backgrounds;
    public float columnMin = -5.3f;
    public float ColumnMax = 1.3f;
    private Vector2 objectPoolPosition = new Vector2(-15,-25);
    private float timeSinceLastSpawn = 3;
    //public float spawnRate = 10f;
    private float spawnXposition = 16;
    private int CurrentColumn = 0;
    private GameObject[] top;
    private GameObject[] bottom;


    //public  int difficultyLevel =10;
    bool setup;

    float prevSpawnTime;
    // Variables to track number of columns spawned for each level
    private int columnsSpawnedLevel1 = 0;
    private int columnsSpawnedLevel2 = 0;
    // Flag to pause spawning
    private bool pauseSpawning = false;
   // public float initialDelay = 3f; // Initial delay before starting to spawn columns

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
       // _state = 0;

        setup = false;

        //timeSinceLastSpawn = 0;

    }

    // Update is called once per frame
    void Update()
    {
        prevSpawnTime += Time.deltaTime;
        if (!setup)
        {
            // Debug.Log(AppData.startGameLevel);
            //_state = (AppData.startGamePerformace)>0? AppData.startGamePerformace: 0;
           
            //FlappyColumnPool.instance.difficultyLevel = FlappyGameControl.instance.startGameLevelRom;
            columns = new GameObject[columnPoolSize];
            for (int i = 0; i < columnPoolSize; i++)
            {
                columns[i] = (GameObject)Instantiate(columnPrefab[_state], objectPoolPosition, Quaternion.identity);
            }
            top = GameObject.FindGameObjectsWithTag("Top");

            chooseBackground();
            //FlappyColumnPool.instance.difficultyLevel = 6;

            setup = true;
            // Start the coroutine to spawn columns after the initial delay
           // StartCoroutine(SpawnColumnsWithDelay());
        }
        
     
    }

    public void chooseBackground()
    {
        foreach (GameObject obj in backgrounds)
        {
            obj.SetActive(false);
        }
        backgrounds[_state].SetActive(true);
    }
    //private IEnumerator SpawnColumnsWithDelay()
    //{
    //    // Wait for the initial delay
    //    yield return new WaitForSeconds(initialDelay);

    //    // Start the regular spawning process
    //    while (!FlappyGameControl.instance.gameOver)
    //    {
    //        spawnColumn();
    //        yield return new WaitForSeconds(FlappyGameControl.instance.columnSpawnRate);
    //    }
    //}
    public void spawnColumn()
    {
        FlappyGameControl.instance.RandomAngle();
        //Check Game State and Spawn Timing:
        if (!FlappyGameControl.instance.gameOver && prevSpawnTime > FlappyGameControl.instance.columnSpawnRate)
        {
            //Reset Spawn Timers:
            prevSpawnTime = 0;
            timeSinceLastSpawn = 0;
            //Determine Spawn Position:
            float spawnYposition = Random.Range(columnMin, ColumnMax);

            //Debug.Log(CurrentColumn);
            //Set Column Position and Tag:
            columns[CurrentColumn].transform.position = new Vector2(BirdControl.rb2d.transform.position.x + spawnXposition, FlappyGameControl.instance.ypos);
            columns[CurrentColumn].tag = "Target";
            //Update Tags for Previous Column:
            if (CurrentColumn == 0)
            {
                columns[columnPoolSize - 1].tag = "Untagged";
            }
            else
            {
                columns[CurrentColumn - 1].tag = "Untagged";

            }
            //Increment Current Column Index:
            //FB_spawnTargets.instance.trailDuration = Mathf.Clamp((BirdControl.rb2d.transform.position.x + -columns[CurrentColumn].transform.position.x) / FlappyGameControl.instance.scrollSpeed,2,4);
            CurrentColumn += 1;
            if (CurrentColumn >= columnPoolSize)
            {
                CurrentColumn = 0;
            }
            // Call ColumnSpawned method
            //Notify Game Controller:
            FlappyGameControl.instance.ColumnSpawned();

           
        }
    }
}
