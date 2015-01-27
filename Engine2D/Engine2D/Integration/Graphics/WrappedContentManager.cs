using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A mechanism for managing content
  public class WrappedContentManager : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // An XNA content manager object
    private ContentManager content;

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
        return String.Format("WCMn{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return this.Name;
    }

    // content accessor
    public ContentManager Content
    {
      get
      {
        return this.content;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public WrappedContentManager()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.content = new ContentManager(Globals.Game1.Services);
      this.content.RootDirectory = "Content";

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Load a font
    public WrappedFont LoadFont(String path)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Load
      SpriteFont font = this.content.Load<SpriteFont>(path);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return new WrappedFont(font, path);
    }

    // Load a texture
    public WrappedTexture LoadTexture(String path)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Load
      Texture2D texture = this.content.Load<Texture2D>(path);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return new WrappedTexture(texture, path);
    }



    #endregion
  }
}
