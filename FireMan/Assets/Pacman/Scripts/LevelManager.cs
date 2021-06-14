using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
namespace Pacman
{
    public class LevelManager : MonoBehaviour
    {
        private List<IPausable> pausables = new List<IPausable>();
        [SerializeField] private Text timeText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text highscoreText;
        [SerializeField] private TextMesh scorePopupText;
        [SerializeField] private Text gameEndText;
        [SerializeField] private int timeLeft;
        [SerializeField] private AudioClip losingSound;
        [SerializeField] private AudioClip winningSound;
        [SerializeField] private AudioClip newHighScoreSound;

        [SerializeField] private string highscoreTag;
        private AudioSource audioSrc;

        [Header("Score settings")]
        [SerializeField] private int clearLevelScore;
        [SerializeField] private int friendRescureScore = 200;
        [SerializeField] private int fireExtinguishedScore = 100;
        [SerializeField] private int maxLevelScore = 2000;
        private int highScore = 0;
        private int score = 0;
        private bool isHighScore = false;
        public bool gameOver = false;

        [SerializeField] private string levelTag;

        private void OnEnable()
        {
            Follower.OnFollow += (follower) => ScorePopup(friendRescureScore, follower.transform.position);
            Fire.OnFireExtinguished += (fire) => ScorePopup(fireExtinguishedScore, fire.transform.position);
            Fire.OnFireHit += Lose;
            TriggerZoneFireMan.OnPlayerExit += Win;
        }

        private void OnDisable()
        {
            Follower.OnFollow -= (follower) => ScorePopup(friendRescureScore, follower.transform.position);
            Fire.OnFireExtinguished -= (fire) => ScorePopup(fireExtinguishedScore, fire.transform.position);
            Fire.OnFireHit -= Lose;
            TriggerZoneFireMan.OnPlayerExit -= Win;
        }

        private void OnDestroy()
        {
            Follower.OnFollow -= (follower) => ScorePopup(friendRescureScore, follower.transform.position);
            Fire.OnFireExtinguished -= (fire) => ScorePopup(fireExtinguishedScore, fire.transform.position);
            Fire.OnFireHit -= Lose;
            TriggerZoneFireMan.OnPlayerExit -= Win;
        }
        private void Start()
        {
            StartCoroutine(LevelTimer());
            
            audioSrc = GetComponent<AudioSource>();            
            if (PlayerPrefs.GetInt(highscoreTag) == null)
                highScore = 0;
            else
                highScore = PlayerPrefs.GetInt(highscoreTag);
            highscoreText.text = highScore.ToString();
        }

        public void AddPausable(IPausable pausable)
        {
            if (pausables.Contains(pausable))
                return;
            
            pausables.Add(pausable);
        }
        
        public void RemovePausable(IPausable pausable)
        {
            if (!pausables.Contains(pausable))
                return;
            
            pausables.Remove(pausable);
        }
        
        public void RequestFocus(GameObject focusedObject)
        {
            var focusedPausable = focusedObject.GetComponent<IPausable>();

            foreach (var pausable in pausables)
            {
                if (focusedPausable == pausable)
                    continue;
                
                pausable.Pause();
            }
        }

        public void Unfocus()
        {
            if (gameOver)
                return;

            foreach (var pausable in pausables)
                pausable.Resume();
        }
        public void PauseAll()
        {
            foreach (var pausable in pausables)
                pausable.Pause();
        }
        public void StopAll()
        {
            foreach (var pausable in pausables)
                pausable.Stop();
        }

        private IEnumerator LevelTimer()
        {
            while (timeLeft > 0)
            {
                timeLeft--;
                timeText.text = timeLeft.ToString();
                yield return new WaitForSeconds(1);
            }
            Lose();
            PauseAll();
        }

        private void Lose()
        {            
            StopAll();
            gameOver = true;
            StopAllCoroutines();
            audioSrc.Stop();
            audioSrc.PlayOneShot(losingSound);
            if (score > PlayerPrefs.GetInt(highscoreTag))
                PlayerPrefs.SetInt(highscoreTag, score);

            StartCoroutine(LoseCoroutine());
        }


        private void Win()
        {
            if (!gameOver)
            {
                StopAll();
                StopAllCoroutines();
                UpdateScore(clearLevelScore);
                if (timeLeft <= 200)
                {
                    UpdateScore(timeLeft);
                }
                else
                {
                    UpdateScore(200);
                }
                gameOver = true;
                audioSrc.Stop();
                audioSrc.PlayOneShot(winningSound);

                if (score > PlayerPrefs.GetInt(highscoreTag))
                    PlayerPrefs.SetInt(highscoreTag,score);
            }
            StartCoroutine(WinCoroutine());
        }



        public void UpdateScore(int value)
        {
            if (score > maxLevelScore)
                return;

            score += value;

            if (score > highScore && !isHighScore && highScore!=0)
            {
                audioSrc.PlayOneShot(newHighScoreSound);
                isHighScore = true;
            }               

            if (score > maxLevelScore)
                score = maxLevelScore;

            scoreText.text = score.ToString();
        }

        public void ScorePopup(int value, Vector3 position)
        {
            UpdateScore(value);
            if (score >= maxLevelScore)
                return;

            scorePopupText.text = value.ToString();
            scorePopupText.transform.position = position + new Vector3(0,1.5f,0);
            scorePopupText.gameObject.SetActive(true);
            StartCoroutine(ScorePopupFade(0.5f));
        }

        IEnumerator ScorePopupFade(float delay)
        {
            yield return new WaitForSeconds(delay);
            scorePopupText.gameObject.SetActive(false);
        }

        IEnumerator LoseCoroutine()
        {
            yield return new WaitForSeconds(2);
            gameEndText.text = "YOU LOSE";
            gameEndText.color = Color.red;
            gameEndText.gameObject.SetActive(true);
            StartCoroutine(Blink(2, 0.2f));
        }

        IEnumerator WinCoroutine()
        {
            gameEndText.text = "YOU WIN";
            gameEndText.color = Color.green;
            gameEndText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2);
            UnlockNextLevel();
            LoadMenuScene();
        }
        IEnumerator Blink(float time,float interval)
        {
            float timer = 0;
            float intervalTimer = 0;
            while (timer < time)
            {
                if (intervalTimer < interval)
                {
                    intervalTimer += Time.deltaTime;
                }
                else
                {
                    intervalTimer = 0;
                    foreach (var pausable in pausables)
                        pausable.BlinkVisual();
                }
                timer += Time.deltaTime;

                yield return null;
            }
            LoadMenuScene();
        }

        private void LoadMenuScene()
        {
            SceneManager.LoadScene("PacmanMenu");
           
        }
        private void UnlockNextLevel()
        {
            if (levelTag == null)
                return;
            PlayerPrefs.SetInt(levelTag, 1);
        }
    }
}