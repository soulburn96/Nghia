using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTrigger : MonoBehaviour
{
    bool neverDone;
    public Dialogue dialogue;
    IEnumerator ToFar()
    {
        yield return new WaitForSeconds(3F);
        FindObjectOfType<DialogueSystem>().EndDialogue();

    }
    void Start()
    {
        neverDone = true;
    }
    void Update()
    {
        if (neverDone)
        {
            TriggerDialogue();
            neverDone = false;
        }
    }
  
    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueSystem>().StartDialogue(dialogue);
    }
}
