using UnityEngine;

[System.Serializable]
public class AiiaHint
{
    [TextArea(2, 6)]
    public string text;

    [Tooltip("Назва емоції з Character: neutral, thinking, smile, angry, other, other2, fullbody")]
    public string emotionName = "thinking";
}

[CreateAssetMenu(fileName = "Mail", menuName = "Scriptable Objects/Mail")]
public class Mail : ScriptableObject
{
    [Header("ID")]
    public string mailID;

    [Header("Content")]
    public string title;

    [TextArea(5, 30)]
    public string body;

    [Header("Appearance")]
    public Sprite backgroundImage;
    public Color textColor = Color.black;

    [Header("Attachment")]
    public bool hasAttachment;
    public string attachmentName = "file.txt";
    public Sprite attachmentIcon;

    [TextArea(3, 10)]
    public string attachmentLockedText = "Файл захищено паролем.";

    [TextArea(5, 30)]
    public string attachmentUnlockedText;

    [Header("Attachment Password")]
    public string attachmentPassword = "архів";

    [Tooltip("Скільки неправильних спроб дозволено. 0 = без обмеження.")]
    public int maxPasswordAttempts = 0;

    [Tooltip("Якщо true, після правильного пароля файл збережеться в AIIA архіві.")]
    public bool saveAttachmentToAiiaArchive = true;

    [Header("AIIA Dynamic Hints")]
    [Tooltip("Підказки показуються по черзі. Можна зробити 1 підказку або багато.")]
    public AiiaHint[] aiiaHints;

    [TextArea(2, 6)]
    public string aiiaNoMoreHintsText = "Більше підказок немає. Тепер тільки ти, текст і твоє терпіння.";

    [Tooltip("Назва емоції з Character")]
    public string aiiaNoMoreHintsEmotion = "neutral";

    [TextArea(2, 6)]
    public string aiiaFailedAttemptsHint = "Спроби закінчилися. Можна повернутися до листа й перечитати його уважніше.";

    [Tooltip("Назва емоції з Character")]
    public string aiiaWrongPasswordEmotion = "angry";

    [Tooltip("Назва емоції з Character")]
    public string aiiaUnlockedEmotion = "smile";

    [Header("Timer")]
    public bool hasTimer;

    public int hours;
    public int minutes;
    public int seconds;

    [Tooltip("Скільки секунд реально чекати перед продовженням сюжету")]
    public float displayDuration = 15f;
}
