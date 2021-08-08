using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    public class FireContainState : FireState
    {
        [Header("Components")]
        [SerializeField] private House house;

        [Header("Configuration")]
        [SerializeField] private float duration;


        private MovementTile previousTile;
        private float elapsedTime;

        public bool IsFinished { get; private set; }
        public float Duration { get => duration; set => duration = value; }

        public override void OnEnter()
        {
            previousTile = movementMap.GetTileAtPosition(transform.position);
            ResetState();
        }

        public override void OnUpdate()
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= duration)
            {
                IsFinished = true;
                house.OpenDoor();
            }

            if (!mover.IsMoving)
                MoveToNextTile();
        }

        public override void OnExit()
        {
            house.AddEscapeFire(this.fire);
        }

        private void MoveToNextTile()
        {
            var currentTile = movementMap.GetTileAtPosition(transform.position);
            var walkableTiles = new List<MovementTile>();

            if (currentTile.EastNeighbor.IsWalkable)
                walkableTiles.Add(currentTile.EastNeighbor);
            if (currentTile.NorthNeighbor.IsWalkable)
                walkableTiles.Add(currentTile.NorthNeighbor);
            if (currentTile.WestNeighbor.IsWalkable)
                walkableTiles.Add(currentTile.WestNeighbor);
            if (currentTile.SouthNeighbor.IsWalkable)
                walkableTiles.Add(currentTile.SouthNeighbor);

            walkableTiles.Remove(previousTile);

            var randomIndex = Random.Range(0, walkableTiles.Count);
            var randomTile = walkableTiles[randomIndex];

            mover.Move(randomTile);
            previousTile = randomTile;
        }

        private void ResetState()
        {
            elapsedTime = 0;
            IsFinished = false;
        }
    }
}