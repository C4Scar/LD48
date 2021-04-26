using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    public bool globalInteractionFlag;
    public bool pauseGame;
    public float tickTimer;
    public float tickTurnTimeLimit;
    public bool neutralTerritoryExists;
    public Territory[] territories; //set in inspector
    public Material neutralMaterial;
    public List<Player> playersWithBadRep;
    public List<Player> players; //set in inspector?
    public List<Territory> startingLocations; //set in inspector
    public List<Territory> neutralTerritory;
    public List<Territory> neutralSmallTerr;
    public List<Territory> neutralMedTerr;
    public List<Territory> neutralLargeTerr;
    public int numberOfTurnsPassed;

    public bool gameStartedBool;
    public GameObject mainMenuCanvas;
    public GameObject gameplayCanvas;
    public GameObject populationPrefab;
    public Transform populationPrefabsHolder;
    public GameObject reputationAlert;
    public GameObject tooStrongAlert;

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
    private void Start()
    {
        gameStartedBool = false;
        
}
// Update is called once per frame
void Update()
    {
        if (gameStartedBool)
        {
            tickTimer += Time.deltaTime;
            if (tickTimer >= tickTurnTimeLimit)
            {
                NewTurn();
                tickTimer = 0;
                numberOfTurnsPassed++;
            }
        }
    }
    public void NewTurn()
    {
        neutralTerritory.Clear();
        neutralSmallTerr.Clear();
        neutralMedTerr.Clear();
        neutralLargeTerr.Clear();
        Debug.Log("new turn");
        for (int j = 0; j < territories.Length; j++)
        {
            territories[j].TakeTurn();
        }
        Debug.Log("territories turns done");
        for(int i =0; i < players.Count; i++)
        {
            players[i].TakeTurn();
        }
        Debug.Log("Players turns done");
    }
    public void AllocateStartingTerritory()
    {
        for(int i = 0; i < players.Count; i++)
        {
            int randomNumber = Random.Range(0, startingLocations.Count);
            players[i].territoriesOwned.Add(startingLocations[randomNumber]);
            startingLocations[randomNumber].SetupNewOwner(players[i]);
            startingLocations.Remove(startingLocations[randomNumber]);
        }
    }
    public void StartGame()
    {
        mainMenuCanvas.SetActive(false);
        gameplayCanvas.SetActive(true);
        gameStartedBool = true;

        AllocateStartingTerritory();
        tickTurnTimeLimit = 3;
        PlayerUI.instance.UpdateSpeedUI();
        playersWithBadRep = new List<Player>();
        neutralTerritory = new List<Territory>();
        NewTurn();
    }
    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }
    public void GameOver()
    {

    }
}
