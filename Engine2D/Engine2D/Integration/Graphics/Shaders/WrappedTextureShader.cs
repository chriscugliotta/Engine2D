using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A wrapped Texture.fx file
  public class WrappedTextureShader : WrappedShader
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // An XNA effect object
    private Effect effect = Globals.Game1.Content.Load<Effect>("Shaders/Texture");

    // The active view value
    private static Matrix activeView = 0 * Matrix.Identity;
    // The active projection value
    private static Matrix activeProjection = 0 * Matrix.Identity;
    // The active texture value
    private static WrappedTexture activeTexture = null;
    // The active grayscale value
    private static bool activeGrayscale = false;
    // The active threshold value
    private static float activeThreshold = 0.0f;
    // The active brightness value
    private static float activeBrightness = 1.0f;
    // The active saturation value
    private static float activeSaturation = 1.0f;
    // The active contrast value
    private static float activeContrast = 1.0f;
    // The active alpha value
    private static float activeAlpha = 1.0f;

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
      return String.Format("{{Gr:  {0}, Th:  {1}, Br:  {2}, St:  {3}, Ct:  {4}, Al:  {5}}}", this.Grayscale, this.Threshold, this.Brightness, this.Saturation, this.Contrast, this.Alpha);
    }

    // effect accessor
    public override Effect Effect
    {
      get
      {
        return this.effect;
      }
    }

    // activeView accessor
    public Matrix ActiveView
    {
      get
      {
        return WrappedTextureShader.activeView;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveView) { return; }
        // Set effect value
        this.Effect.Parameters["view"].SetValue(value);
        // Set local value
        WrappedTextureShader.activeView = value;
      }
    }
    // activeProjection accessor
    public Matrix ActiveProjection
    {
      get
      {
        return WrappedTextureShader.activeProjection;
      }
      set
      {

        // Skip if already active
        if (value == this.ActiveProjection) { return; }
        // Set effect value
        this.Effect.Parameters["projection"].SetValue(value);
        // Set local value
        WrappedTextureShader.activeProjection = value;
      }
    }
    // activeTexture accessor
    public WrappedTexture ActiveTexture
    {
      get
      {
        return WrappedTextureShader.activeTexture;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveTexture) { return; }
        // Set effect value
        this.Effect.Parameters["tex"].SetValue(value.Texture);
        // Set local value
        WrappedTextureShader.activeTexture = value;
      }
    }
    // activeGrayscale accessor
    public bool ActiveGrayscale
    {
      get
      {
        return WrappedTextureShader.activeGrayscale;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveGrayscale) { return; }
        // Set effect value
        this.Effect.Parameters["grayscale"].SetValue(value);
        // Set local value
        WrappedTextureShader.activeGrayscale = value;
      }
    }
    // activeThreshold accessor
    public float ActiveThreshold
    {
      get
      {
        return WrappedTextureShader.activeThreshold;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveThreshold) { return; }
        // Set effect value
        this.Effect.Parameters["threshold"].SetValue(value);
        // Set local value
        WrappedTextureShader.activeThreshold = value;
      }
    }
    // activeBrightness accessor
    public float ActiveBrightness
    {
      get
      {
        return WrappedTextureShader.activeBrightness;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveBrightness) { return; }
        // Set effect value
        this.Effect.Parameters["brightness"].SetValue(value);
        // Set local value
        WrappedTextureShader.activeBrightness = value;
      }
    }
    // activeSaturation accessor
    public float ActiveSaturation
    {
      get
      {
        return WrappedTextureShader.activeSaturation;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveSaturation) { return; }
        // Set effect value
        this.Effect.Parameters["saturation"].SetValue(value);
        // Set local value
        WrappedTextureShader.activeSaturation = value;
      }
    }
    // activeContrast accessor
    public float ActiveContrast
    {
      get
      {
        return WrappedTextureShader.activeContrast;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveContrast) { return; }
        // Set effect value
        this.Effect.Parameters["contrast"].SetValue(value);
        // Set local value
        WrappedTextureShader.activeContrast = value;
      }
    }
    // activeAlpha accessor
    public float ActiveAlpha
    {
      get
      {
        return WrappedTextureShader.activeAlpha;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveAlpha) { return; }
        // Set effect value
        this.Effect.Parameters["alpha"].SetValue(value);
        // Set local value
        WrappedTextureShader.activeAlpha = value;
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
    public WrappedTextureShader() : this(false, 0, 1, 1, 1, 1) { }

    // Designated constructor
    public WrappedTextureShader(bool grayscale, float threshold, float brightness, float saturation, float contrast, float alpha)
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
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Pass parameters to graphics card
    public override void PassParameters()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set parameters
      this.ActiveView = Globals.GraphicsDevice.ActiveView;
      this.ActiveProjection = Globals.GraphicsDevice.ActiveProjection;
      this.ActiveTexture = Globals.GraphicsDevice.ActiveTexture;
      this.ActiveGrayscale = this.Grayscale;
      this.ActiveThreshold = this.Threshold;
      this.ActiveBrightness = this.Brightness;
      this.ActiveSaturation = this.Saturation;
      this.ActiveContrast = this.Contrast;
      this.ActiveAlpha = this.Alpha;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
