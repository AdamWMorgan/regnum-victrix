using Godot;
using System;

public partial class Resource : Node
{
	public ResourceType Type {get; set;}
	public int Quantity {get; set;} = 0;
	
	public Resource(ResourceType type){
		this.Type = type;
	}
	
	public Resource(ResourceType type, int quantity){
		this.Type = type;
		this.Quantity = quantity;
	}
	
	public int IncrementResourceQuantity(int amount){
		return this.Quantity += amount;
	}
}
