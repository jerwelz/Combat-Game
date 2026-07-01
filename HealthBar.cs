using Godot;
using System;
using System.ComponentModel;

public partial class HealthBar : ProgressBar {

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MaxValue = 100;
		Value = 100;
	}

	public void SetHealth(int health)
	{
		CreateTween().TweenProperty(this, "value", health, 0.4);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
