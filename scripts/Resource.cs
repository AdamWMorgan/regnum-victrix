using Godot;
using System;

public partial class Resource : Node
{
	public ResourceType Type {get; set;}
	public int Quantity {get; set;}
	
	public Resource(ResourceType type, int quantity){
		this.Type = type;
		this.Quantity = quantity;
	}
}
