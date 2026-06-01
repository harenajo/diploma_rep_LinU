using Ink.Runtime;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomController : MonoBehaviour
{
    [Header("Main Room Hub")]
    [SerializeField] private GameObject roomHubPanel;
    [SerializeField] private TMP_Text roomInfoText;

    [Header("Room Buttons")]
    [SerializeField] private Button laptopButton;
    [SerializeField] private Button bookshelfButton;
    [SerializeField] private Button clockButton;
    [SerializeField] private Button deskButton;
    [SerializeField] private Button bedButton;

    [Header("Laptop Panel To Open")]
    [Tooltip("Сюди можна перетягнути верхню панель ноутбука Laptop. Кнопка ноутбука в кімнаті відкриватиме саме її.")]
    [SerializeField] private GameObject laptopPanel;
    [Tooltip("Сюди перетягни homescreen, якщо хочеш одразу показувати робочий стіл ноутбука.")]
    [SerializeField] private GameObject laptopHomeScreen;
    [Tooltip("Не обов'язково. Якщо перетягнути LaptopController, він відкриватиме ноутбук через його логіку.")]
    [SerializeField] private LaptopController laptopController;

    [Header("Book Panel")]
    [SerializeField] private GameObject bookPanel;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button closeBookButton;

    [Header("Book Spreads")]
    [Tooltip("Додай сюди тільки Розворот1, Розворот2, Розворот3... Не додавай окремі сторінки.")]
    [SerializeField] private GameObject[] bookSpreads;
    [SerializeField] private bool resetBookToFirstSpreadWhenOpened = false;

    [Header("Clock / Next Day")]
    [SerializeField] private GameObject clockConfirmPanel;
    [SerializeField] private TMP_Text clockConfirmText;
    [SerializeField] private Button clockStartNextDayButton;
    [SerializeField] private Button clockCancelButton;
    [SerializeField] private int fallbackNextSceneIndex = 3;

    [Header("Controllers")]
    [SerializeField] private TryCipher cipherTryController;

    [Header("Page Click Protection")]
    [SerializeField] private float pageClickCooldown = 0.15f;

    private Story story;
    private Scene1 scene1Controller;
    private Scene2 scene2Controller;

    private int currentSpreadIndex = 0;
    private bool bookshelfAvailable;
    private bool bookshelfAlreadyChecked;
    private bool deskAlreadyChecked;
    private bool bedAlreadyChecked;
    private string nextDayInkPath = "end_of_day";
    private float lastPageClickTime = -999f;

    public void SetStory(Story inkStory)
    {
        story = inkStory;
    }

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

    private void Awake()
    {
        SetupButtons();
    }

    private void Start()
    {
        SetActive(bookPanel, false);
        SetActive(clockConfirmPanel, false);

        if (cipherTryController != null)
            cipherTryController.CloseAllPanels();

        ShowBookSpread(0);
    }

    private void SetupButtons()
    {
        Connect(laptopButton, OnLaptopButtonClick);
        Connect(bookshelfButton, OnBookshelfButtonClick);
        Connect(clockButton, OnClockButtonClick);
        Connect(deskButton, OnDeskButtonClick);
        Connect(bedButton, OnBedButtonClick);

        Connect(previousPageButton, PreviousBookPage);
        Connect(nextPageButton, NextBookPage);
        Connect(closeBookButton, CloseBookPanel);

        Connect(clockStartNextDayButton, StartNextDayFromClock);
        Connect(clockCancelButton, CloseClockConfirm);
    }

    private void Connect(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    public void OpenRoomHub()
    {
        SetActive(roomHubPanel, true);
        SetActive(bookPanel, false);
        SetActive(clockConfirmPanel, false);

        if (cipherTryController != null)
            cipherTryController.CloseAllPanels();

        SetRoomInfo("Кімната доступна. Можна відкрити ноутбук, книгу з криптографії або годинник для переходу до наступного дня.");
        HideSceneDialogue();
    }

    public void CloseRoomHub()
    {
        SetActive(roomHubPanel, false);
        SetActive(bookPanel, false);
        SetActive(clockConfirmPanel, false);

        if (cipherTryController != null)
            cipherTryController.CloseAllPanels();

        ShowSceneDialogue();
    }

    public void SetBookshelfAvailable(bool value)
    {
        bookshelfAvailable = value;
    }

    public void SetNextDayInkPath(string path)
    {
        if (!string.IsNullOrWhiteSpace(path))
            nextDayInkPath = path.Trim();
    }

    public void OnLaptopButtonClick()
    {
        SetActive(roomHubPanel, false);
        SetActive(bookPanel, false);
        SetActive(clockConfirmPanel, false);

        if (cipherTryController != null)
            cipherTryController.CloseAllPanels();

        if (laptopController != null)
        {
            laptopController.OpenLaptopFromRoom();
        }
        else
        {
            SetActive(laptopPanel, true);

            if (laptopPanel != null)
                laptopPanel.transform.SetAsLastSibling();

            SetActive(laptopHomeScreen, true);
        }

        SetRoomInfo("Ноутбук відкрито.");
        HideSceneDialogue();
    }

    public void OnBookshelfButtonClick()
    {
        SetActive(bookPanel, true);
        SetActive(clockConfirmPanel, false);

        if (cipherTryController != null)
            cipherTryController.CloseAllPanels();

        if (resetBookToFirstSpreadWhenOpened)
            currentSpreadIndex = 0;

        ShowBookSpread(currentSpreadIndex);

        if (bookshelfAvailable && !bookshelfAlreadyChecked)
        {
            bookshelfAlreadyChecked = true;

            if (laptopController != null)
                laptopController.AddNote("У книзі знайдено розділи про шифр Цезаря, Полібія, Віженера та частотний аналіз.");

            SetInkBool("checked_bookshelf", true);
        }

        SetRoomInfo("Відкрита книга з криптографії.");
        HideSceneDialogue();
    }

    public void NextBookPage()
    {
        if (!CanTurnPageNow())
            return;

        ShowBookSpread(currentSpreadIndex + 1);
    }

    public void PreviousBookPage()
    {
        if (!CanTurnPageNow())
            return;

        ShowBookSpread(currentSpreadIndex - 1);
    }

    private bool CanTurnPageNow()
    {
        if (Time.unscaledTime - lastPageClickTime < pageClickCooldown)
            return false;

        lastPageClickTime = Time.unscaledTime;
        return true;
    }

    public void ShowBookSpread(int index)
    {
        if (bookSpreads == null || bookSpreads.Length == 0)
        {
            currentSpreadIndex = 0;
            SetButtonInteractable(previousPageButton, false);
            SetButtonInteractable(nextPageButton, false);
            return;
        }

        currentSpreadIndex = Mathf.Clamp(index, 0, bookSpreads.Length - 1);

        for (int i = 0; i < bookSpreads.Length; i++)
            SetActive(bookSpreads[i], i == currentSpreadIndex);

        SetButtonInteractable(previousPageButton, currentSpreadIndex > 0);
        SetButtonInteractable(nextPageButton, currentSpreadIndex < bookSpreads.Length - 1);
    }

    public void CloseBookPanel()
    {
        SetActive(bookPanel, false);

        if (cipherTryController != null)
            cipherTryController.CloseAllPanels();

        SetRoomInfo("Книга закрита.");
        HideSceneDialogue();
    }

    public void OnDeskButtonClick()
    {
        SetActive(bookPanel, false);
        SetActive(clockConfirmPanel, false);

        if (cipherTryController != null)
            cipherTryController.CloseAllPanels();

        if (!deskAlreadyChecked)
        {
            deskAlreadyChecked = true;
            SetInkBool("checked_desk", true);

            if (laptopController != null)
                laptopController.AddNote("Стіл оглянуто. Тут можна буде залишити майбутні предмети для загадок.");
        }

        SetRoomInfo("На столі лежать конспекти, зарядка й кілька дрібниць.");
        HideSceneDialogue();
    }

    public void OnBedButtonClick()
    {
        SetActive(bookPanel, false);
        SetActive(clockConfirmPanel, false);

        if (cipherTryController != null)
            cipherTryController.CloseAllPanels();

        if (!bedAlreadyChecked)
        {
            bedAlreadyChecked = true;
            SetInkBool("checked_bed", true);

            if (laptopController != null)
                laptopController.AddNote("Ліжко оглянуто. Нічого підозрілого, окрім явного недосипу.");
        }

        SetRoomInfo("Ліжко не виглядає підозріло.");
        HideSceneDialogue();
    }

    public void OnClockButtonClick()
    {
        SetActive(clockConfirmPanel, true);
        SetActive(bookPanel, false);

        if (cipherTryController != null)
            cipherTryController.CloseAllPanels();

        if (clockConfirmText != null)
            clockConfirmText.text = "Почати наступний день? Перед цим можна ще перевірити ноутбук, книгу або архів AIIA.";

        SetRoomInfo("Годинник завершує поточний день.");
        HideSceneDialogue();
    }

    public void CloseClockConfirm()
    {
        SetActive(clockConfirmPanel, false);
        SetRoomInfo("День ще не завершено.");
        HideSceneDialogue();
    }

    public void StartNextDayFromClock()
    {
        SetActive(clockConfirmPanel, false);
        SetActive(roomHubPanel, false);
        SetActive(bookPanel, false);

        if (cipherTryController != null)
            cipherTryController.CloseAllPanels();

        if (scene2Controller != null)
        {
            scene2Controller.FinishDayFromRoom(nextDayInkPath);
            return;
        }

        if (scene1Controller != null)
        {
            scene1Controller.ShowDialogue();
            return;
        }

        SceneManager.LoadScene(fallbackNextSceneIndex);
    }

    private void SetButtonInteractable(Button button, bool value)
    {
        if (button != null)
            button.interactable = value;
    }

    private void SetInkBool(string variableName, bool value)
    {
        if (story != null && story.variablesState.Contains(variableName))
            story.variablesState[variableName] = value;
    }

    private void SetRoomInfo(string text)
    {
        if (roomInfoText != null)
            roomInfoText.text = text;
    }

    private void HideSceneDialogue()
    {
        scene1Controller?.HideDialogue();
        scene2Controller?.HideDialogue();
    }

    private void ShowSceneDialogue()
    {
        scene1Controller?.ShowDialogue();
        scene2Controller?.ShowDialogue();
    }

    private void SetActive(GameObject obj, bool value)
    {
        if (obj != null)
            obj.SetActive(value);
    }
}
