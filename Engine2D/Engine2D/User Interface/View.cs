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
  // A graphic user interface object
  public class View
  {
    // =====
    #region Variables

    // A unique instance name
    public String Name;
    // The view's origin and size in its parent's coordinate system
    public Box Frame;
    // The view's origin and size in its own coordinate system
    public Box Bounds;
    // Equals true if this view has a child
    public bool HasChild;
    // List of child views
    public List<View> Children;
    // Equals true if this view is a child
    public bool IsChild;
    // If this view is a child, it's parent
    public View Parent;

    // Background color
    public Color Color;
    // Text content
    public String Text;

    // Position and size of bounds on screen, after being properly nested in
    // each parent coordinate system.  The contents of this rectangle are not
    // necessarily visible.
    protected Box screenBounds;
    // Clipping rectangle, expressed in screen coordinates, used for on-screeen
    // drawing
    protected Box screenFrame;
    // Equals true if this view's screen rectangles need to be updated
    protected bool needsToUpdateScreenRectangle;


    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{Fr:  {0}, Bn:  {1}, Cn:  {2}, iC:  {3}}}", this.Frame, this.Bounds, this.Children.Count, this.IsChild);
    }

    // screenFrame accessor
    public Box ScreenFrame
    {
      get
      {
        return this.screenFrame;
      }
    }
    // screenBounds accessor
    public Box ScreenBounds
    {
      get
      {
        return this.screenFrame;
      }
    }
    // needsToUpdateScreenRectangles accessor
    public bool NeedsToUpdateScreenRectangles
    {
      get
      {
        return this.needsToUpdateScreenRectangle;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor, i.e. 'do nothing'
    public View() { }

    // Designated constructor
    public View(Box frame, Box bounds)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      this.Name = Globals.NameHelper.GetName("View");
      this.Frame = frame;
      this.Bounds = bounds;
      this.HasChild = false;
      this.Children = new List<View>();
      this.IsChild = false;

      this.Text = null;
      this.Color = new Color(0, 0, 0, 0);

      this.needsToUpdateScreenRectangle = false;
      this.screenBounds = new Box(frame.X + bounds.X, frame.Y + bounds.Y, bounds.Width, bounds.Height);
      this.screenFrame = frame;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add child
    public void AddChild(View child)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, child.Name));
      #endif

      // Check if view is already someone else's child
      if (child.IsChild)
      {
        // If so, remove this child from its former parent
        child.Parent.Children.Remove(child);
      }

      // Mark this view as a child
      child.IsChild = true;
      // Mark this view as having a parent
      child.Parent = this;
      // Mark this parent as having a child
      this.HasChild = true;
      // Call for an update
      child.chainWakeUp();
      // Add to list
      this.Children.Add(child);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, child.Name));
      #endif
    }

    // Remove child
    public void RemoveChild(View child)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, child.Name));
      #endif

      // Check if we're truly the parent
      bool isParent = false;
      for (int i = 0; i < this.Children.Count; i++)
      {
        if (this.Children[i] == child) { isParent = true; }
      }

      // If not, this is none of our business.  Stop here.
      if (!isParent) { return; }

      // If so, remove child from list
      this.Children.Remove(child);
      // This child is no longer a 'child'
      child.IsChild = false;
      // This child no longer has a parent
      child.Parent = null;
      
      // Check our remaining child count
      if (this.Children.Count == 0)
      {
        // If zero, we're no longer a parent
        this.HasChild = false;
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, child.Name));
      #endif
    }

    // Call for this view and all of its children to be updated
    protected void chainWakeUp()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Wake up children first
      for (int i = 0; i < this.Children.Count; i++)
      {
        this.Children[i].chainWakeUp();
      }

      // After that, wake up self
      this.needsToUpdateScreenRectangle = true;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Update

    // Update
    public virtual void Update()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

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

    // Update screen rectangles
    public void UpdateScreenRectangles()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Reset screen rectangles
      this.screenBounds = new Box(this.Frame.X + this.Bounds.X, this.Frame.Y + this.Bounds.Y, this.Bounds.Width, this.Bounds.Height);
      this.screenFrame = this.Frame;

      //Trace.WriteLine("=====");
      //Trace.WriteLine("this.Name = " + this.Name);
      //Trace.WriteLine("this.Frame = " + this.Frame);
      //Trace.WriteLine("this.Bounds = " + this.Bounds);
      //Trace.WriteLine("this.screenFrame = " + this.screenFrame);
      //Trace.WriteLine("this.screenBounds = " + this.screenBounds);

      // Transform into parent coordinate system
      if (this.IsChild)
      {
        // Get parent
        View parent = this.Parent;
        //Trace.WriteLine("Checking parent " + parent.Name);
        //Trace.WriteLine("parent.ScreenBounds = " + parent.ScreenBounds);
        //Trace.WriteLine("parent.ScreenFrame = " + parent.ScreenFrame);

        // Translate bounds into parent coordinate system
        this.screenBounds.TranslateBy(new Vector2(parent.ScreenFrame.X + parent.Bounds.X, parent.ScreenFrame.Y + parent.Bounds.Y));
        //Trace.WriteLine("Translated bounds = " + this.screenBounds);

        // Translate frame into parent coordinate system
        this.screenFrame.TranslateBy(new Vector2(parent.ScreenFrame.X + parent.Bounds.X, parent.ScreenFrame.Y + parent.Bounds.Y));
        //Trace.WriteLine("Translated frame = " + this.screenBounds);

        // Frame must be inside parent frame
        this.screenFrame = this.screenFrame.IntersectionWith(parent.screenFrame);
        //Trace.WriteLine("Frame inside parent frame = " + this.screenFrame);

        // Frame must be inside parent bounds
        this.screenFrame = this.screenFrame.IntersectionWith(parent.screenBounds);
        //Trace.WriteLine("Frame inside parent bounds = " + this.screenFrame);
      }

      // Go back to sleep
      this.needsToUpdateScreenRectangle = false;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Draw

    // Draw
    public virtual void Draw()
    {
      // Entry logging
      #if IS_LOGGING_DRAW
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // First, draw children
      for (int i = 0; i < this.Children.Count; i++)
      {
        this.Children[i].Draw();
      }
      
      // Skip root
      if (this.Name == "View0001") { return; }

      // Create a rasterizer state
      RasterizerState rasterizerState = new RasterizerState()
      {
        // Enable clipped drawing
        ScissorTestEnable = true
      };

      // Get scissor rectangle
      Box rect = this.screenBounds.IntersectionWith(this.screenFrame);
      // Set scissor rectangle
      // Globals.Game1.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

      // Begin clipped drawing
      Globals.DrawHelper.SpriteBatch.Begin(
        SpriteSortMode.Deferred,
        BlendState.AlphaBlend,
        null,
        null,
        rasterizerState);

      #if IS_DRAWING_PRIMITIVES

      // Proceed according to area
      if (this.screenFrame.Width == 0 || this.screenFrame.Height == 0)
      {
        // Case 1:  Zero area
        Globals.DrawHelper.DrawPoint(this.screenBounds.TopLeft, Color.Black, false);
      }
      else
      {
        // Case 2:  Non-zero area
        Globals.DrawHelper.FillRectangle(rect, this.Color);
        rect.Width -= 1;
        rect.Height -= 1;
        Globals.DrawHelper.DrawRectangle(rect, Color.Black * 0.2f, false);
      }
      
      #endif

      // If exists, draw text
      if (!String.IsNullOrEmpty(this.Text))
      {
        Globals.DrawHelper.DrawString(Globals.FontManager.DefaultFont.Font, this.Text, this.screenBounds.TopLeft, 0, Color.Black);
      }

      // End clipped drawing
      Globals.DrawHelper.SpriteBatch.End();

      // Exit logging
      #if IS_LOGGING_DRAW
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
