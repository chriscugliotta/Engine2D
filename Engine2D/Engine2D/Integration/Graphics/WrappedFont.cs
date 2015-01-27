using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A wrapped font
  public class WrappedFont : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // The texture's file path
    private String path;
    // An XNA font object
    private SpriteFont font;

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
        return String.Format("WFnt{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Pt:  {0}}}", this.Path);
    }

    // path accessor
    public String Path
    {
      get
      {
        return this.path;
      }
    }
    // font accessor
    public SpriteFont Font
    {
      get
      {
        return this.font;
      }
    }

    #endregion


    // =====
    #region Constructors

    // 2-argument constructor
    public WrappedFont(SpriteFont font, String path)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.path = path;
      this.font = font;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
