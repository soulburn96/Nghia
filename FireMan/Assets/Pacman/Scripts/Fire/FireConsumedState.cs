using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    public class FireConsumedState : FireState
    {
        public bool IsFinished { get; private set; }

        [SerializeField] private Transform target;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public override void OnEnter()
        {
            ResetState();
            spriteRenderer.enabled = false;
            mover.Stop();
            transform.position = target.transform.position;
            IsFinished = true;            
        }

        public override void OnExit()
        {
            spriteRenderer.enabled = true;
            fire.ResetState();
            
        }

        private void ResetState()
        {            
            IsFinished = false;
        }


    }
}

