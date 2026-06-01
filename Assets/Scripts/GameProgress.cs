using Ink.Runtime;
using UnityEngine;

public static class GameProgress
{
    public const string ClassRoute = "class_route";
    public const string LevPoints = "lev_points";
    public const string StudyPoints = "study_points";
    public const string Fear = "fear";
    public const string ToldLevAboutMessage = "told_lev_about_message";
    public const string GotFirstMail = "got_first_mail";

    private static readonly string[] IntVariables =
    {
        "curiosity",
        "trust",
        "ariya_points",
        "lev_points",
        "roman_points",
        "markian_points",
        "study_points",
        "failed_cipher_attempts",
        "fear"
    };

    private static readonly string[] BoolVariables =
    {
        "class_route",
        "told_lev_about_message",
        "got_first_mail",
        "told_ariya_truth",
        "hinted_ariya",
        "hid_truth_from_ariya",
        "asked_ariya_about_book",
        "found_aiia_hint",
        "solved_cipher_2",
        "got_second_mail",
        "walked_with_ariya",
        "met_lev_on_walk",
        "checked_bookshelf",
        "checked_desk",
        "checked_bed",
        "opened_locked_file",
        "made_caesar_name_mistake"
    };

    public static int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static void AddInt(string key, int amount)
    {
        SetInt(key, GetInt(key) + amount);
    }

    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void LoadIntoStory(Story story)
    {
        if (story == null)
            return;

        foreach (string variableName in IntVariables)
        {
            if (HasVariable(story, variableName))
                story.variablesState[variableName] = GetInt(variableName, 0);
        }

        foreach (string variableName in BoolVariables)
        {
            if (HasVariable(story, variableName))
                story.variablesState[variableName] = GetBool(variableName, false);
        }
    }

    public static void SaveFromStory(Story story)
    {
        if (story == null)
            return;

        foreach (string variableName in IntVariables)
        {
            if (HasVariable(story, variableName))
                SetInt(variableName, ToInt(story.variablesState[variableName]));
        }

        foreach (string variableName in BoolVariables)
        {
            if (HasVariable(story, variableName))
                SetBool(variableName, ToBool(story.variablesState[variableName]));
        }

        PlayerPrefs.Save();
    }

    public static bool HasVariable(Story story, string variableName)
    {
        if (story == null || string.IsNullOrEmpty(variableName))
            return false;

        try
        {
            object value = story.variablesState[variableName];
            return value != null;
        }
        catch
        {
            return false;
        }
    }

    private static int ToInt(object value)
    {
        if (value is int intValue)
            return intValue;

        if (value is float floatValue)
            return Mathf.RoundToInt(floatValue);

        if (value is double doubleValue)
            return Mathf.RoundToInt((float)doubleValue);

        if (int.TryParse(value?.ToString(), out int parsed))
            return parsed;

        return 0;
    }

    private static bool ToBool(object value)
    {
        if (value is bool boolValue)
            return boolValue;

        if (value is int intValue)
            return intValue != 0;

        if (bool.TryParse(value?.ToString(), out bool parsed))
            return parsed;

        return false;
    }
}
