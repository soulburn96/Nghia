using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    [CreateAssetMenu(fileName = "FireCollection.asset", menuName = "Pacman/FireCollection")]
    public class FireCollection : ScriptableObject
    {
        private List<Fire> fires = new List<Fire>();
        public  List<Fire> Fires => fires;

        public void Add(Fire fire)
        {
            if (fires.Contains(fire))
                return;
            
            fires.Add(fire);
        }

        public void Remove(Fire fire)
        {
            if (!fires.Contains(fire))
                return;

            fires.Remove(fire);
        }
    }
}