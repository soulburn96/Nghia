using UnityEngine;

namespace ScriptableObjectArchitecture
{
    [CreateAssetMenu(fileName = "Vector2RawVariable.asset",
                 menuName = "Variables/Structs/Vector2Raw")]
    public class Vector2RawVariable : RawVariable<Vector2>
    {
    }
}
