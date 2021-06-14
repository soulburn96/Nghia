using UnityEngine;

namespace Pacman
{
    public class Extinguisher : MonoBehaviour
    {
        private Collider2D collider;
        private SpriteRenderer spriteRenderer;
        private AudioSource audioSrc;

        [SerializeField] public float maxSprayDistance;
        public float sprayDistance;
        [SerializeField] private float spraySpeed;
        [SerializeField] private GameObject sprayEffectPrefab;
        [SerializeField] private ParticleSystem sprayParticle;
        [SerializeField] private AudioClip pickupSound;


        private void Awake()
        {
            collider = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            audioSrc = GetComponent<AudioSource>();
            sprayDistance = maxSprayDistance;
        }

        private void Update()
        {
            spriteRenderer.sortingOrder = 3 + Mathf.RoundToInt(transform.position.y * 100f) * -1;
        }

        public void OnEquipped(GameObject picker)
        {
            transform.SetParent(picker.transform);
            transform.position   = picker.transform.position;
            transform.localScale = Vector3.one * 0.8f;
            spriteRenderer.enabled = false;
            collider.enabled = false;
            audioSrc.PlayOneShot(pickupSound);
        }

        public void SprayCollider(Vector3 direction)
        {
            var spawned = Instantiate(sprayEffectPrefab);
            var effect  = spawned.GetComponent<SprayEffect>();
            
            if (effect != null)
            {
                effect.Perform(transform.position, direction, sprayDistance, spraySpeed);
                audioSrc.Play();
            }                
        }

        public void StartSpray(Vector2 direction)
        {
            sprayParticle.transform.localScale = new Vector3(sprayParticle.transform.localScale.x, sprayParticle.transform.localScale.y, sprayDistance);

            switch (direction)
            {               
                case Vector2 v when v.Equals(Vector2.up):
                    sprayParticle.transform.localRotation = Quaternion.Euler(-90, 90, 0); 
                    break;
                case Vector2 v when v.Equals(Vector2.down):
                    sprayParticle.transform.localRotation = Quaternion.Euler(90, 90, 0);
                    break;
                case Vector2 v when v.Equals(Vector2.left):
                    sprayParticle.transform.localRotation = Quaternion.Euler(180, 90, 0);
                    break;
                case Vector2 v when v.Equals(Vector2.right):
                    sprayParticle.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    break;
                case Vector2 v when v.Equals(Vector2.zero):
                    return;                    
            }
            sprayParticle.Play();

        }
        public void StopSpray()
        {
            if(audioSrc !=null)
                audioSrc.Stop();
            if(sprayParticle!=null)
                sprayParticle.Stop();
        }
    }
}
