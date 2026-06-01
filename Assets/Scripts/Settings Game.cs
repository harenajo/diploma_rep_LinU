using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class SettingsGame : MonoBehaviour
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

    private bool isSettingsOpen = false;

    private void Start()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0);

        float textSliderValue = PlayerPrefs.GetFloat("TextSliderValue", 0.5f);
        float textSpeed = Mathf.Lerp(0.015f, 0.12f, textSliderValue);

        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        textSpeedSlider.value = textSliderValue;

        fullscreenToggle.isOn = fullscreen;

        audioMixer.SetFloat("MusicVolume", musicVolume);
        audioMixer.SetFloat("SFXVolume", sfxVolume);

        Screen.fullScreen = fullscreen;

        settingsPanel.SetActive(false);

        StartPreview(textSpeed);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isSettingsOpen)
            {
                CloseSettings();
            }
            else
            {
                OpenSettings();
            }
        }
    }

    // =========================
    // OPEN SETTINGS
    // =========================

    public void OpenSettings()
    {
        clickSound.Play();

        isSettingsOpen = true;

        settingsPanel.SetActive(true);

        Time.timeScale = 0f;

        float value = PlayerPrefs.GetFloat("TextSliderValue", 0.5f);

        float speed = Mathf.Lerp(0.015f, 0.12f, value);

        StartPreview(speed);
    }

    // =========================
    // CLOSE SETTINGS
    // =========================

    public void CloseSettings()
    {
        clickSound.Play();

        isSettingsOpen = false;

        settingsPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    // =========================
    // BUTTON SOUND
    // =========================

    public void ButtonSound()
    {
        clickSound.Play();
    }

    // =========================
    // MUSIC VOLUME
    // =========================

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);

        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    // =========================
    // SFX VOLUME
    // =========================

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);

        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    // =========================
    // TEXT SPEED
    // =========================

    public void SetTextSpeed(float value)
    {
        float speed = Mathf.Lerp(0.015f, 0.12f, value);

        PlayerPrefs.SetFloat("TextSliderValue", value);

        PlayerPrefs.SetFloat("TextSpeed", speed);

        StartPreview(speed);
    }

    // =========================
    // FULLSCREEN
    // =========================

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    // =========================
    // TEXT PREVIEW
    // =========================

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