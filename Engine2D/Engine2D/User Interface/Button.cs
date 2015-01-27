using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Engine2D
{
  // A button, which may trigger a response when pressed
  public class Button : View
  {
    // =====
    #region Variables

    // Clickable area
    public Box Shape;

    // Equals true if button is currently being pressed
    private bool isPressed;

    // Response methods
    public RespondToPress respondToPress;
    public RespondToRelease respondToRelease;

    #endregion


    // =====
    #region Delegates

    // Respond to button press
    public delegate void RespondToPress(Button button);
    // Respond to button release
    public delegate void RespondToRelease(Button button);

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{Fr:  {0}, Bn:  {1}, Cn:  {2}, iC:  {3}}}", this.Frame, this.Bounds, this.Children.Count, this.IsChild);
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Button(Box size, Box shape)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      // this.Name = Globals.NameHelper.GetName("Bttn");
      this.Frame = size;
      this.Bounds = new Box(0, 0, size.Width, size.Height);
      this.Text = null;
      this.HasChild = false;
      this.Children = new List<View>();
      this.IsChild = false;
      this.Shape = shape;

      this.isPressed = false;
      this.needsToUpdateScreenRectangle = false;
      this.screenBounds = size;
      this.screenFrame = size;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods



    #endregion


    // =====
    #region Update

    // Update
    public override void Update()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Check if button was pressed
      if (Globals.InputHandler.MouseLeftButtonPressed && Globals.InputHandler.MouseLeftButtonTime == 0 && !this.isPressed && this.screenFrame.Contains(Globals.InputHandler.MousePosition))
      {
        this.isPressed = true;
        this.Color = Color.Black * 0.4f;
        this.respondToPress(this);
        Trace.WriteLine(this.Name + " pressed!");
      }

      // Check if button was released
      else if (this.isPressed && !Globals.InputHandler.MouseLeftButtonPressed && Globals.InputHandler.MouseLeftButtonTime == 0)
      {
        this.isPressed = false;
        this.Color = Color.Black * 0.2f;
        // Check if mouse was over button upon release
        if (this.screenFrame.Contains(Globals.InputHandler.MousePosition))
        {
          this.respondToRelease(this);
          Trace.WriteLine(this.Name + " released inside!");
        }
        else
        {
          this.respondToRelease(this);
          Trace.WriteLine(this.Name + " released outside!");
        }
      }

      // Check if rectangle needs to be updated
      if (this.NeedsToUpdateScreenRectangles) { this.UpdateScreenRectangles(); }

      // Update children
      for (int i = 0; i < this.Children.Count; i++)
      {
        this.Children[i].Update();
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Draw

    #endregion
  }
}
