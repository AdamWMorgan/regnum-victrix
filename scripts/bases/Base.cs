using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Base : Node2D
{
	public string ID { get; private set; }
	public Faction CurrentBaseOwner { get; private set; } = Faction.NONE;
	public Vector2 BasePosition { get; private set; }
	public BaseLevel Level { get; private set; } = BaseLevel.ONE;
	public string BaseName { get; private set; } = "Base";
	public List<IUnit> Units = new();
	public List<Resource> Resources = new();

	public Base()
	{
		this.ID = Guid.NewGuid().ToString();
		foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
		{
			this.Resources.Add(new Resource(type, 0));
		}
		GameManager.Instance.BaseRegister(this);
	}

	public override void _Ready()
	{
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

	public Faction ModifyBaseOwner(Faction newOwner){
		this.CurrentBaseOwner = newOwner;
		return CurrentBaseOwner;
	}

	public class Builder
	{
		private Base _base = new();

		public Builder SetOwner(Faction owner)
		{
			_base.CurrentBaseOwner = owner;
			return this;
		}

		public Builder SetPosition(Vector2 position)
		{
			_base.BasePosition = position;
			return this;
		}

		public Builder SetName(string name)
		{
			_base.BaseName = name;
			return this;
		}

		public Builder SetUnits(IUnit unit)
		{
			_base.Units.Add(unit);
			return this;
		}

		public Builder SetResources(List<Resource> resources)
		{
			resources.ForEach(r =>
			{
				Resource resource = _base.Resources.Find(existingResource => existingResource.Type == r.Type);
				if (resource == null)
				{
					_base.Resources.Add(r);
				}
				else
				{
					resource.Quantity += r.Quantity;
				}
			});
			return this;
		}

		public Base Build()
		{
			return _base;
		}
	}

}
