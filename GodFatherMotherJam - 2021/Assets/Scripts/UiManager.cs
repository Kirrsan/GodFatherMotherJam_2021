using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public Text timerText;


    [Header("ObjectiveUI")]
    public RectTransform ObjectiveUiParent;
    public GameObject  ImagePrefab;

    private class ImageAndObjectId
    {
        public Image Image;
        public int objectId;
    }

    private List<ImageAndObjectId> objectiveImageList = new List<ImageAndObjectId>();
    private List<float> objectivesTimers = new List<float>();
    private List<Slider> objectivesTimersSliders = new List<Slider>();
    private int objectiveImageListCount;

    // Update is called once per frame
    void Update()
    {
        timerText.text = GameManager.Instance.GetChrono().ToString("000");
        UpdateObjectiveTimers();
    }

    private void UpdateObjectiveTimers()
    {
        for (int i = 0; i < objectiveImageListCount; i++)
        {
            objectivesTimers[i] -= Time.deltaTime;
            objectivesTimersSliders[i].value = objectivesTimers[i];
            if (objectivesTimers[i] <= 0)
            {
                SetNewObjective(i);
            }
        }
    }

    public void SetNewObjective(int index)
    {
        int objectId = GameManager.Instance.RemoveObjectiveAndGetANewOne(objectiveImageList[index].objectId);

        objectiveImageList[index].Image.sprite = GameManager.Instance.ObjectsContainerScript.objet[objectId].sprite;
        objectiveImageList[index].objectId = objectId;
        objectivesTimers[index] = GameManager.Instance.GetCurrentDifficultyLevel().objectivesTimer;
    }

    public void SeekIndexAndSetNewObjectives(int objectId)
    {
        int index = -1;
        for (int i = 0; i < objectiveImageListCount; i++)
        {
            if (objectiveImageList[i].objectId != objectId) continue;

            index = i;
        }

        if(index != -1)
        {
            SetNewObjective(index);
        }
        else
        {
            Debug.LogError("ObjectId has not been found in objectiveList");
        }

    }


    public void AddImageToObjectiveUi(int objectId)
    {
        ObjectiveImage image = Instantiate(ImagePrefab, ObjectiveUiParent).GetComponent<ObjectiveImage>();
        image.objectiveSlider.maxValue = GameManager.Instance.GetCurrentDifficultyLevel().objectivesTimer;
        image.objectiveSlider.value = image.objectiveSlider.maxValue;
        image.objectiveImage.sprite = GameManager.Instance.ObjectsContainerScript.objet[objectId].sprite;

        ImageAndObjectId newImageAndObjId = new ImageAndObjectId();
        newImageAndObjId.objectId = objectId;
        newImageAndObjId.Image = image.objectiveImage;
        objectivesTimersSliders.Add(image.objectiveSlider);

        objectiveImageList.Add(newImageAndObjId);
        objectivesTimers.Add(GameManager.Instance.GetCurrentDifficultyLevel().objectivesTimer);
        ++objectiveImageListCount;
    }
}
