using UnityEngine;

public class FireCrystal : Item
{
    [SerializeField]
    GameObject FireballPrefab;

    GameObject ActiveFireball;

    public override void Usefunction(Vector2 Point, int xDirection, int yDireciton, Player player = null)
    {
        if (ActiveFireball == null)
        {
            ActiveFireball = Instantiate(FireballPrefab, new Vector3(Point.x + .5f, Point.y + .5f), Quaternion.identity);
            Fireball fireball = ActiveFireball.GetComponent<Fireball>();
            fireball.xDirection = xDirection;
            fireball.yDirection = yDireciton;
            fireball.SetVisualRotation();
        }

    }
}
