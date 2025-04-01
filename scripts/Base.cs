using Godot;
using System;
using System.Collections.Generic;

public partial class Base : Node
{
	public String ID { get; private set; }
	public BaseOwner CurrentBaseOwner { get; private set; } = BaseOwner.NONE;
	public Vector2 Position { get; private set; }
	public string Name { get; private set; } = "Base";
	public int Level { get; private set; } = 1;
	public List<IUnit> Units = new List<IUnit>();
	
	public Base(){
		this.ID = Guid.NewGuid().ToString();
	}
	
	public List<IUnit> AddUnit(IUnit unit){
		Units.Add(unit);
		GD.Print(unit.GetId() + " added to " + CurrentBaseOwner + " base " + ID);
		return Units;
	}
	
	public List<IUnit> RemoveUnit(IUnit unit){
		Units.Remove((IUnit) unit);
		return Units;
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

		public Builder SetLevel(int level)
		{
			_base.Level = level;
			return this;
		}
		
		public Builder SetUnits(IUnit unit){
			_base.Units.Add(unit);
			return this;
		}

		public Base Build()
		{
			return _base;
		}
	}
	
}
