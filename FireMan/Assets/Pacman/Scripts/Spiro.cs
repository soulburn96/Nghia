using System.Collections;
using UnityEngine;
using ScriptableObjectArchitecture;
using System.Collections.Generic;

namespace Pacman
{
    [RequireComponent(typeof(Mover2D))]
    [RequireComponent(typeof(Animator))]
    public class Spiro : MonoBehaviour, IPausable
    {
        private bool isPaused;
        private Mover2D mover;
        private bool isExtinguishing = false;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private bool intervalFlag = false;
        private bool dead = false;

        private float timerExtinguisher = 0;
        [SerializeField] private float extinguisherLifeTime;
        [Header("Components")]
        [SerializeField] private MovementMap movementMap;
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private Vector2RawVariable inputAxis;
        [SerializeField] private bool useKeyboard;

        private Extinguisher equippedExtinguisher;
        private Vector2 currentDirection;

        public Mover2D Mover => mover;
        public bool IsPaused => isPaused;
        public bool IsExtinguishing => isExtinguishing;

        private void Awake()
        {
            mover = GetComponent<Mover2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            mover.OnDirectionChanged += UpdateAnimation;
        }
        
        private void OnEnable()
        {
            levelManager.AddPausable(this);
        }

        private void OnDisable()
        {
            levelManager.RemovePausable(this);
        }

        private void Update()
        {
            if (dead)
                return;

            if (useKeyboard)
            {
                inputAxis.Value = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                
                if (Input.GetKey(KeyCode.Z))
                    UseExtinguisher();
            }


            if (!isExtinguishing)
            {
                var moveDirection = HandleInput(inputAxis.Value);
                HandleMovement(moveDirection);                
            }
                
        }

        public void UseExtinguisher()
        {
            if (equippedExtinguisher == null || levelManager.gameOver)
                return;

            if (!isExtinguishing)
            {
                StartCoroutine(UseExtinguisherCorountine());
                StartCoroutine(ExtinguisherCooldown(0.2f));
            }
        }

        public void StopExtinguisher()
        {
            if (equippedExtinguisher == null || levelManager.gameOver)
                return;
            StartCoroutine(StopExtinguisherCorountine());          
        }
        private IEnumerator UseExtinguisherCorountine()
        {
            isExtinguishing = true;            
            while (isExtinguishing && equippedExtinguisher!=null)
            {
                if (intervalFlag)
                {
                    UpdateAnimation();
                    HandleExtinguisherSpray(currentDirection);                    
                    intervalFlag = false;                    
                }                    

                timerExtinguisher -= Time.deltaTime;
                if (timerExtinguisher <= 0)
                {
                    StopExtinguisher();
                    Destroy(equippedExtinguisher.gameObject);
                    animator.SetBool("IsEquippedExtinguisher", false);
                }
                yield return null;
            }
        }
        private IEnumerator ExtinguisherCooldown(float interval)
        {
            while (true)
            {
                intervalFlag = !intervalFlag;
                yield return new WaitForSeconds(interval);
            }
        }

        private IEnumerator StopExtinguisherCorountine()
        {
            isExtinguishing = false;
            equippedExtinguisher.StopSpray();
            yield return new WaitForSeconds(0.5f);
            UpdateAnimation();
            Resume();
        }
        public void Pause()
        {
            mover.Pause();
            if(!isExtinguishing)
                animator.enabled = false;
        }

        public void Resume()
        {
            if (levelManager.gameOver)
                return;
            mover.Resume();
            animator.enabled = true;
        }
        public void Stop()
        {
            Pause();
            StopAllCoroutines();
        }
        public void EnableVisual()
        {
            spriteRenderer.enabled = true;
        }

        public void DisableVisual()
        {
            spriteRenderer.enabled = false;
        }

        public void BlinkVisual()
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }

        private void UpdateAnimation()
        {
            animator.SetFloat("MoveX", currentDirection.x);
            animator.SetFloat("MoveY", currentDirection.y);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var extinguisher = other.GetComponent<Extinguisher>();

            if (extinguisher != null)
            {
                if (equippedExtinguisher != null)
                    Destroy(equippedExtinguisher.gameObject);
                
                equippedExtinguisher = extinguisher;
                equippedExtinguisher.OnEquipped(this.gameObject);
                timerExtinguisher += extinguisherLifeTime;
                animator.SetBool("IsEquippedExtinguisher", true);
            }
        }

        private void HandleMovement(Vector2 moveDirection)
        {
            var currentTile = movementMap.GetTileAtPosition(this.transform.position);

            if (moveDirection == Vector2.zero)
                return;            

            var destinationTile = currentTile.GetNeighborBasedOnDirection(moveDirection);

            if (destinationTile != null && destinationTile.IsWalkable)
                currentDirection = moveDirection;
            else
                destinationTile = currentTile.GetNeighborBasedOnDirection(currentDirection);

            if (destinationTile != null && destinationTile.IsWalkable)
                mover.Move(destinationTile);
        }

        public Vector2 HandleInput(Vector2 input)
        {
            var filteredInput = Vector2.zero;
            
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                if (input.x > 0)
                    filteredInput.x = 1;
                else if (input.x < 0)
                    filteredInput.x = -1;
            }
            else if (Mathf.Abs(input.x) < Mathf.Abs(input.y))
            {
                if (input.y > 0)
                    filteredInput.y = 1;
                else if (input.y < 0)
                    filteredInput.y = -1;
            }

            return filteredInput;
        }

        private void HandleExtinguisherSpray(Vector3 direction)
        {

            for (int i = 0; i <= equippedExtinguisher.maxSprayDistance; i++)
            {
                var targetTile = movementMap.GetTileAtPosition(transform.position + direction * i);
                if (targetTile != null && targetTile.IsWalkable)
                {
                    equippedExtinguisher.sprayDistance = i;
                }
                else
                {
                    break;
                }
            }
            if (equippedExtinguisher.sprayDistance == 0)
                return;            
            equippedExtinguisher.SprayCollider(direction);
            equippedExtinguisher.StartSpray(direction);
            Pause();
        }

        public void Dead()
        {
            if(equippedExtinguisher!=null)
                equippedExtinguisher.StopSpray();
            Stop();
            animator.enabled = true;
            animator.SetBool("IsDead", true);           
        }
    }
}
