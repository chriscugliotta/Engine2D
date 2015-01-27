using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A wrapped render target
  public class WrappedRenderTarget : WrappedTexture
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // An XNA render target object
    private RenderTarget2D renderTarget;
    // A size index used by the render target manager
    private int targetIndex;

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
        return String.Format("WRTg{0:0000}", this.ID);
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
        return this.renderTarget;
      }
    }
    // renderTarget accessor
    public virtual RenderTarget2D RenderTarget
    {
      get
      {
        return this.renderTarget;
      }
    }
    // index accessor
    public virtual int TargetIndex
    {
      get
      {
        return this.targetIndex;
      }
      set
      {
        this.targetIndex = value;
      }
    }

    // Width in pixels
    public override int Width
    {
      get
      {
        return this.RenderTarget.Width;
      }
    }
    // Height in pixels
    public override int Height
    {
      get
      {
        return this.RenderTarget.Height;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public WrappedRenderTarget() { }

    // 3-argument constructor
    public WrappedRenderTarget(int w, int h, int index)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.renderTarget = new RenderTarget2D(Globals.Game1.GraphicsDevice, w, h, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
      this.targetIndex = index;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Save a screenshot
    public virtual void Save(String path)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Create);
      this.RenderTarget.SaveAsPng(stream, this.Width, this.Height);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
