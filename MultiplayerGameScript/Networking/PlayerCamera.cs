using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	public GameObject Target;

	void LateUpdate() {
		if (Target == null) {
			return;
		}
		transform.position = Target.transform.position;
		transform.rotation = Target.transform.rotation;
	}
}
