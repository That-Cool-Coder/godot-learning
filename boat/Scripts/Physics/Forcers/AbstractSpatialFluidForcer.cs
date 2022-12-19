using Godot;
using System.Linq;

namespace Physics.Forcers
{
    public abstract class AbstractSpatialFluidForcer : Spatial
    {
        // Path to the target of this forcer. If parent is a SpatialFluidEffectable and path is null, then parent is used
        [Export] public NodePath TargetPath { get; set; }
        [Export] public bool Enabled { get; set; } = true;
        protected SpatialFluidEffectable target { get; private set; }

        public override void _Ready()
        {
            if (GetParent() is SpatialFluidEffectable t && TargetPath == null) target = t;
            else target = GetNode<SpatialFluidEffectable>(TargetPath);
            base._Ready();
        }

        // Should return a force in global coordinates
        public abstract Vector3 CalculateForce(Fluids.ISpatialFluid fluid);

        public override void _PhysicsProcess(float delta)
        {
            if (!Enabled) return;

            var totalForce = target.Fluids.Select(x => CalculateForce(x)).Aggregate(Vector3.Zero, (s, d) => s + d);
            var position = target.ToGlobal(Translation);
            position -= target.GlobalTransform.origin;
            target.ApplyImpulse(position, totalForce * delta);
            base._PhysicsProcess(delta);
        }
    }
}