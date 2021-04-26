using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerID;//set in inspector
    public float wealth;
    public bool isPlayer;
    public int population;
    public int reputation;
    public bool badReputation;
    public int resetTargetsTimer;
    public List<Territory> territoriesOwned;
    public Material playerMaterial;
    public List<Territory> territoriesToFocus;
    public bool tooStrongFlag;

    // Start is called before the first frame update
    void Start()
    {
        reputation = 100;
        wealth = 35;
        badReputation = false;
    }
    public void TakeTurn()
    {
        Debug.Log($"Player {playerID} turn");
        
        population = 0;
        for(int i = 0; i < territoriesOwned.Count; i++)
        {
            wealth += territoriesOwned[i].income;
            population += territoriesOwned[i].population;
        }
        reputation += 2;
        if (reputation >= 100)
        {
            reputation = 100;
            
        }
        if (reputation < 70)
        {
            badReputation = true;
            if (!GameMaster.instance.playersWithBadRep.Contains(this))
            {
                GameMaster.instance.playersWithBadRep.Add(this);
            }
        }
        else
        {
            badReputation = false;
            if (GameMaster.instance.playersWithBadRep.Contains(this))
            {
                GameMaster.instance.playersWithBadRep.Remove(this);
            }
        }
        PlayerUI.instance.reputations[playerID].value = reputation;
        if (isPlayer)
        {
            UpdateUI();
            if (reputation < 70)
            {
                GameMaster.instance.reputationAlert.SetActive(true);
            }
            else
            {
                GameMaster.instance.reputationAlert.SetActive(false);
            }
            if(population > 500 || wealth > 1000)
            {
                tooStrongFlag = true;
                GameMaster.instance.tooStrongAlert.SetActive(true);
            }
            else
            {
                tooStrongFlag = false;
                GameMaster.instance.tooStrongAlert.SetActive(false);

            }
        }
        //AI logic
        else
        {
            /*
             * HOW MANY ATTACKS TO DISTRIBUTE
             * divide your wealth by 5
             * fund rebels in that many territories
             * 
             * flip a coin about spending your reputation
             * divide your population by 10
             * divide that by 2
             * attack people with population
             * 
             * if there are neutral territories then fund rebels there
             * 
             * if any players have a bad reputation then attack them
             * 
             * if you are being attacked and losing then fight back
             * 
             * 
             * 
             * othewise 
             * 
             */
            //so that the AI does not endlessly attack something
            if(GameMaster.instance.numberOfTurnsPassed <= 1)
            {
                return;
            }
            resetTargetsTimer += 1;
            if (resetTargetsTimer >= 5)
            {
                territoriesToFocus.Clear();
                resetTargetsTimer = 0;
            }
            int numRebelsToFund = 0;
            int numOfInvasions = 0;
            if (Random.Range(0,5) == 3)
            {
                numOfInvasions = (int)(((int)population / 10)/2);
            }
            if (Random.Range(0, 5) == 3)
            {
                numRebelsToFund = (int)wealth / 5;
            }

            //if you do not have too many territories focused
            if (territoriesToFocus.Count <= 3)
            {
                //if there are players that have a bad reputation
                if (GameMaster.instance.playersWithBadRep.Count > 0 || GameMaster.instance.players[0].tooStrongFlag)
                {
                    if ((GameMaster.instance.playersWithBadRep.Contains(GameMaster.instance.players[0]) || GameMaster.instance.players[0].tooStrongFlag) && GameMaster.instance.players[0].territoriesOwned.Count > 0)
                    {
                        Debug.Log("Playerwithbadrap");
                        int randomPlayerTerr = Random.Range(0, GameMaster.instance.players[0].territoriesOwned.Count);
                        if (Random.Range(0, 2) == 1)
                        {
                            for (int r = 0; r < numRebelsToFund; r++)
                            {
                                GameMaster.instance.players[0].territoriesOwned[randomPlayerTerr].FundRebels(this);
                            }
                        }
                        else
                        {
                            for (int s = 0; s < numOfInvasions; s++)
                            {
                                reputation += 10;
                                GameMaster.instance.players[0].territoriesOwned[randomPlayerTerr].Attack(this);
                            }
                        }
                        territoriesToFocus.Add(GameMaster.instance.players[0].territoriesOwned[randomPlayerTerr]);
                        
                    }
                    else
                    {
                        //try to find players territories to attack based on population
                        int randomNum2 = Random.Range(0, GameMaster.instance.playersWithBadRep.Count);
                        int randomNum3 = Random.Range(0, GameMaster.instance.playersWithBadRep[randomNum2].territoriesOwned.Count);

                        if (Random.Range(0, 2) == 1)
                        {

                            for (int r = 0; r < numRebelsToFund; r++)
                            {
                                GameMaster.instance.playersWithBadRep[randomNum2].territoriesOwned[randomNum3].FundRebels(this);
                            }
                        }
                        else
                        {
                            for (int s = 0; s < numOfInvasions; s++)
                            {
                                reputation += 10;
                                GameMaster.instance.playersWithBadRep[randomNum2].territoriesOwned[randomNum3].Attack(this);
                            }
                        }
                        territoriesToFocus.Add(GameMaster.instance.playersWithBadRep[randomNum2].territoriesOwned[randomNum3]);
                    }
                }
                //if there are neutral territories
                else if (GameMaster.instance.neutralTerritoryExists && GameMaster.instance.neutralTerritory.Count >= 10)
                {
                    Debug.Log("playerswithoutbadwrap");
                    int randomNum = 0;
                    int territoriesSizeToHit = 0;
                    if (GameMaster.instance.neutralSmallTerr.Count > 0)
                    {
                        randomNum = Random.Range(0, GameMaster.instance.neutralSmallTerr.Count);
                        territoriesSizeToHit = 1;
                    }
                    else if (GameMaster.instance.neutralMedTerr.Count > 0)
                    {
                        randomNum = Random.Range(0, GameMaster.instance.neutralMedTerr.Count);
                        territoriesSizeToHit = 2;
                    }
                    else if (GameMaster.instance.neutralLargeTerr.Count > 0)
                    {
                        randomNum = Random.Range(0, GameMaster.instance.neutralLargeTerr.Count);
                        territoriesSizeToHit = 3;
                    }

                    /*if (randomNum == 0)
                    {
                        randomNum = Random.Range(0, GameMaster.instance.neutralTerritory.Count);
                    }*/

                    //try to find neutral territories to attack based on how much money you have
                    if (Random.Range(0, 2) == 1 || badReputation)
                    {
                        for (int r = 0; r < numRebelsToFund; r++)
                        {
                            if(territoriesSizeToHit == 1)
                            {
                                GameMaster.instance.neutralSmallTerr[randomNum].FundRebels(this);
                                territoriesToFocus.Add(GameMaster.instance.neutralSmallTerr[randomNum]);
                            }
                            else if(territoriesSizeToHit == 2)
                            {
                                GameMaster.instance.neutralMedTerr[randomNum].FundRebels(this);
                                territoriesToFocus.Add(GameMaster.instance.neutralMedTerr[randomNum]);
                            }
                            else if(territoriesSizeToHit == 3)
                            {
                                GameMaster.instance.neutralLargeTerr[randomNum].FundRebels(this);
                                territoriesToFocus.Add(GameMaster.instance.neutralLargeTerr[randomNum]);
                            }
                        }
                    }
                    else
                    {
                        for (int s = 0; s < numOfInvasions; s++)
                        {
                            if (territoriesSizeToHit == 1)
                            {
                                GameMaster.instance.neutralSmallTerr[randomNum].Attack(this);
                                territoriesToFocus.Add(GameMaster.instance.neutralSmallTerr[randomNum]);

                            }
                            else if (territoriesSizeToHit == 2)
                            {
                                GameMaster.instance.neutralMedTerr[randomNum].Attack(this);
                                territoriesToFocus.Add(GameMaster.instance.neutralMedTerr[randomNum]);

                            }
                            else if (territoriesSizeToHit == 3)
                            {
                                GameMaster.instance.neutralLargeTerr[randomNum].Attack(this);
                                territoriesToFocus.Add(GameMaster.instance.neutralLargeTerr[randomNum]);
                            }
                        }
                    }
                    //territoriesToFocus.Add(GameMaster.instance.neutralTerritory[randomNum]);
                }
                else
                {
                    int randomNum4 = Random.Range(0, GameMaster.instance.players.Count);

                    while (GameMaster.instance.players[randomNum4] == this)
                    {
                        randomNum4 = Random.Range(0, GameMaster.instance.players.Count);
                    }
                    int randomNum5 = Random.Range(0, GameMaster.instance.players[randomNum4].territoriesOwned.Count);

                    if (Random.Range(0, 2) == 1 || badReputation)
                    {
                        for (int r = 0; r < numRebelsToFund; r++)
                        {
                            GameMaster.instance.players[randomNum4].territoriesOwned[randomNum5].FundRebels(this);
                        }
                    }
                    else
                    {
                        for (int s = 0; s < numOfInvasions; s++)
                        {
                            GameMaster.instance.players[randomNum4].territoriesOwned[randomNum5].Attack(this);
                        }
                    }
                    territoriesToFocus.Add(GameMaster.instance.players[randomNum4].territoriesOwned[randomNum5]);
                }
            }
            else
            {
                int randomTarget = Random.Range(0, territoriesToFocus.Count);
                if(territoriesToFocus[randomTarget].owner == this)
                {
                    territoriesToFocus.RemoveAt(randomTarget);
                }
                else
                {
                    if (Random.Range(0, 2) == 1 || badReputation)
                    {
                        for (int q = 0; q < numRebelsToFund; q++)
                        {
                            territoriesToFocus[randomTarget].FundRebels(this);
                        }
                    }
                    else
                    {
                        for (int t = 0; t < numOfInvasions; t++)
                        {
                            territoriesToFocus[randomTarget].Attack(this);
                        }
                    }
                }
            }
        }
    }
    public void ReducePopulation(int value)
    {
        for(int i = 0; i< value; i++)
        {
            int randomNum = Random.Range(0, territoriesOwned.Count);
            while(!(territoriesOwned[randomNum].population > 0))
            {
                randomNum = Random.Range(0, territoriesOwned.Count);
            }
            territoriesOwned[randomNum].population -= 1;
            population -= 1;
        }
    }
    public void UpdateUI()
    {
        PlayerUI.instance.UpdateUI(this);
    }
}
