using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Visual Novel/Character")]
public class Character : ScriptableObject
{
    public string characterName;

    public Sprite fullbody;
    public Sprite neutral;
    public Sprite thinking;
    public Sprite smile;
    public Sprite angry;
    public Sprite other;
    public Sprite other2;
    public Sprite body1;
    public Sprite body2;
    public Sprite body3;
    public Sprite GetEmotionSprite(string emotionName)
    {
        switch (emotionName)
        {
            case "fullbody":
                return fullbody;

            case "neutral":
                return neutral;

            case "thinking":
                return thinking;

            case "smile":
                return smile;

            case "angry":
                return angry;

            case "other":
                return other;

            case "other2":
                return other2;

            case "body1":
                return body1;

            case "body2":
                return body2;
            case "body3":
                return body3;

            default:
                return neutral;
        }
    }
}