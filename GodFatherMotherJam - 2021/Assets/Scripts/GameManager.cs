using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UiManager UiManager;
    public ScoreManager ScoreManager;

    [Header("Objects")]
    public ObjectContainer ObjectsContainerScript;


    [Header("Timer")]
    public float GameDuration = 20;
    private float _currentTimer = 0;
    private bool _isGamePlaying = true;

    [System.Serializable]
    public struct DifficultyAccordingToTime
    {
        public float timeToChangeDifficulty; //done
        public int numberOfCharactersOnScreen; //done
        public float timeBetweenCharactersSpawns; //done
        public int numbersOfObjectives; //done
        public float objectivesTimer; //done
        public float characterSpeed; //done
        public float characterAnimatorSpeed; //done

        public float scoreMultiplier; // done

        public UnityEvent eventOnEnterDifficulty; //done
    }

    [Header("DifficultyLevels")]
    public DifficultyAccordingToTime[] difficultyLevels;
    private int _currentDifficultyLevel = 0;


    [Header("Objectives")]
    private int numberOfObjectives = 0;
    public float timeToAddToAddedObjectivesCharacters = 5;
    private List<int> _objectiveList = new List<int>();
    private List<ObjectWithCharacter> _objectIndexAvailable = new List<ObjectWithCharacter>();

    private List<int> objectsAvailableForCharactersList = new List<int>();
    private bool _isObjectiveSetupInGame = false;


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
    public float timeToWaitToMakeObjectiveAppear = 2;

    [Header("Characters Spawns")]
    private float timeBetweenCharacter = 1;
    private int maxCharactersOnScene = 5;

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
        OnDifficultyChange();

        int numberOfObjectsTotal = ObjectsContainerScript.objet.Length;
        for (int i = 0; i < numberOfObjectsTotal; i++)
        {
            objectsAvailableForCharactersList.Add(ObjectsContainerScript.objet[i].index);
        }

        StartCoroutine(ShowObjectiveListAfterDelay());
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

        CheckForDifficulty();
        if(_currentTimer <= 0)
        {
            GameFinished();
        }
    }

    private void CheckForDifficulty()
    {
        if (_currentDifficultyLevel >= difficultyLevels.Length - 1) return;

        if(GameDuration - _currentTimer > difficultyLevels[_currentDifficultyLevel + 1].timeToChangeDifficulty)
        {
            _currentDifficultyLevel++;
            OnDifficultyChange();
        }
    }

    private void OnDifficultyChange()
    {
        difficultyLevels[_currentDifficultyLevel].eventOnEnterDifficulty.Invoke();
        maxCharactersOnScene = difficultyLevels[_currentDifficultyLevel].numberOfCharactersOnScreen;
        timeBetweenCharacter = difficultyLevels[_currentDifficultyLevel].timeBetweenCharactersSpawns;
        ScoreManager.SetScoreMultiplier(difficultyLevels[_currentDifficultyLevel].scoreMultiplier);
        ChangeNumberOfObjectives();
    }

    private void ChangeNumberOfObjectives()
    {
        if (!_isObjectiveSetupInGame) return;

        if (numberOfObjectives == difficultyLevels[_currentDifficultyLevel].numbersOfObjectives) return;

        if (numberOfObjectives < difficultyLevels[_currentDifficultyLevel].numbersOfObjectives)
        {
            int differenceBetweenNumbers = difficultyLevels[_currentDifficultyLevel].numbersOfObjectives - numberOfObjectives;
            for (int i = 0; i < differenceBetweenNumbers; i++)
            {
                int objectId = AddNewElementToObjectiveList();
                UiManager.AddImageToObjectiveUi(objectId);
            }
        }
        else if(numberOfObjectives > difficultyLevels[_currentDifficultyLevel].numbersOfObjectives)
        {
            int differenceBetweenNumbers = numberOfObjectives - difficultyLevels[_currentDifficultyLevel].numbersOfObjectives;
            RemoveObjectives(differenceBetweenNumbers);
        }
        numberOfObjectives = difficultyLevels[_currentDifficultyLevel].numbersOfObjectives;
    }

    #region character
    private void SpawnCharactersTimer()
    {
        if (_characterList.Count >= maxCharactersOnScene || !_isGamePlaying) return;


        characterSpawnTimer += Time.deltaTime;
        if (characterSpawnTimer >= timeBetweenCharacter)
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
        newChar.speed = difficultyLevels[_currentDifficultyLevel].characterSpeed;
        newChar.characterAnimator.speed = difficultyLevels[_currentDifficultyLevel].characterAnimatorSpeed;

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
        Debug.Log(_objectIndexAvailable.Count + " available index list count");
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

            ScoreManager.AddScore(ObjectsContainerScript.objet[idToCheck].goodObjectValue);
        }
        else
        {
            //make people unhappy
            ScoreManager.RemoveScore(ObjectsContainerScript.objet[idToCheck].badObjectValue);

            _disableObjectInteraction = true;
            Debug.Log("Stop Interaction");
            StartCoroutine(RestoreClickInteractionAfterTime());
        }
    }

    private IEnumerator ShowObjectiveListAfterDelay()
    {
        yield return new WaitForSeconds(timeToWaitToMakeObjectiveAppear);
        SetupObjectiveList();
    }

    private void SetupObjectiveList()
    {
        for (int i = 0; i < difficultyLevels[_currentDifficultyLevel].numbersOfObjectives; i++)
        {
            int objectId = AddNewElementToObjectiveList();
            UiManager.AddImageToObjectiveUi(objectId);
        }
        numberOfObjectives = difficultyLevels[_currentDifficultyLevel].numbersOfObjectives;
        _isObjectiveSetupInGame = true;
    }

    public int RemoveObjectiveAndGetANewOne(int idToCheck)
    {
        _objectiveList.Remove(idToCheck);

        return AddNewElementToObjectiveList();
    }

    private void RemoveObjectives(int numberOfObjectivesToRemove)
    {
        for (int i = 0; i < numberOfObjectivesToRemove; i++)
        {
            _objectiveList.RemoveAt(_objectiveList.Count - 1);
        }

        UiManager.RemoveObjective(numberOfObjectivesToRemove);
    }

    public int AddNewElementToObjectiveList()
    {
        Debug.Log(_objectIndexAvailable.Count + "Start object Available Count");

        List<ObjectWithCharacter> objWithChar = new List<ObjectWithCharacter>();

        int indexAvailableListCount = _objectIndexAvailable.Count;
        for (int i = indexAvailableListCount - 1; i >= 0; i--)
        {
            if (!_objectiveList.Contains(_objectIndexAvailable[i].ObjectIndex)) continue;

            objWithChar.Add(_objectIndexAvailable[i]);
            _objectIndexAvailable.Remove(_objectIndexAvailable[i]);
        }
        Debug.Log(objWithChar.Count + "Start  objWithChar Count");

        int random = Random.Range(0, _objectIndexAvailable.Count);


        indexAvailableListCount = objWithChar.Count;
        for (int i = 0; i < indexAvailableListCount; i++)
        {
            _objectIndexAvailable.Add(objWithChar[i]);
        }

        Debug.Log(_objectIndexAvailable.Count + "objectAVailableCount        " + random + " random");

        _objectiveList.Add(_objectIndexAvailable[random].ObjectIndex);

        _objectIndexAvailable[random].character.AddTimeToStayOnScreenTimer(timeToAddToAddedObjectivesCharacters);
        _objectIndexAvailable.RemoveAt(random);
        Debug.Log(_objectiveList.Count - 1);


        Debug.Log(_objectiveList.Count - 1);

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
