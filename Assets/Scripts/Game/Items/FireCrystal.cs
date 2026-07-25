using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireCrystal : Item
{
    [SerializeField]
    GameObject FireballPrefab;

    GameObject ActiveFireball;

    // Don't you see we could light up the night
    Light2D light2d;
    float initialFalloff;
    float initialOuterRadius;

    [SerializeField] float pickupFalloff;
    [SerializeField] float pickupOuterRadius;

    private void Start()
    {
        light2d = GetComponent<Light2D>();
        initialFalloff = light2d.falloffIntensity;
        initialOuterRadius = light2d.pointLightOuterRadius;
        
    }

    public override void Usefunction(Vector2Int Point, int xDirection, int yDireciton, Player player = null)
    {
        if (ActiveFireball == null)
        {
            ActiveFireball = Instantiate(FireballPrefab, new Vector3(Point.x + .5f, Point.y + .5f), Quaternion.identity);
            Fireball fireball = ActiveFireball.GetComponent<Fireball>();
            fireball.xDirection = xDirection;
            fireball.yDirection = yDireciton;
            fireball.SetVisualRotation();
            Player.instance.ShootFireball();
        }

    }

    public override void ActivateEffectOnPickup(Player player = null)
    {
        base.ActivateEffectOnPickup(player);
        light2d.falloffIntensity = pickupFalloff;
        light2d.pointLightOuterRadius = pickupOuterRadius;
        // player.GetComponent<Light2D>().enabled = false;
    }

    public override void ActivateEffectOnPutDown(Player player = null)
    {
        base.ActivateEffectOnPutDown(player);
        light2d.falloffIntensity = initialFalloff;
        light2d.pointLightOuterRadius = initialOuterRadius;
        // player.GetComponent<Light2D>().enabled = true;
    }
}
