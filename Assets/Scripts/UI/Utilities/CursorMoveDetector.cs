using UnityEngine;
using UnityEngine.InputSystem;

public class CursorMoveDetector : MonoBehaviour
{
    public static bool Idle;

    void Update()
    {
        Idle = Mouse.current.delta.ReadValue().magnitude == 0;
    }
}