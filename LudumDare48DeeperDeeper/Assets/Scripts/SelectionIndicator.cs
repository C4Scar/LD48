using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionIndicator : MonoBehaviour
{
    public static SelectionIndicator instance;

    public Image ownerColor;
    public TextMeshProUGUI population;
    public TextMeshProUGUI income;

    public TextMeshProUGUI[] troopCounts;

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

    public void UpdateHoverUI(GameObject hoveredObject)
    {
        Territory hoveredTerritory = hoveredObject.GetComponent<Territory>();

        //this.gameObject.transform.position = hoveredObject.transform.position + new Vector3(0,2,0);
        population.text = $"{hoveredTerritory.population} / {hoveredTerritory.populationCap}";
        income.text = $"{hoveredTerritory.income}";
        if(hoveredTerritory.owner != null)
        {
            ownerColor.color = hoveredTerritory.owner.playerMaterial.color;
        }
        else
        {
            ownerColor.color = GameMaster.instance.neutralMaterial.color;
        }
        for (int i = 0; i < troopCounts.Length; i++)
        {
            troopCounts[i].text = $"{hoveredTerritory.attackers[i]}";
        }
    }
}
