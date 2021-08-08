using System;
using System.Collections;
using UnityEngine;

namespace Pacman
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Mover2D : MonoBehaviour
    {
        public event Action OnStart;
        public event Action OnDone;
        public event Action OnDirectionChanged;

        [SerializeField] private MovementMap movementMap;
        [SerializeField] private PathFinder pathFinder;

        public bool IsInteruptable;
        public bool SortLayerBasedOnY;
        public float MoveSpeed;

        private Rigidbody2D rb2D;
        private bool isMoving;
        private bool isPaused;
        private Vector2 currentDirection;
        private Vector2 previousDirection;
        private Vector2 previousPosition;
        private SpriteRenderer spriteRenderer;

        public bool IsMoving => isMoving;
        public bool IsPaused => isPaused;
        public Vector2 CurrentDirection => currentDirection;
        public MovementMap MovementMap => movementMap;

        private void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Pause() => isPaused = true;
        public void Resume() => isPaused = false;
        public void Stop()
        {
            StopAllCoroutines();
            isMoving = false;
        }

        public void Move(Path path)
        {
            if (path == null)
                return;
            
            if (isMoving)
            {
                if (IsInteruptable)
                    StopAllCoroutines();
                else
                    return;
            }

            StartCoroutine(MoveCoroutine(path));
        }

        public void Move(Vector3 destination)
        {
            var sourceTile = movementMap.GetTileAtPosition(transform.position);
            var destinationTile = movementMap.GetTileAtPosition(destination);

            if (sourceTile != null && destinationTile != null)
            {
                var path = pathFinder.CalculatePath(sourceTile, destinationTile, transform.position.z);
                
                Move(path);
            }
        }

        public void Move(MovementTile destinationTile)
        {
            if (isMoving)
            {
                if (IsInteruptable)
                    StopAllCoroutines();
                else
                    return;
            }
            if (isPaused)
                return;

            StartCoroutine(MoveCoroutine(destinationTile));
        }

        private IEnumerator MoveCoroutine(MovementTile destinationTile)
        {
            OnStart?.Invoke();
            
            isMoving = true;

            while (transform.position != destinationTile.Position)
            {
                if (isPaused)
                    yield return new WaitUntil(() => !isPaused);
                
                var position = Vector2.MoveTowards(transform.position, destinationTile.Position, MoveSpeed * Time.fixedDeltaTime);
                rb2D.position = position;

                if (SortLayerBasedOnY)
                    spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
                
                HandleState(position);
                
                yield return new WaitForFixedUpdate();
            }
        
            isMoving = false;
            
            OnDone?.Invoke();
        }
        
        private IEnumerator MoveCoroutine(Path path)
        {
            OnStart?.Invoke();
            
            isMoving = true;

            while (!path.ReachedEndPoint())
            {
                var nextPoint = path.Next();

                while (transform.position != nextPoint)
                {
                    if (isPaused)
                        yield return new WaitUntil(() => !isPaused);
                    
                    var position  = Vector2.MoveTowards(transform.position, nextPoint, MoveSpeed * Time.fixedDeltaTime);
                    rb2D.position = position;

                    if (SortLayerBasedOnY)
                        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
                    
                    HandleState(position);

                    yield return new WaitForFixedUpdate();
                }
            }
            
            isMoving = false;
            
            OnDone?.Invoke();
        }

        private void HandleState(Vector2 currentPosition)
        {
            var moveDirection = (currentPosition - previousPosition).normalized;
            moveDirection = Filter(moveDirection);
            
            if (moveDirection != Vector2.zero)
                currentDirection = moveDirection;
            
            if (currentDirection != previousDirection)
                OnDirectionChanged?.Invoke();

            previousPosition  = currentPosition;
            previousDirection = moveDirection;
        }

        public Vector2 Filter(Vector2 input)
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
    }
}
