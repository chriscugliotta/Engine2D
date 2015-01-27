using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A mechanism for managing multiple cameras
  public class CameraManager : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // List of cameras
    private List<Camera> cameras;

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
        return String.Format("CmMn{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Cn:  {0}}}", this.Cameras.Count);
    }

    // cameras accessor
    public List<Camera> Cameras
    {
      get
      {
        return this.cameras;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public CameraManager()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.cameras = new List<Camera>();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add scene
    public void AddCamera(Camera camera)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, camera.Name));
      #endif

      // Add to primary list
      this.Cameras.Add(camera);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, camera.Name));
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

      // Update cameras
      for (int i = 0; i < this.Cameras.Count; i++)
      {
        this.Cameras[i].Update();
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
