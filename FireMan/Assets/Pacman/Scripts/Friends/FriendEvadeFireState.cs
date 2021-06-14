using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pacman
{
    public class FriendEvadeFireState : MonoBehaviourState
    {
        [Flags]
        private enum PossibleFleeDirection { None = 0, North = 1, South = 2, East = 4, West = 8, All = 15}

        private Friend friend;
        private MovementMap movement;
        private MovementTile previousTile;

        private bool isSafe;
        public  bool IsSafe => isSafe;

        public void InjectComponent(Friend friend)
        {
            this.friend = friend;
            movement = friend.Mover.MovementMap;
        }

        public override void OnUpdate()
        {
            if (friend.Mover.IsMoving)
                return;

            var nearestFire = friend.DetectNearestFire(8);

            if (nearestFire != null)
            {
                var v = friend.transform.position - nearestFire.transform.position;
                var fleeDirectionMask = GetFleeDirectionMask(v);
                var fleeTile = CalculateFleeTile(fleeDirectionMask);

                if (fleeTile != null)
                {
                    friend.Mover.Move(fleeTile);
                    friend.PlayWalkAnimation();
                }
                else
                {
                    friend.PlayPanicAnimation();
                }

                isSafe = false;
            }
            else
            {
                isSafe = true;
            }
        }

        private PossibleFleeDirection GetFleeDirectionMask(Vector2 chaserDirection)
        {
            var fleeDirectionMask = PossibleFleeDirection.All;

            if (chaserDirection.x > 0)
                fleeDirectionMask ^= PossibleFleeDirection.West;
            else if (chaserDirection.x < 0)
                fleeDirectionMask ^= PossibleFleeDirection.East;

            if (chaserDirection.y > 0)
                fleeDirectionMask ^= PossibleFleeDirection.South;
            else if (chaserDirection.y < 0)
                fleeDirectionMask ^= PossibleFleeDirection.North;
            
            return fleeDirectionMask;
        }

        private MovementTile CalculateFleeTile(PossibleFleeDirection fleeDirectionMask)
        {
            var currentTile = movement.GetTileAtPosition(transform.position);
            var walkableTiles = new List<MovementTile>();

            if (fleeDirectionMask.HasFlag(PossibleFleeDirection.East) && currentTile.EastNeighbor != null && currentTile.EastNeighbor.IsWalkable)
                walkableTiles.Add(currentTile.EastNeighbor);
            
            if (fleeDirectionMask.HasFlag(PossibleFleeDirection.West) && currentTile.WestNeighbor != null && currentTile.WestNeighbor.IsWalkable)
                walkableTiles.Add(currentTile.WestNeighbor);
            
            if (fleeDirectionMask.HasFlag(PossibleFleeDirection.North) && currentTile.NorthNeighbor != null && currentTile.NorthNeighbor.IsWalkable)
                walkableTiles.Add(currentTile.NorthNeighbor);
            
            if (fleeDirectionMask.HasFlag(PossibleFleeDirection.South) && currentTile.SouthNeighbor != null && currentTile.SouthNeighbor.IsWalkable)
                walkableTiles.Add(currentTile.SouthNeighbor);

            if (walkableTiles.Count == 0)
                return null;
            
            walkableTiles.Remove(previousTile);
            
            var randomIndex = Random.Range(0, walkableTiles.Count);
            var randomTile  = walkableTiles[randomIndex];
            previousTile = randomTile;
            
            return randomTile;
        }
    }
}