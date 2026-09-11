using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Elements")]
    public GameObject dialogueBox;
    public GameObject darkOverlay;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image leftPortraitImage;
    public Image rightPortraitImage;

    [Header("Choice UI Elements")]
    public GameObject choicePanel;
    public GameObject choiceButtonPrefab;
    public Transform choiceButtonContainer;

    [Header("Settings")]
    public float typingSpeed = 0.03f;

    private Queue<DialogueLine> linesQueue = new Queue<DialogueLine>();
    private bool isTyping;
    private string currentSentence;
    private Coroutine typingCoroutine;

    private DialogueLine currentLine;
    private bool isWaitingForChoice;
    private bool isProcessingChoice;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        EndDialogue();
    }

    public void StartDialogue(List<DialogueLine> lines)
    {
        Time.timeScale = 0f;

        if (dialogueBox != null) dialogueBox.SetActive(true);
        if (darkOverlay != null) darkOverlay.SetActive(true);

        linesQueue.Clear();
        foreach (DialogueLine line in lines)
        {
            linesQueue.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
    if (isWaitingForChoice || isProcessingChoice) return;

    // Jika teks masih diketik dan pemain menekan E/Klik, langsung tampilkan teks utuh
    if (isTyping)
        {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = currentSentence;
        isTyping = false;
        return;
        }

    if (linesQueue.Count == 0)
        {
        EndDialogue();
        return;
        }

    currentLine = linesQueue.Dequeue();

    // PERCABANGAN BERDASARKAN TIPE DIALOGUE
    if (currentLine.dialogueType == DialogueType.Optional)
        {
        ShowChoices(currentLine.choices);
        }
    else
        {
        // Default Dialogue
        nameText.text = currentLine.speakerName;
        currentSentence = currentLine.sentence;
        SetupPortraits(currentLine);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        isTyping = true;

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    void ShowChoices(List<DialogueChoice> choices)
    {
        if (choiceButtonContainer == null || choiceButtonPrefab == null) return;

        isWaitingForChoice = true;
        if (choicePanel != null) choicePanel.SetActive(true);

        foreach (Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (DialogueChoice choice in choices)
        {
            if (choice == null) continue;

            GameObject btnObj = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null) btnText.text = choice.choiceText;

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null) btn.onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    void OnChoiceSelected(DialogueChoice choice)
    {
        if (isProcessingChoice) return;
        StartCoroutine(HandleChoiceSelectionRoutine(choice));
    }

    IEnumerator HandleChoiceSelectionRoutine(DialogueChoice choice)
    {
    isProcessingChoice = true;
    isWaitingForChoice = false;
    if (choicePanel != null) choicePanel.SetActive(false);

    yield return null; // Jeda 1 frame dari klik UI

    // 1. UCAPAN LARAS (MC)
    if (!string.IsNullOrEmpty(choice.mcResponseSentence))
        {
        nameText.text = "LARAS";

        // Laras di Left Portrait
        if (leftPortraitImage != null) leftPortraitImage.color = Color.white;
        if (rightPortraitImage != null) rightPortraitImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);

        currentSentence = choice.mcResponseSentence;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
        yield return typingCoroutine;

        // Menunggu input E / Klik dari pemain
        yield return WaitForPlayerInput();
        }

    // 2. BALASAN NPC
    if (!string.IsNullOrEmpty(choice.singleResponseSentence))
        {
        nameText.text = string.IsNullOrEmpty(choice.npcSpeakerName) ? "NPC" : choice.npcSpeakerName;

        // Ganti sprite NPC jika diset di choice
        if (choice.npcPortrait != null)
            {
            if (choice.isNpcOnLeft && leftPortraitImage != null) leftPortraitImage.sprite = choice.npcPortrait;
            else if (!choice.isNpcOnLeft && rightPortraitImage != null) rightPortraitImage.sprite = choice.npcPortrait;
            }

        // Highlight NPC
        if (choice.isNpcOnLeft)
            {
            if (leftPortraitImage != null) leftPortraitImage.color = Color.white;
            if (rightPortraitImage != null) rightPortraitImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);
            }
        else
            {
            if (rightPortraitImage != null) rightPortraitImage.color = Color.white;
            if (leftPortraitImage != null) leftPortraitImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);
            }

        currentSentence = choice.singleResponseSentence;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
        yield return typingCoroutine;

        // Menunggu input E / Klik setelah NPC selesai ngomong
        yield return WaitForPlayerInput();
        }

    // 3. ALUR LANJUTAN (EXIT vs LOOP)
    if (choice.isExitChoice)
        {
        // Berhenti di sini, siap masuk ke Default Dialogue berikutnya saat pencet E lagi di luar
        isProcessingChoice = false;
        }
    else
        {
        // Setelah tombol E ditekan, baru balik nampilin menu opsi pilihan
        ShowChoices(currentLine.choices);
        isProcessingChoice = false;
        }
    }

    IEnumerator WaitForPlayerInput()
    {
        yield return null;
        while (!Input.GetKeyDown(KeyCode.E) && !Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
    }

    void SetupPortraits(DialogueLine line)
    {
        if (line.leftPortrait != null)
        {
            leftPortraitImage.gameObject.SetActive(true);
            leftPortraitImage.sprite = line.leftPortrait;
            leftPortraitImage.color = line.isLeftSpeaker ? Color.white : new Color(0.4f, 0.4f, 0.4f, 1f);
        }
        else if (leftPortraitImage != null) leftPortraitImage.gameObject.SetActive(false);

        if (line.rightPortrait != null)
        {
            rightPortraitImage.gameObject.SetActive(true);
            rightPortraitImage.sprite = line.rightPortrait;
            rightPortraitImage.color = !line.isLeftSpeaker ? Color.white : new Color(0.4f, 0.4f, 0.4f, 1f);
        }
        else if (rightPortraitImage != null) rightPortraitImage.gameObject.SetActive(false);
    }

    void EndDialogue()
    {
        Time.timeScale = 1f;
        isWaitingForChoice = false;
        isProcessingChoice = false;

        if (dialogueBox != null) dialogueBox.SetActive(false);
        if (darkOverlay != null) darkOverlay.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
        if (leftPortraitImage != null) leftPortraitImage.gameObject.SetActive(false);
        if (rightPortraitImage != null) rightPortraitImage.gameObject.SetActive(false);
    }
}