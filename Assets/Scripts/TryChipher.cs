using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TryCipher : MonoBehaviour
{
    private const string UpperAlphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ";
    private const string LowerAlphabet = "абвгґдеєжзиіїйклмнопрстуфхцчшщьюя";
    private const string PolybiusAlphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ!.)";
    private readonly string[] UkrainianFrequencyHints = { "О", "А", "І", "Н", "Т", "Е", "Р", "С", "В", "Л" };

    [Header("Panels")]
    [SerializeField] private GameObject caesarPanel;
    [SerializeField] private GameObject vigenerePanel;
    [SerializeField] private GameObject polybiusPanel;
    [SerializeField] private GameObject frequencyPanel;
    [SerializeField] private GameObject bookPanel;

    [Header("Caesar UI")]
    [SerializeField] private TMP_InputField caesarInputText;
    [SerializeField] private TMP_Text caesarShiftText;
    [SerializeField] private TMP_Text caesarResultText;
    [SerializeField] private Button caesarEncryptButton;
    [SerializeField] private Button caesarDecryptButton;
    [SerializeField] private Button caesarBackButton;
    [SerializeField] private Button caesarShiftLeftButton;
    [SerializeField] private Button caesarShiftRightButton;
    [SerializeField] private int caesarShift = 3;

    [Header("Caesar Wheel Visual")]
    [Tooltip("Сюди перетягни внутрішнє кільце Цезаря, яке має обертатися.")]
    [SerializeField] private RectTransform caesarInnerRing;
    [Tooltip("Якщо кільце крутиться не в той бік, увімкни/вимкни цей параметр.")]
    [SerializeField] private bool invertCaesarRingRotation = false;
    [Tooltip("Український алфавіт має 33 літери, тому один крок = 360 / 33.")]
    [SerializeField] private int caesarAlphabetSize = 33;
    [Tooltip("Початковий кут кільця, якщо спрайт намальований не з нульового положення.")]
    [SerializeField] private float caesarRingBaseAngle = 0f;

    [Header("Vigenere UI")]
    [SerializeField] private TMP_InputField vigenereInputText;
    [SerializeField] private TMP_InputField vigenereKeyInput;
    [SerializeField] private TMP_Text vigenereResultText;
    [SerializeField] private TMP_Text vigenereRepeatedKeyText;
    [SerializeField] private TMP_Text vigenereAlphabetText;
    [SerializeField] private TMP_Text vigenereAlphabetResultText;
    [SerializeField] private Button vigenereEncryptButton;
    [SerializeField] private Button vigenereDecryptButton;
    [SerializeField] private Button vigenereBackButton;

    [Header("Polybius UI")]
    [SerializeField] private TMP_InputField polybiusInputText;
    [SerializeField] private TMP_Text polybiusResultText;
    [SerializeField] private TMP_Text[] polybiusCellTexts;
    [SerializeField] private Button polybiusEncryptButton;
    [SerializeField] private Button polybiusDecryptButton;
    [SerializeField] private Button polybiusBackButton;

    [Header("Frequency Analysis UI")]
    [SerializeField] private TMP_InputField frequencyCipherInput;
    [SerializeField] private TMP_Text frequencyResultText;
    [SerializeField] private TMP_InputField frequencyResultInput;
    [SerializeField] private TMP_Text[] frequencyLetters;
    [SerializeField] private TMP_Text[] frequencyPercents;
    [SerializeField] private TMP_InputField[] replacementFromInputs;
    [SerializeField] private TMP_Text[] replacementToTexts;
    [SerializeField] private Button frequencyUpdateButton;
    [SerializeField] private Button frequencyResetButton;
    [SerializeField] private Button frequencyBackButton;

    private void Start()
    {
        CloseAllPanels();
        FillPolybiusGrid();
        NormalizeCaesarShift();
        RefreshCaesarShiftText();
        RefreshCaesarWheel();
        RefreshVigenereAlphabetPreview();
        SetupButtons();
    }

    private void SetupButtons()
    {
        Connect(caesarEncryptButton, EncryptCaesar);
        Connect(caesarDecryptButton, DecryptCaesar);
        Connect(caesarBackButton, BackToBook);
        Connect(caesarShiftLeftButton, DecreaseCaesarShift);
        Connect(caesarShiftRightButton, IncreaseCaesarShift);

        Connect(vigenereEncryptButton, EncryptVigenere);
        Connect(vigenereDecryptButton, DecryptVigenere);
        Connect(vigenereBackButton, BackToBook);

        Connect(polybiusEncryptButton, EncodePolybius);
        Connect(polybiusDecryptButton, DecodePolybius);
        Connect(polybiusBackButton, BackToBook);

        Connect(frequencyUpdateButton, UpdateFrequencyAnalysis);
        Connect(frequencyResetButton, ResetFrequencyAnalysis);
        Connect(frequencyBackButton, BackToBook);
    }

    private void Connect(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    public void OpenCaesar()
    {
        OpenOnly(caesarPanel);
        NormalizeCaesarShift();
        RefreshCaesarShiftText();
        RefreshCaesarWheel();
    }

    public void OpenVigenere()
    {
        OpenOnly(vigenerePanel);
        RefreshVigenereAlphabetPreview();
    }

    public void OpenPolybius()
    {
        OpenOnly(polybiusPanel);
        FillPolybiusGrid();
    }

    public void OpenFrequency()
    {
        OpenOnly(frequencyPanel);
    }

    public void CloseAllPanels()
    {
        SetActive(caesarPanel, false);
        SetActive(vigenerePanel, false);
        SetActive(polybiusPanel, false);
        SetActive(frequencyPanel, false);
    }

    public void BackToBook()
    {
        CloseAllPanels();
        SetActive(bookPanel, true);
    }

    private void OpenOnly(GameObject panel)
    {
        CloseAllPanels();
        SetActive(bookPanel, false);
        SetActive(panel, true);
    }

    public void IncreaseCaesarShift()
    {
        caesarShift++;
        NormalizeCaesarShift();
        RefreshCaesarShiftText();
        RefreshCaesarWheel();
    }

    public void DecreaseCaesarShift()
    {
        caesarShift--;
        NormalizeCaesarShift();
        RefreshCaesarShiftText();
        RefreshCaesarWheel();
    }

    private void NormalizeCaesarShift()
    {
        int size = Mathf.Max(1, caesarAlphabetSize);
        caesarShift %= size;

        if (caesarShift < 0)
            caesarShift += size;
    }

    private void RefreshCaesarShiftText()
    {
        SetText(caesarShiftText, caesarShift.ToString());
    }

    private void RefreshCaesarWheel()
    {
        if (caesarInnerRing == null)
            return;

        int size = Mathf.Max(1, caesarAlphabetSize);
        float stepAngle = 360f / size;
        float direction = invertCaesarRingRotation ? 1f : -1f;
        float angle = caesarRingBaseAngle + direction * caesarShift * stepAngle;

        caesarInnerRing.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void EncryptCaesar()
    {
        string input = caesarInputText != null ? caesarInputText.text : "";
        SetText(caesarResultText, CaesarShift(input, caesarShift));
        RefreshCaesarWheel();
    }

    public void DecryptCaesar()
    {
        string input = caesarInputText != null ? caesarInputText.text : "";
        SetText(caesarResultText, CaesarShift(input, -caesarShift));
        RefreshCaesarWheel();
    }

    private string CaesarShift(string input, int shift)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        char[] result = new char[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            int upperIndex = UpperAlphabet.IndexOf(c);

            if (upperIndex >= 0)
            {
                result[i] = ShiftChar(UpperAlphabet, upperIndex, shift);
                continue;
            }

            int lowerIndex = LowerAlphabet.IndexOf(c);

            if (lowerIndex >= 0)
            {
                result[i] = ShiftChar(LowerAlphabet, lowerIndex, shift);
                continue;
            }

            result[i] = c;
        }

        return new string(result);
    }

    private char ShiftChar(string alphabet, int index, int shift)
    {
        int length = alphabet.Length;
        int newIndex = (index + shift) % length;

        if (newIndex < 0)
            newIndex += length;

        return alphabet[newIndex];
    }

    public void EncryptVigenere()
    {
        string input = vigenereInputText != null ? vigenereInputText.text : "";
        string key = vigenereKeyInput != null ? vigenereKeyInput.text : "";

        SetText(vigenereResultText, Vigenere(input, key, true));
        RefreshVigenereKeyPreview(input, key);
    }

    public void DecryptVigenere()
    {
        string input = vigenereInputText != null ? vigenereInputText.text : "";
        string key = vigenereKeyInput != null ? vigenereKeyInput.text : "";

        SetText(vigenereResultText, Vigenere(input, key, false));
        RefreshVigenereKeyPreview(input, key);
    }

    private string Vigenere(string input, string key, bool encrypt)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";

        key = NormalizeLettersOnly(key).ToUpper();

        if (string.IsNullOrEmpty(key))
            return "Введіть ключ.";

        List<char> result = new List<char>();
        int keyIndex = 0;

        foreach (char c in input)
        {
            bool isUpper = UpperAlphabet.Contains(c);
            bool isLower = LowerAlphabet.Contains(c);

            if (!isUpper && !isLower)
            {
                result.Add(c);
                continue;
            }

            string alphabet = isUpper ? UpperAlphabet : LowerAlphabet;
            int charIndex = alphabet.IndexOf(c);
            int keyShift = UpperAlphabet.IndexOf(key[keyIndex % key.Length]);

            if (keyShift < 0)
                keyShift = 0;

            int shift = encrypt ? keyShift : -keyShift;
            result.Add(ShiftChar(alphabet, charIndex, shift));
            keyIndex++;
        }

        return new string(result.ToArray());
    }

    private string NormalizeLettersOnly(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        string result = "";

        foreach (char c in value)
        {
            char up = char.ToUpper(c);

            if (UpperAlphabet.Contains(up))
                result += up;
        }

        return result;
    }

    private void RefreshVigenereKeyPreview(string input, string key)
    {
        if (vigenereRepeatedKeyText == null)
            return;

        key = NormalizeLettersOnly(key).ToUpper();

        if (string.IsNullOrEmpty(key))
        {
            vigenereRepeatedKeyText.text = "";
            return;
        }

        string repeated = "";
        int index = 0;

        foreach (char c in input)
        {
            char up = char.ToUpper(c);

            if (UpperAlphabet.Contains(up))
            {
                repeated += key[index % key.Length];
                index++;
            }
            else
            {
                repeated += c;
            }
        }

        vigenereRepeatedKeyText.text = repeated;
    }

    private void RefreshVigenereAlphabetPreview()
    {
        SetText(vigenereAlphabetText, UpperAlphabet);
        SetText(vigenereAlphabetResultText, "");
    }

    private void FillPolybiusGrid()
    {
        if (polybiusCellTexts == null)
            return;

        for (int i = 0; i < polybiusCellTexts.Length; i++)
        {
            if (polybiusCellTexts[i] == null)
                continue;

            polybiusCellTexts[i].text = i < PolybiusAlphabet.Length ? PolybiusAlphabet[i].ToString() : "";
        }
    }

    public void EncodePolybius()
    {
        string input = polybiusInputText != null ? polybiusInputText.text : "";
        List<string> codes = new List<string>();

        foreach (char rawChar in input)
        {
            char c = char.ToUpper(rawChar);

            if (c == ' ')
            {
                codes.Add("/");
                continue;
            }

            int index = PolybiusAlphabet.IndexOf(c);

            if (index < 0)
                continue;

            int row = index / 6 + 1;
            int col = index % 6 + 1;

            codes.Add(row.ToString() + col.ToString());
        }

        SetText(polybiusResultText, string.Join(" ", codes));
    }

    public void DecodePolybius()
    {
        string input = polybiusInputText != null ? polybiusInputText.text : "";
        string[] parts = input.Split(' ', '\n', '\r', '\t');
        string result = "";

        foreach (string part in parts)
        {
            if (string.IsNullOrWhiteSpace(part))
                continue;

            if (part == "/")
            {
                result += " ";
                continue;
            }

            if (part.Length != 2 || !char.IsDigit(part[0]) || !char.IsDigit(part[1]))
                continue;

            int row = part[0] - '0';
            int col = part[1] - '0';

            if (row < 1 || row > 6 || col < 1 || col > 6)
                continue;

            int index = (row - 1) * 6 + (col - 1);

            if (index >= 0 && index < PolybiusAlphabet.Length)
                result += PolybiusAlphabet[index];
        }

        SetText(polybiusResultText, result);
    }

    public void UpdateFrequencyAnalysis()
    {
        string text = frequencyCipherInput != null ? frequencyCipherInput.text : "";
        Dictionary<char, int> counts = CountLetters(text);
        int total = counts.Values.Sum();
        List<KeyValuePair<char, int>> ordered = counts.OrderByDescending(pair => pair.Value).ToList();

        FillFrequencyColumns(ordered, total);
        FillAutomaticFrequencyHypotheses(ordered);
        ApplyFrequencyReplacements();
    }

    private void FillFrequencyColumns(List<KeyValuePair<char, int>> ordered, int total)
    {
        int rows = frequencyLetters != null ? frequencyLetters.Length : 0;

        for (int i = 0; i < rows; i++)
        {
            TMP_Text letterTarget = frequencyLetters[i];
            TMP_Text percentTarget = frequencyPercents != null && i < frequencyPercents.Length ? frequencyPercents[i] : null;

            if (i < ordered.Count && total > 0)
            {
                char letter = ordered[i].Key;
                int percent = Mathf.RoundToInt((ordered[i].Value / (float)total) * 100f);

                SetText(letterTarget, letter.ToString());
                SetText(percentTarget, percent.ToString());
            }
            else
            {
                SetText(letterTarget, "");
                SetText(percentTarget, "");
            }
        }
    }

    private void FillAutomaticFrequencyHypotheses(List<KeyValuePair<char, int>> ordered)
    {
        int rows = Mathf.Min(
            replacementFromInputs != null ? replacementFromInputs.Length : 0,
            replacementToTexts != null ? replacementToTexts.Length : 0
        );

        for (int i = 0; i < rows; i++)
        {
            if (replacementFromInputs[i] != null)
                replacementFromInputs[i].text = i < ordered.Count ? ordered[i].Key.ToString() : "";

            SetText(replacementToTexts[i], i < UkrainianFrequencyHints.Length && i < ordered.Count ? UkrainianFrequencyHints[i] : "");
        }
    }

    private Dictionary<char, int> CountLetters(string text)
    {
        Dictionary<char, int> counts = new Dictionary<char, int>();

        foreach (char rawChar in text)
        {
            char c = char.ToUpper(rawChar);

            if (!UpperAlphabet.Contains(c))
                continue;

            if (!counts.ContainsKey(c))
                counts[c] = 0;

            counts[c]++;
        }

        return counts;
    }

    private void ApplyFrequencyReplacements()
    {
        string text = frequencyCipherInput != null ? frequencyCipherInput.text : "";
        Dictionary<char, char> replacements = new Dictionary<char, char>();

        int count = Mathf.Min(
            replacementFromInputs != null ? replacementFromInputs.Length : 0,
            replacementToTexts != null ? replacementToTexts.Length : 0
        );

        for (int i = 0; i < count; i++)
        {
            string from = replacementFromInputs[i] != null ? replacementFromInputs[i].text.Trim().ToUpper() : "";
            string to = replacementToTexts[i] != null ? replacementToTexts[i].text.Trim().ToUpper() : "";

            if (from.Length == 0 || to.Length == 0)
                continue;

            char fromChar = from[0];
            char toChar = to[0];

            if (UpperAlphabet.Contains(fromChar) && UpperAlphabet.Contains(toChar))
                replacements[fromChar] = toChar;
        }

        string result = "";

        foreach (char rawChar in text)
        {
            char upper = char.ToUpper(rawChar);

            if (replacements.ContainsKey(upper))
                result += replacements[upper];
            else if (UpperAlphabet.Contains(upper))
                result += "_";
            else
                result += rawChar;
        }

        SetText(frequencyResultText, result);

        if (frequencyResultInput != null)
            frequencyResultInput.text = result;
    }

    public void ResetFrequencyAnalysis()
    {
        if (frequencyCipherInput != null)
            frequencyCipherInput.text = "";

        ClearInputFields(replacementFromInputs);
        ClearTexts(replacementToTexts);
        ClearTexts(frequencyLetters);
        ClearTexts(frequencyPercents);

        SetText(frequencyResultText, "");

        if (frequencyResultInput != null)
            frequencyResultInput.text = "";
    }

    private void ClearInputFields(TMP_InputField[] inputs)
    {
        if (inputs == null)
            return;

        foreach (TMP_InputField input in inputs)
        {
            if (input != null)
                input.text = "";
        }
    }

    private void ClearTexts(TMP_Text[] texts)
    {
        if (texts == null)
            return;

        foreach (TMP_Text text in texts)
            SetText(text, "");
    }

    private void SetText(TMP_Text target, string value)
    {
        if (target != null)
            target.text = value;
    }

    private void SetActive(GameObject obj, bool value)
    {
        if (obj != null)
            obj.SetActive(value);
    }
}
