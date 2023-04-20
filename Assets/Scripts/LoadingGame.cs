using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingGame : MonoBehaviour
{
    [SerializeField]
    float loadTime = 3.0f;
    
    float timeLoaded = 0;
    Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("LoadGame", loadTime);
        slider = GetComponent<Slider>();
        slider.maxValue = loadTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLoaded < loadTime)
        {
            timeLoaded += Time.deltaTime;
            slider.value = timeLoaded;
        }
    }

    private void LoadGame()
    {
        Debug.Log("Load Game() called");
        SceneManager.LoadScene(1);
    }
}
