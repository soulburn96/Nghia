using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys parent after some time. Used to destroy Swooshes
/// </summary>

public class SelfDestroySwooshes : MonoBehaviour
{

	public float lifespan = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
		Invoke("SelfDestroy", lifespan);
    }


	void SelfDestroy()
	{
		Destroy(gameObject);
	}
}
