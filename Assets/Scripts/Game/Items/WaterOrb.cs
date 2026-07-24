using UnityEngine;

public class WaterOrb : Item
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ActivateEffectOnPickup(Player player) 
    {
        base.ActivateEffectOnPickup(player);
        player.canWalkInWater = true;//TODO might want to make these a function so that we can add a visual effect or something
    }
    
    public override void ActivateEffectOnPutDown(Player player)
    {
        base.ActivateEffectOnPutDown(player);
        player.canWalkInWater = false;
    }
}
