using Godot;
using System;

public partial class ResourceStats : RichTextLabel
{
	private Node2D attachedBase;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{		
		CallDeferred(nameof(InitialiseValues));
		// Enable BBCode parsing so [img] tags work
		BbcodeEnabled  = true;
		FitContent = true;
	}

	public override void _Process(double delta)
	{
		Clear();
		InitialiseValues();
	}


	private void InitialiseValues(){
		Node attachedBase = GetParent()?.GetParent();

		if(attachedBase is IBaseProvider provider){
			GD.Print();
			AppendText("[img]res://game/assets/materials/gold.png[/img] " + provider.GetAttachedBase().Resources.Find(resource => resource.Type == ResourceType.GOLD).Quantity);
		}
	}
}
