using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TimeRun : MonoBehaviour
{
    public Text timeText;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Button restartButton;
    private CharacterController2D characterController;
    [SerializeField] private AudioSource audioSrc;
    [SerializeField] private AudioClip deadSound;
    public float time = 30;
    void Start()
    {
        characterController = FindObjectOfType<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
    }
    public void UpdateTime()
    {
        if (!characterController.gameOver)
        {
            time = time - Time.deltaTime;
            timeText.text = "Time: " + Mathf.Round(time);
            if (time < 0)
            {
                GameOver();
                
            }
        }
    }
    void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        characterController.gameOver = true;
        audioSrc.PlayOneShot(deadSound);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        characterController.gameOver = false;
    }
}