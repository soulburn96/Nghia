using UnityEngine;

namespace Pacman
{ 
    public class FireScatterState : FireState
    {
        [Header("Configuration")]
        [SerializeField] private float duration;

        private float elapsedTime;
        
        public bool IsFinished { get; private set; }

        public override void OnEnter()
        {
            ResetState();
            Wander();
        }

        public override void OnUpdate()
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= duration)
                IsFinished = true;
        }

        public override void OnFixedUpdate()
        {
            if (mover.IsMoving)
                return;
            
            Wander();
        }

        private void ResetState()
        {
            elapsedTime = 0;
            IsFinished  = false;
        }

        private void Wander()
        {
            var destinationTile = movementMap.RandomWalkableTile();

            if (destinationTile != null)
            {
                var sourceTile = movementMap.GetTileAtPosition(mover.transform.position);
                var path = pathFinder.CalculatePath(sourceTile, destinationTile, mover.transform.position.z);
                
                mover.Move(path);
            }
        }
    }
}