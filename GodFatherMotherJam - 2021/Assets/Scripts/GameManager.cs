using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public UiManager UiManager;

    [Header("Objects")]
    public ObjectContainer ObjectsContainerScript;


    [Header("Timer")]
    public float GameDuration = 20;
    private float _currentTimer = 0;
    private bool _isGamePlaying = true;
    [System.Serializable]
    public struct DifficultyAccordingToTime
    {
        public float timeToChangeDifficulty;
        public int numberOfCharactersOnScreen;
        public float objectivesTimer;
        public float characterSpeed;
    }    [Header("DifficultyLevels")]
    public DifficultyAccordingToTime[] difficultyLevels;    private int _currentDifficultyLevel;

    [Header("Objectives")]
    public int numberOfStartObjectives = 4;
    public float timeToAddToAddedObjectivesCharacters = 5;
    private List<int> _objectiveList = new List<int>();
    private List<ObjectWithCharacter> _objectIndexAvailable = new List<ObjectWithCharacter>();

    private List<int> objectsAvailableForCharactersList = new List<int>();



    private struct ObjectWithCharacter
    {
        public Character character;
        public int ObjectIndex;
    }

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

    [Header("On Fail Click")]
    public float _timeClickInteractionIsDisabledOnFail = 2;
    private bool _disableObjectInteraction = false;



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
        _currentTimer = GameDuration;

        int numberOfObjectsTotal = ObjectsContainerScript.objet.Length;
        for (int i = 0; i < numberOfObjectsTotal; i++)
        {
            objectsAvailableForCharactersList.Add(ObjectsContainerScript.objet[i].index);
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
    #region character
    private void SpawnCharactersTimer()
    {
        if (_characterList.Count >= maxCharactersOnScene || !_isGamePlaying) return;


        characterSpawnTimer += Time.deltaTime;
        if(_start)
        {
            if(characterSpawnTimer >= timeBetweenStartCharacterSpawns)
            {
                SpawnCharacter();
                ++_numberOfCharactersSpawnedAtStart;
                if(_numberOfCharactersSpawnedAtStart >= numberOfCharactersToSpawnAtStart)
                {
                    _start = false;

                    SetupObjectiveList();
                }
                characterSpawnTimer = 0;
            }
        }
        else if (characterSpawnTimer >= TimeBetweenCharacter)
        {
            SpawnCharacter();
            characterSpawnTimer = 0;
        }
    }

    private void SpawnCharacter()
    {
        //a changer, on utilisera pas de prefabs, a la place, il va falloir faire du random pour d�terminer les diff�rentes parties du visage du character
        int random = Random.Range(0, charactersPrefabs.Length);


        int randomPos = Random.Range(0, placesToInstantiateCharacters.Length);
        Character newChar = Instantiate(charactersPrefabs[random], placesToInstantiateCharacters[randomPos].position, Quaternion.identity, charactersHolder).GetComponent<Character>();
        newChar.SetupCharacter(patrolPoints);

        //get a random index from the available object index list
        random = Random.Range(0, objectsAvailableForCharactersList.Count);



        //setup char associated Object
        ObjectContainer.ObjectStruct objectToUse = ObjectsContainerScript.GetObjectWithIndex(objectsAvailableForCharactersList[random]);
        //remove that index until it is not on the scene anymore
        objectsAvailableForCharactersList.RemoveAt(random);



        newChar.associatedObject.SetupObject(objectToUse.sprite, objectToUse.index);

        //subscribe Remove char from list to an event that is launched when the character disappear
        newChar.OnCharacterDisappearance += RemoveCharFromList;

        _characterList.Add(newChar);


        ObjectWithCharacter newObjectWithChar;
        newObjectWithChar.ObjectIndex = objectToUse.index;  
        newObjectWithChar.character = newChar;  
        _objectIndexAvailable.Add(newObjectWithChar);
    }



    public void RemoveCharFromList(Character newChar)
    {
        _characterList.Remove(newChar);
        objectsAvailableForCharactersList.Add(newChar.associatedObject.id);

    }
    #endregion

    #region objective
    public void CheckObject(int idToCheck)
    {
        if(_objectiveList.Contains(idToCheck))
        {
            UiManager.SeekIndexAndSetNewObjectives(idToCheck);

            //add score --> ScoreManager ?
        }
        else
        {
            //make people unhappy
            _disableObjectInteraction = true;
            Debug.Log("Stop Interaction");
            StartCoroutine(RestoreClickInteractionAfterTime());
        }
    }

    private void SetupObjectiveList()
    {
        for (int i = 0; i < numberOfStartObjectives; i++)
        {
            int objectId = AddNewElementToObjectiveList();
            UiManager.AddImageToObjectiveUi(objectId);
        }
    }

    public int RemoveObjectiveAndGetANewOne(int idToCheck)
    {
        _objectiveList.Remove(idToCheck);

        return AddNewElementToObjectiveList();
    }

    public int AddNewElementToObjectiveList()
    {

        List<ObjectWithCharacter> objWithChar = new List<ObjectWithCharacter>();

        int indexAvailableListCount = _objectIndexAvailable.Count;
        for (int i = indexAvailableListCount - 1; i >= 0; i--)
        {
            if (!_objectiveList.Contains(_objectIndexAvailable[i].ObjectIndex)) continue;

            objWithChar.Add(_objectIndexAvailable[i]);
            _objectIndexAvailable.Remove(_objectIndexAvailable[i]);
        }

        int random = Random.Range(0, _objectIndexAvailable.Count);


        indexAvailableListCount = objWithChar.Count;
        for (int i = 0; i < indexAvailableListCount; i++)
        {
            _objectIndexAvailable.Add(objWithChar[i]);
        }

        _objectiveList.Add(_objectIndexAvailable[random].ObjectIndex);

        _objectIndexAvailable[random].character.AddTimeToStayOnScreenTimer(timeToAddToAddedObjectivesCharacters);
        _objectIndexAvailable.RemoveAt(random);

        return _objectiveList[_objectiveList.Count - 1];
    }
    #endregion

    #region mouseInteraction
    private IEnumerator RestoreClickInteractionAfterTime()
    {
        yield return new WaitForSeconds(_timeClickInteractionIsDisabledOnFail);
        Debug.Log("Resume Interaction");
        _disableObjectInteraction = false;
    }

    public bool GetDisableObjectInteraction()
    {
        return _disableObjectInteraction;
    }
    #endregion

    #region gameFinished
    private void GameFinished()
    {
        _isGamePlaying = false;
        Debug.Log("Show End Screen here");
        //add end screen behaviour here
    }

    public bool GetIsGamePlaying()
    {
        return _isGamePlaying;
    }
    #endregion

    public float GetChrono()
    {
        return _currentTimer;
    }

    public DifficultyAccordingToTime GetCurrentDifficultyLevel()
    {
        return difficultyLevels[_currentDifficultyLevel];
    }

}
