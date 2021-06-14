using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;
using System.Linq;

namespace Pacman
{
    [RequireComponent(typeof(Mover2D))]
    public class Rescuer : MonoBehaviour
    {
        [SerializeField] private int maximumFollowers;
        
        public UnityEvent OnMaximumFollower;

        private Mover2D mover;
        private AudioSource audioSrc;
        private List<Vector3> movementHistory = new List<Vector3>();
        private Dictionary<Follower, int> followerSpot = new Dictionary<Follower, int>();        
        

        private void Awake()
        {
            mover = GetComponent<Mover2D>();
            audioSrc = GetComponent<AudioSource>();
            mover.OnStart += UpdateMovementHistory;
        }

        private void OnEnable()
        {
            TriggerZoneFireMan.OnEnterTriggerZone += LeadFollowerToExit;
            FriendExitState.OnFollowerLeaveScene += RemoveFollower;
        }

        private void OnDisable()
        {
            TriggerZoneFireMan.OnEnterTriggerZone -= LeadFollowerToExit;
            FriendExitState.OnFollowerLeaveScene -= RemoveFollower;
        }

        private void Start()
        {
            for (int i = 0; i < maximumFollowers; i++)
                UpdateMovementHistory();
        }
        
        public Vector3 RetrieveSpot(Follower follower)
        {
            var spotIndex = followerSpot[follower];

            return movementHistory[movementHistory.Count - 1 - spotIndex];
        }

        public void Lead(Follower newFollower)
        {
            if (followerSpot.ContainsKey(newFollower))
                return;

            var spotIndex = followerSpot.Count;
            followerSpot.Add(newFollower, spotIndex);
            audioSrc.Play();
            Debug.Log(newFollower.name);

            if (followerSpot.Count == maximumFollowers)
            {
                OnMaximumFollower.Invoke();
            }
        }

        public void RemoveFollower(Follower follower)
        {
            if (followerSpot.ContainsKey(follower))
                followerSpot.Remove(follower);
        }

        public void LeadFollowerToExit(Vector3 exitPosition)
        {
            StartCoroutine(Delay(1,exitPosition));
        }

        public bool CheckFollower()
        {
            if (followerSpot.Count != 0)
                return true;
            else
                return false;
        }

        IEnumerator Delay(float time, Vector3 exitPosition)
        {
            foreach (var entry in followerSpot.Keys.ToList())
            {
                entry.gameObject.GetComponent<FriendExitState>().EnterExitZone(exitPosition);
                yield return new WaitForSeconds(time);
            }
            yield break;
        }

        private void UpdateMovementHistory()
        {
            if (movementHistory.Count == maximumFollowers)
                movementHistory.RemoveAt(0);
            
            movementHistory.Add(transform.position);
        }
    }
}