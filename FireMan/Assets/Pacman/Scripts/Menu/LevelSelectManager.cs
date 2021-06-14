using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pacman
{
    public class LevelSelectManager : MonoBehaviour
    {
        [SerializeField]
        private string[] levelTags;

        [SerializeField]
        private string[] highScoreTags;

        [SerializeField]
        private GameObject[] locks;

        [SerializeField]
        private TextMesh[] highScoreTexts;

        [SerializeField]
        private bool[] levelUnlocked;

        [SerializeField] private string level1tag;

        [SerializeField] private string[] levelName;

        [SerializeField] private float moveSpeed;

        [SerializeField] private bool isPressed;

        public  int positionSelector;

        [SerializeField] private float distance;

        [SerializeField] private bool useKeyboard;

        [SerializeField] private Vector2RawVariable inputAxis;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            PlayerPrefs.SetInt(level1tag, 1);

            LoadUnlockedLevel();

            transform.position = locks[positionSelector].transform.position + new Vector3(0, distance, 0);
        }

        private void Update()
        {
            HandleInput();
            HandleMovement();
        }
        void LoadUnlockedLevel()
        {
            for (int i = 0; i < levelTags.Length; i++)
            {
                if (PlayerPrefs.GetInt(levelTags[i]) == null || PlayerPrefs.GetInt(levelTags[i]) == 0)
                {
                    levelUnlocked[i] = false;
                }
                else
                {
                    levelUnlocked[i] = true;
                    locks[i].SetActive(false);
                    highScoreTexts[i].text = PlayerPrefs.GetInt(highScoreTags[i]).ToString();
                }
            }
        }

        void HandleMovement()
        {
            transform.position = Vector3.MoveTowards(transform.position, locks[positionSelector].transform.position + new Vector3(0, distance, 0), moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, locks[positionSelector].transform.position + new Vector3(0, distance, 0)) < 0.1f)
                animator.SetFloat("MoveX", 0);
        }
        void HandleInput()
        {
            if (useKeyboard)
            {
                inputAxis.Value = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            }
            if (!isPressed)
            {
                if (Mathf.Abs(inputAxis.Value.x) > Mathf.Abs(inputAxis.Value.y))
                {
                    if (inputAxis.Value.x > 0.15f)
                    {
                        positionSelector += 1;
                        animator.SetFloat("MoveX",1);
                    }
                        
                    else if (inputAxis.Value.x < -0.15f)
                    {
                        positionSelector -= 1;
                        animator.SetFloat("MoveX", -1);
                    }
                        

                    if (positionSelector >= levelTags.Length)
                    {
                        positionSelector = levelTags.Length - 1;
                    }
                    if (positionSelector < 0)
                    {
                        positionSelector = 0;
                    }
                    isPressed = true;
                }
            }
            else
            {
                if (inputAxis.Value.x < 0.25f && inputAxis.Value.x > -0.25f)
                {
                    isPressed = false;
                }                    
            }            
        }

        public void LoadLevel()
        {
            if (levelUnlocked[positionSelector])
            {
                SceneManager.LoadScene(levelName[positionSelector]);

            }
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR            
                    UnityEditor.EditorApplication.isPlaying = false;
            #else
                    Application.Quit();
            #endif
        }
    }
}

