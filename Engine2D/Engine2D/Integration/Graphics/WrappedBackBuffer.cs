using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A wrapped back buffer render target
  public class WrappedBackBuffer : WrappedRenderTarget
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // The current frame's back buffer width
    private int width;
    // The current frame's back buffer height
    private int height;
    // The previous frame's back buffer width
    private int previousWidth;
    // The previous frame's back buffer height
    private int previousHeight;
    // Equals true if the back buffer has been resized
    private bool isResized;

    // A back-buffer-sized quadrilateral vertex buffer
    private WrappedVertexBuffer quadVertexBuffer;
    // A back-buffer-sized quadrilateral index buffer
    private WrappedIndexBuffer quadIndexBuffer;

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
        return String.Format("WBBT{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{ID:  {0}, Ix:  {1}, W:  {2}, H:  {3}}}", this.ID, this.TargetIndex, this.Width, this.Height);
    }

    // path accessor
    public override String Path
    {
      get
      {
        return null;
      }
    }
    // texture accessor
    public override Texture2D Texture
    {
      get
      {
        return null;
      }
    }
    // renderTarget accessor
    public override RenderTarget2D RenderTarget
    {
      get
      {
        return null;
      }
    }
    // index accessor
    public override int TargetIndex
    {
      get
      {
        return -1;
      }
      set
      {
        return;
      }
    }

    // currentWidth accessor
    public override int Width
    {
      get
      {
        return this.width;
      }
    }
    // currentHeight accessor
    public override int Height
    {
      get
      {
        return this.height;
      }
    }
    // previousWidth accessor
    public virtual int PreviousWidth
    {
      get
      {
        return this.previousWidth;
      }
    }
    // previousHeight accessor
    public virtual int PreviousHeight
    {
      get
      {
        return this.previousHeight;
      }
    }
    // isResized accessor
    public virtual bool IsResized
    {
      get
      {
        return this.isResized;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public WrappedBackBuffer()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.width = 0;
      this.height = 0;
      this.previousWidth = -1;
      this.previousHeight = -1;
      this.isResized = false;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Save a screenshot
    public override void Save(String path)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // We can't save a screenshot of the back buffer!

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Check size
    public virtual void CheckSize()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Update the previous frame's back buffer size
      this.previousWidth = this.Width;
      this.previousHeight = this.Height;

      // Update the current frame's back buffer size
      this.width = Globals.Game1.GraphicsDevice.PresentationParameters.BackBufferWidth;
      this.height = Globals.Game1.GraphicsDevice.PresentationParameters.BackBufferHeight;

      // Check if the back buffer has changed
      if (this.Width != this.PreviousWidth || this.Height != this.PreviousHeight) { this.isResized = true; }
      else { this.isResized = false; }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
