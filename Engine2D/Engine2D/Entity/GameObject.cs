using System;
using System.Diagnostics;

namespace Engine2D
{
  // A game object
  public class GameObject : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // A geometry
    private Geometry geometry;
    // A body
    private Body body;
    // A sprite
    private Sprite sprite;

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
        return String.Format("GmOb{0:0000}", this.ID);
      }
    }
    // Description
    public override string ToString()
    {
      return this.Name;
    }

    // geometry accessor
    public Geometry Geometry
    {
      get
      {
        return this.Geometry;
      }
      set
      {
        this.geometry = value;
      }
    }
    // sprite accessor
    public Sprite Sprite
    {
      get
      {
        return this.sprite;
      }
      set
      {
        this.sprite = value;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public GameObject()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
