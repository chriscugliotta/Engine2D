using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A mechanism for managing multiple fonts
  public class FontManager : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // A list of all fonts
    private List<WrappedFont> fonts;

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
        return String.Format("FnMn{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Cn:  {0}}}", this.Fonts.Count);
    }

    // fonts accessor
    public List<WrappedFont> Fonts
    {
      get
      {
        return this.fonts;
      }
    }

    // The default font
    public WrappedFont DefaultFont
    {
      get
      {
        return this.Fonts[0];
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public FontManager()
    {
      // Get unique instance ID
      FontManager.nextID++;
      this.id = FontManager.nextID;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.fonts = new List<WrappedFont>();
      // Add the default font
      this.fonts.Add(Globals.Content.LoadFont("Fonts/SegoeUIMono"));
      
      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add sprite layer
    public void AddFont(WrappedFont font)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, font.Name));
      #endif

      // Add to list
      this.Fonts.Add(font);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, font.Name));
      #endif
    }

    #endregion
  }
}
