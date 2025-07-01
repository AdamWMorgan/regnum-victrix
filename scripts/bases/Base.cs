using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Base : Node2D
{
	public string ID { get; private set; }
	public Faction CurrentBaseOwner { get; protected set; } = Faction.NONE;
	public Vector2 BasePosition { get; protected set; }
	public BaseLevel Level { get; protected set; } = BaseLevel.ONE;
	public string BaseName { get; protected set; } = "Base";
	public List<IUnit> Units { get; private set; } = new();
	public List<Resource> Resources { get; private set; } = new();

	public override void _Ready()
	{
		ID = Guid.NewGuid().ToString();
		foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
		{
			Resources.Add(new Resource(type, 0));
		}
		GameManager.Instance.BaseRegister(this);
		AddToGroup("Bases");
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
}
