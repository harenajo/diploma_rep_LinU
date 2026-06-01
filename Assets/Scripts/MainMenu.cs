using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   
    [SerializeField] GameObject FadeOut;
    [SerializeField] AudioSource ClickSound;
    [SerializeField] int SceneToLoad;
    [SerializeField] int SaveTransferValue;
    void Start()
    {

        
    }

    public void StartGame()
    {

        ClickSound.Play();
        FadeOut.SetActive(true);
        StartCoroutine(TrasferToScene());

    }
    public void LoadGame()
    {
        SaveTransferValue = PlayerPrefs.GetInt("LoadState");
        if (SaveTransferValue >= 0)
        {
            SceneToLoad = SaveTransferValue + 1;
            ClickSound.Play();
            FadeOut.SetActive(true);
            StartCoroutine(LoadScene());
        }


    }
   

    void Update()
    {
       

    }
    public void QuitGame()
    {
        ClickSound.Play();
        StartCoroutine(QuitCoroutine());
    }

    IEnumerator QuitCoroutine()
    {
        
        yield return new WaitForSeconds(1);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }


    IEnumerator TrasferToScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(1);

    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneToLoad);

    }


}