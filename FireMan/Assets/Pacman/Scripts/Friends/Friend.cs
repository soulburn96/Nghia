using Node;
using UnityEngine;
using System.Collections;

namespace Pacman
{
    [RequireComponent(typeof(Mover2D))]
    public class Friend : Follower, IPausable
    {
        [Header("Components")]
        [SerializeField] private FireCollection fireCollection;
        [SerializeField] private LevelManager levelManager;

        [Header("States")]
        [SerializeField] private FriendIdleState idle;
        [SerializeField] private FriendFollowState follow;
        [SerializeField] private FriendEvadeFireState evadeFire;
        [SerializeField] private FriendExitState exit;

        private bool isPaused;
        private FSM fsm;
        private Mover2D mover;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        public Mover2D Mover => mover;
        public bool IsPaused => isPaused;

        private void Awake()
        {
            mover = GetComponent<Mover2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            idle.InjectComponent(this);
            follow.InjectComponent(this);
            evadeFire.InjectComponent(this);
            exit.InjectComponent(this);
            
            levelManager.AddPausable(this);
        }

        private void OnEnable()
        {
            levelManager.AddPausable(this);
        }

        private void OnDisable()
        {
            levelManager.RemovePausable(this);
        }

        private void Start()
        {
            fsm = new FSM();

            fsm.AddTransition(idle, evadeFire, () => idle.DetectFire && idle.IsFinished);
            fsm.AddTransition(evadeFire, idle, () => evadeFire.IsSafe);
            fsm.AddTransitionFromAnyNode(follow, () => isBeingRescued);
            fsm.AddTransition(follow, exit, () => exit.IsExit);

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

        public void Stop()
        {
            Pause();
            StopAllCoroutines();
        }

        public void Resume()
        {
            fsm.Resume();
            mover.Resume();
            animator.enabled = true;
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

        public void PlayPanicAnimation()
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);
        }

        public void PlayWalkAnimation()
        {
            animator.SetFloat("MoveX", mover.CurrentDirection.x);
            animator.SetFloat("MoveY", mover.CurrentDirection.y);
        }

        public void ResumeAnimation()
        {
            animator.enabled = true;
        }
        
        public void PauseAnimation()
        {
            animator.enabled = false;
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

        public Fire DetectNearestFire(float radius = 1000f)
        {
            var fires = fireCollection.Fires;

            if (fires.Count == 0)
                return null;

            var nearestFireIndex = 0;
            var nearestDistanceSoFar = Vector2.Distance(fires[0].transform.position, transform.position);

            for (int i = 1; i < fires.Count; i++)
            {
                var newDistance = Vector2.Distance(fires[i].transform.position, transform.position);

                if (newDistance < nearestDistanceSoFar)
                {
                    nearestDistanceSoFar = newDistance;
                    nearestFireIndex = i;
                }
            }
            
            if (nearestDistanceSoFar < radius)
                return fires[nearestFireIndex];

            return null;
        }
        public void Dead()
        {
            Stop();
            animator.enabled = true;
            animator.SetBool("IsDead", true);            
        }
    }
}
