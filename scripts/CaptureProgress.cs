using Godot;
using System;
using System.Runtime.ConstrainedExecution;

public partial class CaptureProgress : Node2D
{
	[Export] public int captureProgess = 100;
	[Export] public StyleBoxFlat colour;
	private ProgressBar captureProgessBar;
	public int CurrentCaptureProgess { get; private set; }
	private int MAX_PROGRESS = 100;

	public override void _Ready()
	{
		CurrentCaptureProgess = captureProgess;
		captureProgessBar = GetNode<ProgressBar>("CaptureProgressBar");
		captureProgessBar.Value = CurrentCaptureProgess;
		captureProgessBar.MaxValue = MAX_PROGRESS;
	}

	public int Increase(int increaseValue)
	{
		CurrentCaptureProgess += Mathf.Clamp(CurrentCaptureProgess + increaseValue, 0, MAX_PROGRESS);
		captureProgessBar.Value = CurrentCaptureProgess;
		return CurrentCaptureProgess;
	}

	public int Decrease(int decreaseValue)
	{
		CurrentCaptureProgess -= Mathf.Clamp(decreaseValue, 0, CurrentCaptureProgess);
		captureProgessBar.Value = CurrentCaptureProgess;
		return CurrentCaptureProgess;
	}

	public void ColourChange(StyleBoxFlat newColour){
		colour = newColour;
		captureProgessBar.AddThemeStyleboxOverride("fill", newColour);
	}

	public void ResetCapturePoint(){
		Increase(MAX_PROGRESS);
	}
}
