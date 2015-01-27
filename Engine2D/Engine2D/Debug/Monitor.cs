using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Engine2D
{
  // A mechanism for monitoring game performance
  public class Monitor : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // Updates per second
    private double updatesPerSecond;
    // Frames per second
    private double framesPerSecond;
    // Sleep per update
    private double sleepPerUpdate;
    // Data text
    private String dataText;
    // Sprite
    private SpriteText spriteText;

    // Window size, i.e. maximum elapsedRealTime before reset
    private double windowSize;
    // Total elapsed time since last reset
    private double totalTime;
    // Elapsed time since last StopStart
    private double elapsedTime;
    // Total updates completed over totalTime
    private int updateCount;
    // Total frames drawn over totalTime
    private int frameCount;

    // A multi-purpose stopwatch
    private Stopwatch timer;
    // A list of component times over totalTime
    private List<double> times;
    // The index of the next next component to measure
    private int index;
    // An ordered list of component descriptions
    private List<String> components;

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
        return String.Format("Mntr{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{Up:  {0:##.0000}, Fr:  {0:##.0000}, Sl:  {0:##.0000}", 
        this.UpdatesPerSecond,
        this.FramesPerSecond,
        this.SleepPerUpdate);
    }

    // updatesPerSecond accessor
    public double UpdatesPerSecond
    {
      get
      {
        return this.updatesPerSecond;
      }
    }
    // framesPerSecond accessor
    public double FramesPerSecond
    {
      get
      {
        return this.framesPerSecond;
      }
    }
    // sleepPerUpdate accessor
    public double SleepPerUpdate
    {
      get
      {
        return this.sleepPerUpdate;
      }
    }
    // dataText accessor
    public String DataText
    {
      get
      {
        return this.dataText;
      }
    }
    // spriteText accessor
    public SpriteText SpriteText
    {
      get
      {
        return this.spriteText;
      }
    }

    // windowSize accessor
    public double WindowSize
    {
      get
      {
        return this.windowSize;
      }
      set
      {
        this.windowSize = value;
      }
    }
    // totalTime accessor
    public double TotalTime
    {
      get
      {
        return this.totalTime;
      }
    }
    // elapsedTime accessor
    public double ElapsedTime
    {
      get
      {
        return this.elapsedTime;
      }
    }
    // updateCount accessor
    public int UpdateCount
    {
      get
      {
        return this.updateCount;
      }
    }
    // frameCount accessor
    public int FrameCount
    {
      get
      {
        return this.frameCount;
      }
    }

    // timer accessor
    public Stopwatch Timer
    {
      get
      {
        return this.timer;
      }
    }
    // times accessor
    public List<double> Times
    {
      get
      {
        return this.times;
      }
    }
    // index accessor
    public int Index
    {
      get
      {
        return this.index;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Monitor()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif
      
      // Set instance variables
      this.updatesPerSecond = 0.0;
      this.framesPerSecond = 0.0;
      this.sleepPerUpdate = 0.0;
      this.dataText = null;
      this.spriteText = new SpriteText(Rectangle.Empty, this.DataText, Globals.FontManager.DefaultFont, new WrappedTextureShader());

      this.windowSize = 1.0;
      this.totalTime = double.PositiveInfinity;
      this.elapsedTime = 0.0;
      this.updateCount = 0;
      this.frameCount = 0;

      this.timer = new Stopwatch();
      this.times = new List<double>();
      for (int i = 0; i < 11; i++) { this.times.Add(0.0); }
      this.index = 0;

      this.components = new List<String>();
      this.components.Add("Montr");
      this.components.Add("BUpdt");
      this.components.Add("Input");
      this.components.Add("Views");
      this.components.Add("Scene");
      this.components.Add("Camra");
      this.components.Add("Trigg");
      this.components.Add("Sprte");
      this.components.Add("GDraw");
      this.components.Add("BDraw");
      this.components.Add("Sleep");

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // ====
    #region Update

    // Update
    public void Update()
    {
      // Note:  This is the very first method to get called in the game loop.
      // It will get called every iteration.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Protect against a 'double draw'
      if (this.index > this.Times.Count - 1) { this.index = this.Times.Count - 1; }

      // Remember old time
      double oldTime = this.elapsedTime;
      // Get current time
      this.elapsedTime = this.timer.Elapsed.TotalSeconds;
      // Record elapsed time
      this.times[this.index] += this.elapsedTime - oldTime;

      // Note:  The elapsed time above may or may not refer to sleep time.  It
      // depends on whether or not the thread had left over time last cycle.
      
      // Reset index
      this.index = 0;

      // Check if elapsedRealTime exceeds windowSize
      if (this.totalTime > this.windowSize)
      {
        // If so, we update our statistics and reset the counters
        this.updatesPerSecond = this.updateCount / this.totalTime;
        this.framesPerSecond = this.frameCount / this.totalTime;
        this.sleepPerUpdate = this.times[this.times.Count - 1] / this.totalTime;
        this.updateCount = 0;
        this.frameCount = 0;

        // In addition, we reset our timers
        this.timer.Restart();
        this.totalTime = 0.0;
        this.elapsedTime = 0.0;
        for (int i = 0; i < this.times.Count; i++) { this.times[i] = 0.0; }
      }

      // Update totalTicks
      this.totalTime = this.elapsedTime;

      // Increment updateCount
      this.updateCount++;

      // Build string
      String s = "";

      s += String.Format("TotalRealTime = {0:hh\\:mm\\:ss\\.ffff}\n", Globals.Clock.TotalRealTimeSpan);
      s += String.Format("TotalGameTime = {0:hh\\:mm\\:ss\\.ffff}\n", Globals.Clock.TotalGameTimeSpan);
      s += String.Format("GameTimeBias = {0:0.0000}\n", (Globals.Clock.TotalGameTime - Globals.Clock.TotalRealTime) / Globals.Clock.TotalRealTime);
      s += String.Format("UpdatesPerSecond = {0:0.0000}\n", this.UpdatesPerSecond);
      s += String.Format("FramesPerSecond = {0:0.0000}\n", this.FramesPerSecond);
      s += String.Format("SleepPerUpdate = {0:0.0000}\n", this.SleepPerUpdate);
      s += String.Format("DrawCalls = {0:000}\n", Globals.GraphicsDevice.DrawCalls);
      // s += String.Format("IsRunningSlowly = {0}\n", Globals.Game1.GameTime.IsRunningSlowly);
      // s += String.Format("TestUpdateCount = {0:0000}\n", Globals.TestUpdateCount);
      // s += String.Format("TestDrawCount   = {0:0000}\n", Globals.TestDrawCount);
      // s += String.Format("DifferenceCount = {0:0000}\n", Globals.TestUpdateCount - Globals.TestDrawCount);
      s += String.Format("TargetElapsedTime = {0}\n", Globals.Game1.TargetElapsedTime);

      // s += String.Format("Globals.Game1.Window.ClientBounds = {0}\n", Globals.Game1.Window.ClientBounds);
      // s += String.Format("DisplayMode = {0}\n", Globals.Game1.GraphicsDevice.DisplayMode);
      s += String.Format("Screen = {0}\n", Globals.GraphicsDevice.Screen);
      s += String.Format("Window = {0}\n", Globals.GraphicsDevice.Window);
      s += String.Format("Viewport = {0}\n", Globals.Game1.GraphicsDevice.Viewport);
      s += String.Format("PP.BackBufferWidth = {0}, PP.BackBufferHeight = {1}\n",
        Globals.Game1.GraphicsDevice.PresentationParameters.BackBufferWidth,
        Globals.Game1.GraphicsDevice.PresentationParameters.BackBufferHeight);
      // s += String.Format("Camera = {0}\n", Globals.Camera);
      
      s += String.Format("MouseState = {0}\n", Mouse.GetState());
      // s += String.Format("LeftButton = {0}\n", Globals.InputHandler.MouseLeftButtonState);
      
      s += "Scene0001 = " + Globals.Scene + "\n";
      /* for (int i = 0; i < Globals.Scene.Entities.Count; i++)
      {
        s += String.Format("{0} = {1}\n", Globals.Scene.Entities[i].Name, Globals.Scene.Entities[i]);
      } */
      for (int i = 0; i < Globals.Scene.Bodies.Count; i++)
      {
        s += String.Format("{0} = {1}\n", Globals.Scene.Bodies[i].Name, Globals.Scene.Bodies[i]);
      }

      double sum = 0.0;
      double ratio;
      for (int i = 0; i < this.times.Count; i++)
      {
        if (this.totalTime == 0.0) { ratio = 1 / this.times.Count; }
        else { ratio = times[i] / this.totalTime; }
        sum += ratio;
        s += String.Format("{0} = {1:000.00%}\n", this.components[i], ratio);
      }
      if (sum == 0) { sum = 1; }
      s += String.Format("Total = {0:000.00%}\n", sum);
      s += String.Format("Buckt = {0:000.00%}\n", this.totalTime / this.windowSize);

      s += Globals.Scene.ContactManager.GetString();
      

      // Update text
      this.dataText = s;

      // Stop timing self
      this.StopStart();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Stop the previous and begin the next component timer
    public void StopStart()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Protect against a 'double draw'
      if (this.index > this.times.Count - 1) { return; }

      // Remember old time
      double oldTicks = this.elapsedTime;
      // Get current time
      this.elapsedTime = this.timer.Elapsed.TotalSeconds;
      // Record elapsed time
      this.times[this.index] += this.elapsedTime - oldTicks;
      // Increment index
      this.index++;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Draw

    // Draw
    public void Draw()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif



      // Update frameCount
      this.frameCount++;

      // Update box
      this.SpriteText.Rectangle = new Rectangle(0, 0, 800, 800, 0);
      // Update text
      this.SpriteText.Text = this.DataText;
      // Draw sprite
      Globals.GraphicsDevice.DrawText(this.SpriteText);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
