// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Main : Spatial {

	public AudioStreamPlayer Audio { get; } = new AudioStreamPlayer();

	private Dictionary<int, Spatial> pressedIndexesToDots = new Dictionary<int, Spatial>();

	private PackedScene dotP = GD.Load<PackedScene>("res://scenes/Dot.tscn");

	private void InitSound() {
		if (!Lib.Node.SoundEnabled) {
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
		}
	}

	public override void _Notification(int what) {
		if (what is MainLoop.NotificationWmGoBackRequest) {
			GetTree().ChangeScene("res://scenes/Menu.tscn");
		}
	}

	public override void _Ready() {
		GetNode<WorldEnvironment>("sky").Environment.BackgroundColor = new Color(Lib.Node.BackgroundColorHtmlCode);
		InitSound();
		AddChild(Audio);

	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventScreenDrag sd) {
			if (pressedIndexesToDots.ContainsKey(sd.Index)) {
				var mPos = sd.Position;
				var origin = GetNode<Camera>("cam").ProjectRayOrigin(mPos);
				var dest = origin + GetNode<Camera>("cam").ProjectRayNormal(mPos) * 5;

				var tempTrans = pressedIndexesToDots[sd.Index].Transform;
				tempTrans.origin = dest;
				pressedIndexesToDots[sd.Index].Transform = tempTrans;
			}
		} else if (@event is InputEventScreenTouch te && te.Pressed) {
			var mPos = te.Position;
			var origin = GetNode<Camera>("cam").ProjectRayOrigin(mPos);
			var dest = origin + GetNode<Camera>("cam").ProjectRayNormal(mPos) * 5;

			var dot = (Spatial)dotP.Instance();
			dot.Translate(dest);
			AddChild(dot);
			pressedIndexesToDots[te.Index] = dot;
		} else if (@event is InputEventScreenTouch re && !re.Pressed) {
			if (pressedIndexesToDots.ContainsKey(re.Index)) {
				pressedIndexesToDots[re.Index].GetNode<RigidBody>("DotPhysics").GravityScale = 1;
				pressedIndexesToDots.Remove(re.Index);
			}
		}
	}

	public override void _Process(float delta) {
		pressedIndexesToDots = pressedIndexesToDots.ToDictionary(z => z.Key, z => {
			if (z.Value.Scale.y < 1.5) {
				z.Value.Scale = z.Value.Scale.LinearInterpolate(z.Value.Scale *= 3, delta);
			}
			return z.Value;
		});
	}

}
