using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VarjoExample;

[CreateAssetMenu(fileName ="TargetCollection.asset", menuName ="Varjo/TargetCollection")]
public class TargetCollection : ScriptableObject
{
    private List<VarjoGazeTarget> targetsList = new List<VarjoGazeTarget>();
    public List<VarjoGazeTarget> TargetsList => targetsList;

    public void Add(VarjoGazeTarget target)
    {
        if (targetsList.Contains(target))
            return;
        targetsList.Add(target);
    }

    public void Remove(VarjoGazeTarget target)
    {
        if (!targetsList.Contains(target))
            return;

        targetsList.Remove(target);
    }
}
