using Node;
using UnityEngine;

namespace Pacman
{
    public class MonoBehaviourState : MonoBehaviour, INode
    {
        public virtual void OnEnter() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnExit() { }
    }
}