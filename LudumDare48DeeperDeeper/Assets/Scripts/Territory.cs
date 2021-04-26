using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Territory : MonoBehaviour
{
    public int id;
    public int population;
    public int populationCap;
    public float income;
    public int[] attackers;
    public int numberOfAllies;
    public int numberOfDefenders;
    public bool attacked;
    public int attackersLeft;
    public int attackersLeftID;
    public int defendersLeft;
    public int smallMedLarge; //1 small, 2 med, 3 large
    public Player currentWinner;

    public SpriteRenderer checkers1;
    public SpriteRenderer checkers2;

    public Image checkers1Slidecolor;
    public Image checkers2Slidecolor;
    public Slider fightIndication;

    public Material material;
    public Renderer territoryRenderer;
    public GameObject selectionIndicator;
    public Player owner;
    public GameObject[] populationCapsulesPoints;
    public List<GameObject> populationCapsules;

    // Start is called before the first frame update
    void Start()
    {
        populationCap = ((int)this.transform.localScale.x * (int)this.transform.localScale.z) + 10;
        population = populationCap;
        income = population * 0.2f;
        populationCapsules = new List<GameObject>();
    }
    public void SetupNewOwner(Player player)
    {
        owner = player;
        material = player.playerMaterial;
        territoryRenderer.material = material;
    }
    public void SetupConqueror(Player player)
    {
        if(owner != null)
        {
            owner.territoriesOwned.Remove(this);
        }
        
        owner = player;
        owner.territoriesOwned.Add(this);
        material = player.playerMaterial;
        territoryRenderer.material = material;
        fightIndication.gameObject.SetActive(false);
    }
    public void FundRebels(Player player)
    {
        if(player.wealth >= 5)
        {
            attackers[player.playerID] += 3;
            player.wealth -= 5;
            attacked = true;
            if(player.playerID == 0)
            {
                player.UpdateUI();
            }
        }
        SetCheckersPattern();
    }
    public void Attack(Player player)
    {
        if(player.population >= 10)
        {
            attackers[player.playerID] += 10;
            player.reputation -= 10;
            player.ReducePopulation(10);
            attacked = true;
            if (player.playerID == 0)
            {
                player.UpdateUI();
            }
            PlayerUI.instance.reputations[player.playerID].value = player.reputation;
        }
        SetCheckersPattern();
    }
    public void SetCheckersPattern()
    {
        Debug.Log($"Set checkers");
        int mostStrengthID = 0;
        int mostStrength = 0;
        int secondMostStrengthID = 0;
        int secondMostStrength = 0;
        fightIndication.maxValue = 0;
        for (int i = 0; i < attackers.Length; i++)
        {
            fightIndication.maxValue += attackers[i];
            if(attackers[i] > mostStrength)
            {
                mostStrengthID = i;
                mostStrength = attackers[i];
            }
            else if(attackers[i] > secondMostStrength)
            {
                secondMostStrengthID = i;
                secondMostStrength = attackers[i];
            }
        }
        fightIndication.maxValue += population;
        if(mostStrength > 0)
        {
            fightIndication.gameObject.SetActive(true);
            fightIndication.value = mostStrength;
            checkers1.color = GameMaster.instance.players[mostStrengthID].playerMaterial.color;
            checkers1Slidecolor.color = checkers1.color;
            checkers1.gameObject.SetActive(true);
        }
        else
        {
            fightIndication.gameObject.SetActive(false);
            checkers1.gameObject.SetActive(false);
        }
        if(secondMostStrength > 0)
        {
            checkers2.color = GameMaster.instance.players[secondMostStrengthID].playerMaterial.color;
            checkers2.gameObject.SetActive(true);
            checkers2Slidecolor.color = checkers2.color;
        }
        else
        {
            checkers2.gameObject.SetActive(false);
            if(owner != null)
            {
                checkers2Slidecolor.color = owner.playerMaterial.color;
            }
            else
            {
                checkers2Slidecolor.color = GameMaster.instance.neutralMaterial.color;
            }
        }
    }
    public void SetUIValues()
    {

    }
    public void TakeTurn()
    {
        if(population < populationCap)
        {
            //population += (1 + (int)((float)((population+1) / (float)populationCap) * 10));
            population += 2;
        }
        else if(population <= 0)
        {
            population = 1;
        }
        //income = population * 0.2f;
        income = 1;
        if(owner == null)
        {
            GameMaster.instance.neutralTerritoryExists = true;
            GameMaster.instance.neutralTerritory.Add(this);
            if (smallMedLarge == 1)
            {
                GameMaster.instance.neutralSmallTerr.Add(this);
            }
            else if (smallMedLarge == 2)
            {
                GameMaster.instance.neutralMedTerr.Add(this);
            }
            else if (smallMedLarge == 3)
            {
                GameMaster.instance.neutralLargeTerr.Add(this);
            }
        }
        SetPopulationCapsules();
        attackersLeft = 0;
        defendersLeft = 0;
        attackersLeftID = 0;
        if (attacked)
        {
            for (int i = 0; i < attackers.Length; i++)
            {
                if (attackers[i] >= 1)
                {
                    attackers[i] -= (int)(attackers[i]/5.0) + 1;
                    if (attackers[i] >= 1)
                    {
                        attackersLeft += 1;
                        attackersLeftID = i;
                    }
                }
            }
            if (population > 1 && attackersLeft == 1)
            {
                //probbably need to make the player population reduce as well
                //int tempPopulation = population;
                population -= (int)(attackers[attackersLeftID]/3.0) + 2;
                attackers[attackersLeftID] = (int)((attackers[attackersLeftID] * 1.25) + 1.25);//do this to negate the natural attrition when battling another attacker
                attackers[attackersLeftID] -= 2;//-1 for a minimum amount of attrition
                if(attackers[attackersLeftID] < 0)
                {
                    attackers[attackersLeftID] = 0;
                }
                if(population > 1)
                {
                    defendersLeft += 1;
                }
                else if(population < 0)
                {
                    population = 1;
                }
            }
            if(attackersLeft == 1 && defendersLeft == 0)
            {
                if(GameMaster.instance.players[attackersLeftID] != owner)
                {
                    SetupConqueror(GameMaster.instance.players[attackersLeftID]);
                    population += attackers[attackersLeftID];
                    attackers[attackersLeftID] = 0;
                }
                else
                {
                    population += attackers[attackersLeftID];
                    attackers[attackersLeftID] = 0;
                }
            }
            if(attackersLeft == 0)
            {
                attacked = false;
                fightIndication.gameObject.SetActive(false);
            }
        }
        GameMaster.instance.players[0].UpdateUI();
        SetCheckersPattern();
    }
    public void SetPopulationCapsules()
    {
        Debug.Log("setupCapsules");
        int numberOfCapsules = (int)(population / 5.0);
        if(numberOfCapsules > 13)
        {
            numberOfCapsules = 13;
        }
        if (populationCapsules.Count < numberOfCapsules)
        {
            for (int j = populationCapsules.Count; j < numberOfCapsules; j++)
            {
                GameObject capsule = Instantiate(GameMaster.instance.populationPrefab, populationCapsulesPoints[j].transform.position, Quaternion.identity, GameMaster.instance.populationPrefabsHolder);
                capsule.transform.localPosition = new Vector3(capsule.transform.position.x, 0, capsule.transform.position.z);
                populationCapsules.Add(capsule);
            }
        }
        else if(populationCapsules.Count > numberOfCapsules)
        {
            for (int i = populationCapsules.Count - 1; i > numberOfCapsules; i--)
            {
                Destroy(populationCapsules[i]);
                populationCapsules.RemoveAt(i);
            }
        }
        
    }

}
