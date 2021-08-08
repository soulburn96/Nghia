using UnityEngine;
using System;
namespace Pacman
{
    public class Follower : MonoBehaviour
    {
        protected Rescuer currentRescuer;
        protected bool isBeingRescued;
        public bool IsBeingRescued => isBeingRescued;


        public static event Action<Follower> OnFollow;

        private void OnDestroy()
        {
            OnFollow = null;
        }

        private void OnTriggerStay2D(Collider2D other)
        {

            if (isBeingRescued)
                return;
            
            var rescuer = other.GetComponent<Rescuer>();
            var friend = gameObject.GetComponent<Friend>();

            if (rescuer != null)
            {
                Rect followerRect = new Rect(transform.position, GetComponent<BoxCollider2D>().bounds.size / 4);
                Rect rescuerRect = new Rect(rescuer.transform.position, rescuer.GetComponent<BoxCollider2D>().bounds.size / 4);

                if (followerRect.Overlaps(rescuerRect))
                {
                    rescuer.Lead(this);
                    OnFollow?.Invoke(this);
                    isBeingRescued = true;
                    currentRescuer = rescuer;
                }          
            }
        }

        public Vector3 RetrieveSpot()
        {
            return currentRescuer.RetrieveSpot(this);
        }

        public Vector3 SpiroPosition()
        {
            return currentRescuer.gameObject.transform.position;
        }
    }
}