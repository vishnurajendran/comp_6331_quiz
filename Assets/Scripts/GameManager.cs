using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton 
    public static GameManager Instance;
    
    //Game states (see below)
    public GameState State;
    // This is to create events
    public static event Action<GameState> OnGameStateChanged;

    //number of units
    [SerializeField] private int rockUnits;
    [SerializeField] private int paperUnits;
    [SerializeField] private int scissorsUnits;
    
    public int[] units = new int[3];

    [Header("Simulation Settings")]
    [SerializeField, Range(1, 20)] private float timeScale=1;
    [SerializeField] private bool simulate = false;
    
    private static GameManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        
        //DontDestroyOnLoad(this.gameObject);
        Time.timeScale = timeScale;
        Instance = this;
    }
    
    private void Start()
    {
        //get all the number of units
        updateUnitNumbers();
        //set the game state
        UpdateGameState(GameState.FuzzyLogic);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReloadScene();
        }
    }

    public void UpdateGameState(GameState newState)
    {
        //we have 3 states in the game,
        State = newState;
        switch (newState)
        {
            //The game play
            case GameState.FuzzyLogic:
                HandleFuzzyLogic();
                break;
            //Decide the results and see if there is a winner. In this state, gameplay still goes on. 
            case GameState.Decide:
                HandleDecide();
                break;
            //The game has finished
            case GameState.Victory:
                HandleVictory();
                break;
            default:
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }


    private void HandleFuzzyLogic()
    {
        return;
    }


    private void HandleDecide()
    {
        updateUnitNumbers();
        //if one of the units reached to 0, then the game ends.
        if(!simulate)
            Debug.Log($"Units Rock: {units[0]}, Paper: {units[1]}, Scissors: {units[2]}");
        
        if (units.Min() == 0)
        {
            UpdateGameState(GameState.Victory);
        }
        else
        {
            UpdateGameState(GameState.FuzzyLogic);
        }
        
    }

    private void updateUnitNumbers()
    {
               
        // Get the number of each unit in the game
        rockUnits = GameObject.FindGameObjectsWithTag("Rock").Length;
        paperUnits = GameObject.FindGameObjectsWithTag("Paper").Length;
        scissorsUnits = GameObject.FindGameObjectsWithTag("Scissors").Length;

        //And set them as variables
        units[0] = rockUnits;
        units[1] = paperUnits;
        units[2] = scissorsUnits;
    }

    private void HandleVictory()
    {
        //When the game ends, all the objects freeze. 
        Time.timeScale = 0;
        if (simulate)
            ReloadScene();
        return;
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
}


// These are Game States. You can change them as you see appropriate. 

public enum GameState{
    FuzzyLogic,
    Decide,
    Victory
}