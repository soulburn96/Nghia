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

    // Start is called before the first frame update
    void Start()
    {
        dialogHolder.SetActive(false);
        sentences = new Queue<string>();
    }
        // Update is called once per frame
    public void StartDialogue(Dialogue dialogue)
    {
        dialogHolder.SetActive(true);
        animator.SetBool("IsOpen", true);
        sentences.Clear();

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
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
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
        dialogHolder.SetActive(false);
        animator.SetBool("IsOpen", false);

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            DisplayNextSentence();
        }
    }
}
