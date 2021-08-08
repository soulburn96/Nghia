namespace Pacman
{
    public class FireState : MonoBehaviourState
    {
        protected Fire fire;
        protected Mover2D mover;
        protected PathFinder pathFinder;
        protected MovementMap movementMap;

        public void InjectComponents(Fire fire, Mover2D mover, PathFinder pathFinder, MovementMap movementMap)
        {
            this.fire  = fire;
            this.mover = mover;
            this.pathFinder  = pathFinder;
            this.movementMap = movementMap;
        }
    }
}