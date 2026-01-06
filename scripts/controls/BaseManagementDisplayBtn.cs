using Godot;
using System;

public partial class BaseManagementDisplayBtn : Button
{
	private Control baseManagementPanel;
	
	public override void _Ready()
	{
		baseManagementPanel = GetParent().GetNode<Control>("BaseManagementPanel");
		this.Pressed += OnPressed;
	}

	private void OnPressed()
	{
		baseManagementPanel.Visible = !baseManagementPanel.Visible;
	}
}
