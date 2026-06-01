using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene2 : MonoBehaviour
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
    [SerializeField] private Character Ariya;
    [SerializeField] private Character Roman;
    [SerializeField] private Character Markian;

    [Header("Character Slots")]
    [SerializeField] private Image leftCharacterImage;
    [SerializeField] private Image centerCharacterImage;
    [SerializeField] private Image rightCharacterImage;

    [SerializeField] private Image leftCharacterEmotion;
    [SerializeField] private Image centerCharacterEmotion;
    [SerializeField] private Image rightCharacterEmotion;

    [Header("Special Close Slots")]
    [SerializeField] private Image romanCloseCharacterImage;
    [SerializeField] private Image markianCloseCharacterImage;

    [SerializeField] private Image romanCloseCharacterEmotion;
    [SerializeField] private Image markianCloseCharacterEmotion;

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

    [Header("Scene")]
    [SerializeField] private int nextSceneIndex = 2;

    [Header("Background Objects")]
    [SerializeField] private GameObject morningBackground;
    [SerializeField] private GameObject dayBackground;
    [SerializeField] private GameObject eveningBackground;
    [SerializeField] private GameObject hallwayBackground;
    [SerializeField] private GameObject classroom1Background;
    [SerializeField] private GameObject classroom2Background;

    [Header("Walk Backgrounds")]
    [SerializeField] private GameObject walk1Background;
    [SerializeField] private GameObject walk2Background;
    [SerializeField] private GameObject walkCatBackground;

    [Header("Room Backgrounds")]
    [SerializeField] private GameObject roomEveningBackground;
    [SerializeField] private GameObject roomNightBackground;

    [Header("Text Box Colors")]
    [SerializeField] private Image textBoxFrame;
    [SerializeField] private Color redFrame;
    [SerializeField] private Color blueFrame;
    [SerializeField] private Color blackFrame;
    [SerializeField] private Color yellowFrame;
    [SerializeField] private Color pinkFrame;
    [SerializeField] private Color violetFrame;

    [Header("Controllers")]
    [SerializeField] private LaptopController laptopController;
    [SerializeField] private RoomController roomController;

    [Header("Laptop Mail UI")]
    [SerializeField] private GameObject caesarMailPanel;
    [SerializeField] private TMP_Text caesarMailTitleText;
    [SerializeField] private TMP_Text caesarMailBodyText;
    [SerializeField] private Button closeLaptopButton;

    [Header("Room Investigation UI")]
    [SerializeField] private GameObject roomInvestigationPanel;
    [SerializeField] private TMP_Text roomInvestigationText;
    [SerializeField] private Button roomBooksButton;
    [SerializeField] private Button roomDeskButton;
    [SerializeField] private Button roomLaptopButton;
    [SerializeField] private Button roomFinishButton;

    private Coroutine typingCoroutine;
    private float textSpeed;
    private bool isTyping = false;
    private bool allowSFX = false;
    private bool firstLineShown = false;
    private bool isStartingScene = true;
    private float lastSFXTime = 0f;
    private string currentSpeaker = "";
    private bool waitingForLaptopClose = false;
    private bool waitingForRoomFinish = false;
    private bool waitingForRoomHub = false;
    private bool roomBooksChecked = false;
    private bool roomDeskChecked = false;
    private bool roomLaptopChecked = false;
    private string roomNextPath = "";
    private string roomClockNextPath = "end_of_day";

    private Dictionary<string, string> characterPositions = new Dictionary<string, string>();

    private void Start()
    {
        textSpeed = PlayerPrefs.GetFloat("TextSpeed", 0.03f);

        allowSFX = false;
        firstLineShown = false;
        isStartingScene = true;

        story = new Story(inkJSON.text);
        GameProgress.LoadIntoStory(story);
        story.ChoosePathString("start");

        if (laptopController != null)
        {
            laptopController.SetStory(story);
            laptopController.SetSceneController(this);
        }

        if (roomController != null)
        {
            roomController.SetStory(story);
            roomController.SetSceneController(this);
        }

        SetActiveSafe(dialoguePanel, false);
        SetActiveSafe(nextButton, false);
        SetActiveSafe(choicePanel, false);
        SetActiveSafe(caesarMailPanel, false);
        SetActiveSafe(roomInvestigationPanel, false);

        if (closeLaptopButton != null)
        {
            closeLaptopButton.onClick.RemoveAllListeners();
            closeLaptopButton.onClick.AddListener(CloseLaptopButton);
        }

        SetupRoomButtons();
        HideAllCharacters();

        StartCoroutine(StartScene());
    }

    private IEnumerator StartScene()
    {
        yield return new WaitForSeconds(2f);

        if (fadeIn != null)
            fadeIn.SetActive(false);

        yield return new WaitForSeconds(1f);

        ShowDialogue();
        ContinueStory();

        isStartingScene = false;
    }

    public void ContinueStory()
    {
        if (isTyping || waitingForLaptopClose || waitingForRoomFinish || waitingForRoomHub)
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

            if (waitingForRoomHub)
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
            if (!waitingForLaptopClose && !waitingForRoomFinish)
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

    private bool HasSpeakerTag(List<string> tags)
    {
        foreach (string tag in tags)
        {
            if (tag.Trim().StartsWith("speaker:"))
                return true;
        }

        return false;
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
        else if (!waitingForLaptopClose && !waitingForRoomFinish)
            SetActiveSafe(nextButton, true);
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

                TMP_Text buttonText = null;
                if (i < choiceTexts.Length)
                    buttonText = choiceTexts[i];
                else
                    buttonText = choiceButtons[i].GetComponentInChildren<TMP_Text>();

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

            if (tag == "unlock_notes_1")
            {
                laptopController?.AddNote("Марк помітив: у шифрі повторюється зсув літер. Це схоже на шифр Цезаря.");
                continue;
            }

            if (tag == "room_bookshelf_available")
            {
                roomController?.SetBookshelfAvailable(true);
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
                case "textbox": SetTextBoxColor(value); break;
                case "speaker": SetCharacter(value); break;
                case "emotion": SetEmotion(value); break;
                case "show": ShowCharacter(value); break;
                case "hide": HideCharacter(value); break;
                case "hidepos": HidePosition(value); break;
                case "clear": if (value == "all") HideAllCharacters(); break;
                case "sfx": PlaySFX(value); break;
                case "laptop": HandleLaptop(value); break;
                case "mail_icon": HandleMailIcon(value); break;
                case "add_mail": HandleAddMail(value); break;
                case "next_after_mail": HandleNextAfterMail(value); break;
                case "mail": ShowMail(value); break;
                case "room_mode": if (value == "open") OpenRoomHub(); break;
                case "room_next": roomNextPath = value; break;
                case "room_clock_next": roomClockNextPath = value; if (roomController != null) roomController.SetNextDayInkPath(value); break;
                case "room_bookshelf_available": if (value == "true") roomController?.SetBookshelfAvailable(true); break;
                case "protected_file_next": laptopController?.SetProtectedFileNextPath(value); break;
                case "hide_dialogue": if (value == "true") HideDialogue(); break;
                case "show_dialogue": if (value == "true") ShowDialogue(); break;
                case "save": GameProgress.SaveFromStory(story); break;
            }
        }
    }

    private void SetBackground(string bgName)
    {
        SetActiveSafe(morningBackground, false);
        SetActiveSafe(dayBackground, false);
        SetActiveSafe(eveningBackground, false);
        SetActiveSafe(hallwayBackground, false);
        SetActiveSafe(classroom1Background, false);
        SetActiveSafe(classroom2Background, false);
        SetActiveSafe(walk1Background, false);
        SetActiveSafe(walk2Background, false);
        SetActiveSafe(walkCatBackground, false);
        SetActiveSafe(roomEveningBackground, false);
        SetActiveSafe(roomNightBackground, false);

        switch (bgName)
        {
            case "morning": SetActiveSafe(morningBackground, true); break;
            case "day": SetActiveSafe(dayBackground, true); break;
            case "evening": SetActiveSafe(eveningBackground, true); break;
            case "hallway": SetActiveSafe(hallwayBackground, true); break;
            case "classroom1": SetActiveSafe(classroom1Background, true); break;
            case "classroom": SetActiveSafe(classroom1Background, true); break;
            case "classroom2": SetActiveSafe(classroom2Background, true); break;
            case "walk1": SetActiveSafe(walk1Background, true); break;
            case "walk2": SetActiveSafe(walk2Background, true); break;
            case "walk_cat": SetActiveSafe(walkCatBackground, true); break;
            case "room_evening": SetActiveSafe(roomEveningBackground != null ? roomEveningBackground : eveningBackground, true); break;
            case "room_night": SetActiveSafe(roomNightBackground != null ? roomNightBackground : eveningBackground, true); break;
            case "night": SetActiveSafe(roomNightBackground != null ? roomNightBackground : eveningBackground, true); break;
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
            case "black": textBoxFrame.color = blackFrame; break;
            case "yellow": textBoxFrame.color = yellowFrame; break;
            case "pink": textBoxFrame.color = pinkFrame; break;
            case "violet": textBoxFrame.color = violetFrame; break;
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
        HidePosition("center");
        HidePosition("right");
        HidePosition("romanClose");
        HidePosition("markianClose");
        characterPositions.Clear();
    }

    private Character GetCharacter(string characterName)
    {
        switch (characterName)
        {
            case "Mark": return Mark;
            case "Lev": return Lev;
            case "Ariya": return Ariya;
            case "Roman": return Roman;
            case "Markian": return Markian;
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
            case "Ariya": return "right";
            case "Markian": return "markianClose";
            case "Lev":
            case "Roman": return "right";
        }

        return "right";
    }

    private Image GetFullbodyImage(string position)
    {
        switch (position)
        {
            case "left": return leftCharacterImage;
            case "center": return centerCharacterImage;
            case "right": return rightCharacterImage;
            case "romanClose":
            case "closeRight": return romanCloseCharacterImage;
            case "markianClose":
            case "closeCenter": return markianCloseCharacterImage;
        }

        return null;
    }

    private Image GetEmotionImage(string position)
    {
        switch (position)
        {
            case "left": return leftCharacterEmotion;
            case "center": return centerCharacterEmotion;
            case "right": return rightCharacterEmotion;
            case "romanClose":
            case "closeRight": return romanCloseCharacterEmotion;
            case "markianClose":
            case "closeCenter": return markianCloseCharacterEmotion;
        }

        return null;
    }

    private void HandleLaptop(string value)
    {
        if (laptopController == null)
            return;

        switch (value)
        {
            case "open":
                waitingForLaptopClose = true;
                laptopController.OpenLaptopForStory();
                break;
            case "close":
                waitingForLaptopClose = false;
                laptopController.CloseLaptopFromInk();
                break;
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

    private void ShowMail(string value)
    {
        if (laptopController == null)
            return;

        // Старий тег # mail: caesar тепер не відкриває окрему вручну зібрану панель.
        // Він додає лист у пошту, відкриває ноутбук і показує Inbox.
        laptopController.AddMail(value);
        waitingForLaptopClose = true;
        laptopController.OpenLaptopForStory();
        laptopController.OpenMailPanel();
        HideDialogue();
    }

    public void CloseLaptopButton()
    {
        if (clickSound != null)
            clickSound.Play();

        SetActiveSafe(caesarMailPanel, false);

        if (laptopController != null)
        {
            laptopController.SetMailIconNormal();
            laptopController.CloseLaptop();
        }

        waitingForLaptopClose = false;
        GameProgress.SaveFromStory(story);
        ShowDialogue();
        ContinueStory();
    }

    private void SetupRoomButtons()
    {
        if (roomBooksButton != null)
        {
            roomBooksButton.onClick.RemoveAllListeners();
            roomBooksButton.onClick.AddListener(() =>
            {
                roomBooksChecked = true;
                SetRoomText("Марк переглянув книги. Між сторінками однієї з них була стара бібліотечна картка з майже стертим штампом: «Архів». Це слово вже вдруге траплялося йому сьогодні.");
            });
        }

        if (roomDeskButton != null)
        {
            roomDeskButton.onClick.RemoveAllListeners();
            roomDeskButton.onClick.AddListener(() =>
            {
                roomDeskChecked = true;
                SetRoomText("На столі лежав зошит, зарядка, чужа ручка і записка Лева: «Не чіпай мої речі». Марк автоматично відсунув енергетик подалі від краю.");
            });
        }

        if (roomLaptopButton != null)
        {
            roomLaptopButton.onClick.RemoveAllListeners();
            roomLaptopButton.onClick.AddListener(() =>
            {
                roomLaptopChecked = true;
                SetRoomText("Ноутбук виглядав звичайно. Але іконка AIIA на робочому столі з'явилась там, де вчора її точно не було.");
            });
        }

        if (roomFinishButton != null)
        {
            roomFinishButton.onClick.RemoveAllListeners();
            roomFinishButton.onClick.AddListener(FinishRoomInvestigation);
        }
    }

    private void OpenRoomInvestigation()
    {
        OpenRoomHub();
    }

    private void OpenRoomHub()
    {
        waitingForRoomHub = true;
        waitingForRoomFinish = false;
        HideDialogue();

        if (roomController != null)
        {
            roomController.SetNextDayInkPath(roomClockNextPath);
            roomController.OpenRoomHub();
        }
        else
        {
            SetActiveSafe(roomInvestigationPanel, true);
            SetRoomText("Кімната доступна. Додай RoomController, щоб працювали ноутбук, книга й годинник.");
        }
    }

    public void FinishDayFromRoom(string nextPath)
    {
        waitingForRoomHub = false;
        waitingForRoomFinish = false;

        if (!string.IsNullOrWhiteSpace(nextPath))
        {
            JumpToKnot(nextPath);
            return;
        }

        if (!string.IsNullOrWhiteSpace(roomClockNextPath))
        {
            JumpToKnot(roomClockNextPath);
            return;
        }

        StartCoroutine(EndScene());
    }

    private void SetRoomText(string text)
    {
        if (roomInvestigationText != null)
            roomInvestigationText.text = text;
    }

    private void FinishRoomInvestigation()
    {
        if (clickSound != null)
            clickSound.Play();

        SetActiveSafe(roomInvestigationPanel, false);
        waitingForRoomFinish = false;
        ShowDialogue();

        SetInkBool("checked_books", roomBooksChecked);
        SetInkBool("checked_bookshelf", roomBooksChecked);
        SetInkBool("checked_desk", roomDeskChecked);
        SetInkBool("checked_laptop", roomLaptopChecked);

        GameProgress.SaveFromStory(story);

        if (!string.IsNullOrWhiteSpace(roomNextPath))
        {
            string path = roomNextPath;
            roomNextPath = "";
            JumpToKnot(path);
            return;
        }

        ContinueStory();
    }

    private void SetInkBool(string variableName, bool value)
    {
        if (story != null && story.variablesState.Contains(variableName))
            story.variablesState[variableName] = value;
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

    public void HideDialogue()
    {
        SetActiveSafe(dialoguePanel, false);
        SetActiveSafe(nextButton, false);
    }

    public void ShowDialogue()
    {
        SetActiveSafe(dialoguePanel, true);

        if (nextButton != null && !isTyping && (choicePanel == null || !choicePanel.activeSelf))
            nextButton.SetActive(true);
    }

    public void JumpToKnot(string knotName)
    {
        if (story == null || string.IsNullOrEmpty(knotName))
            return;

        waitingForLaptopClose = false;
        waitingForRoomFinish = false;
        waitingForRoomHub = false;
        SetActiveSafe(choicePanel, false);

        GameProgress.SaveFromStory(story);
        story.ChoosePathString(knotName);
        ShowDialogue();
        ContinueStory();
    }

    public void ChooseInkPath(string knotName)
    {
        JumpToKnot(knotName);
    }

    private IEnumerator EndScene()
    {
        GameProgress.SaveFromStory(story);

        if (fadeOut != null)
            fadeOut.SetActive(true);

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void SetActiveSafe(GameObject obj, bool value)
    {
        if (obj != null)
            obj.SetActive(value);
    }
}
