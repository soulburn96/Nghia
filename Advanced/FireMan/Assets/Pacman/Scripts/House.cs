using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace Pacman
{
    public class House : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MovementMap movementMap;
        [SerializeField] private TileBase doorTileBase;
        [SerializeField] private Transform[] doors;

        private List<Fire> inHouseFires = new List<Fire>();
        private List<Fire> escapingFires = new List<Fire>();
        
        private bool isOpen;
        public  bool IsOpen => isOpen;
        
        public void OpenDoor()
        {
            foreach (var door in doors)
            {
                movementMap.UpdateCost(door.position, 1);
                movementMap.PaintTile(door.position, null);
            }
            isOpen = true;
        }
        
        public void CloseDoor()
        {
            foreach (var door in doors)
            {
                movementMap.UpdateCost(door.position, MovementTile.MaxMovementCost);
                movementMap.PaintTile(door.position, doorTileBase);
            }
            isOpen = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var fire = other.GetComponent<Fire>();
            
            if (fire != null)
                inHouseFires.Add(fire);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var fire = other.GetComponent<Fire>();

            if (fire != null)
            {
                escapingFires.Remove(fire);
                inHouseFires.Remove(fire);
                if(escapingFires.Count ==0)
                    CloseDoor();
            }
        }
        public void AddEscapeFire(Fire fire)
        {
            escapingFires.Add(fire);
        }

        public bool IsInHouse(Fire fire)
        {
            return inHouseFires.Contains(fire);
        }
    }
}
