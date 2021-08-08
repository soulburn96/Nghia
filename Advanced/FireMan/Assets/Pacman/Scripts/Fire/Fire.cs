using Node;
using UnityEngine;
using System.Collections;
using System;

namespace Pacman
{
    [RequireComponent(typeof(Mover2D))]
    [RequireComponent(typeof(PathFinder))]
    [RequireComponent(typeof(Animator))]
    public class Fire : MonoBehaviour, IPausable
    {
        [Header("Components")]
        [SerializeField] private MovementMap movementMap;
        [SerializeField] private FireCollection fireCollection;
        [SerializeField] private LevelManager levelManager;
        

        private bool isPaused;
        private bool isExtinguished;
        private SpriteRenderer spriteRenderer;
        private Mover2D mover;
        private Animator animator;
        private PathFinder pathFinder;
        private FireChaseState chase;
        private FireScatterState scatter;
        private FireInHouseState earlyInHouse;
        private FireContainState containInHouse;
        private FireConsumedState consumed;

        private FSM fsm = new FSM();

        public bool IsPaused => isPaused;
        public bool IsExtinguished => isExtinguished;

        public static event Action<Fire> OnFireExtinguished;
        public static event Action OnFireHit;
        
        private void OnEnable()
        {
            fireCollection.Add(this);
            levelManager.AddPausable(this);
        }

        private void OnDisable()
        {
            fireCollection.Remove(this);
            levelManager.RemovePausable(this);
        }

        private void OnDestroy()
        {
            OnFireExtinguished = null;
            OnFireHit = null;
        }
        private void Awake()
        {
            mover = GetComponent<Mover2D>();
            animator = GetComponent<Animator>();
            pathFinder = GetComponent<PathFinder>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            chase   = GetComponent<FireChaseState>();
            earlyInHouse = GetComponent<FireInHouseState>();
            containInHouse = GetComponent<FireContainState>();
            scatter = GetComponent<FireScatterState>();
            consumed = GetComponent<FireConsumedState>();

            chase.InjectComponents(this, mover, pathFinder, movementMap);
            earlyInHouse.InjectComponents(this, mover, pathFinder, movementMap);
            containInHouse.InjectComponents(this, mover, pathFinder, movementMap);
            scatter.InjectComponents(this, mover, pathFinder, movementMap);
            consumed.InjectComponents(this, mover, pathFinder, movementMap);
        }

        private void Start()
        {
            fsm.AddTransition(earlyInHouse, chase, () => earlyInHouse.IsFinished);
            fsm.AddTransition(containInHouse, chase, () => containInHouse.IsFinished);
            fsm.AddTransition(chase, scatter, () => chase.IsFinished);
            fsm.AddTransition(scatter, chase, () => scatter.IsFinished);
            fsm.AddTransition(consumed, containInHouse, () => consumed.IsFinished);
            fsm.AddTransitionFromAnyNode(consumed, () => isExtinguished);
            fsm.Start();
        }

        private void Update()
        {
            fsm.Update();
        }

        private void FixedUpdate()
        {
            fsm.FixedUpdate();
        }
        
        public void Pause()
        {
            fsm.Pause();
            mover.Pause();
            animator.enabled = false;
        }

        public void Resume()
        {
            fsm.Resume();
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

        public void Focus(float duration)
        {
            StartCoroutine(GainFocus(duration));
        }

        private IEnumerator GainFocus(float duration)
        {
            levelManager.RequestFocus(this.gameObject);

            yield return new WaitForSeconds(duration);

            levelManager.Unfocus();
        }
       
        public void GetExtinguished()
        {
            if (!isExtinguished)
            {
                isExtinguished = true;
                OnFireExtinguished?.Invoke(this);
            }            
        }
        public void ResetState()
        {
            isExtinguished = false;
            isPaused = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var spiro = collision.GetComponent<Spiro>();
            var friend = collision.GetComponent<Friend>();            

            if(spiro!=null || friend != null)
            {
                OnFireHit?.Invoke();
                spiro?.Dead();
                friend?.Dead();
            }
        }
    }
}
