using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI instance;
    public TextMeshProUGUI playerPopulation;
    public TextMeshProUGUI playerWealth;
    public TextMeshProUGUI gameSpeed;
    public Slider[] reputations;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);//Note:this will make it also de-link from UI and other stuff
    }
    public void UpdateUI(Player player)
    {
        playerPopulation.text = $"{player.population}";
        playerWealth.text = $"{player.wealth}";
    }
    public void DecreaseSpeed()
    {
        if(GameMaster.instance.tickTurnTimeLimit < 5)
        {
            GameMaster.instance.tickTurnTimeLimit += 1;
        }
        UpdateSpeedUI();
    }
    public void IncreaseSpeed()
    {
        if (GameMaster.instance.tickTurnTimeLimit > 1)
        {
            GameMaster.instance.tickTurnTimeLimit -= 1;
        }
        UpdateSpeedUI();
    }
    public void UpdateSpeedUI()
    {
        gameSpeed.text = $"{6 - GameMaster.instance.tickTurnTimeLimit}";
    }
}
