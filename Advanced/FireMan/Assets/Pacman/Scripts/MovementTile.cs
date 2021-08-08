using System;
using UnityEngine;

namespace Pacman
{
    public class MovementTile
    {
        public static int MinMovementCost = 0;
        public static int MaxMovementCost = Int32.MaxValue;

        private Vector3 position;
        private Vector3Int coordinate;
        private Vector3Int tileMapCoordinate;

        public int Cost;

        public MovementTile EastNeighbor;
        public MovementTile WestNeighbor;
        public MovementTile NorthNeighbor;
        public MovementTile SouthNeighbor;

        public Vector3 Position => position;
        public Vector3Int Coordinate => coordinate;
        public Vector3Int TileMapCoordinate => tileMapCoordinate;

        public bool IsWalkable => !Cost.Equals(MaxMovementCost);
        
        public MovementTile(Vector3 position, Vector3Int coordinate, Vector3Int tileMapCoordinate)
        {
            this.position = position;
            this.coordinate = coordinate;
            this.tileMapCoordinate = tileMapCoordinate;
        }

        public MovementTile GetNeighborBasedOnDirection(Vector2 direction)
        {
            var filteredDirection = FilterDirection(direction);

            if (filteredDirection.x > 0)
                return EastNeighbor;
            
            if (filteredDirection.x < 0)
                return WestNeighbor;
            
            if (filteredDirection.y < 0)
                return SouthNeighbor;
            
            if (filteredDirection.y > 0)
                return NorthNeighbor;
            
            return null;
        }
        
        public Vector2 FilterDirection(Vector2 direction)
        {
            var filteredDirection = Vector2.zero;
            
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                    filteredDirection.x = 1;
                else if (direction.x < 0)
                    filteredDirection.x = -1;
            }
            else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
            {
                if (direction.y > 0)
                    filteredDirection.y = 1;
                else if (direction.y < 0)
                    filteredDirection.y = -1;
            }

            return filteredDirection;
        }

        public void ForEachCardinalNeighbor(Action<MovementTile> action)
        {
            if (EastNeighbor != null)
                action(EastNeighbor);

            if (SouthNeighbor != null)
                action(SouthNeighbor);
            
            if (WestNeighbor != null)
                action(WestNeighbor);
            
            if (NorthNeighbor != null)
                action(NorthNeighbor);
        }
    }
}
