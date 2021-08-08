using UnityEngine;
using ScriptableObjectArchitecture;

public class JoystickInput : MonoBehaviour
{
    [SerializeField] private Joystick joystick = null;

    [SerializeField] Vector2RawVariable inputAxis = null;

    private void Update()
    {
        inputAxis.Value = joystick.Direction;
    }
}
