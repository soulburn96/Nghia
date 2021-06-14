namespace Pacman
{
    public class FriendFollowState : MonoBehaviourState
    {
        private Friend friend;

        public void InjectComponent(Friend friend)
        {
            this.friend = friend;
        }

        public override void OnEnter()
        {
            friend.Focus(1);
        }

        public override void OnFixedUpdate()
        {
            var followSpot = friend.RetrieveSpot();
            friend.Mover.Move(followSpot);
            
            friend.PlayWalkAnimation();
        }
    }
}