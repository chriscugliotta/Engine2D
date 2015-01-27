using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A wrapped HorizontalBlur.fx file
  public class WrappedHorizontalBlurShader : WrappedShader
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // An XNA effect object
    private Effect effect = Globals.Game1.Content.Load<Effect>("Shaders/HorizontalBlur");

    // The active view value
    private static Matrix activeView = 0 * Matrix.Identity;
    // The active projection value
    private static Matrix activeProjection = 0 * Matrix.Identity;
    // The active texture value
    private static WrappedTexture activeTexture = null;
    // The active width value
    private static int activeWidth = 0;
    // The active sample size value
    private static int activeSampleSize = 0;
    // The active offsets value
    private static float[] activeOffsets = new float[0];
    // The active weights value
    private static float[] activeWeights = new float[0];

    // The pixel sample size
    private int sampleSize;
    // An array of offsets
    private float[] offsets;
    // An array of weights
    private float[] weights;

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
        return String.Format("ShHB{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Sz:  {0}}}", this.SampleSize);
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
        return WrappedHorizontalBlurShader.activeView;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveView) { return; }
        // Set effect value
        this.Effect.Parameters["view"].SetValue(value);
        // Set local value
        WrappedHorizontalBlurShader.activeView = value;
      }
    }
    // activeProjection accessor
    public Matrix ActiveProjection
    {
      get
      {
        return WrappedHorizontalBlurShader.activeProjection;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveProjection) { return; }
        // Set effect value
        this.Effect.Parameters["projection"].SetValue(value);
        // Set local value
        WrappedHorizontalBlurShader.activeProjection = value;
      }
    }
    // activeTexture accessor
    public WrappedTexture ActiveTexture
    {
      get
      {
        return WrappedHorizontalBlurShader.activeTexture;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveTexture) { return; }
        // Set effect value
        this.Effect.Parameters["tex"].SetValue(value.Texture);
        // Set local value
        WrappedHorizontalBlurShader.activeTexture = value;
      }
    }
    // activeWidth accessor
    public int ActiveWidth
    {
      get
      {
        return WrappedHorizontalBlurShader.activeWidth;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveWidth) { return; }
        // Set effect value
        this.Effect.Parameters["width"].SetValue(value);
        // Set local value
        WrappedHorizontalBlurShader.activeWidth = value;
      }
    }
    // activeSampleSize accessor
    public int ActiveSampleSize
    {
      get
      {
        return WrappedHorizontalBlurShader.activeSampleSize;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveSampleSize) { return; }
        // Set effect value
        this.Effect.Parameters["sampleSize"].SetValue(value);
        // Set local value
        WrappedHorizontalBlurShader.activeSampleSize = value;
      }
    }
    // activeOffsets accessor
    public float[] ActiveOffsets
    {
      get
      {
        return WrappedHorizontalBlurShader.activeOffsets;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveOffsets) { return; }
        // Set effect value
        this.Effect.Parameters["offsets"].SetValue(value);
        // Set local value
        WrappedHorizontalBlurShader.activeOffsets = value;
      }
    }
    // activeWeights accessor
    public float[] ActiveWeights
    {
      get
      {
        return WrappedHorizontalBlurShader.activeWeights;
      }
      set
      {
        // Skip if already active
        if (value == this.ActiveWeights) { return; }
        // Set effect value
        this.Effect.Parameters["weights"].SetValue(value);
        // Set local value
        WrappedHorizontalBlurShader.activeWeights = value;
      }
    }

    // sampleSize accessor
    public int SampleSize
    {
      get
      {
        return this.sampleSize;
      }
      set
      {
        #region Error check:  sampleSize must be a positive, odd integer
        #if IS_ERROR_CHECKING

        if (value < 0 || value % 2 != 1)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "sampleSize must be a positive, odd integer!\n";
          s += String.Format("sampleSize = {0}", value);

          // Throw exception
          throw new ArgumentException(s);
        }

        #endif
        #endregion

        this.sampleSize = value;
      }
    }
    // offsets accessor
    public float[] Offsets
    {
      get
      {
        return this.offsets;
      }
      set
      {
        this.offsets = value;
      }
    }
    // weights accessor
    public float[] Weights
    {
      get
      {
        return this.weights;
      }
      set
      {
        this.weights = value;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Gaussian constructor
    public static WrappedHorizontalBlurShader Gaussian(int sampleSize, float sigma)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize arrays
      float[] offsets = new float[sampleSize];
      float[] weights = new float[sampleSize];
      float m = (float)Math.Floor(sampleSize / 2.0);
      for (int i = 0; i < sampleSize; i++) { offsets[i] = i - m; }
      for (int i = 0; i < sampleSize; i++) { weights[i] = Globals.MathHelper.NormalDensity(offsets[i], 0.0f, sigma); }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Return result
      return new WrappedHorizontalBlurShader(sampleSize, offsets, weights);
    }

    // Designated constructor
    public WrappedHorizontalBlurShader(int sampleSize, float[] offsets, float[] weights)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.SampleSize = sampleSize;
      this.Offsets = offsets;
      this.Weights = weights;

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
      this.ActiveWidth = Globals.GraphicsDevice.ActiveTexture.Width;
      this.ActiveSampleSize = this.SampleSize;
      this.ActiveOffsets = this.Offsets;
      this.ActiveWeights = this.Weights;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
