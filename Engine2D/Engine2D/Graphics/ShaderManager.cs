using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A mechanism for managing multiple shaders
  public class ShaderManager : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // A list of custom shaders
    private List<WrappedShader> shaders;

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
        return String.Format("ShMn{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Cn:  {0}}}", this.Shaders.Count);
    }

    // defaultShader accessor
    public WrappedShader DefaultShader
    {
      get
      {
        return this.Shaders[0];
      }
    }
    // shaders accessor
    public List<WrappedShader> Shaders
    {
      get
      {
        return this.shaders;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public ShaderManager()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.shaders = new List<WrappedShader>();
      // Add the default shader
      this.shaders.Add(new WrappedTextureShader());

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add sprite layer
    public void AddShader(WrappedShader shader)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, shader.Name));
      #endif

      // Add to list
      this.Shaders.Add(shader);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, shader.Name));
      #endif
    }

    #endregion
  }
}
