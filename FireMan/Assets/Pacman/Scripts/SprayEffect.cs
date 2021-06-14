using UnityEngine;
using System.Collections;
using System;
namespace Pacman
{
    [RequireComponent(typeof(Animator))]
    public class SprayEffect : MonoBehaviour
    {
        private Animator animator;
        private Collider2D collider;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            collider = GetComponent<Collider2D>();

            animator.enabled = false;
        }

        public void Perform(Vector3 origin, Vector3 direction, float maximumDistance, float speed)
        {
            StartCoroutine(PerformCoroutine(origin, direction, maximumDistance, speed));
        }

        private IEnumerator PerformCoroutine(Vector3 origin, Vector3 direction, float maximumDistance, float speed)
        {
            transform.up = direction;
            transform.position = origin;
            
            var destination = origin + (direction * maximumDistance);

            animator.enabled = true;

            while (transform.position != destination)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }

            transform.position = destination;

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
            collider.enabled = false;
            
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f);
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var fire = other.GetComponent<Fire>();
            
            if (fire != null)
            {
                fire.GetExtinguished();              
            }                
        }
    }
}
