using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueue;

namespace Pacman
{
    public class PathFinder : MonoBehaviour
    {
        [SerializeField] private bool drawLine;

        private LineRenderer lineRenderer;

        private void Awake()
        {
            var drawer = new GameObject("Path Drawer");

            drawer.transform.SetParent(this.transform);

            lineRenderer = drawer.AddComponent<LineRenderer>();

            var randomColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth   = 0.05f;
            //lineRenderer.material   = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
            lineRenderer.startColor = randomColor;
            lineRenderer.endColor   = randomColor;
            lineRenderer.sortingOrder   = 3;
            lineRenderer.numCapVertices = 10;
        }

        public Path CalculatePath(MovementTile source, MovementTile destination, float z)
        {
            var path = CalculateAStarPath(source, destination, z);

            if (drawLine && path != null)
                DrawPath(path);

            return path;
        }

        public Path CalculateAStarPath(MovementTile source, MovementTile destination, float z)
        {
            var frontier  = new SimplePriorityQueue<MovementTile>();
            var cameFrom  = new Dictionary<MovementTile, MovementTile>();
            var costSoFar = new Dictionary<MovementTile, int>();

            costSoFar.Add(source, 0);
            frontier.Enqueue(source, 0);

            while (frontier.Any())
            {
                var current = frontier.Dequeue();

                if (current == destination)
                    break;

                current.ForEachCardinalNeighbor(next =>
                {
                    var newCost = costSoFar[current] + next.Cost;
                
                    if ((!cameFrom.ContainsKey(next) || newCost < costSoFar[next]) && next.IsWalkable)
                    {
                        costSoFar[next] = newCost;
                        frontier.Enqueue(next, newCost + HeuristicDistance(next.Position, destination.Position));
                        cameFrom[next] = current;
                    }
                });
            }

            return ConstructPath(cameFrom, source, destination, z);
        }

        private Path ConstructPath(Dictionary<MovementTile, MovementTile> cameFrom, MovementTile source, MovementTile destination, float z)
        {
            if (!cameFrom.ContainsKey(destination))
                return null;

            var path = new Path();

            var currentTile = destination;

            while (currentTile != source)
            {
                path.AddPoint(currentTile.Position.x, currentTile.Position.y, z);
                currentTile = cameFrom[currentTile];
            }

            path.Reverse();

            return path;
        }

        public float HeuristicDistance(Vector2 a, Vector2 b)
        {
            return Mathf.Abs(a.x + b.x) + Mathf.Abs(a.y + b.y);
        }
        
        private void DrawPath(Path path)
        {
            int i = 0;
            lineRenderer.positionCount = path.Count;

            path.ForEachPoint(p => lineRenderer.SetPosition(i++, p));
        }
    }
}
