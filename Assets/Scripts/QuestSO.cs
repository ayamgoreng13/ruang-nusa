using UnityEngine;

public enum QuestStatus { NotStarted, InProgress, Completed }

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest")]
public class QuestSO : ScriptableObject
{
    public string questID;
    public string questName;
    [TextArea(2, 4)]
    public string description;

    [Header("Runtime State")]
    public QuestStatus status = QuestStatus.NotStarted;

    // Reset status saat game dimulai (opsional untuk testing)
    private void OnEnable()
    {
        status = QuestStatus.NotStarted;
    }
}