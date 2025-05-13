using Godot;

public static class Combat
{
    
    public static float ATTACK_VARIANCE = 0.2f; // 20% variance

    public static int CalculateAttackDamage(int attack)
    {
        return (int)(attack * (1 + GD.RandRange(-ATTACK_VARIANCE, ATTACK_VARIANCE)));
    }
}
