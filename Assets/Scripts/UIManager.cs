using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiObj;
    [SerializeField] private TextMeshProUGUI _stateText;
    [SerializeField] private TextMeshProUGUI _statsText;
    [SerializeField] private TextMeshProUGUI _agroText;

    [SerializeField] private int rockUnits;
    [SerializeField] private int paperUnits;
    [SerializeField] private int scissorsUnits;
    
    public int[] units = new int[3];

    [TextArea(7,7)]
    [SerializeField] private string statText;

    [FormerlySerializedAs("agressiveText")] [TextArea] [SerializeField] private string aggressiveText;
    
    private static bool freshInstance = true;
    // Start is called before the first frame update
    void Start()
    {
        if (freshInstance)
        {
            freshInstance = false;
            PlayerPrefs.DeleteAll();
        }

        if (!_statsText)
            return;
        
        int games = PlayerPrefs.GetInt("games", 0);
        
        int rockLost = PlayerPrefs.GetInt("rock_lost", 0);
        int papersLost = PlayerPrefs.GetInt("papers_lost", 0);
        int scissorsLost = PlayerPrefs.GetInt("scissors_lost", 0);
        
        float rockWinPerc = ((float)rockLost / (games<=0?1:games)*100);
        float paperWinPerc = ((float)papersLost / (games<=0?1:games))*100;
        float scissorsWinPerc = ((float)scissorsLost / (games<=0?1:games))*100;

        var statString = String.Format(statText, games,rockLost, papersLost, scissorsLost, $"{(int)rockWinPerc}%", $"{(int)paperWinPerc}%",
            $"{(int)scissorsWinPerc}%");

        _statsText.text = statString;
        games += 1;
        PlayerPrefs.SetInt("games", games);
        PlayerPrefs.Save();
    }

    private void Update()
    {
        if(!_agroText)
            return;
        
        var rockAggro = GroupAI.GetGroupAIForType(GroupAI.AIof.Rocks).Aggressiveness;
        var papersAggro = GroupAI.GetGroupAIForType(GroupAI.AIof.Papers).Aggressiveness;
        var scissorsAggro = GroupAI.GetGroupAIForType(GroupAI.AIof.Scissors).Aggressiveness;

        var str = String.Format(aggressiveText, rockAggro, papersAggro, scissorsAggro);
        _agroText.text = str;
    }

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private bool once = true;
    private void GameManagerOnGameStateChanged(GameState state)
    {
        if (state == GameState.Victory)
        {
            if(!once)
                return;
            once = false;
            
            uiObj.gameObject.SetActive(true);
            var key = "";
            if (units[0] == GameObject.FindGameObjectsWithTag("Rock").Length)
            {
                key = "rock_lost";
                _stateText.text = "ROCK LOST";
            }
            else if (units[1] == GameObject.FindGameObjectsWithTag("Paper").Length)
            {
                key = "papers_lost";
                _stateText.text = "PAPER LOST";
            }
            else
            {
                key = "scissors_lost";
                _stateText.text = "SCISSORS LOST";
            }
            
            PlayerPrefs.SetInt(key,PlayerPrefs.GetInt(key, 0) + 1);
            PlayerPrefs.Save();
        }
    }


}
