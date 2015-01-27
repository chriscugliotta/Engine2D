using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Engine2D
{
  // A tool for testing the speed of code
  public class SpeedTest
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // A jagged array of treatments and responses
    private float[][] data;
    // An array of response means
    private float[] means;
    // An array of response standard deviations
    private float[] deviations;
    // The total number of treatments
    private int treatments;
    // The step size between consecutive treatments
    private int stepSize;
    // The number of observations per treatment
    private int sampleSize;
    // A stopwatch
    public Stopwatch stopwatch;
    // The block of code to be tested
    public CodeBlock block;

    #endregion


    // =====
    #region Delegates

    // A block of code
    public delegate void CodeBlock();

    #endregion


    // =====
    #region Properties

    // nextID accessor
    public static int NextID
    {
      get
      {
        return SpeedTest.nextID;
      }
    }
    // id accessor
    public int ID
    {
      get
      {
        return this.id;
      }
    }
    // Name
    public String Name
    {
      get
      {
        return String.Format("SpTs{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Tr:  {0}, Sp:  {1}, Sz:  {2}}}", this.Treatments, this.StepSize, this.SampleSize);
    }

    // data accessor
    public float[][] Data
    {
      get
      {
        return this.data;
      }
    }
    // means accessor
    public float[] Means
    {
      get
      {
        return this.means;
      }
    }
    // deviations accessor
    public float[] Deviations
    {
      get
      {
        return this.deviations;
      }
    }
    // treatments accessor
    public int Treatments
    {
      get
      {
        return this.treatments;
      }
    }
    // stepSize accessor
    public int StepSize
    {
      get
      {
        return this.stepSize;
      }
    }
    // sampleSize accessor
    public int SampleSize
    {
      get
      {
        return this.sampleSize;
      }
    }
    // stopWatch accessor
    public Stopwatch Stopwatch
    {
      get
      {
        return this.stopwatch;
      }
    }

    #endregion


    // =====
    #region Constructors

    // 3-argument constructor
    public SpeedTest(int treatments, int stepSize, int sampleSize) : this(treatments, stepSize, sampleSize, null) { }

    // Designated constructor
    public SpeedTest(int treatments, int stepSize, int sampleSize, CodeBlock codeBlock)
    {
      // Get unique instance ID
      SpeedTest.nextID++;
      this.id = SpeedTest.nextID;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.data = new float[treatments][];
      for (int i = 0; i < treatments; i++) { this.data[i] = new float[sampleSize]; } 
      this.means = new float[treatments];
      this.deviations = new float[treatments];
      this.treatments = treatments;
      this.stepSize = stepSize;
      this.sampleSize = sampleSize;
      this.stopwatch = new Stopwatch();
      this.block = codeBlock;
      if (codeBlock == null) { this.block = new CodeBlock(this.DefaultBlock); }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // The default code block, if none specified
    public void DefaultBlock()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Trace.WriteLine("hi");
      // float x = 1;
      // Globals.TestEffect.SetParameter("threshold", 1.0f);
      // if (Globals.TestString == "0123456789") { float x = 1; }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Run test
    public String GetResult()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Loop through treatments
      for (int i = 0; i < this.Treatments; i++)
      {
        // Get current iteration count
        int iterations = (i + 1) * this.StepSize;

        // Loop through observations
        for (int j = 0; j < this.SampleSize; j++)
        {
          // Start timer
          this.Stopwatch.Restart();
          // Perform iterations
          for (int k = 0; k < iterations; k++) { this.block(); }
          // Stop timer
          this.Stopwatch.Stop();
          // Store results
          float t = this.Stopwatch.ElapsedMilliseconds;
          this.Data[i][j] = t;
          this.Means[i] += t;
        }
      }

      // Calculate means
      for (int i = 0; i < this.Treatments; i++) { this.Means[i] /= this.SampleSize; }

      // If possible, calculate standard deviations
      if (this.SampleSize > 1)
      {
        // Loop through treatments
        for (int i = 0; i < this.Treatments; i++)
        {
          // Get current mean
          float mu = this.Means[i];

          // Loop through observations
          for (int j = 0; j < this.SampleSize; j++)
          {
            // Get current observation
            float y = this.Data[i][j];

            // Add square error to sum
            this.Deviations[i] += (y - mu) * (y - mu);
          }

          // Divide by (n - 1)
          this.Deviations[i] /= this.SampleSize - 1;
          this.Deviations[i] = (float)Math.Sqrt(this.Deviations[i]);
        }
      }

      // Create a header row
      String s = "\nIterations\tMean\tDeviation";

      // Loop through treatments
      for (int i = 0; i < this.Treatments; i++)
      {
        // Add row
        s += String.Format("\n{0}\t{1}\t{2}", (i + 1) * this.StepSize, this.Means[i], this.Deviations[i]);

        // Loop through observations
        for (int j = 0; j < this.SampleSize; j++)
        {
          // Append to row
          s += String.Format("\t{0}", this.Data[i][j]);
        }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return s;
    }

    #endregion
  }
}
