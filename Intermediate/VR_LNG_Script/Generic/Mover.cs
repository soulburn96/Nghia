using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mover : MonoBehaviour
{
    public UnityEvent onMovedToPosition;

    public void MoveToPosition(Vector3 toPosition)
    {
        transform.position = toPosition;

        if (onMovedToPosition != null)
            onMovedToPosition.Invoke();
    }
}
