using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RandomTextAssign : MonoBehaviour
{

    private List<GameObject> TextImages = new List<GameObject>();
    public List<Transform> InitialPositions; // = new List<Transform>();
    public List<Transform> SlotPositions;

    public GameObject GameNextLevelGo;
    public GameObject GameOverGo;
    public GameObject ArrangeTextGo;

    public GameObject Red;
    public GameObject Yellow;
    public GameObject Green;

    public GameObject TrainPrefab;

    private int currentLevel;
    private bool isLevelAboutToLoad;
    private float trainSpeed;
    ParticleSystem fire;

    private enum Colors
    {
        Red, Yellow, Green
    }

    // Start is called before the first frame update
    void Start()
    {
        trainSpeed = 300f;
        currentLevel = 1;
        LoadLevel(currentLevel);
        fire = TrainPrefab.GetComponentInChildren<ParticleSystem>();
        fire.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsCorrectSlotsFilled() && isLevelAboutToLoad && currentLevel<=3)
        {
            StartCoroutine(WaitForSignalUpdate());
        }
    }

    //Turning on SetLight and turning off remaining
    void SetLight(Colors SetLight)
    {
        Color presentRedColour = Red.GetComponent<SpriteRenderer>().color;
        Color presentYellowColour = Yellow.GetComponent<SpriteRenderer>().color;
        Color presentGreenColour = Green.GetComponent<SpriteRenderer>().color;

        switch (SetLight)
        {
            case Colors.Red:
                Red.GetComponent<SpriteRenderer>().color = new Color(presentRedColour.r, presentRedColour.g, presentRedColour.b, 1);
                Yellow.GetComponent<SpriteRenderer>().color = new Color(presentYellowColour.r, presentYellowColour.g, presentYellowColour.b, 0.1f);
                Green.GetComponent<SpriteRenderer>().color = new Color(presentGreenColour.r, presentGreenColour.g, presentGreenColour.b, 0.1f);
                break;
            case Colors.Green:
                Red.GetComponent<SpriteRenderer>().color = new Color(presentRedColour.r, presentRedColour.g, presentRedColour.b, 0.1f);
                Yellow.GetComponent<SpriteRenderer>().color = new Color(presentYellowColour.r, presentYellowColour.g, presentYellowColour.b, 0.1f);
                Green.GetComponent<SpriteRenderer>().color = new Color(presentGreenColour.r, presentGreenColour.g, presentGreenColour.b, 1);
                break;
            default:
                Red.GetComponent<SpriteRenderer>().color = new Color(presentRedColour.r, presentRedColour.g, presentRedColour.b, 0.1f);
                Yellow.GetComponent<SpriteRenderer>().color = new Color(presentYellowColour.r, presentYellowColour.g, presentYellowColour.b, 1);
                Green.GetComponent<SpriteRenderer>().color = new Color(presentGreenColour.r, presentGreenColour.g, presentGreenColour.b, 0.1f);
                break;
        }
    }


    IEnumerator WaitForSignalUpdate()
    {
        SetLight(Colors.Green);
        DisableDragImages();
        AddImagesIntoTrain();
        isLevelAboutToLoad = false; // current level updates green signal and train move


        if(currentLevel == 3)
        {
            MoveTrain(trainSpeed *10);
        }
        else
        {
            MoveTrain(trainSpeed);
        }

        yield return new WaitForSeconds(2);
        fire.Stop();
        currentLevel++;
        LoadLevel(currentLevel);
    }

    private void AddImagesIntoTrain()
    {
        for (int i = 0; i < TextImages.Count; i++)
        {
            //TextImages[i].transform.parent = TrainPrefab.transform;
            TextImages[i].transform.SetParent(TrainPrefab.transform);
        }
    }

    private void MoveTrain(float trainSpeed)
    {
        fire.Play();
        TrainPrefab.GetComponent<Rigidbody2D>().AddForce(Vector2.left * trainSpeed, ForceMode2D.Impulse);
    }

    private void DisableDragImages()
    {
        for (int i = 0; i < TextImages.Count; i++)
        {
            Destroy(TextImages[i].GetComponent<Draggrable>());
        }
    }

    private void LoadLevel(int currentLevel)
    {
        SetLight(Colors.Yellow);
        isLevelAboutToLoad = true;
        if (currentLevel > 3)
        {
            ArrangeTextGo.SetActive(false);
            GameOverGo.SetActive(true);
            GameNextLevelGo.SetActive(false);
        }
        else if (currentLevel <= 3) //IsCorrectSlotsFilled() && 
        {
            SetExistingImagesInactive();
            StartCoroutine(WaitForLevelLoadingText());
        }
    }

    IEnumerator WaitForLevelLoadingText()
    {
        ArrangeTextGo.SetActive(false);
        GameOverGo.SetActive(false);
        GameNextLevelGo.SetActive(true);
        yield return new WaitForSeconds(2);
        MakeLevelImagesActive(currentLevel);
        //currentLevel++;
    }

    //private void MakeLevelImagesActive(int currentLevel)
    //{
    //    ArrangeTextGo.SetActive(true);
    //    GameOverGo.SetActive(false);
    //    GameNextLevelGo.SetActive(false);

    //    LoadLevelTextImageSet(currentLevel);
    //    if (TextImages.Count == InitialPositions.Count)
    //    {
    //        for (int i = 0; i < TextImages.Count; i++)
    //        {
    //            Vector3 selectedPosition = InitialPositions[i].position;

    //            int textImageSelected = UnityEngine.Random.Range(0, TextImages.Count);

    //            // Loop until the random text found
    //            while (TextImages[textImageSelected].gameObject.activeInHierarchy)
    //            {
    //                textImageSelected = UnityEngine.Random.Range(0, TextImages.Count);
    //            }
    //            TextImages[textImageSelected].transform.position = selectedPosition;
    //            TextImages[textImageSelected].SetActive(true);
    //            InitialPositions[i].gameObject.SetActive(true);
    //        }
    //    }
    //}

    private void MakeLevelImagesActive(int currentLevel)
    {
        ArrangeTextGo.SetActive(true);
        GameOverGo.SetActive(false);
        GameNextLevelGo.SetActive(false);

        LoadLevelTextImageSet(currentLevel);

        List<GameObject> TempTextImages = new List<GameObject>();

        for (int i = 0; i < TextImages.Count; i++)
        {
            TempTextImages.Add(TextImages[i]);
        }

        if (TextImages.Count == InitialPositions.Count)
        {
            for (int i = 0; i < TextImages.Count; i++)
            {
                Vector3 selectedPosition = InitialPositions[i].position;

                int textImageSelectedIndex = UnityEngine.Random.Range(0, TempTextImages.Count);

                var textImageSelected = TempTextImages[textImageSelectedIndex];
                textImageSelected.transform.position = selectedPosition;
                textImageSelected.gameObject.SetActive(true);
                InitialPositions[i].gameObject.SetActive(true);
                TempTextImages.Remove(textImageSelected);
            }
        }
        TempTextImages.Clear();
    }

    private void LoadLevelTextImageSet(int level)
    {
        TextImages.Clear();
        Draggrable[] gameObjects = (Draggrable[])Resources.FindObjectsOfTypeAll(typeof(Draggrable));

        if (gameObjects.Length > 0)
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (gameObjects[i].tag == ("Level_" + level + "_Image"))
                    TextImages.Add(gameObjects[i].gameObject);
            }
        }

        TextImages.Sort((x, y) => x.gameObject.name.CompareTo(y.gameObject.name));
        Array.Clear(gameObjects, 0, gameObjects.Length);
    }

    private void SetExistingImagesInactive()
    {
        for (int i = 0; i < TextImages.Count; i++)
        {
            TextImages[i].SetActive(false);
        }
        TextImages.Clear();
    }

    bool IsApproximatelyEqual(float x, float y, float precision = 0.001f)
    {
        float variance = x > y ? x - y : y - x;
        return variance < precision;
    }

    public bool IsCorrectSlotsFilled()
    {
        int verified = 0;
        bool isCorrectSequence = false;

        if (TextImages.Count == SlotPositions.Count)
        {
            for (int j = 0; j < TextImages.Count; j++)
            {
                if (IsApproximatelyEqual(SlotPositions[j].position.x, TextImages[j].gameObject.transform.position.x) &&
                    IsApproximatelyEqual(SlotPositions[j].position.y, TextImages[j].gameObject.transform.position.y))
                {
                    verified++;
                }
            }
        }

        if (verified == TextImages.Count && TextImages.Count > 0)
        {
            isCorrectSequence = true;
        }
        else if (verified < TextImages.Count && TextImages.Count > 0)
        {
            if (CheckWhetherAllPositionsFilled())
            {
                SetLight(Colors.Red);
            }
        }

        return isCorrectSequence;
    }

    private bool CheckWhetherAllPositionsFilled()
    {
        int verified = 0;
        bool isSequenceFilled = false;
        if (SlotPositions.Count == TextImages.Count)
        {
            for (int i = 0; i < SlotPositions.Count; i++)
            {
                for (int j = 0; j < TextImages.Count; j++)
                {
                    if (IsApproximatelyEqual(SlotPositions[i].position.x, TextImages[j].gameObject.transform.position.x) &&
                        IsApproximatelyEqual(SlotPositions[i].position.y, TextImages[j].gameObject.transform.position.y))
                    {
                        verified++;
                        break;
                    }
                }
            }
        }
        if (verified == TextImages.Count && TextImages.Count > 0)
        {
            isSequenceFilled = true;
        }
        return isSequenceFilled;
    }
}
