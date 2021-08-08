using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    public class FriendExitState : MonoBehaviourState
    {
        private Friend friend;
        private bool isExit;
        private bool isLeftScene = false;
        public bool IsExit => isExit;
        private Vector3 exitPosition;
        [SerializeField] private AudioClip escapeAudio;
        private AudioSource audioSrc;

        public static event Action<Follower> OnFollowerLeaveScene;

        private void Awake()
        {
            audioSrc = GetComponent<AudioSource>();
        }

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
            if (isLeftScene)
                return;
            else
            {
                if (transform.position == exitPosition)
                {
                    GetComponent<SpriteRenderer>().enabled = false;
                    audioSrc.PlayOneShot(escapeAudio);
                    isLeftScene = true;
                    OnFollowerLeaveScene?.Invoke(GetComponent<Follower>());
                }
                var spiroPos = friend.SpiroPosition();
                friend.Mover.Move(exitPosition);
                friend.PlayWalkAnimation();
            }               
        }

        public void EnterExitZone(Vector3 position)
        {
            isExit = true;
            exitPosition = position;
        }
    }
}

