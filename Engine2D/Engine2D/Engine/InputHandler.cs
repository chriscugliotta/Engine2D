using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace Engine2D
{
  // A mechanism for detecting and responding to user input
  public class InputHandler : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // A time limit, i.e. 'how long we remember archived input'
    public float MemorySpan;

    // The following lists form a data table,
    // where cell (i, j) corresponds to index i of list j.

    // An archive of user input timestamps, expressed in terms of TotalGameTime
    public List<float> Times;
    // An archive of user input devices, represented by strings
    public List<String> Devices;
    // An archive of device signals, represented by strings
    public List<String> Signals;

    // Maximum time between consecutive clicks to produce a double click
    public float DoubleClickThreshold;
    // XNA MouseState object
    public MouseState MouseState;

    // Equals true if, last update, the mouse's left button was pressed down
    public bool MouseLeftButtonPressed;
    // Time elapsed since the mouse's left button entered its current state
    public float MouseLeftButtonTime;
    // Equals true if, over the course of the past few updates, the mouse's
    // left button has been clicked once
    public bool MouseLeftButtonClicked;
    public bool MouseLeftButtonSingleClicked;
    public bool MouseLeftButtonDoubleClicked;

    // Equals true if, last update, the mouse's right button was pressed down
    private bool mouseRightButtonPressed;
    // Time elapsed since the mouse's right button entered its current state
    private float mouseRightButtonTime;
    // Equals true if, over the course of the past few updates, the mouse's
    // right button has been clicked once
    private bool mouseRightButtonClicked;

    // Equals true if, last update, the mouse's middle button was pressed down
    private bool mouseMiddleButtonPressed;
    // Time elapsed since the mouse's left button entered its current state
    private float mouseMiddleButtonTime;
    // Equals true if, over the course of the past few updates, the mouse's
    // middle button has been clicked once
    private bool mouseMiddleButtonClicked;

    // Response methods
    public RespondToMouseLeftButtonClick respondToMouseLeftButtonClick;
    public RespondToMouseLeftButtonDoubleClick respondToMouseLeftButtonDoubleClick;
    public RespondToMouseRightButtonClick respondToMouseRightButtonClick;
    public RespondToMouseRightButtonDoubleClick respondToMouseRightButtonDoubleClick;
    public RespondToMouseMiddleButtonClick respondToMouseMiddleButtonClick;
    public RespondToMouseMiddleButtonDoubleClick respondToMouseMiddleButtonDoubleClick;



    #endregion


    // =====
    #region Delegates

    // Mouse left click response
    public delegate void RespondToMouseLeftButtonClick();

    // Mouse left double click response
    public delegate void RespondToMouseLeftButtonDoubleClick();

    // Mouse right click response
    public delegate void RespondToMouseRightButtonClick();

    // Mouse left double click response
    public delegate void RespondToMouseRightButtonDoubleClick();

    // Mouse middle click response
    public delegate void RespondToMouseMiddleButtonClick();

    // Mouse middle double click response
    public delegate void RespondToMouseMiddleButtonDoubleClick();

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
        return String.Format("InHn{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Cn: {0}, Tm: {1:0.0000}}}", this.Times.Count, this.MemorySpan);
    }

    // Mouse description
    public String MouseLeftButtonState
    {
      get
      {
        return String.Format("{{Pr:  {0}, Tm:  {1:0.0000}, Ck:  {2}}}",
          this.MouseLeftButtonPressed, this.MouseLeftButtonTime, this.MouseLeftButtonClicked);
      }
    }
    // Current mouse position
    public Vector2 MousePosition
    {
      get
      {
        return new Vector2(this.MouseState.X, this.MouseState.Y);
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public InputHandler()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.MemorySpan = 10.0f;
      this.Times = new List<float>();
      this.Devices = new List<String>();
      this.Signals = new List<String>();

      this.DoubleClickThreshold = 0.2f;

      this.MouseLeftButtonPressed = false;
      this.MouseLeftButtonTime = 0.0f;
      this.MouseLeftButtonClicked = false;

      this.mouseRightButtonPressed = false;
      this.mouseRightButtonTime = 0.0f;
      this.mouseRightButtonClicked = false;

      this.mouseMiddleButtonPressed = false;
      this.mouseMiddleButtonTime = 0.0f;
      this.mouseMiddleButtonClicked = false;

      this.MouseLeftButtonSingleClicked = false;
      this.MouseLeftButtonDoubleClicked = false;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    #endregion


    // =====
    #region Update

    // Read, archive, and respond to current user input
    public void Update()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Get totalGameTime
      float totalGameTime = Globals.Clock.TotalGameTime;
      // Get elapsedGameTime
      float elapsedGameTime = (float)Globals.Clock.TargetElapsedTime.TotalSeconds;

      #region Mouse

      // Get mouse state
      this.MouseState = Mouse.GetState();

      #region Left button

      // Check if left button is pressed
      if (MouseState.LeftButton == ButtonState.Pressed)
      {
        #region Case 1:  Pressed

        // If so, check if it was pressed last update
        if (this.MouseLeftButtonPressed)
        {
          // If so, it's being held.  Check how long.
          if (this.MouseLeftButtonTime > this.DoubleClickThreshold)
          {
            // If too long, a double click is no longer feasible
            this.MouseLeftButtonClicked = false;
            this.MouseLeftButtonTime += elapsedGameTime;
          }
          else
          {
            // Otherwise, a double click is still feasible
            this.MouseLeftButtonTime += elapsedGameTime;
          }
        }
        else
        {
          // Otherwise, it's been pressed for the first time.  Check if it's a
          // double click.
          if (this.MouseLeftButtonClicked)
          {
            // If so, respond to a double click
            this.MouseLeftButtonClicked = false;
            this.MouseLeftButtonPressed = true;
            this.MouseLeftButtonTime = 0.0f;
            if (this.respondToMouseLeftButtonDoubleClick != null) { this.respondToMouseLeftButtonDoubleClick(); }
          }
          else
          {
            // Otherwise, respond to a click
            this.MouseLeftButtonClicked = true;
            this.MouseLeftButtonPressed = true;
            this.MouseLeftButtonTime = 0.0f;
            if (this.respondToMouseLeftButtonClick != null) { this.respondToMouseLeftButtonClick(); }
          }
        }

        #endregion
      }
      else
      {
        #region Case 2:  Released

        // Otherwise, check if it was pressed last update
        if (MouseLeftButtonPressed)
        {
          // If so, it's been released for the first time
          this.MouseLeftButtonPressed = false;
          this.MouseLeftButtonTime = 0.0f;
        }
        else
        {
          // If a click occured recently, check how long ago
          if (this.MouseLeftButtonClicked && this.MouseLeftButtonTime > this.DoubleClickThreshold)
          {
            // If too long, a double click is no longer feasible
            this.MouseLeftButtonClicked = false;
            this.MouseLeftButtonTime += elapsedGameTime;
          }
          else
          {
            // Otherwise, a double click is still feasible
            this.MouseLeftButtonTime += elapsedGameTime;
          }
        }

        #endregion
      }

      #endregion

      #region Right button

      // Check if right button is pressed
      if (MouseState.RightButton == ButtonState.Pressed)
      {
        #region Case 1:  Pressed

        // If so, check if it was pressed last update
        if (this.mouseRightButtonPressed)
        {
          // If so, it's being held.  Check how long.
          if (this.mouseRightButtonTime > this.DoubleClickThreshold)
          {
            // If too long, a double click is no longer feasible
            this.mouseRightButtonClicked = false;
            this.mouseRightButtonTime += elapsedGameTime;
          }
          else
          {
            // Otherwise, a double click is still feasible
            this.mouseRightButtonTime += elapsedGameTime;
          }
        }
        else
        {
          // Otherwise, it's been pressed for the first time.  Check if it's a
          // double click.
          if (this.mouseRightButtonClicked)
          {
            // If so, respond to a double click
            this.mouseRightButtonClicked = false;
            this.mouseRightButtonPressed = true;
            this.mouseRightButtonTime = 0.0f;
            if (this.respondToMouseRightButtonDoubleClick != null) { this.respondToMouseRightButtonDoubleClick(); }
          }
          else
          {
            // Otherwise, respond to a click
            this.mouseRightButtonClicked = true;
            this.mouseRightButtonPressed = true;
            this.mouseRightButtonTime = 0.0f;
            if (this.respondToMouseRightButtonDoubleClick != null) { this.respondToMouseRightButtonClick(); }
          }
        }

        #endregion
      }
      else
      {
        #region Case 2:  Released

        // Otherwise, check if it was pressed last update
        if (mouseRightButtonPressed)
        {
          // If so, it's been released for the first time
          this.mouseRightButtonPressed = false;
          this.mouseRightButtonTime = 0.0f;
        }
        else
        {
          // If a click occured recently, check how long ago
          if (this.mouseRightButtonClicked && this.mouseRightButtonTime > this.DoubleClickThreshold)
          {
            // If too long, a double click is no longer feasible
            this.mouseRightButtonClicked = false;
            this.mouseRightButtonTime += elapsedGameTime;
          }
          else
          {
            // Otherwise, a double click is still feasible
            this.mouseRightButtonTime += elapsedGameTime;
          }
        }

        #endregion
      }

      #endregion

      #region Middle button

      // Check if middle button is pressed
      if (MouseState.MiddleButton == ButtonState.Pressed)
      {
        #region Case 1:  Pressed

        // If so, check if it was pressed last update
        if (this.mouseMiddleButtonPressed)
        {
          // If so, it's being held.  Check how long.
          if (this.mouseMiddleButtonTime > this.DoubleClickThreshold)
          {
            // If too long, a double click is no longer feasible
            this.mouseMiddleButtonClicked = false;
            this.mouseMiddleButtonTime += elapsedGameTime;
          }
          else
          {
            // Otherwise, a double click is still feasible
            this.mouseMiddleButtonTime += elapsedGameTime;
          }
        }
        else
        {
          // Otherwise, it's been pressed for the first time.  Check if it's a
          // double click.
          if (this.mouseMiddleButtonClicked)
          {
            // If so, respond to a double click
            this.mouseMiddleButtonClicked = false;
            this.mouseMiddleButtonPressed = true;
            this.mouseMiddleButtonTime = 0.0f;
            if (this.respondToMouseMiddleButtonDoubleClick != null) { this.respondToMouseMiddleButtonDoubleClick(); }
          }
          else
          {
            // Otherwise, respond to a click
            this.mouseMiddleButtonClicked = true;
            this.mouseMiddleButtonPressed = true;
            this.mouseMiddleButtonTime = 0.0f;
            if (this.respondToMouseMiddleButtonClick != null) { this.respondToMouseMiddleButtonClick(); }
          }
        }

        #endregion
      }
      else
      {
        #region Case 2:  Released

        // Otherwise, check if it was pressed last update
        if (mouseMiddleButtonPressed)
        {
          // If so, it's been released for the first time
          this.mouseMiddleButtonPressed = false;
          this.mouseMiddleButtonTime = 0.0f;
        }
        else
        {
          // If a click occured recently, check how long ago
          if (this.mouseMiddleButtonClicked && this.mouseMiddleButtonTime > this.DoubleClickThreshold)
          {
            // If too long, a double click is no longer feasible
            this.mouseMiddleButtonClicked = false;
            this.mouseMiddleButtonTime += elapsedGameTime;
          }
          else
          {
            // Otherwise, a double click is still feasible
            this.mouseMiddleButtonTime += elapsedGameTime;
          }
        }

        #endregion
      }

      #endregion

      #endregion

      #region Delete old inputs

      // Initialize countOld, the number of records (starting from zero) that
      // are too old and should be removed.
      int countOld = 0;

      // Loop through times
      for (int i = 0; i < this.Times.Count; i++)
      {
        // Point to current time
        float time = this.Times[i];
        // Calculate age
        float age = totalGameTime - time;
        // Check if age is too old
        if (age > this.MemorySpan)
        {
          // If so, increment count
          countOld++;
        }
        else
        {
          // If not, surely no larger index will be older (since new indeces
          // are added to the END of the list).  We may stop here.
          break;
        }
      }

      // Remove expired inputs
      this.Times.RemoveRange(0, countOld);
      this.Devices.RemoveRange(0, countOld);
      this.Signals.RemoveRange(0, countOld);

      #endregion

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
