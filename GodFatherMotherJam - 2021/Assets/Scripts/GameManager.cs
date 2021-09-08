using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Objects")]
    public ObjectContainer ObjecsContainerScript;


    [Header("Timer")]
    public float GameDuration = 20;
    private float _currentTimer = 0;
    private bool _isGamePlaying = true;

    [Header("Objectives")]
    public int numberOfStartObjectives = 4;
    private List<int> _objectiveList = new List<int>();

    [Header("Characters")]
    public GameObject[] charactersPrefabs;
    public Transform[] placesToInstantiateCharacters;
    public PatrolPoint[] patrolPoints;
    public Transform charactersHolder;
    private List<Character> _characterList = new List<Character>();

    [Header("StartSettings")]
    public int numberOfCharactersToSpawnAtStart = 5;
    public float timeBetweenStartCharacterSpawns = 0.2f;
    private int _numberOfCharactersSpawnedAtStart = 0;
    private bool _start = true;

    [Header("Characters Spawns")]
    public float TimeBetweenCharacter = 1;
    public int maxCharactersOnScene = 5;

    private float characterSpawnTimer = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupObjectiveList();
        _currentTimer = GameDuration;
    }

    private void SetupObjectiveList()
    {
        for (int i = 0; i < numberOfStartObjectives; i++)
        {
            int random = Random.Range(0, ObjecsContainerScript.objet.Length);
            _objectiveList.Add(random);
            //UI manager gets sprite form the random int calculated;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ReduceGameTimer();
        SpawnCharactersTimer();
    }

    private void ReduceGameTimer()
    {
        if (!_isGamePlaying) return;

        _currentTimer -= Time.deltaTime;
        if(_currentTimer <= 0)
        {
            GameFinished();
        }
    }

    private void SpawnCharactersTimer()
    {
        if (_characterList.Count >= maxCharactersOnScene) return;


        characterSpawnTimer += Time.deltaTime;
        if(_start)
        {
            if(characterSpawnTimer >= timeBetweenStartCharacterSpawns)
            {
                SpawnCharacter();
                if(_numberOfCharactersSpawnedAtStart >= numberOfCharactersToSpawnAtStart)
                {
                    _start = false;
                }
            }
        }
        else if (characterSpawnTimer >= TimeBetweenCharacter)
        {
            SpawnCharacter();
        }
    }

    private void SpawnCharacter()
    {
        int random = Random.Range(0, charactersPrefabs.Length);
        int randomPos = Random.Range(0, placesToInstantiateCharacters.Length);
        Character newChar = Instantiate(charactersPrefabs[random], placesToInstantiateCharacters[randomPos].position, Quaternion.identity, charactersHolder).GetComponent<Character>();
        newChar.SetupCharacter(patrolPoints);

        random = Random.Range(0, ObjecsContainerScript.objet.Length);
        ObjectContainer.ObjectStruct objectToUse = ObjecsContainerScript.GetObjectWithIndex(random);
        newChar.associatedObject.SetupObject(objectToUse.sprite, objectToUse.index);
        newChar.OnCharacterDisappearance += RemoveCharFromList;

        _characterList.Add(newChar);
    }

    public void RemoveCharFromList(Character newChar)
    {
        _characterList.Remove(newChar);
    }

    private void GameFinished()
    {
        _isGamePlaying = false;
        Debug.Log("Show End Screen here");
        //add end screen behaviour here
    }

    public void CheckObject(int idToCheck)
    {
        if(_objectiveList.Contains(idToCheck))
        {
            //remove id and object from objective list
            //add score --> ScoreManager ?
            //add new object to objective list? (check if it's by timer or just when one has been removed)
        }
    }
}
