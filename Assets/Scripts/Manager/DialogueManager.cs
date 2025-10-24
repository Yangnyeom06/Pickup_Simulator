using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;      // 대화창 패널
    public TextMeshProUGUI dialogueText;  // 대사 텍스트

    [Header("Settings")]
    public float autoNextDelay = 3f;      // 다음 대사로 넘어가는 시간

    private Queue<SerialDialogueLine> lines;    // 대사 큐
    private float timer;                  // 경과 시간
    private bool isDialogueActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        lines = new Queue<SerialDialogueLine>();
        dialoguePanel.SetActive(false);
    }

    // 대화 시작
    public void StartSerialDialogue(List<SerialDialogueLine> SerialdialogueLines)
    {
        dialoguePanel.SetActive(true);
        lines.Clear();

        foreach (var line in SerialdialogueLines)
            lines.Enqueue(line);

        isDialogueActive = true;
        SerialDisplayNextLine();
    }

    // 매 프레임 체크해서 자동 진행
    private void Update()
    {
        if (!isDialogueActive) return;

        timer += Time.deltaTime;
        if (timer >= autoNextDelay)
        {
            SerialDisplayNextLine();
        }
    }

    // 다음 대사 출력
    public void SerialDisplayNextLine()
    {
        timer = 0f; // 타이머 리셋

        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        SerialDialogueLine line = lines.Dequeue();

        dialogueText.text = line.text;
    }

    public void StartDialogue(List<DialogueLine> DialogueLines, string DialogueName)
    {
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        timer = 0f;

        // 해당 이름의 대사를 찾아서 표시
        for (int i = 0; i < DialogueLines.Count; i++)
        {
            if (DialogueName == DialogueLines[i].name)
            {
                dialogueText.text = DialogueLines[i].text;
                break;
            }
        }

        // 자동으로 대화 종료 코루틴 실행
        StartCoroutine(AutoCloseDialogue());
    }

    // 일정 시간 후 대화 종료
    private IEnumerator AutoCloseDialogue()
    {
        yield return new WaitForSeconds(autoNextDelay); // autoNextDelay 후 실행
        EndDialogue();
    }



    // 대화 종료
    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
    }
}
