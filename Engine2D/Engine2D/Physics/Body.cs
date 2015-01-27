using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A physical object with uniformly distributed mass
  public abstract class Body : EngineObject
  {
    // =====
    #region Variables

    #endregion


    // =====
    #region Properties

    // The scene to which this entity belongs
    public abstract Scene Scene { get; }

    // Geometry
    public abstract Geometry Geometry { get; }
    // Material
    public abstract String Material { get; set; }
    // Mass
    public abstract float Mass { get; set; }
    // Velocity
    public abstract Vector2 Velocity { get; set; }
    // Angular velocity
    public abstract float AngularVelocity { get; set; }
    // Rotational axis
    public abstract Vector2 RotationalAxis { get; set; }
    // Moment of inertia
    public abstract float MomentOfInertia { get; set; }
    // List of queued applied impulses
    public abstract List<Impulse> AppliedImpulses { get; set; }
    // List of contacts with neighboring bodies
    public abstract List<Contact> Contacts { get; set; }
    // Sweep
    public abstract Sweep Sweep { get; set; }
    // Contact rank
    public abstract int Rank { get; set; }

    // Equals true if object is at rest
    public abstract bool IsSleeping { get; set; }
    // Equals true if object can be translated
    public abstract bool IsTranslatable { get; set; }
    // Equals true if object can be rotated
    public abstract bool IsRotatable { get; set; }

    // Density
    public abstract float Density { get; set; }
    // Translational kinetic energy
    public abstract float TranslationalEnergy { get; }
    // Rotational kinetic energy
    public abstract float RotationalEnergy { get; }
    // Total kinetic energy
    public abstract float KineticEnergy { get; }

    #endregion


    // =====
    #region Methods

    // Get velocity at point
    public abstract Vector2 GetVelocityAtPoint(Vector2 point);

    // Get projected velocity
    public abstract float GetProjectedVelocity(Vector2 axis, bool fromLeft);

    // Move
    public abstract void Move(float t);

    // Add field impulses to queue
    public abstract void GetFieldImpulses(float dt);

    #endregion


    // =====
    #region Update

    // Update
    public abstract void Update();

    #endregion
  }
}
