using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public List<SerialDialogueLine> SerialdialogueLines;
    public List<DialogueLine> DialogueLines;

    public void TriggerSerialDialogue()
    {
        DialogueManager.Instance.StartSerialDialogue(SerialdialogueLines);
    }

    public void TriggerDialouge(string DialougeName)
    {
        DialogueManager.Instance.StartDialogue(DialogueLines, DialougeName);
    }
}
