using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A player
  public class Player : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

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
        return String.Format("Play{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return this.Name;
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public Player()
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
