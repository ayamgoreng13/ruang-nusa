using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("All Quests in Game")]
    public List<QuestSO> allQuests;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartQuest(QuestSO quest)
    {
        if (quest != null && quest.status == QuestStatus.NotStarted)
        {
            quest.status = QuestStatus.InProgress;
            Debug.Log("Quest Dimulai: " + quest.questName);
        }
    }

    public void CompleteQuest(QuestSO quest)
    {
        if (quest != null && quest.status == QuestStatus.InProgress)
        {
            quest.status = QuestStatus.Completed;
            Debug.Log("Quest Selesai: " + quest.questName);
        }
    }

    public QuestStatus GetQuestStatus(QuestSO quest)
    {
        return quest != null ? quest.status : QuestStatus.NotStarted;
    }
}