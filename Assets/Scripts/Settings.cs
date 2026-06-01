using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Text Speed")]
    [SerializeField] private Slider textSpeedSlider;
    [SerializeField] private TMP_Text textPreview;

    [Header("Screen")]
    [SerializeField] private Toggle fullscreenToggle;

    [Header("UI")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Sounds")]
    [SerializeField] private AudioSource clickSound;

    private Coroutine previewCoroutine;

    private void Start()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0);

        float textSliderValue = PlayerPrefs.GetFloat("TextSliderValue", 0.5f);
        float textSpeed = Mathf.Lerp(0.08f, 0.015f, textSliderValue);

        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
        textSpeedSlider.value = textSliderValue;
        fullscreenToggle.isOn = fullscreen;

        audioMixer.SetFloat("MusicVolume", musicVolume);
        audioMixer.SetFloat("SFXVolume", sfxVolume);
        Screen.fullScreen = fullscreen;

        StartPreview(textSpeed);
    }

    public void OpenSettings()
    {
        clickSound.Play();
        settingsPanel.SetActive(true);

        float value = PlayerPrefs.GetFloat("TextSliderValue", 0.5f);
        float speed = Mathf.Lerp(0.03f, 0.015f, value);
        StartPreview(speed);
    }

    public void CloseSettings()
    {
        clickSound.Play();
        settingsPanel.SetActive(false);
    }

    public void ButtonSound()
    {
        clickSound.Play();
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetTextSpeed(float value)
    {
        float speed = Mathf.Lerp(0.08f, 0.015f, value);

        PlayerPrefs.SetFloat("TextSliderValue", value);
        PlayerPrefs.SetFloat("TextSpeed", speed);

        StartPreview(speed);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    private void StartPreview(float speed)
    {
        if (previewCoroutine != null)
        {
            StopCoroutine(previewCoroutine);
        }

        previewCoroutine = StartCoroutine(PreviewText(speed));
    }

    private IEnumerator PreviewText(float speed)
    {
        string text = "Це приклад швидкості тексту...";
        textPreview.text = "";

        foreach (char letter in text)
        {
            textPreview.text += letter;
            yield return new WaitForSecondsRealtime(speed);
        }
    }
}