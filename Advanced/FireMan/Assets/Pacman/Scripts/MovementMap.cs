using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pacman
{
    public class MovementMap : MonoBehaviour
    {
        [SerializeField] private Tilemap environmentMap;
        
        [Header("Visualization")]
        [SerializeField] private Tilemap visualMovementMap;
        [SerializeField] private TileBase visualTile;
        
        private MovementTile[,] movementMap;
        private int rows;
        private int columns;

        private void Awake()
        {
            InitializeMovementMapFrom(environmentMap);
            PaintVisualMovementMap();
        }

        private void PaintVisualMovementMap()
        {
            foreach (var movementTile in movementMap)
            {
                var color = movementTile.IsWalkable ? Color.white : Color.red;
                
                visualMovementMap.SetTile(movementTile.TileMapCoordinate, visualTile);
                visualMovementMap.SetTileFlags(movementTile.TileMapCoordinate, TileFlags.None);
                visualMovementMap.SetColor(movementTile.TileMapCoordinate, color);
                visualMovementMap.SetTileFlags(movementTile.TileMapCoordinate, TileFlags.LockAll);
            }
        }

        public void PaintTile(Vector3 postion, TileBase tileBase)
        {
            var tileCoordinate = environmentMap.WorldToCell(postion);
            
            environmentMap.SetTile(tileCoordinate, tileBase);
        }

        public void UpdateCost(Vector3 postion, int cost)
        {
            var tile = GetTileAtPosition(postion);

            if (tile != null)
            {
                tile.Cost = cost;
                PaintVisualMovementMap();
            }
        }
        
        public MovementTile GetTileAtCoordinate(int x, int y)
        {
            if (x < 0 || x >= columns)
                return null;
            
            if (y < 0 || y >= rows)
                return null;

            return movementMap[x, y];
        }

        public MovementTile GetTileAtCoordinate(Vector2Int coodinate)
        {
            return GetTileAtCoordinate(coodinate.x, coodinate.y);
        }
        
        public MovementTile GetTileAtPosition(float x, float y, float z)
        {
            var origin = movementMap[0, 0].Position;
            origin.x -= 0.5f;
            origin.y -= 0.5f;

            if (x < origin.x || x > origin.x + columns)
                return null;

            if (y < origin.y || y > origin.y + rows)
                return null;
            
            float fractX = Mathf.Abs(x - origin.x);
            float fractY = Mathf.Abs(y - origin.y);

            int i = fractX == 0 ? 0 : Mathf.CeilToInt(fractX) - 1;
            int j = fractY == 0 ? 0 : Mathf.CeilToInt(fractY) - 1;

            return movementMap[i, j];
        }
        
        public MovementTile GetTileAtPosition(Vector3 position)
        {
            return GetTileAtPosition(position.x, position.y, position.z);
        }
        
        public MovementTile RandomWalkableTile()
        {
            var randomCoordinateX = Random.Range(0, columns);
            var randomCoordinateY = Random.Range(0, rows);
            
            var desinationTile = GetTileAtCoordinate(randomCoordinateX, randomCoordinateY);

            while (!desinationTile.IsWalkable)
            {
                randomCoordinateX = Random.Range(0, columns);
                randomCoordinateY = Random.Range(0, rows);
            
                desinationTile = GetTileAtCoordinate(randomCoordinateX, randomCoordinateY);
            }

            return desinationTile;
        }

        private void InitializeMovementMapFrom(Tilemap tilemap)
        {
            rows    = tilemap.size.y;
            columns = tilemap.size.x;

            movementMap = new MovementTile[columns, rows];

            for (int x = tilemap.cellBounds.xMin, i = 0; x < tilemap.cellBounds.xMax; x++, i++)
            {
                for (int y = tilemap.cellBounds.yMin, j = 0; y < tilemap.cellBounds.yMax; y++, j++)
                {
                    var cellCoordinate = new Vector3Int(x, y, 0);
                    var cellCenterPosition = tilemap.GetCellCenterWorld(cellCoordinate);
                    
                    movementMap[i, j] = new MovementTile(cellCenterPosition, new Vector3Int(i, j, 0), cellCoordinate);

                    if (tilemap.HasTile(cellCoordinate))
                        movementMap[i, j].Cost = MovementTile.MaxMovementCost;
                    else
                        movementMap[i, j].Cost = 1;
                }
            }

            LinkNeighborTiles();
        }

        private void LinkNeighborTiles()
        {
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    var movementTile = movementMap[i, j];

                    if (i != 0)
                        movementTile.WestNeighbor  = movementMap[i - 1, j];
                    
                    if (i != columns - 1)
                        movementTile.EastNeighbor  = movementMap[i + 1, j];

                    if (j != 0)
                        movementTile.SouthNeighbor = movementMap[i, j - 1];
                    
                    if (j != rows -1)
                        movementTile.NorthNeighbor = movementMap[i, j + 1];
                }
            }
        }
    } 
}
