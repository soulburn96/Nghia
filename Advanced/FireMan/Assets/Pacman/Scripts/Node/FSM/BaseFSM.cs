using System;
using System.Linq;
using System.Collections.Generic;
using Cyotek.Collections.Generic;
using UnityEngine;

namespace Node
{
    public abstract class BaseFSM
    {
        protected readonly string name;
        protected readonly CircularBuffer<INode> nodeStack;
        protected readonly List<INode> nodesPointedByEveryNode;
        protected readonly Dictionary<INode, List<Transition>> transitionsFromNode;

        protected INode selectorNode;
        protected INode exitNode;
        protected INode currentNode;
        protected INode previousNode;
        
        protected List<Transition> currentTransitionSet;

        protected bool isPaused;
        protected int maxNodeStackSize = 20;

        public string Name => name;
        public INode CurrentNode => currentNode;
        public IReadOnlyCollection<INode> Nodes => transitionsFromNode.Keys;
        public IReadOnlyCollection<Transition> CurrentTransitionSet => currentTransitionSet;
        public IReadOnlyCollection<Transition> TransitionsFrom(INode node) => transitionsFromNode[node];
        
        protected BaseFSM(string name = "Untitled FSM")
        {
            this.name = name;
            nodeStack = new CircularBuffer<INode>(maxNodeStackSize);
            
            transitionsFromNode     = new Dictionary<INode, List<Transition>>();
            nodesPointedByEveryNode = new List<INode>();
            currentTransitionSet    = new List<Transition>();
            
            selectorNode = new EmptyNode(); 
            exitNode     = new EmptyNode(); 
            currentNode  = new EmptyNode();
            previousNode = new EmptyNode();
            
            transitionsFromNode.Add(selectorNode, new List<Transition>());
            SetCurrentNode(selectorNode);
        }

        public void Pause() => isPaused = true;
        public void Resume() => isPaused = false;

        public void Start()
        {
            var qualifiedTransition = CheckForQualifiedTransition(transitionsFromNode[selectorNode]);

            if (qualifiedTransition != null)
                SetCurrentNode(qualifiedTransition.Destination);
            else
                Debug.Log($"{name}: does not have any node to start with.");
            
            currentNode.OnEnter();
        }

        public void Update()
        {
            if (isPaused)
                return;
            
            var qualifiedTransition = CheckForQualifiedTransition(currentTransitionSet);
            
            if (qualifiedTransition != null)
            {
                currentNode.OnExit();
                
                if (qualifiedTransition.Destination == previousNode && nodeStack.Size >= 2)
                {
                    nodeStack.GetLast();
                    qualifiedTransition = new Transition(qualifiedTransition.Source, nodeStack.GetLast(), qualifiedTransition.Condition);
                }
                
                SetCurrentNode(qualifiedTransition.Destination);
                
                currentNode.OnEnter();
            }
            
            currentNode.OnUpdate();
        }

        public void FixedUpdate()
        {
            if (isPaused)
                return;
            
            currentNode.OnFixedUpdate();
        }

        public bool Contain(INode node) => transitionsFromNode.ContainsKey(node);

        protected Transition CheckForQualifiedTransition(List<Transition> transitionSet)
        {
            Transition qualifiedTransition = null;

            foreach (var transition in transitionSet)
            {
                if (transition.Condition() && transition.Destination != currentNode)
                {
                    qualifiedTransition = transition;
                    break;
                }
            } 
            
            return qualifiedTransition;
        }

        public void AddNode(INode node)
        {
            if (!IsValidNode(node))
                return;
            
            // Handle first node
            if (transitionsFromNode.Count == 1)
                transitionsFromNode[selectorNode].Add(new Transition(selectorNode, node, () => true));
            
            if (!transitionsFromNode.ContainsKey(node))
                transitionsFromNode.Add(node, new List<Transition>());
        }
        
        public void AddTransition(INode source, INode destination, Func<bool> condition)
        {
            if (!IsValidNode(source) || !IsValidNode(destination))
                return;
            
            AddNode(source);
            AddNode(destination);
            
            transitionsFromNode[source].Add(new Transition(source, destination, condition));
            
            foreach (var node in nodesPointedByEveryNode.Where(node => node != selectorNode))
                transitionsFromNode[source].Insert(0, new Transition(source, node, condition));
        }
        
        public void AddTransitionFromAnyNode(INode destination, Func<bool> condition)
        {
            if (nodesPointedByEveryNode.Contains(destination))
                return;

            nodesPointedByEveryNode.Add(destination);
            
            AddNode(destination);

            foreach (var node in Nodes.Where(node => node != destination && node != selectorNode))
                transitionsFromNode[node].Insert(0, new Transition(node, destination, condition));
        }

        public void AddTransitionToPreviousNode(INode source, Func<bool> condition)
        {
            AddTransition(source, previousNode, condition);
        }
        
        public void AddTransitionToExitNode(INode source, Func<bool> condition)
        {
            AddTransition(source, exitNode, condition);
        }

        public void AddTransitionFromSelectorNode(INode destination, Func<bool> condition)
        {
            AddTransition(selectorNode, destination, condition);
        }
        
        protected void SetCurrentNode(INode node)
        {
            currentNode = node;
            
            nodeStack.Put(currentNode);
            
            currentTransitionSet = transitionsFromNode[currentNode];
        }
        
        public void SetEntry(INode node)
        {
            if (!transitionsFromNode.ContainsKey(node))
            {
                Debug.Log($"{name}: entry node must be contained in this FSM.");
                return;
            }
            
            transitionsFromNode[selectorNode].Insert(0, new Transition(selectorNode, node, () => true));
        }

        public void RemoveAllTransitionFrom(INode node)
        {
            if (!transitionsFromNode.ContainsKey(node))
                return;
            
            transitionsFromNode[node].Clear();
        }

        public abstract bool IsValidNode(INode node);
    }
}