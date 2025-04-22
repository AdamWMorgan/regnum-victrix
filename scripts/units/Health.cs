using Godot;
using System;

public partial class Health : Node2D
{
	[Export] public int health = 100;
	[Export] public StyleBoxFlat colour;
	private ProgressBar healthBar;
	public int CurrentHealth { get; private set; }

	public override void _Ready()
	{
		CurrentHealth = health;
		healthBar = GetNode<ProgressBar>("HealthBar");
		healthBar.Value = CurrentHealth;
		healthBar.MaxValue = CurrentHealth;
		healthBar.AddThemeStyleboxOverride("fill", colour);
	}

	public int Damage(int damageValue)
	{
		CurrentHealth -= Mathf.Clamp(damageValue, 0, CurrentHealth);
		healthBar.Value = CurrentHealth;
		return CurrentHealth;
	}

	public int Heal(int healValue)
	{
		CurrentHealth += Mathf.Clamp(healValue, 0, CurrentHealth);
		healthBar.Value = CurrentHealth;
		return CurrentHealth;
	}
}
