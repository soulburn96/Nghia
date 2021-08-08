using System.Linq;
using UnityEngine;

namespace Node
{
    public class FSM : BaseFSM
    {
        public FSM(string name = "Untitled FSM") : base(name) { }

        public override bool IsValidNode(INode node)
        {
            if (node == null)
            {
                Debug.Log($"{name}: node can not be NULL.");
                return false;
            }
            
            // check if node is owned by any SubFSM
            var subFSMs = from n in Nodes 
                          where n is SubFSM 
                          select n as SubFSM;
            
            var containedSubFSM = subFSMs.FirstOrDefault(sub => sub.Nodes.Contains(node));
            
            if (containedSubFSM != null)
            {
                Debug.Log($"{name}: node is already owned by {containedSubFSM.Name}.");
                return false;
            }
            
            return true;
        }
    }
}