using Godot;
using System;

public partial class ResourceStats : RichTextLabel
{
	private double REFRESH_RATE = 5.0;
	private double _timeAccumulator = 0.0;

	public override void _Ready()
	{
		CallDeferred(nameof(InitialiseValues));
		BbcodeEnabled = true;
		FitContent = true;
	}

	public override void _Process(double delta)
	{
		_timeAccumulator += delta;

		if (_timeAccumulator >= REFRESH_RATE)
		{
			_timeAccumulator = 0;
			Clear();
			InitialiseValues();
		}
	}

	private void InitialiseValues()
	{
		Node attachedBase = GetParent()?.GetParent();

		if (attachedBase is IBaseProvider provider)
		{
			var gold = provider.GetAttachedBase().Resources
				.Find(resource => resource.Type == ResourceType.GOLD);

			var water = provider.GetAttachedBase().Resources
				.Find(resource => resource.Type == ResourceType.WATER);

			var wheat = provider.GetAttachedBase().Resources
				.Find(resource => resource.Type == ResourceType.WHEAT);

			var wood = provider.GetAttachedBase().Resources
				.Find(resource => resource.Type == ResourceType.WOOD);

			var iron = provider.GetAttachedBase().Resources
				.Find(resource => resource.Type == ResourceType.IRON);

			if (gold != null)
			{
				AppendText($"[img]res://game/assets/materials/gold.png[/img] {gold.Quantity}");
			}

			if (water != null)
			{
				AppendText($"[img]res://game/assets/materials/water.png[/img] {water.Quantity}");
			}

			if (wheat != null)
			{
				AppendText($"[img]res://game/assets/materials/wheat.png[/img] {wheat.Quantity}");
			}

			if (wood != null)
			{
				AppendText($"[img]res://game/assets/materials/wood.png[/img] {wood.Quantity}");
			}

			if (iron != null)
			{
				AppendText($"[img]res://game/assets/materials/iron.png[/img] {iron.Quantity}");
			}
		}
	}
}
