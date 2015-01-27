using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine2D
{
  // An instance of a texture shader
  public class TextureShader : Shader
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // The .fx file
    private static WrappedEffectFile effectFile = new WrappedEffectFile("Shaders/Texture");

    // Equals true if grayscale
    private bool grayscale;
    // The luminance threshold
    private float threshold;
    // A brightness value
    private float brightness;
    // A saturation value
    private float saturation;
    // A contrast value
    private float contrast;
    // An alpha value
    private float alpha;

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
        return String.Format("ShTx{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Al:  {0}}}", this.Alpha);
    }

    // effectFile accessor
    public override WrappedEffectFile EffectFile
    {
      get
      {
        return TextureShader.effectFile;
      }
    }

    // grayscale accessor
    public bool Grayscale
    {
      get
      {
        return this.grayscale;
      }
      set
      {
        this.grayscale = value;
      }
    }
    // threshold accessor
    public float Threshold
    {
      get
      {
        return this.threshold;
      }
      set
      {
        #region Error check:  threshold must be between zero and one
        #if IS_ERROR_CHECKING

        if (value < 0 || value > 1)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "threshold must be between zero and one!\n";
          s += String.Format("threshold = {0}", value);

          // Throw exception
          throw new ArgumentException(s);
        }

        #endif
        #endregion

        this.threshold = value;
      }
    }
    // brightness accessor
    public float Brightness
    {
      get
      {
        return this.brightness;
      }
      set
      {
        #region Error check:  brightness must be non-negative
        #if IS_ERROR_CHECKING

        if (value < 0)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "brightness must be non-negative!\n";
          s += String.Format("brightness = {0}", value);

          // Throw exception
          throw new ArgumentException(s);
        }

        #endif
        #endregion

        this.brightness = value;
      }
    }
    // saturation accessor
    public float Saturation
    {
      get
      {
        return this.saturation;
      }
      set
      {
        #region Error check:  saturation must be non-negative
        #if IS_ERROR_CHECKING

        if (value < 0)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "saturation must be non-negative!\n";
          s += String.Format("saturation = {0}", value);

          // Throw exception
          throw new ArgumentException(s);
        }

        #endif
        #endregion

        this.saturation = value;
      }
    }
    // contrast accessor
    public float Contrast
    {
      get
      {
        return this.contrast;
      }
      set
      {
        #region Error check:  contrast must be non-negative
        #if IS_ERROR_CHECKING

        if (value < 0)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "contrast must be non-negative!\n";
          s += String.Format("contrast = {0}", value);

          // Throw exception
          throw new ArgumentException(s);
        }

        #endif
        #endregion

        this.contrast = value;
      }
    }
    // alpha accessor
    public float Alpha
    {
      get
      {
        return this.alpha;
      }
      set
      {
        #region Error check:  alpha must be between zero and one
        #if IS_ERROR_CHECKING

        if (value < 0 || value > 1)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "alpha must be between zero and one!\n";
          s += String.Format("alpha = {0}", value);

          // Throw exception
          throw new ArgumentException(s);
        }

        #endif
        #endregion

        this.alpha = value;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public TextureShader() : this(false, 0, 1, 1, 1, 1) { }

    // Designated constructor
    public TextureShader(bool grayscale, float threshold, float brightness, float saturation, float contrast, float alpha)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.Grayscale = grayscale;
      this.Threshold = threshold;
      this.Brightness = brightness;
      this.Saturation = saturation;
      this.Contrast = contrast;
      this.Alpha = alpha;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Get .fx file
    public static WrappedEffectFile GetEffectFile()
    {
      return TextureShader.effectFile;
    }

    // Pass parameters into .fx file
    public override void PassParameters()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set parameters
      this.EffectFile.SetParameter("view", Globals.GraphicsDevice.ActiveView);
      this.EffectFile.SetParameter("projection", Globals.GraphicsDevice.ActiveProjection);
      this.EffectFile.SetParameter("tex", Globals.GraphicsDevice.ActiveTexture);
      this.EffectFile.SetParameter("grayscale", this.Grayscale);
      this.EffectFile.SetParameter("threshold", this.Threshold);
      this.EffectFile.SetParameter("brightness", this.Brightness);
      this.EffectFile.SetParameter("saturation", this.Saturation);
      this.EffectFile.SetParameter("contrast", this.Contrast);
      this.EffectFile.SetParameter("alpha", this.Alpha);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
