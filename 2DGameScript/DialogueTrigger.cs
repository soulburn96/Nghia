using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea(3, 10)]
    public string[] sentences;
    public Dialogue dialogue;
    void OnTriggerEnter2D(Collider2D collider)
    {
        var p = collider.gameObject.GetComponent<CharacterController>();
        TriggerDialogue();
    }
    void OnTriggerExit2D(Collider2D other)
    {
        StartCoroutine(ToFar());
        
    }
    IEnumerator ToFar()
    {
        yield return new WaitForSeconds(3F);
        FindObjectOfType<DialogueSystem>().EndDialogue();  
   
    }
    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueSystem>().StartDialogue(dialogue);
    }
}
