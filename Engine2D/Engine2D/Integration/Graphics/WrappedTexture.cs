using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A wrapped texture
  public class WrappedTexture : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // The texture's file path
    private String path;
    // An XNA texture object
    private Texture2D texture;

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
        return String.Format("WTex{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Pt:  {0}}}", this.Path);
    }

    // path accessor
    public virtual String Path
    {
      get
      {
        return this.path;
      }
    }
    // texture accessor
    public virtual Texture2D Texture
    {
      get
      {
        return this.texture;
      }
    }

    // Width in pixels
    public virtual int Width
    {
      get
      {
        return this.Texture.Width;
      }
    }
    // Height in pixels
    public virtual int Height
    {
      get
      {
        return this.Texture.Height;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public WrappedTexture() { }

    // 2-argument constructor
    public WrappedTexture(Texture2D texture, String path)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.path = path;
      this.texture = texture;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
