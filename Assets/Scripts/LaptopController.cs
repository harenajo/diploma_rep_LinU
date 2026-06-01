using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LaptopController : MonoBehaviour
{
    [Header("Main Laptop UI")]
    [SerializeField] private GameObject laptopUI;
    [SerializeField] private GameObject homeScreen;
    [SerializeField] private GameObject mailPanel;
    [SerializeField] private GameObject messageReaderPanel;
    [SerializeField] private GameObject notesPanel;
    [SerializeField] private GameObject protectedFilePanel;
    [SerializeField] private GameObject aiiaPanel;

    [Header("Mail Database")]
    [SerializeField] private Mail[] allMails;

    [Header("Desktop App Buttons")]
    [SerializeField] private Button mailAppButton;
    [SerializeField] private Button aiiaAppButton;
    [SerializeField] private Button notesAppButton;
    [SerializeField] private Button exitLaptopButton;

    [Header("Window Close / Back Buttons")]
    [SerializeField] private Button closeMailButton;
    [SerializeField] private Button closeMessageReaderButton;
    [SerializeField] private Button closeNotesButton;
    [SerializeField] private Button closeAiiaButton;
    [SerializeField] private Button closeProtectedFileButton;
    [SerializeField] private Button backFromProtectedFileButton;

    [Header("Mail Icon")]
    [SerializeField] private Image mailIconImage;
    [SerializeField] private Sprite mailIconNormal;
    [SerializeField] private Sprite mailIconNew;

    [Header("Inbox UI")]
    [SerializeField] private Transform mailListContent;
    [SerializeField] private Button mailButtonPrefab;
    [SerializeField] private TMP_Text emptyInboxText;

    [Header("Mail Reader UI")]
    [SerializeField] private TMP_Text mailTitleText;
    [SerializeField] private TMP_Text mailBodyText;
    [SerializeField] private TMP_Text mailTimerText;
    [SerializeField] private Image mailBackgroundImage;

    [Header("Attachment Button UI")]
    [SerializeField] private GameObject attachmentButtonObject;
    [SerializeField] private Button attachmentButton;
    [SerializeField] private Image attachmentIconImage;
    [SerializeField] private TMP_Text attachmentNameText;

    [Header("AIIA Archive UI")]
    [SerializeField] private Transform archiveFileContent;
    [SerializeField] private Button archiveFileButtonPrefab;
    [SerializeField] private TMP_Text archiveEmptyText;

    [Header("Notes Text UI")]
    [SerializeField] private TMP_Text notesText;

    [Header("Protected File UI")]
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text protectedFileTitleText;
    [SerializeField] private TMP_Text protectedFileText;
    [SerializeField] private TMP_Text passwordAttemptsText;
    [SerializeField] private Button passwordSubmitButton;

    [Header("AIIA App UI")]
    [SerializeField] private Character aiiaCharacter;
    [SerializeField] private TMP_Text aiiaText;
    [SerializeField] private Image aiiaEmotionImage;
    [SerializeField] private Button aiiaHintButton;

    [Header("AIIA Random Facts")]
    [SerializeField] private float randomFactDelay = 18f;
    [TextArea(2, 5)]
    [SerializeField] private string[] randomFacts;

    [Header("Timer Settings")]
    [SerializeField] private bool autoCloseLaptopWhenTimerEnds = true;

    private Story story;
    private Scene1 scene1Controller;
    private Scene2 scene2Controller;

    private readonly List<Mail> inboxMails = new List<Mail>();
    private readonly List<Mail> archiveFiles = new List<Mail>();
    private readonly List<string> aiiaNotes = new List<string>();
    private readonly Dictionary<Mail, int> passwordMistakesByMail = new Dictionary<Mail, int>();
    private readonly Dictionary<Mail, int> shownHintsByMail = new Dictionary<Mail, int>();
    private readonly HashSet<Mail> unlockedAttachments = new HashSet<Mail>();
    private readonly HashSet<Mail> openedMails = new HashSet<Mail>();

    private Mail currentMail;
    private Mail openedFileMail;
    private Coroutine factRoutine;
    private Coroutine timerRoutine;
    private string nextInkPathAfterMail;
    private string protectedFileNextPath;
    private bool storyControlledLaptop;

    public void SetStory(Story inkStory) => story = inkStory;

    public void SetSceneController(Scene1 controller)
    {
        scene1Controller = controller;
        scene2Controller = null;
    }

    public void SetSceneController(Scene2 controller)
    {
        scene2Controller = controller;
        scene1Controller = null;
    }

    public void SetNextInkPathAfterMail(string path) => nextInkPathAfterMail = path;
    public void SetProtectedFileNextPath(string path) => protectedFileNextPath = path;

    private void Awake()
    {
        if (laptopUI == null)
            laptopUI = gameObject;
    }

    private void Start()
    {
        CloseLaptopOnly();
        SetMailIconNormal();
        SetupButtons();
        RefreshInboxList();
        RefreshArchiveList();
        RefreshNotesText();
        ClearMailTimer();
    }

    private void SetupButtons()
    {
        Connect(mailAppButton, OpenMailPanel);
        Connect(aiiaAppButton, OpenAiiaPanel);
        Connect(notesAppButton, OpenNotesPanel);
        Connect(exitLaptopButton, CloseLaptop);

        Connect(closeMailButton, ShowHomeScreen);
        Connect(closeMessageReaderButton, CloseMailReaderAndContinue);
        Connect(closeNotesButton, ShowHomeScreen);
        Connect(closeAiiaButton, ShowHomeScreen);
        Connect(closeProtectedFileButton, ShowHomeScreen);
        Connect(backFromProtectedFileButton, OpenAiiaPanel);

        Connect(attachmentButton, OpenCurrentAttachmentFromMail);
        Connect(passwordSubmitButton, TryPassword);
        Connect(aiiaHintButton, RequestAiiaHint);
    }

    private void Connect(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    public void OpenLaptop()
    {
        storyControlledLaptop = false;

        if (laptopUI == null)
            laptopUI = gameObject;

        SetActive(laptopUI, true);
        laptopUI.transform.SetAsLastSibling();
        ShowHomeScreen();
        HideSceneDialogue();
        StartAiiaFacts();
        ApplyLaptopMode();
    }

    public void OpenLaptopFromRoom()
    {
        storyControlledLaptop = false;

        if (laptopUI == null)
            laptopUI = gameObject;

        SetActive(laptopUI, true);
        laptopUI.transform.SetAsLastSibling();

        CloseAllWindows();

        SetActive(homeScreen, true);

        HideSceneDialogue();
        StartAiiaFacts();
        ApplyLaptopMode();
    }

    public void OpenLaptopForStory()
    {
        storyControlledLaptop = true;

        if (laptopUI == null)
            laptopUI = gameObject;

        SetActive(laptopUI, true);
        laptopUI.transform.SetAsLastSibling();
        ShowHomeScreen();
        HideSceneDialogue();
        StartAiiaFacts();
        ApplyLaptopMode();
    }

    public void CloseLaptop()
    {
        if (storyControlledLaptop)
            return;

        StopAiiaFacts();
        StopTimer();
        CloseLaptopOnly();
        ShowSceneDialogue();
    }

    public void CloseLaptopFromInk()
    {
        storyControlledLaptop = false;
        StopAiiaFacts();
        StopTimer();
        CloseLaptopOnly();
        ShowSceneDialogue();
    }

    private void CloseLaptopOnly()
    {
        SetActive(laptopUI, false);
        CloseAllWindows();
        SetActive(homeScreen, false);
    }

    public bool IsLaptopOpen()
    {
        return laptopUI != null && laptopUI.activeSelf;
    }

    public void ShowHomeScreen()
    {
        if (laptopUI == null)
            laptopUI = gameObject;

        SetActive(laptopUI, true);
        CloseAllWindows();
        SetActive(homeScreen, true);
        ApplyLaptopMode();
    }

    private void ApplyLaptopMode()
    {
        SetActive(exitLaptopButton != null ? exitLaptopButton.gameObject : null, !storyControlledLaptop);
        SetActive(closeMailButton != null ? closeMailButton.gameObject : null, !storyControlledLaptop);
        SetActive(closeMessageReaderButton != null ? closeMessageReaderButton.gameObject : null, !storyControlledLaptop);
        SetActive(closeProtectedFileButton != null ? closeProtectedFileButton.gameObject : null, !storyControlledLaptop);
        SetActive(backFromProtectedFileButton != null ? backFromProtectedFileButton.gameObject : null, !storyControlledLaptop);
    }

    public void CloseAllWindows()
    {
        SetActive(mailPanel, false);
        SetActive(messageReaderPanel, false);
        SetActive(notesPanel, false);
        SetActive(protectedFilePanel, false);
        SetActive(aiiaPanel, false);
    }

    public void AddMail(string mailID)
    {
        Mail mail = FindMail(mailID);
        if (mail == null)
        {
            Debug.LogWarning("Mail not found: " + mailID);
            return;
        }

        if (!inboxMails.Contains(mail) && !openedMails.Contains(mail))
            inboxMails.Add(mail);

        SetMailIconNew();
        RefreshInboxList();
    }

    private Mail FindMail(string mailID)
    {
        if (allMails == null)
            return null;

        foreach (Mail mail in allMails)
        {
            if (mail != null && mail.mailID.Trim() == mailID.Trim())
                return mail;
        }

        return null;
    }

    public void OpenMailPanel()
    {
        SetActive(laptopUI, true);
        SetActive(homeScreen, true);
        CloseAllWindows();
        SetActive(mailPanel, true);
        RefreshInboxList();
        HideSceneDialogue();
        ApplyLaptopMode();
    }

    private void RefreshInboxList()
    {
        ClearChildren(mailListContent);

        bool hasMails = inboxMails.Count > 0;
        SetActive(emptyInboxText != null ? emptyInboxText.gameObject : null, !hasMails);

        if (!hasMails)
        {
            if (emptyInboxText != null)
                emptyInboxText.text = "Нових повідомлень немає.";

            SetMailIconNormal();
            return;
        }

        SetMailIconNew();

        foreach (Mail mail in inboxMails)
        {
            Button button = CreateListButton(mailListContent, mailButtonPrefab, mail.title, mail.attachmentIcon);
            if (button == null)
                continue;

            Mail capturedMail = mail;
            button.onClick.AddListener(() => OpenMailReader(capturedMail));
        }
    }

    private Button CreateListButton(Transform parent, Button prefab, string title, Sprite icon)
    {
        if (parent == null || prefab == null)
            return null;

        Button button = Instantiate(prefab, parent);
        button.gameObject.SetActive(true);

        TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
        if (text != null)
            text.text = title;

        Image[] images = button.GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            if (image.gameObject == button.gameObject)
                continue;

            if (icon != null)
            {
                image.sprite = icon;
                image.enabled = true;
            }

            break;
        }

        return button;
    }

    private void OpenMailReader(Mail mail)
    {
        currentMail = mail;
        openedMails.Add(mail);

        CloseAllWindows();
        SetActive(messageReaderPanel, true);

        if (mailTitleText != null)
        {
            mailTitleText.text = mail.title;
            mailTitleText.color = mail.textColor;
        }

        if (mailBodyText != null)
        {
            mailBodyText.text = mail.body;
            mailBodyText.color = mail.textColor;
        }

        if (mailBackgroundImage != null)
            mailBackgroundImage.sprite = mail.backgroundImage;

        if (mailTimerText != null)
        {
            mailTimerText.color = mail.textColor;
            mailTimerText.gameObject.SetActive(mail.hasTimer);
            mailTimerText.text = mail.hasTimer ? FormatTime(mail.hours, mail.minutes, mail.seconds) : "";
        }

        RefreshAttachmentButton(mail);
        ApplyLaptopMode();

        if (mail.hasTimer)
        {
            StopTimer();
            timerRoutine = StartCoroutine(MailTimerRoutine(mail));
        }
    }

    private void CloseMailReaderAndContinue()
    {
        if (storyControlledLaptop)
            return;

        StopTimer();

        if (currentMail != null)
        {
            if (currentMail.hasAttachment)
                AddFileToAiiaArchive(currentMail);

            inboxMails.Remove(currentMail);
            RefreshInboxList();
        }

        ShowHomeScreen();

        if (!string.IsNullOrWhiteSpace(nextInkPathAfterMail))
        {
            string path = nextInkPathAfterMail;
            nextInkPathAfterMail = "";
            JumpScene(path);
            return;
        }

        ContinueSceneStory();
    }

    public void CloseMailAndContinue()
    {
        CloseMailReaderAndContinue();
    }

    private void RefreshAttachmentButton(Mail mail)
    {
        bool show = mail != null && mail.hasAttachment;
        SetActive(attachmentButtonObject, show);

        if (!show)
            return;

        if (attachmentNameText != null)
            attachmentNameText.text = mail.attachmentName;

        if (attachmentIconImage != null)
        {
            attachmentIconImage.sprite = mail.attachmentIcon;
            attachmentIconImage.enabled = mail.attachmentIcon != null;
        }
    }

    public void OpenCurrentAttachmentFromMail()
    {
        if (storyControlledLaptop)
            return;

        if (currentMail == null || !currentMail.hasAttachment)
            return;

        AddFileToAiiaArchive(currentMail);
        inboxMails.Remove(currentMail);
        RefreshInboxList();
        OpenProtectedFile(currentMail, true);
    }

    private void AddFileToAiiaArchive(Mail mail)
    {
        if (mail == null || !mail.hasAttachment)
            return;

        if (!archiveFiles.Contains(mail))
            archiveFiles.Add(mail);

        AddNote("Отримано файл: " + mail.attachmentName);
        RefreshArchiveList();
    }

    public void OpenAiiaPanel()
    {
        SetActive(laptopUI, true);
        SetActive(homeScreen, true);
        CloseAllWindows();
        SetActive(aiiaPanel, true);
        RefreshArchiveList();
        ShowAiiaMessage("AIIA активна. Архів зберігає файли, які вже були відкриті з листів.", "neutral");
        HideSceneDialogue();
    }

    private void RefreshArchiveList()
    {
        ClearChildren(archiveFileContent);

        bool hasFiles = archiveFiles.Count > 0;
        SetActive(archiveEmptyText != null ? archiveEmptyText.gameObject : null, !hasFiles);

        if (!hasFiles)
        {
            if (archiveEmptyText != null)
                archiveEmptyText.text = "Архів порожній. Файли з'являться після першого відкриття вкладення з пошти.";

            return;
        }

        foreach (Mail mail in archiveFiles)
        {
            Button button = CreateListButton(archiveFileContent, archiveFileButtonPrefab, mail.attachmentName, mail.attachmentIcon);
            if (button == null)
                continue;

            Mail capturedMail = mail;
            button.onClick.AddListener(() => OpenProtectedFile(capturedMail, false));
        }
    }

    private void OpenProtectedFile(Mail mail, bool openedFromMail)
    {
        if (mail == null)
            return;

        openedFileMail = mail;
        CloseAllWindows();
        SetActive(protectedFilePanel, true);

        if (protectedFileTitleText != null)
            protectedFileTitleText.text = string.IsNullOrWhiteSpace(mail.attachmentName) ? "Захищений файл" : mail.attachmentName;

        if (passwordInput != null)
        {
            passwordInput.gameObject.SetActive(!unlockedAttachments.Contains(mail));
            passwordInput.text = "";
        }

        if (passwordSubmitButton != null)
            passwordSubmitButton.gameObject.SetActive(!unlockedAttachments.Contains(mail));

        if (protectedFileText != null)
        {
            if (unlockedAttachments.Contains(mail))
                protectedFileText.text = mail.attachmentUnlockedText;
            else
                protectedFileText.text = string.IsNullOrWhiteSpace(mail.attachmentLockedText) ? "Файл захищено паролем." : mail.attachmentLockedText;
        }

        RefreshPasswordAttemptsText(mail);
        HideSceneDialogue();
        ApplyLaptopMode();
    }

    public void TryPassword()
    {
        if (passwordInput == null || openedFileMail == null)
            return;

        Mail mail = openedFileMail;

        if (unlockedAttachments.Contains(mail))
        {
            ShowAiiaMessage("Файл уже відкритий. Його можна читати з архіву.", "neutral");
            return;
        }

        if (mail.maxPasswordAttempts > 0 && GetPasswordMistakes(mail) >= mail.maxPasswordAttempts)
        {
            ShowAiiaMessage(mail.aiiaFailedAttemptsHint, mail.aiiaWrongPasswordEmotion);
            return;
        }

        string playerAnswer = NormalizePassword(passwordInput.text);
        string correctAnswer = NormalizePassword(mail.attachmentPassword);

        if (playerAnswer == correctAnswer)
        {
            UnlockProtectedFile(mail);
            return;
        }

        int mistakes = GetPasswordMistakes(mail) + 1;
        passwordMistakesByMail[mail] = mistakes;
        SetInkInt("caesar_mistakes", mistakes);
        SetInkInt("failed_cipher_attempts", mistakes);
        RefreshPasswordAttemptsText(mail);

        if (mail.maxPasswordAttempts > 0 && mistakes >= mail.maxPasswordAttempts)
        {
            ShowAiiaMessage(mail.aiiaFailedAttemptsHint, mail.aiiaWrongPasswordEmotion);
            return;
        }

        ShowNextHint(mail);
    }

    private void UnlockProtectedFile(Mail mail)
    {
        unlockedAttachments.Add(mail);

        SetInkBool("file_opened", true);
        SetInkBool("opened_locked_file", true);
        SetInkBool("solved_cipher_2", true);

        if (protectedFileText != null)
            protectedFileText.text = mail.attachmentUnlockedText;

        if (passwordInput != null)
            passwordInput.gameObject.SetActive(false);

        if (passwordSubmitButton != null)
            passwordSubmitButton.gameObject.SetActive(false);

        RefreshPasswordAttemptsText(mail);
        AddNote("Файл відкрито: " + mail.attachmentName + "\n" + mail.attachmentUnlockedText);
        ShowAiiaMessage("Файл відкрито. Я зберегла його в архіві.", mail.aiiaUnlockedEmotion);

        if (!string.IsNullOrWhiteSpace(protectedFileNextPath))
        {
            string path = protectedFileNextPath;
            protectedFileNextPath = "";
            storyControlledLaptop = false;
            CloseLaptopOnly();
            JumpScene(path);
        }
    }

    public void OpenNotesPanel()
    {
        SetActive(laptopUI, true);
        SetActive(homeScreen, true);
        CloseAllWindows();
        SetActive(notesPanel, true);
        RefreshNotesText();
        HideSceneDialogue();
    }

    public void AddNote(string note)
    {
        if (string.IsNullOrWhiteSpace(note))
            return;

        if (!aiiaNotes.Contains(note))
            aiiaNotes.Add(note);

        RefreshNotesText();
    }

    public void AddToAiiaArchive(string text)
    {
        AddNote(text);
    }

    private void RefreshNotesText()
    {
        if (notesText == null)
            return;

        if (aiiaNotes.Count == 0)
        {
            notesText.text = "Нотаток поки немає.";
            return;
        }

        notesText.text = "";
        foreach (string note in aiiaNotes)
            notesText.text += "• " + note + "\n\n";
    }

    public void RequestAiiaHint()
    {
        Mail mail = openedFileMail != null ? openedFileMail : currentMail;

        if (mail == null || !mail.hasAttachment)
        {
            ShowAiiaMessage("Поки що немає активного файла для підказки.", "neutral");
            return;
        }

        if (unlockedAttachments.Contains(mail))
        {
            ShowAiiaMessage("Цей файл уже відкритий. Його можна перечитати в архіві.", "smile");
            return;
        }

        ShowNextHint(mail);
    }

    private void ShowNextHint(Mail mail)
    {
        int index = shownHintsByMail.ContainsKey(mail) ? shownHintsByMail[mail] : 0;

        if (mail.aiiaHints == null || index >= mail.aiiaHints.Length)
        {
            ShowAiiaMessage(mail.aiiaNoMoreHintsText, mail.aiiaNoMoreHintsEmotion);
            return;
        }

        AiiaHint hint = mail.aiiaHints[index];
        shownHintsByMail[mail] = index + 1;

        if (hint == null || string.IsNullOrWhiteSpace(hint.text))
            ShowAiiaMessage(mail.aiiaNoMoreHintsText, mail.aiiaNoMoreHintsEmotion);
        else
            ShowAiiaMessage(hint.text, hint.emotionName);
    }

    public void ShowAiiaMessage(string message)
    {
        ShowAiiaMessage(message, "neutral");
    }

    public void ShowAiiaMessage(string message, string emotionName)
    {
        SetActive(aiiaPanel, true);

        if (aiiaText != null)
            aiiaText.text = "AIIA: " + message;

        SetAiiaEmotion(emotionName);
    }

    private void SetAiiaEmotion(string emotionName)
    {
        if (aiiaEmotionImage == null || aiiaCharacter == null)
            return;

        Sprite sprite = aiiaCharacter.GetEmotionSprite(string.IsNullOrWhiteSpace(emotionName) ? "neutral" : emotionName.Trim());

        if (sprite == null)
            sprite = aiiaCharacter.GetEmotionSprite("neutral");

        aiiaEmotionImage.sprite = sprite;
        aiiaEmotionImage.enabled = sprite != null;
    }

    private IEnumerator MailTimerRoutine(Mail mail)
    {
        int hours = Mathf.Max(0, mail.hours);
        int minutes = Mathf.Clamp(mail.minutes, 0, 59);
        int seconds = Mathf.Clamp(mail.seconds, 0, 59);
        float displayTime = Mathf.Max(0f, mail.displayDuration);

        while (displayTime > 0f)
        {
            SetMailTimerText(hours, minutes, seconds, mail.textColor);
            yield return new WaitForSecondsRealtime(1f);

            displayTime -= 1f;
            seconds--;

            if (seconds < 0)
            {
                seconds = 59;
                minutes--;
            }

            if (minutes < 0)
            {
                minutes = 59;
                hours--;
            }

            if (hours < 0)
            {
                hours = 0;
                minutes = 0;
                seconds = 0;
            }
        }

        timerRoutine = null;

        if (mail != null)
        {
            if (mail.hasAttachment)
                AddFileToAiiaArchive(mail);

            inboxMails.Remove(mail);
            openedMails.Add(mail);
            RefreshInboxList();
        }

        if (autoCloseLaptopWhenTimerEnds)
            CloseLaptopOnly();

        bool wasStoryControlled = storyControlledLaptop;
        storyControlledLaptop = false;

        if (!string.IsNullOrWhiteSpace(nextInkPathAfterMail))
        {
            string path = nextInkPathAfterMail;
            nextInkPathAfterMail = "";
            JumpScene(path);
        }
        else if (!wasStoryControlled)
        {
            ContinueSceneStory();
        }
    }

    private void SetMailTimerText(int h, int m, int s, Color color)
    {
        if (mailTimerText == null)
            return;

        mailTimerText.gameObject.SetActive(true);
        mailTimerText.text = FormatTime(h, m, s);
        mailTimerText.color = color;
    }

    private string FormatTime(int h, int m, int s)
    {
        return $"{h:00}:{m:00}:{s:00}";
    }

    private void ClearMailTimer()
    {
        if (mailTimerText == null)
            return;

        mailTimerText.text = "";
        mailTimerText.gameObject.SetActive(false);
    }

    private void StopTimer()
    {
        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }
    }

    private int GetPasswordMistakes(Mail mail)
    {
        return passwordMistakesByMail.ContainsKey(mail) ? passwordMistakesByMail[mail] : 0;
    }

    private void RefreshPasswordAttemptsText(Mail mail)
    {
        if (passwordAttemptsText == null)
            return;

        if (mail == null || unlockedAttachments.Contains(mail))
        {
            passwordAttemptsText.text = "";
            return;
        }

        if (mail.maxPasswordAttempts <= 0)
        {
            passwordAttemptsText.text = "";
            return;
        }

        int left = Mathf.Max(0, mail.maxPasswordAttempts - GetPasswordMistakes(mail));
        passwordAttemptsText.text = "Спроб залишилось: " + left;
    }

    private string NormalizePassword(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        return value.Trim().ToLower().Replace(" ", "").Replace("'", "").Replace("’", "");
    }

    private void StartAiiaFacts()
    {
        if (factRoutine == null)
            factRoutine = StartCoroutine(RandomFactRoutine());
    }

    private void StopAiiaFacts()
    {
        if (factRoutine != null)
        {
            StopCoroutine(factRoutine);
            factRoutine = null;
        }
    }

    private IEnumerator RandomFactRoutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(randomFactDelay);

            if (randomFacts != null && randomFacts.Length > 0 && IsLaptopOpen())
                ShowAiiaMessage(randomFacts[Random.Range(0, randomFacts.Length)], "neutral");
        }
    }

    public void SetMailIconNew()
    {
        if (mailIconImage != null && mailIconNew != null)
            mailIconImage.sprite = mailIconNew;
    }

    public void SetMailIconNormal()
    {
        if (mailIconImage != null && mailIconNormal != null)
            mailIconImage.sprite = mailIconNormal;
    }

    private void JumpScene(string path)
    {
        if (scene1Controller != null)
            scene1Controller.ChooseInkPath(path);
        else if (scene2Controller != null)
            scene2Controller.JumpToKnot(path);
    }

    private void HideSceneDialogue()
    {
        if (scene1Controller != null)
            scene1Controller.HideDialogue();

        if (scene2Controller != null)
            scene2Controller.HideDialogue();
    }

    private void ShowSceneDialogue()
    {
        if (scene1Controller != null)
            scene1Controller.ShowDialogue();

        if (scene2Controller != null)
            scene2Controller.ShowDialogue();
    }

    private void ContinueSceneStory()
    {
        if (scene1Controller != null)
            scene1Controller.ContinueStory();

        if (scene2Controller != null)
            scene2Controller.ContinueStory();
    }

    private void SetInkBool(string variableName, bool value)
    {
        if (story != null && story.variablesState.Contains(variableName))
            story.variablesState[variableName] = value;
    }

    private void SetInkInt(string variableName, int value)
    {
        if (story != null && story.variablesState.Contains(variableName))
            story.variablesState[variableName] = value;
    }

    private void ClearChildren(Transform parent)
    {
        if (parent == null)
            return;

        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    private void SetActive(GameObject obj, bool value)
    {
        if (obj != null)
            obj.SetActive(value);
    }
}
