using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine2D
{
  // An instance of a horizontal blur shader
  public class HorizontalBlurShader : Shader
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // The .fx file
    private static WrappedEffectFile effectFile = new WrappedEffectFile("Shaders/HorizontalBlur");

    // Sample size
    private int sampleSize;
    // An array of offsets
    private float[] offsets;
    // An array of weights
    private float[] weights;
    // Downsampling factor
    private int downsampling;

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
      return String.Format("{{Sz:  {0}, DS:  {1}}}", this.SampleSize, this.Downsampling);
    }

    // shader accessor
    public override WrappedEffectFile EffectFile
    {
      get
      {
        return HorizontalBlurShader.effectFile;
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
    // downsampling accessor
    public int Downsampling
    {
      get
      {
        return this.downsampling;
      }
      set
      {
        #region Error check:  downsampling must be a non-negative integer
        #if IS_ERROR_CHECKING

        if (value < 0)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "downsampling must be a non-negative integer!\n";
          s += String.Format("downsampling = {0}", value);

          // Throw exception
          throw new ArgumentException(s);
        }

        #endif
        #endregion

        this.downsampling = value;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public HorizontalBlurShader(int sampleSize, int downsampling)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.SampleSize = sampleSize;
      this.Offsets = new float[sampleSize];
      this.Weights = new float[sampleSize];
      this.Downsampling = downsampling;

      // Initialize offsets
      for (int i = 0; i < sampleSize; i++) { this.offsets[i] = i - (float)Math.Floor(sampleSize / 2.0); }
      // Initialize weights
      for (int i = 0; i < sampleSize; i++) { this.weights[i] = Globals.MathHelper.NormalDensity(offsets[i], 0.0f, 2.0f); }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Get .fx file
    public static WrappedEffectFile GetEffectFile()
    {
      return HorizontalBlurShader.effectFile;
    }

    // Copy parameters into .fx file
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
      this.EffectFile.SetParameter("width", Globals.GraphicsDevice.ActiveRenderTarget.Width);
      this.EffectFile.SetParameter("sampleSize", this.SampleSize);
      this.EffectFile.SetParameter("offsets", this.Offsets);
      this.EffectFile.SetParameter("weights", this.Weights);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
