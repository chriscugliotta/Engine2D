using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine2D
{
  // A mechanism for managing multiple views
  public class ViewManager
  {
    // =====
    #region Variables

    // A unique instance name
    public String Name;
    // A list of all views
    public List<View> Views;
    // The root view
    public View Root;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{Cn:  {0}}}", this.Views.Count);
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public ViewManager()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      // this.Name = Globals.NameHelper.GetName("VwMn");
      this.Views = new List<View>();
      this.Root = new View(Globals.GraphicsDevice.Screen, Globals.GraphicsDevice.Screen);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add view to root
    public void AddView(View view)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, view.Name));
      #endif

      // Add child
      this.Root.AddChild(view);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, view.Name));
      #endif
    }

    // Remove view from root
    public void RemoveView(View view)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, view.Name));
      #endif

      // Remove child
      this.Root.RemoveChild(view);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, view.Name));
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

      // Update root
      this.Root.Update();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Draw

    // Draw
    public void Draw()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Draw root
      this.Root.Draw();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
