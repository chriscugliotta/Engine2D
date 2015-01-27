using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // The game clock
  public class Clock : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // Game start time
    private DateTime gameStartTime;
    // Current real time
    private DateTime currentRealTime;
    // Total real time since game start
    private TimeSpan totalRealTime;
    // Elapsed real time since last update
    private TimeSpan elapsedRealTime;
    // Total game time since game start
    private TimeSpan totalGameTime;
    // Total game time since last update
    private TimeSpan elapsedGameTime;
    // Total update count
    private long totalUpdateCount;

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
        return String.Format("Cloc{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{TR:  {0:hh\\:mm\\:ss\\.ffff}, TG: {1:hh\\:mm\\:ss\\.ffff}}",
        this.totalRealTime,
        this.totalGameTime);
    }

    // gameStartTime accessor
    public DateTime GameStartTime
    {
      get
      {
        return this.gameStartTime;
      }
    }
    // currentRealTime accessor
    public DateTime CurrentRealTime
    {
      get
      {
        return this.currentRealTime;
      }
    }
    // totalRealTime accessor
    public float TotalRealTime
    {
      get
      {
        return (float)this.totalRealTime.TotalSeconds;
      }
    }
    // elapsedRealTime accessor
    public float ElapsedRealTime
    {
      get
      {
        return (float)this.elapsedRealTime.TotalSeconds;
      }
    }
    // totalGameTime accessor
    public float TotalGameTime
    {
      get
      {
        return (float)this.totalGameTime.TotalSeconds;
      }
    }
    // elapsedGameTime accessor
    public float ElapsedGameTime
    {
      get
      {
        return (float)this.elapsedGameTime.TotalSeconds;
      }
    }
    // totalUpdateCount accessor
    public long TotalUpdateCount
    {
      get
      {
        return this.totalUpdateCount;
      }
    }

    // XNA's TargetElapsedTime accessor
    public TimeSpan TargetElapsedTime
    {
      get
      {
        return Globals.Game1.TargetElapsedTime;
      }
      set
      {
        Globals.Game1.TargetElapsedTime = value;
      }
    }
    // Alternate totalRealTime accessor
    public TimeSpan TotalRealTimeSpan
    {
      get
      {
        return this.totalRealTime;
      }
    }
    // Alternate elapsedRealTime accessor
    public TimeSpan ElapsedRealTimeSpan
    {
      get
      {
        return this.elapsedRealTime;
      }
    }
    // Alternate totalGameTime accessor
    public TimeSpan TotalGameTimeSpan
    {
      get
      {
        return this.totalGameTime;
      }
    }
    // Alternate gameTimeSpan accessor
    public TimeSpan ElapsedGameTimeSpan
    {
      get
      {
        return this.elapsedGameTime;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Clock()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Get current time
      DateTime now = DateTime.Now;

      // Set instance variables
      this.gameStartTime = now;
      this.currentRealTime = now;
      this.totalRealTime = new TimeSpan(0, 0, 0);
      this.elapsedRealTime = new TimeSpan(0, 0, 0);
      this.totalGameTime = new TimeSpan(0, 0, 0);
      this.elapsedGameTime = new TimeSpan(0, 0, 0);
      this.totalUpdateCount = 0;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // ====
    #region Update

    // Update
    public void Update(TimeSpan elapsedGameTime, TimeSpan totalGameTime)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Get current time
      DateTime now = DateTime.Now;

      // Update members
      this.elapsedRealTime = now - this.currentRealTime;
      this.totalRealTime = now - this.gameStartTime;
      this.elapsedGameTime = elapsedGameTime;
      this.totalGameTime = totalGameTime;
      this.currentRealTime = now;
      this.totalUpdateCount++;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
