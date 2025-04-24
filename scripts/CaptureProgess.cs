using Godot;
using System;

public partial class CaptureProgessh : Node2D
{
	[Export] public int captureProgess = 100;
	[Export] public StyleBoxFlat colour;
	private ProgressBar captureProgessBar;
	public int CurrentCaptureProgess { get; private set; }

	public override void _Ready()
	{
		CurrentCaptureProgess = captureProgess;
		captureProgessBar = GetNode<ProgressBar>("CaptureProgressBar");
		captureProgessBar.Value = CurrentCaptureProgess;
		captureProgessBar.MaxValue = CurrentCaptureProgess;
		captureProgessBar.AddThemeStyleboxOverride("fill", colour);
	}

	public int Increase(int increaseValue)
	{
		CurrentCaptureProgess -= Mathf.Clamp(increaseValue, 0, CurrentCaptureProgess);
		captureProgessBar.Value = CurrentCaptureProgess;
		return CurrentCaptureProgess;
	}

	public int Decrease(int decreaseValue)
	{
		CurrentCaptureProgess += Mathf.Clamp(decreaseValue, 0, CurrentCaptureProgess);
		captureProgessBar.Value = CurrentCaptureProgess;
		return CurrentCaptureProgess;
	}

	public void ColourChange(StyleBoxFlat newColour){
		colour = newColour;
		captureProgessBar.AddThemeStyleboxOverride("fill", newColour);
	}
}
