using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    
    [TextArea(2, 4)]
    public string mcResponseSentence;
    
    [Header("Balasan NPC")]
    public string npcSpeakerName;
    public Sprite npcPortrait;
    public bool isNpcOnLeft;
    
    [TextArea(2, 4)]
    public string singleResponseSentence;

    [Header("Alur Dialog")]
    public bool isExitChoice;
}