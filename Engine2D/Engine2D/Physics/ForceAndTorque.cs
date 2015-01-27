using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A translational force and a torque
  public struct ForceAndTorque
  {
    // Translational force
    public Vector2 Force;
    // Torque
    public float Torque;

    // Description
    public override String ToString()
    {
      return String.Format("{{Fc:  {0}, Tq:  {1:0.0000}}}", this.Force, this.Torque);
    }

    // Designated constructor
    public ForceAndTorque(Vector2 force, float torque)
    {
      // Set instance variables
      this.Force = force;
      this.Torque = torque;
    }
  }
}
