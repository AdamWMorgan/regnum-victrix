using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Base : Node2D
{
	[Export] public CaptureProgress captureProgress;
	public string ID { get; private set; }
	public Faction CurrentBaseOwner { get; protected set; } = Faction.NONE;
	public Vector2 BasePosition { get; protected set; }
	public BaseLevel Level { get; protected set; } = BaseLevel.ONE;
	public string BaseName { get; protected set; } = "Base";
	public List<IUnit> Units { get; private set; } = new();
	public List<Resource> Resources { get; private set; } = new();
	public Area2D captureArea;
	private bool captureInProgress = false;
	private float CAPTURE_SPEED = 1f;
	private float timeSinceLastCaptureDeplete = 0f;
	private int capturingUnits = 0;

	public override void _Ready()
	{
		captureArea = GetNode<Area2D>("CaptureArea");
		captureArea.BodyEntered += OnBodyEnteredCaptureArea;
		captureArea.BodyExited += OnBodyExitedCaptureArea;
		ID = Guid.NewGuid().ToString();
		foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
		{
			Resources.Add(new Resource(type, 0));
		}
		GameManager.Instance.BaseRegister(this);
		AddToGroup("Bases");
	}

	public override void _Process(double delta)
	{
		if (capturingUnits != 0) { captureInProgress = true; } else { captureInProgress = false; }

		if (captureInProgress)
		{
			if (captureProgress.CurrentCaptureProgess == 0)
			{
				captureInProgress = false;
				capturingUnits = 0;
				SwitchOwnership(CurrentBaseOwner == Faction.ENEMY ? Faction.ALLY : Faction.ENEMY);
			}

			if (timeSinceLastCaptureDeplete > CAPTURE_SPEED)
			{
				captureProgress.Decrease(10);
				timeSinceLastCaptureDeplete = 0.0f;
			}
			else
			{
				timeSinceLastCaptureDeplete += (float)delta;
			}
		}
	}

	private void SwitchOwnership(Faction newOwner)
	{
		GameManager.Instance.BaseSwitch(this);
		// Update ownership state
		ModifyBaseOwner(newOwner);

		// Reset capture progress bar

		captureProgress.ResetCapturePoint();

		// Update UI color
		var style = new StyleBoxFlat
		{
			BgColor = newOwner == Faction.ENEMY
				? ColourPalette.ENEMY.ToColor()
				: ColourPalette.ALLY.ToColor()
		};
		captureProgress.ColourChange(style);
	}

	public List<IUnit> AddUnit(IUnit unit)
	{
		Units.Add(unit);
		return Units;
	}

	public List<IUnit> RemoveUnit(IUnit unit)
	{
		Units.Remove(unit);
		return Units;
	}

	public int ReceiveResource(ResourceType type, int quantity)
	{
		Resource resource = Resources.Find(res => res.Type == type);
		resource.Quantity += quantity;
		return resource.Quantity;
	}

	public bool SpendResources(Dictionary<ResourceType, int> resourceCosts)
	{
		// Check if we have enough resources
		foreach (var kvp in resourceCosts)
		{
			Resource resource = Resources.Find(res => res.Type == kvp.Key);
			if (resource == null || resource.Quantity < kvp.Value)
			{
				return false; // Not enough resources
			}
		}

		// Deduct resources
		foreach (var kvp in resourceCosts)
		{
			Resource resource = Resources.Find(res => res.Type == kvp.Key);
			resource.Quantity -= kvp.Value;
		}

		return true; // Successfully spent resources
	}

	public BaseLevel LevelUp()
	{
		this.Level = LevellingUtil<BaseLevel>.LevelUp((int)this.Level);
		return this.Level;
	}

	public Faction ModifyBaseOwner(Faction newOwner)
	{
		this.CurrentBaseOwner = newOwner;
		return CurrentBaseOwner;
	}

	private void OnBodyEnteredCaptureArea(Node body)
	{
		if ((body.IsInGroup("Player") || body.IsInGroup("Ally")) && CurrentBaseOwner != Faction.ALLY)
		{
			capturingUnits++;
		}
		else if (body.IsInGroup("Enemy") && CurrentBaseOwner != Faction.ENEMY)
		{
			capturingUnits++;
		}
	}

	// Called when a body exits the Area2D
	private void OnBodyExitedCaptureArea(Node body)
	{
		if ((body.IsInGroup("Player") || body.IsInGroup("Ally")) && CurrentBaseOwner != Faction.ALLY)
		{
			capturingUnits--;
		}
		if (body.IsInGroup("Enemy") && CurrentBaseOwner != Faction.ENEMY)
		{
			capturingUnits--;
		}
	}
}
