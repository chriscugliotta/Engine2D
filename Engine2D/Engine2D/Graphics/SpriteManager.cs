using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A mechanism for managing multiple sprites
  public class SpriteManager : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // A list of sprite layers, ordered from bottom to top
    private List<SpriteLayer> layers;

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
        return String.Format("SpMn{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Cn:  {0}}}", this.Layers.Count);
    }

    // layers accessor
    public List<SpriteLayer> Layers
    {
      get
      {
        return this.layers;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public SpriteManager()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.layers = new List<SpriteLayer>();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add sprite layer
    public void AddLayer(SpriteLayer layer)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, layer.Name));
      #endif

      // Add to primary list
      this.Layers.Add(layer);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, layer.Name));
      #endif
    }

    #endregion


    // =====
    #region Update

    // Update
    public void Update()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Update layers
      for (int i = 0; i < this.Layers.Count; i++)
      {
        this.Layers[i].Update();
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
