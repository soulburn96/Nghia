using System;

namespace Node
{
    public class Transition
    {
        public INode Source { get; }
        public INode Destination { get; }
        public Func<bool> Condition { get; }

        public Transition(INode source, INode destination, Func<bool> condition)
        {
            Source      = source;
            Destination = destination;
            Condition   = condition;
        }
    }
}