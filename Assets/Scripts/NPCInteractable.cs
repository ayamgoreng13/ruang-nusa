using System.Collections.Generic;
using UnityEngine;

public class InteractableNPC : MonoBehaviour
{
    public GameObject promptObject;

    [Header("Default Dialogue (Jika tidak ada quest aktif)")]
    public List<DialogueLine> defaultDialogue;

    [Header("Conditional Quest Dialogues")]
    public List<QuestDialogueBranch> questDialogues;

    private bool isPlayerInRange;
    private bool isTalking;

    void Start()
    {
        if (promptObject != null) promptObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                isTalking = true;
                if (promptObject != null) promptObject.SetActive(false);
                
                List<DialogueLine> selectedDialogue = GetCurrentDialogue();
                DialogueManager.Instance.StartDialogue(selectedDialogue);
            }
            else
            {
                DialogueManager.Instance.DisplayNextSentence();
            }
        }
    }

    List<DialogueLine> GetCurrentDialogue()
    {
        if (questDialogues != null)
        {
            foreach (var branch in questDialogues)
            {
                if (branch.relatedQuest != null && QuestManager.Instance != null &&
                    QuestManager.Instance.GetQuestStatus(branch.relatedQuest) == branch.requiredStatus)
                {
                    return branch.dialogueLines;
                }
            }
        }
        return defaultDialogue;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (promptObject != null && !isTalking) promptObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            isTalking = false;
            if (promptObject != null) promptObject.SetActive(false);
        }
    }
}

// Struct ditaruh di bawah class MonoBehaviour utama
[System.Serializable]
public struct QuestDialogueBranch
{
    public QuestSO relatedQuest;
    public QuestStatus requiredStatus;
    public List<DialogueLine> dialogueLines;
}