using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Engine2D
{
  public struct WrappedColor
  {
    // =====
    #region Variables

    // An XNA color object
    private Color color;

    #endregion


    // =====
    #region Properties

    // Red
    public byte R
    {
      get
      {
        return this.color.R;
      }
      set
      {
        this.color.R = value;
      }
    }
    // Green
    public byte G
    {
      get
      {
        return this.color.G;
      }
      set
      {
        this.color.G = value;
      }
    }
    // Blue
    public byte B
    {
      get
      {
        return this.color.B;
      }
      set
      {
        this.color.B = value;
      }
    }
    // Alpha
    public byte A
    {
      get
      {
        return this.color.A;
      }
      set
      {
        this.color.B = value;
      }
    }
    
    #endregion


    // =====
    #region Constructors

    // 3-argument constructor
    public WrappedColor(byte r, byte g, byte b) : this(r, g, b, 1) { }

    // Designated constructor
    public WrappedColor(byte r, byte g, byte b, byte a)
    {
      this.color = new Color(r, g, b, a);
    }

    #endregion
  }
}
