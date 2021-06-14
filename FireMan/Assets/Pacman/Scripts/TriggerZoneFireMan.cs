using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pacman
{
    public class TriggerZoneFireMan : MonoBehaviour
    {
        private BoxCollider2D boxCollider;
        [SerializeField] Vector3 offSet;

        public static event OnTriggerZoneEnter OnEnterTriggerZone;
        public delegate void OnTriggerZoneEnter(Vector3 position);
        public static Action OnPlayerExit;

        [SerializeField]
        private LevelManager levelManager;

        private void Start()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.offset = offSet;
        }

        private void OnDestroy()
        {
            OnPlayerExit = null;
            OnEnterTriggerZone = null;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var rescuer = collision.GetComponent<Rescuer>();
            if (rescuer != null)
            {
                rescuer.LeadFollowerToExit(transform.position);
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            var rescuer = collision.GetComponent<Rescuer>();
            if(rescuer !=null)
            {
                Rect exitZoneRect = new Rect(transform.position + new Vector3(offSet.x * transform.localScale.x,offSet.y* transform.localScale.y,0), boxCollider.bounds.size / 4);                
                Rect spiroRect = new Rect(collision.transform.position, collision.GetComponent<BoxCollider2D>().bounds.size / 4);

                if (exitZoneRect.Overlaps(spiroRect)&& !rescuer.CheckFollower())
                {
                    OnPlayerExit?.Invoke();
                }                
            }
        }
    }
}
