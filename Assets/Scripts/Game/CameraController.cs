using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector2 targPos;
    public float speed = 5;

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)(targPos - (Vector2)transform.position) * Time.deltaTime * speed;
    }
}
