using System;
using System.Diagnostics;

namespace Engine2D
{
  // An engine object
  public class EngineObject
  {
    // =====
    #region Variables

    // The object's serialized index
    private int serializedIndex;

    #endregion


    // =====
    #region Properties

    // index accessor
    public int SerializedIndex
    {
      get
      {
        return this.serializedIndex;
      }
      internal set
      {
        this.serializedIndex = value;
      }
    }
    // A unique instance ID
    public virtual int ID
    {
      get
      {
        return -1;
      }
    }
    // A unique instance name
    public virtual String Name
    {
      get
      {
        return "";
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public EngineObject() { this.serializedIndex = -1; }

    #endregion


    // =====
    #region Methods

    // Serialize this object
    public virtual void Serialize()
    {
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
