using UnityEngine;

namespace Pacman
{ 
    public class FireChaseState : FireState
    {
        [Header("Configuration")]
        [SerializeField] private Transform target;
        [SerializeField] private float duration;

        private float elapsedTime;
        
        public bool IsFinished { get; private set; }

        public override void OnEnter()
        {
            ResetState();
        }

        public override void OnUpdate()
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= duration)
                IsFinished = true;
        }

        public override void OnFixedUpdate()
        {
            ChaseTarget();
        }

        private void ResetState()
        {
            elapsedTime = 0;
            IsFinished  = false;
        }

        private void ChaseTarget()
        {
            if (mover.IsMoving)
                return;
            var destinationTile = movementMap.GetTileAtPosition(target.position);

            if (destinationTile != null)
            {
                var sourceTile = movementMap.GetTileAtPosition(mover.transform.position);
                var path = pathFinder.CalculatePath(sourceTile, destinationTile, mover.transform.position.z);
                
                mover.Move(path);
            }
        }
    }
}