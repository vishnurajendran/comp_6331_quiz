using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[DefaultExecutionOrder(-99)]
public class GroupAI : MonoBehaviour
{
    public enum AIof
    {
        Papers,
        Scissors,
        Rocks
    };

    public AIof thisAI;

    [SerializeField] private float enemyNumbers;
    [SerializeField] private float targetNumbers;
    [SerializeField] private float friendlyNumbers;
    [SerializeField] private float aggressiveness;
    
    [SerializeField] private MinMax enemyThresholds = new MinMax(){Min = 1, Max = 6};
    [SerializeField] private MinMax targetThresholds = new MinMax(){Min = 3, Max = 8};
    [SerializeField] private MinMax friendsThresholds = new MinMax(){Min = 2, Max = 7};

    [SerializeField] private float maxAvgSpeed = 6;
    [SerializeField] private float maxCalmSpeed = 2;
    [SerializeField] private float maxAggroSpeed = 10;
    [SerializeField] private float maxSpeed = 10;
    
    private GameObject[] friends;
    
    private List<GameObject> targets;
    private List<GameObject> usedTargets;
    
    private static Dictionary<AIof, GroupAI> _instanceMap;

    public GameObject[] Friends => friends;

    public float Aggressiveness => aggressiveness;
    
    public static GroupAI GetGroupAIForType(AIof type)
    {
        if (_instanceMap == null || !_instanceMap.ContainsKey(type))
            return null;
        
        return _instanceMap[type];
    }
    
    private void Awake()
    {
        if (_instanceMap == null)
            _instanceMap = new Dictionary<AIof, GroupAI>();
        
        _instanceMap.Add(thisAI, this);

        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }
    
    private void OnDestroy()
    {
        _instanceMap.Remove(thisAI);
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState state)
    {
        if (state == GameState.FuzzyLogic)
        {
            if (thisAI == AIof.Scissors)
            {
                //if this code attached to the ScissorsAI, count number of game objects.
                friends = GameObject.FindGameObjectsWithTag("Scissors");
                friendlyNumbers = friends.Length;
                enemyNumbers = GameObject.FindGameObjectsWithTag("Rock").Length;
                targets = GameObject.FindGameObjectsWithTag("Paper").ToList();
                targetNumbers = targets.Count;

                //calculate aggresiveness
                aggressiveness = RunFuzzyLogic(friendlyNumbers, enemyNumbers, targetNumbers);

                //Assign aggressiveness to each unit in the fiction. 
                foreach (GameObject units in GameObject.FindGameObjectsWithTag("Scissors"))
                {
                    units.GetComponent<IndividualAI>().Agresiveness = aggressiveness;
                }
            }
            if (thisAI == AIof.Rocks)
            {
                //if this code attached to the RocksAI, count number of game objects.
                friends = GameObject.FindGameObjectsWithTag("Rock");
                friendlyNumbers = friends.Length;
                enemyNumbers = GameObject.FindGameObjectsWithTag("Paper").Length;
                targets = GameObject.FindGameObjectsWithTag("Scissors").ToList();
                targetNumbers = targets.Count;

                //calculate aggresiveness
                aggressiveness = RunFuzzyLogic(friendlyNumbers, enemyNumbers, targetNumbers);

                //Assign aggressiveness to each unit in the fiction. 
                foreach (GameObject units in GameObject.FindGameObjectsWithTag("Rock"))
                {
                    units.GetComponent<IndividualAI>().Agresiveness = aggressiveness;
                }
            }
            if (thisAI == AIof.Papers)
            {
                //if this code attached to the PaperAI, count number of game objects.
                friends = GameObject.FindGameObjectsWithTag("Paper");
                friendlyNumbers = friends.Length;
                enemyNumbers = GameObject.FindGameObjectsWithTag("Scissors").Length;
                targets = GameObject.FindGameObjectsWithTag("Rock").ToList();
                targetNumbers = targets.Count;

                //calculate aggresiveness
                aggressiveness = RunFuzzyLogic(friendlyNumbers, enemyNumbers, targetNumbers);

                //Assign aggressiveness to each unit in the fiction. 
                foreach (GameObject units in GameObject.FindGameObjectsWithTag("Paper"))
                {
                    units.GetComponent<IndividualAI>().Agresiveness = aggressiveness;
                }
            }

        }

    }

    // Start is called before the first frame update
    void Start()
    {
        //use if you need
    }

    // Update is called once per frame
    void Update()
    {
        //use if you need
    }

