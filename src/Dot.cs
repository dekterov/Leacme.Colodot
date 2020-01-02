using Godot;
using System;

public class Dot : Spatial {

	MeshInstance dotMesh;
	private float timeRemains;
	private Color newCol;

	public override void _Ready() {
		GetNode<RigidBody>("DotPhysics").GravityScale = 0;

		dotMesh = GetNode<MeshInstance>("DotPhysics/DotCollision/Dot");
		var mat = new SpatialMaterial();
		mat.AlbedoColor = Color.FromHsv(1, 1, 1, 0);
		mat.EmissionEnabled = true;
		mat.Emission = Color.FromHsv(GD.Randf(), 1, 1);
		mat.EmissionEnergy = 1;
		dotMesh.MaterialOverride = mat;
	}

	public override void _Process(float delta) {
		if (GetNode<RigidBody>("DotPhysics").Transform.origin.y < -110) {
			QueueFree();
		}

		if (timeRemains <= delta) {
			((SpatialMaterial)dotMesh.MaterialOverride).AlbedoColor = newCol;
			newCol = Color.FromHsv(GD.Randf(), 1, 1);
			timeRemains = 1.0f;
		} else {
			((SpatialMaterial)dotMesh.MaterialOverride).AlbedoColor = ((SpatialMaterial)dotMesh.MaterialOverride).AlbedoColor.LinearInterpolate(newCol, delta / timeRemains);
			timeRemains -= delta;
		}

	}

}
