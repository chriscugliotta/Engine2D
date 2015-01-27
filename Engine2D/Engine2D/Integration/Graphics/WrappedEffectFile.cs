using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A wrapped .fx file
  public class WrappedEffectFile : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // An XNA effect object
    private Effect effect;

    #endregion


    // =====
    #region Properties

    // id accessor
    public override int ID
    {
      get
      {
        return this.id;
      }
    }
    // Name
    public override String Name
    {
      get
      {
        return String.Format("WEff{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return this.Name;
    }

    // effect accessor
    public Effect Effect
    {
      get
      {
        return this.effect;
      }
    }

    #endregion


    // =====
    #region Constructors

    // 1-argument constructor
    public WrappedEffectFile(String path)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.effect = Globals.Game1.Content.Load<Effect>(path);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Set a boolean parameter
    public void SetParameter(String parameterName, bool value)
    {
      this.Effect.Parameters[parameterName].SetValue(value);
    }
    // Set an integer parameter
    public void SetParameter(String parameterName, int value)
    {
      this.Effect.Parameters[parameterName].SetValue(value);
    }
    // Set an integer array parameter
    public void SetParameter(String parameterName, int[] value)
    {
      this.Effect.Parameters[parameterName].SetValue(value);
    }
    // Set a float parameter
    public void SetParameter(String parameterName, float value)
    {
      this.Effect.Parameters[parameterName].SetValue(value);
    }
    // Set a float array parameter
    public void SetParameter(String parameterName, float[] value)
    {
      this.Effect.Parameters[parameterName].SetValue(value);
    }
    // Set a vector2 parameter
    public void SetParameter(String parameterName, Microsoft.Xna.Framework.Vector2 value)
    {
      this.Effect.Parameters[parameterName].SetValue(value);
    }
    // Set a vector3 parameter
    public void SetParameter(String parameterName, Microsoft.Xna.Framework.Vector3 value)
    {
      this.Effect.Parameters[parameterName].SetValue(value);
    }
    // Set a vector3 parameter
    public void SetParameter(String parameterName, Microsoft.Xna.Framework.Vector4 value)
    {
      this.Effect.Parameters[parameterName].SetValue(value);
    }
    // Set a matrix parameter
    public void SetParameter(String parameterName, Microsoft.Xna.Framework.Matrix value)
    {
      this.Effect.Parameters[parameterName].SetValue(value);
    }
    // Set a texture parameter
    public void SetParameter(String parameterName, WrappedTexture value)
    {
      this.Effect.Parameters[parameterName].SetValue(value.Texture);
    }
    // Set a texture parameter
    public void SetParameter(String parameterName, WrappedRenderTarget value)
    {
      this.Effect.Parameters[parameterName].SetValue(value.RenderTarget);
    }

    #endregion
  }
}
