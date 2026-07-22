using UnityEngine;
using UnityEngine.Diagnostics;

public class CameraRegion : MonoBehaviour
{
    public Collider2D bounds;

    public Vector2 LockPos;

    public bool lockX;
    public bool lockY;

    public bool useGameObjPos;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if(player != null)
        {
            if (useGameObjPos)
                player.cam.transform.position = new Vector3(transform.position.x, transform.position.y, player.cam.transform.position.z);
            else
                player.cam.transform.position = new Vector3(LockPos.x, LockPos.y, player.cam.transform.position.z);
        }
    }

}