    private float RunFuzzyLogic(float numberOfFriendly, float numberOfEnemy, float numberOfTarget)
    {
        //First, fuzzyification. Here, we convert unit numbers values between 1 -0 and find membership values
        (float, float)[] degreesOfMemberships = Fuzzyification(numberOfFriendly, numberOfEnemy, numberOfTarget);
        //Then, using the degrreso of membership, we evaluate rules. You should get (2x2x2=8 trules) 
        float[] FuzzyOutput= RuleEvaluation(degreesOfMemberships);
        // finally, Defuzzification process, generates one output
        float crispOutput = Defuzzification(FuzzyOutput);

        return crispOutput;
    }


    // fill the codes below
    private (float Low, float High)[] Fuzzyification(float numberOfFriendly, float numberOfEnemy, float numberOfTarget)
    {
        (float,float)[] membershipFunctionResults = { Fuzzyification_friendly(numberOfFriendly), Fuzzyification_enemy(numberOfEnemy), Fuzzyification_target(numberOfTarget) };
                 
        return  membershipFunctionResults;
    }

    private (float Low, float High) Fuzzyification_friendly(float theNumberOfUnits)
    {
        float high = 0;
        float low = 0;
        if (theNumberOfUnits <= friendsThresholds.Min)
        {
            low = 1;
            high = 0;
        }
        else if (theNumberOfUnits >= friendsThresholds.Max)
        {
            high = 1;
            low = 0;
        }
        else
        {
            high = Mathf.Lerp(0,1, theNumberOfUnits/friendsThresholds.Max);
        }
        return new (0,0);
    }

    private (float Low, float High) Fuzzyification_enemy(float theNumberOfUnits)
    {
        float high = 0;
        float low = 0;
        if (theNumberOfUnits <= enemyThresholds.Min){
            low = 1;
            high = 0;
        }
        else if(theNumberOfUnits >= enemyThresholds.Max)
        {
            high = 1;
            low = 0;
        }
        else
        {
            high = Mathf.Lerp(0,1, theNumberOfUnits/enemyThresholds.Max);
            low = 1 - high;
        }
        return (low, high);
    }

    private (float Low, float High) Fuzzyification_target(float theNumberOfUnits)
    {
        float high = 0;
        float low = 0;
        if (theNumberOfUnits <= targetThresholds.Min){
            low = 1;
            high = 0;
        }
        else if(theNumberOfUnits >= targetThresholds.Max)
        {
            high = 1;
            low = 0;
        }
        else
        {
            high = Mathf.Lerp(0,1, theNumberOfUnits/targetThresholds.Max);
            low = 1 - high;
        }
        return (low, high);
    }

    private float[] RuleEvaluation ((float Low, float High)[] fuzzyInput)
    {
        float[] outputVariable = new float[4];
        var friendly = fuzzyInput[0];
        var enemy = fuzzyInput[1];
        var target = fuzzyInput[2];

        //average speeds
        outputVariable[0] += FuzzyAND(enemy.High, friendly.High) * maxAvgSpeed;
        outputVariable[0] += FuzzyOR(enemy.Low, target.High) * maxAvgSpeed;
        outputVariable[0] += FuzzyAND(FuzzyOR(target.High, friendly.High), FuzzyNOT(enemy.High))  * maxAvgSpeed;
        
        //calm speed
        outputVariable[1] += FuzzyOR(enemy.Low, target.Low) * maxCalmSpeed;
        outputVariable[1] += FuzzyOR(FuzzyOR(target.Low, friendly.Low), enemy.Low) * maxCalmSpeed;
        
        //aggressive
        outputVariable[2] += FuzzyAND(enemy.High, friendly.Low) * maxAggroSpeed;
        outputVariable[2] += FuzzyAND(FuzzyOR(target.High, friendly.High), FuzzyNOT(enemy.High)) * maxAggroSpeed;
        
        return outputVariable;
    }

    private float Defuzzification(float[] fuzzyResults)
    {
        float speedOfTheUnits=0.0f;
        speedOfTheUnits = Mathf.Clamp(Mathf.Max(fuzzyResults), 0, maxSpeed);
        return speedOfTheUnits;
    }

    float FuzzyAND(float val1, float val2)
    {
        return Mathf.Min(val1, val2);
    }
    
    float FuzzyOR(float val1, float val2)
    {
        return Mathf.Max(val1, val2);
    }

    float FuzzyNOT(float val)
    {
        return 1 - val;
    }

    public Transform GetClosestTarget()
    {
        Transform nextTraget = null;
        float min = Mathf.Infinity;

        foreach (var target in targets)
        {
            if (target == null)
                continue;
            
            var dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist < min)
            {
                min = dist;
                nextTraget = target.transform;
            }
        }

        if (nextTraget == null)
            return null;
        
        //targets.Remove(nextTraget.gameObject);
        return nextTraget;
    }
}
