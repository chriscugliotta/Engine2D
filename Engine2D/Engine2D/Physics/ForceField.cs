using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  public abstract class ForceField : EngineObject
  {
    // =====
    #region Properties

    // A unique instance name
    public abstract String Name { get; }
    // The scene to which this object belongs
    public abstract Scene Scene { get; }
    // Area of effect
    public abstract Geometry Geometry { get; }
    // Attractive direction
    public abstract Vector2 Direction { get; }
    // Coefficient of strength, e.g. 'mass'
    public abstract float Coefficient { get; }
    // Exponent of distance, e.g. '2'
    public abstract float Exponent { get; }

    #endregion


    // =====
    #region Methods

    // Apply force on a physical object
    public abstract Impulse ApplyForceOn(RigidBody body);

    // Calculate force on a nearby object
    public abstract Vector2 CalculateForceOn(float mass, Vector2 position);

    // Calculate impulse on a nearby body over some time
    public abstract Impulse GetImpulse(Body body, float dt);

    #endregion


    // =====
    #region Update

    // Update
    public abstract void Update();

    #endregion
  }
}
