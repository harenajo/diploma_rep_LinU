using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene1 : MonoBehaviour
{
    [Header("Ink")]
    [SerializeField] private TextAsset inkJSON;
    private Story story;

    [Header("Fade")]
    [SerializeField] private GameObject fadeIn;
    [SerializeField] private GameObject fadeOut;

    [Header("Characters")]
    [SerializeField] private Character Mark;
    [SerializeField] private Character Lev;

    [Header("Character Images")]
    [SerializeField] private Image leftCharacterImage;
    [SerializeField] private Image rightCharacterImage;
    [SerializeField] private Image leftCharacterImageEmotion;
    [SerializeField] private Image rightCharacterImageEmotion;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text charNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject nextButton;

    [Header("Choices")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TMP_Text[] choiceTexts;

    [Header("Sounds")]
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip notificationSFX;
    [SerializeField] private AudioClip glitchSFX;
    [SerializeField] private AudioClip doorSFX;

    [Header("Background Objects")]
    [SerializeField] private GameObject nightBackground;
    [SerializeField] private GameObject morningBackground;
    [SerializeField] private GameObject dayBackground;
    [SerializeField] private GameObject eveningBackground;
    [SerializeField] private GameObject classroomBackground;

    [Header("Text Box Colors")]
    [SerializeField] private Image textBoxFrame;
    [SerializeField] private Color redFrame;
    [SerializeField] private Color blueFrame;
    [SerializeField] private Color pinkFrame;
    [SerializeField] private Color blackFrame;
    [SerializeField] private Color yellowFrame;

    [Header("Laptop")]
    [SerializeField] private LaptopController laptopController;

    [Header("Scene")]
    [SerializeField] private int nextSceneIndex = 2;

    private Coroutine typingCoroutine;
    private float textSpeed;
    private bool isTyping;
    private bool allowSFX;
    private bool firstLineShown;
    private bool isStartingScene = true;
    private float lastSFXTime;
    private string currentSpeaker = "";

    private readonly Dictionary<string, string> characterPositions = new Dictionary<string, string>();

    private void Start()
    {
        textSpeed = PlayerPrefs.GetFloat("TextSpeed", 0.03f);

        story = new Story(inkJSON.text);
        GameProgress.LoadIntoStory(story);
        story.ChoosePathString("start");

        if (laptopController != null)
        {
            laptopController.SetStory(story);
            laptopController.SetSceneController(this);
        }

        HideAllCharacters();

        SetActiveSafe(dialoguePanel, false);
        SetActiveSafe(nextButton, false);
        SetActiveSafe(choicePanel, false);

        StartCoroutine(StartScene());
    }

    private IEnumerator StartScene()
    {
        yield return new WaitForSeconds(1.5f);

        if (fadeIn != null)
            fadeIn.SetActive(false);

        ShowDialogue();
        ContinueStory();
        isStartingScene = false;
    }

    public void ContinueStory()
    {
        if (isTyping)
            return;

        if (choicePanel != null && choicePanel.activeSelf)
            return;

        if (story == null)
            return;

        if (!story.canContinue)
        {
            if (story.currentChoices.Count > 0)
            {
                ShowChoices();
                return;
            }

            if (laptopController != null && laptopController.IsLaptopOpen())
                return;

            GameProgress.SaveFromStory(story);
            StartCoroutine(EndScene());
            return;
        }

        textSpeed = PlayerPrefs.GetFloat("TextSpeed", 0.03f);

        string text = story.Continue().Trim();
        List<string> tags = story.currentTags;

        bool hasSpeaker = HasSpeakerTag(tags);
        if (!hasSpeaker)
        {
            currentSpeaker = "";
            if (charNameText != null)
                charNameText.text = "";
        }

        ParseTags(tags);

        if (string.IsNullOrEmpty(text))
        {
            ContinueStory();
            return;
        }

        if (!firstLineShown)
        {
            firstLineShown = true;
            allowSFX = !isStartingScene;
        }
        else
        {
            allowSFX = true;
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(text));
    }

    public void ChooseInkPath(string path)
    {
        if (story == null || string.IsNullOrWhiteSpace(path))
            return;

        story.ChoosePathString(path);
        ContinueStory();
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        SetActiveSafe(nextButton, false);

        if (dialogueText != null)
            dialogueText.text = "";

        foreach (char letter in text)
        {
            if (dialogueText != null)
                dialogueText.text += letter;

            yield return new WaitForSecondsRealtime(textSpeed);
        }

        isTyping = false;

        if (story.currentChoices.Count > 0)
            ShowChoices();
        else if (nextButton != null && (laptopController == null || !laptopController.IsLaptopOpen()))
            nextButton.SetActive(true);
    }

    private void ShowChoices()
    {
        if (choicePanel == null)
            return;

        choicePanel.SetActive(true);
        SetActiveSafe(nextButton, false);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < story.currentChoices.Count)
            {
                Choice choice = story.currentChoices[i];
                choiceButtons[i].gameObject.SetActive(true);

                TMP_Text buttonText = i < choiceTexts.Length ? choiceTexts[i] : choiceButtons[i].GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                    buttonText.text = choice.text;

                int choiceIndex = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => ChooseOption(choiceIndex));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void ChooseOption(int index)
    {
        if (clickSound != null)
            clickSound.Play();

        SetActiveSafe(choicePanel, false);
        SetActiveSafe(nextButton, true);

        story.ChooseChoiceIndex(index);
        GameProgress.SaveFromStory(story);
        ContinueStory();
    }

    private void ParseTags(List<string> tags)
    {
        foreach (string rawTag in tags)
        {
            string tag = rawTag.Trim();

            if (tag == "open_laptop")
            {
                laptopController?.OpenLaptop();
                continue;
            }

            if (tag == "close_laptop")
            {
                laptopController?.CloseLaptopFromInk();
                continue;
            }

            string[] splitTag = tag.Split(':');

            if (splitTag.Length != 2)
                continue;

            string key = splitTag[0].Trim();
            string value = splitTag[1].Trim();

            switch (key)
            {
                case "bg": SetBackground(value); break;
                case "clear": if (value == "all") HideAllCharacters(); break;
                case "textbox": SetTextBoxColor(value); break;
                case "speaker": SetCharacter(value); break;
                case "emotion": SetEmotion(value); break;
                case "show": ShowCharacter(value); break;
                case "hide": HideCharacter(value); break;
                case "hidepos": HidePosition(value); break;
                case "sfx": PlaySFX(value); break;
                case "laptop": HandleLaptop(value); break;
                case "mail_icon": HandleMailIcon(value); break;
                case "add_mail": HandleAddMail(value); break;
                case "next_after_mail": HandleNextAfterMail(value); break;
                case "hide_dialogue": if (value == "true") HideDialogue(); break;
                case "show_dialogue": if (value == "true") ShowDialogue(); break;
                case "save": GameProgress.SaveFromStory(story); break;
            }
        }
    }

    private bool HasSpeakerTag(List<string> tags)
    {
        foreach (string tag in tags)
        {
            if (tag.Trim().StartsWith("speaker:"))
                return true;
        }

        return false;
    }

    private void SetBackground(string bgName)
    {
        SetActiveSafe(nightBackground, false);
        SetActiveSafe(morningBackground, false);
        SetActiveSafe(dayBackground, false);
        SetActiveSafe(eveningBackground, false);
        SetActiveSafe(classroomBackground, false);

        switch (bgName)
        {
            case "night": SetActiveSafe(nightBackground, true); break;
            case "morning": SetActiveSafe(morningBackground, true); break;
            case "day": SetActiveSafe(dayBackground, true); break;
            case "evening": SetActiveSafe(eveningBackground, true); break;
            case "classroom":
            case "classroom1": SetActiveSafe(classroomBackground, true); break;
        }
    }

    private void SetTextBoxColor(string colorName)
    {
        if (textBoxFrame == null)
            return;

        switch (colorName)
        {
            case "red": textBoxFrame.color = redFrame; break;
            case "blue": textBoxFrame.color = blueFrame; break;
            case "pink": textBoxFrame.color = pinkFrame; break;
            case "black": textBoxFrame.color = blackFrame; break;
            case "yellow": textBoxFrame.color = yellowFrame; break;
        }
    }

    private void SetCharacter(string characterName)
    {
        currentSpeaker = characterName;

        if (characterName == "none")
        {
            currentSpeaker = "";
            if (charNameText != null)
                charNameText.text = "";
            return;
        }

        Character character = GetCharacter(characterName);
        if (character != null && charNameText != null)
            charNameText.text = character.characterName;
    }

    private void SetEmotion(string emotionName)
    {
        Character character = GetCharacter(currentSpeaker);
        string position = GetCurrentPosition(currentSpeaker);
        Image fullbodyImage = GetFullbodyImage(position);
        Image emotionImage = GetEmotionImage(position);

        if (character == null || fullbodyImage == null || emotionImage == null)
            return;

        Sprite sprite = character.GetEmotionSprite(emotionName);
        if (sprite == null)
            return;

        SetSpriteToSlot(fullbodyImage, emotionImage, sprite, emotionName);
    }

    private void ShowCharacter(string value)
    {
        string[] data = value.Split(',');
        if (data.Length < 3)
            return;

        string characterName = data[0].Trim();
        string position = data[1].Trim();
        string emotionName = data[2].Trim();

        Character character = GetCharacter(characterName);
        Image fullbodyImage = GetFullbodyImage(position);
        Image emotionImage = GetEmotionImage(position);

        if (character == null || fullbodyImage == null || emotionImage == null)
            return;

        Sprite sprite = character.GetEmotionSprite(emotionName);
        if (sprite == null)
            return;

        HideCharacter(characterName);
        HidePosition(position);
        SetSpriteToSlot(fullbodyImage, emotionImage, sprite, emotionName);
        characterPositions[characterName] = position;
    }

    private void SetSpriteToSlot(Image fullbodyImage, Image emotionImage, Sprite sprite, string emotionName)
    {
        if (emotionName == "fullbody")
        {
            emotionImage.gameObject.SetActive(false);
            fullbodyImage.sprite = sprite;
            fullbodyImage.gameObject.SetActive(true);
        }
        else
        {
            fullbodyImage.gameObject.SetActive(false);
            emotionImage.sprite = sprite;
            emotionImage.gameObject.SetActive(true);
        }
    }

    private void HideCharacter(string characterName)
    {
        if (characterPositions.ContainsKey(characterName))
        {
            HidePosition(characterPositions[characterName]);
            characterPositions.Remove(characterName);
            return;
        }

        HidePosition(GetDefaultPosition(characterName));
    }

    private void HidePosition(string position)
    {
        Image fullbodyImage = GetFullbodyImage(position);
        Image emotionImage = GetEmotionImage(position);

        if (fullbodyImage != null)
            fullbodyImage.gameObject.SetActive(false);

        if (emotionImage != null)
            emotionImage.gameObject.SetActive(false);
    }

    private void HideAllCharacters()
    {
        HidePosition("left");
        HidePosition("right");
        characterPositions.Clear();
    }

    private Character GetCharacter(string characterName)
    {
        switch (characterName)
        {
            case "Mark": return Mark;
            case "Lev": return Lev;
        }

        return null;
    }

    private string GetCurrentPosition(string characterName)
    {
        if (characterPositions.ContainsKey(characterName))
            return characterPositions[characterName];

        return GetDefaultPosition(characterName);
    }

    private string GetDefaultPosition(string characterName)
    {
        switch (characterName)
        {
            case "Mark": return "left";
            case "Lev": return "right";
        }

        return "right";
    }

    private Image GetFullbodyImage(string position)
    {
        switch (position)
        {
            case "left": return leftCharacterImage;
            case "right": return rightCharacterImage;
        }

        return null;
    }

    private Image GetEmotionImage(string position)
    {
        switch (position)
        {
            case "left": return leftCharacterImageEmotion;
            case "right": return rightCharacterImageEmotion;
        }

        return null;
    }

    private void PlaySFX(string sfxName)
    {
        if (!allowSFX || sfxSource == null)
            return;

        if (Time.time - lastSFXTime < 0.1f)
            return;

        lastSFXTime = Time.time;

        switch (sfxName)
        {
            case "notification": if (notificationSFX != null) sfxSource.PlayOneShot(notificationSFX); break;
            case "glitch": if (glitchSFX != null) sfxSource.PlayOneShot(glitchSFX); break;
            case "door": if (doorSFX != null) sfxSource.PlayOneShot(doorSFX); break;
        }
    }

    private void HandleLaptop(string value)
    {
        if (laptopController == null)
            return;

        switch (value)
        {
            case "open": laptopController.OpenLaptopForStory(); break;
            case "close": laptopController.CloseLaptopFromInk(); break;
        }
    }

    private void HandleMailIcon(string value)
    {
        if (laptopController == null)
            return;

        switch (value)
        {
            case "new": laptopController.SetMailIconNew(); break;
            case "normal": laptopController.SetMailIconNormal(); break;
        }
    }

    private void HandleAddMail(string mailID)
    {
        if (laptopController == null)
            return;

        laptopController.AddMail(mailID);
        GameProgress.SaveFromStory(story);
    }

    private void HandleNextAfterMail(string path)
    {
        if (laptopController == null)
            return;

        laptopController.SetNextInkPathAfterMail(path);
    }

    public void HideDialogue()
    {
        SetActiveSafe(dialoguePanel, false);
        SetActiveSafe(nextButton, false);
        SetActiveSafe(choicePanel, false);
    }

    public void ShowDialogue()
    {
        SetActiveSafe(dialoguePanel, true);

        if (nextButton != null && !isTyping && (choicePanel == null || !choicePanel.activeSelf))
            nextButton.SetActive(true);
    }

    private IEnumerator EndScene()
    {
        GameProgress.SaveFromStory(story);

        if (fadeOut != null)
            fadeOut.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(nextSceneIndex);


    }

    private void SetActiveSafe(GameObject obj, bool active)
    {
        if (obj != null)
            obj.SetActive(active);
    }

    private void SetActiveSafe(Behaviour behaviour, bool active)
    {
        if (behaviour != null)
            behaviour.gameObject.SetActive(active);
    }
}
