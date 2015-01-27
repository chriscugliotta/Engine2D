using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A mechanism for managing multiple scenes
  public class SceneManager : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // The active scene
    private Scene activeScene;
    // A list of all scenes
    private List<Scene> scenes;

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
        return String.Format("ScMn{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Ac:  {0}, Cn:  {1}}}", this.ActiveScene.Name, this.Scenes.Count);
    }

    // activeScene accessor
    public Scene ActiveScene
    {
      get
      {
        return this.activeScene;
      }
    }
    // scenes accessor
    public List<Scene> Scenes
    {
      get
      {
        return this.scenes;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public SceneManager()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.activeScene = null;
      this.scenes = new List<Scene>();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add a scene
    public void AddScene(Scene scene)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, scene.Name));
      #endif

      // Add to list
      this.Scenes.Add(scene);

      // Temporary:  Activate it automatically for now...
      this.activeScene = scene;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, scene.Name));
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

      // Update active scene
      this.ActiveScene.Update();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
