using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Base : Node2D
{
	public String ID { get; private set; }
	public BaseOwner CurrentBaseOwner { get; private set; } = BaseOwner.NONE;
	public Vector2 Position { get; private set; }
	public BaseLevel level {get; private set;} = BaseLevel.ONE;
	public string Name { get; private set; } = "Base";
	public List<IUnit> Units = new List<IUnit>();
	public List<Resource> Resources = new List<Resource>();
		
	public Base(){
		this.ID = Guid.NewGuid().ToString();
		foreach (ResourceType type in Enum.GetValues(typeof(ResourceType))){
			this.Resources.Add(new Resource(type, 0));
		}
	}
	
	public override void _Ready(){
		AddToGroup("Bases");
	}
	
	public List<IUnit> AddUnit(IUnit unit){
		Units.Add(unit);
		return Units;
	}
	
	public List<IUnit> RemoveUnit(IUnit unit){
		Units.Remove((IUnit) unit);
		this.Resources.ForEach(res => GD.Print(res.Type));
		return Units;
	}
	
	public int receiveResource(ResourceType type, int quantity){
		Resource resource = Resources.Find(res => res.Type == type);
		resource.Quantity += quantity;
		GD.Print("Base " + ID + " now has " + resource.Quantity + " " + resource.Type);
		return resource.Quantity;
	}
	
	public BaseLevel LevelUp(){
		this.level = LevellingUtil<BaseLevel>.LevelUp((int)this.level);
		return this.level;
	}
	
	public enum BaseOwner
	{
		ALLY,
		ENEMY,
		NONE
	}
	
	public class Builder{
		private Base _base = new Base();

		public Builder SetOwner(Base.BaseOwner owner)
		{
			_base.CurrentBaseOwner = owner;
			return this;
		}

		public Builder SetPosition(Vector2 position)
		{
			_base.Position = position;
			return this;
		}

		public Builder SetName(string name)
		{
			_base.Name = name;
			return this;
		}
		
		public Builder SetUnits(IUnit unit){
			_base.Units.Add(unit);
			return this;
		}

		public Builder SetResources(List<Resource> resources){
			resources.ForEach(r => {
				Resource resource = _base.Resources.Find( existingResource => existingResource.Type == r.Type);
				if(resource==null){
					_base.Resources.Add(r);
				} else {
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
