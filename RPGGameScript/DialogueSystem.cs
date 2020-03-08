using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public GameObject dialogHolder;
    public Text dialogueText;
    public Queue<string> sentences;
    public Animator animator;
	Coroutine lastLine = null;
	
    void Start()
    {
        dialogHolder.SetActive(false);
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogHolder != null)
        {
            dialogHolder.SetActive(true);
        }
        if (animator != null)
        {
            animator.SetBool("IsOpen", true);
        }  
        sentences.Clear();

        //array for sentences.
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
       
        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
        if(lastLine!= null)
        {
            StopCoroutine(lastLine);
        }      
        lastLine = StartCoroutine(TypeSentence(sentence));
    }
    //have the system display letter per letter
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05F); ;
        }
    }
    public void EndDialogue()
    {
        if (dialogHolder != null)
        {
            dialogHolder.SetActive(false);
        }
        if (animator != null)
        {
            animator.SetBool("IsOpen", false);
        }

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            DisplayNextSentence();
        }
    }
}
