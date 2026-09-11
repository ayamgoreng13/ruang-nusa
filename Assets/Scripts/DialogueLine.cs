using System.Collections.Generic;
using UnityEngine;

public enum DialogueType
{
    Default,
    Optional
}

[System.Serializable]
public class DialogueLine
{
    public DialogueType dialogueType = DialogueType.Default;

    [Header("Default Dialogue Data")]
    public string speakerName;
    
    [TextArea(2, 5)]
    public string sentence;
    
    public Sprite leftPortrait;
    public Sprite rightPortrait;
    public bool isLeftSpeaker;

    [Header("Optional Dialogue Data")]
    public List<DialogueChoice> choices;
}