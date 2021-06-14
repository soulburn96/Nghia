using System.Linq;
using UnityEngine;

namespace Node
{
    public class SubFSM : BaseFSM, INode
    {
        private bool isFinished;
        private FSM ownerFSM;

        public bool IsFinished => isFinished;

        public SubFSM(FSM ownerFSM, string name = "Untitled Sub FSM") : base(name)
        {
            this.ownerFSM = ownerFSM;
            this.ownerFSM.AddNode(this);

            this.exitNode = new ActionNode() { EnterAction = () => isFinished = true };
        }
        
        public void OnEnter()
        {
            isFinished = false;
            Start();
        }

        public void OnUpdate() => Update();
        public void OnFixedUpdate() => FixedUpdate();

        public void OnExit() => nodeStack.Clear();

        public override bool IsValidNode(INode node)
        {
            if (node == null)
            {
                Debug.Log($"{name}: node can not be NULL.");
                return false;
            }

            if (node == this)
            {
                Debug.Log($"{name}: can not reference itself.");
                return false;
            }
            
            if (node is SubFSM)
            {
                Debug.Log($"{name}: can not contain another SubFSM.");
                return false;
            }

            if (ownerFSM.Nodes.Contains(node))
            {
                Debug.Log($"{name}: node is already owned by OwnerFSM.");
                return false;
            }
            
            return true;
        }
    }
}