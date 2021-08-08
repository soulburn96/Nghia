using UnityEngine;

namespace Pacman
{
    public class FriendIdleState : MonoBehaviourState
    {
        [SerializeField] private float duration;
        [SerializeField] private float minimumDistanceToFire;

        private Friend friend;
        private float elapsedTime;
        
        public bool DetectFire { get; private set; }
        public bool IsFinished { get; private set; }

        public void InjectComponent(Friend friend)
        {
            this.friend = friend;
        }

        public override void OnEnter()
        {
            ResetStates();
            friend.PlayPanicAnimation();
        }

        public override void OnUpdate()
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > duration)
                IsFinished = true;
        }

        private void ResetStates()
        {
            elapsedTime = 0;
            IsFinished  = false;
        }

        public override void OnFixedUpdate()
        {
            var fire = friend.DetectNearestFire(minimumDistanceToFire);

            if (fire != null)
                DetectFire = true;
        }
    }
}