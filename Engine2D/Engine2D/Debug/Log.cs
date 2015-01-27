using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Engine2D
{
  // A logging mechanism
  public static class Log
  {
    // =====
    #region Variables

    // Log folder path
    private static String logFolder;
    // Current log file name
    private static String logFile;
    // Current log file count
    private static int fileCount;
    // Current line count
    private static int lineCount;
    // Line limit per file
    private static int lineLimit;

    // Equals true if the system should stop logging mid-runtime
    private static bool isSleeping;

    // Physics logging targets
    public static String Subject1 = null;
    public static String Subject2 = null;
    // public static String Subject1 = "RgBd0002";
    // public static String Subject2 = "RgBd0002";

    #endregion


    // =====
    #region Properties

    // Description
    public static String Description
    {
      get
      {
        return "{Pt:  " + Log.logFile + ", Ln:  " + Log.TotalLines() + "}";
      }
    }

    // logFolder accessor
    public static String LogFolder
    {
      get
      {
        return Log.logFolder;
      }
    }
    // logFile accessor
    public static String LogFile
    {
      get
      {
        return Log.logFile;
      }
    }
    // fileCount accessor
    public static int FileCount
    {
      get
      {
        return Log.fileCount;
      }
    }
    // lineCount accessor
    public static int LineCount
    {
      get
      {
        return Log.lineCount;
      }
    }
    // lineLimit accessor
    public static int LineLimit
    {
      get
      {
        return Log.lineLimit;
      }
    }

    // isSleeping accessor
    public static bool IsSleeping
    {
      get
      {
        return Log.isSleeping;
      }
    }

    // Total number of lines logged
    public static int TotalLines()
    {
      return Log.lineLimit * (Log.fileCount - 1) + Log.lineCount;
    }

    #endregion

    
    // =====
    #region Constructors

    // Designated constructor
    public static void Initialize(String folderPath, int lineLimit)
    {
      // Set instance variables
      Log.logFolder = @"C:\Users\Chris\Desktop\GameLog\";
      Log.logFile = Log.logFolder + "log0001.txt";
      Log.fileCount = 1;
      Log.lineCount = 0;
      Log.lineLimit = lineLimit;
      Log.isSleeping = false;

      // Check if folder already exists
      if (Directory.Exists(Log.logFolder))
      {
        // If so, delete old log files
        List<String> logFiles = Directory.EnumerateFiles(Log.logFolder, "log*.txt").ToList<String>();
        for (int i = 0; i < logFiles.Count(); i++) { File.Delete(logFiles[i]); }
      }
      else
      {
        // Otherwise, create new folder
        Directory.CreateDirectory(Log.logFolder);
      }

      // Prepare header string
      String app = System.AppDomain.CurrentDomain.FriendlyName;
      String date = DateTime.Now.ToString();

      // Create new log file
      using (StreamWriter writer = new StreamWriter(Log.logFile, true))
      {
        writer.WriteLine("Now running " + app + " at " + date);
      }

      // Go to sleep
      Log.GoToSleep();
    }

    #endregion


    // =====
    #region Methods

    // Write to a text file
    public static void Write(String s)
    {
      // Check if sleeping
      if (Log.isSleeping) { return; }

      // [1]  First, generate log prefix

      // Initialize prefix string
      String prefix = "";
      prefix += String.Format("[{0:hh\\:mm\\:ss\\.ffff}] {1:0000}", Globals.Clock.TotalGameTimeSpan, Globals.Clock.TotalUpdateCount);

      // Trace the stack
      StackTrace stackTrace = new StackTrace();
      // Get most recent frame
      StackFrame stackFrame = stackTrace.GetFrame(1);
      // Get method base
      MethodBase methodBase = stackFrame.GetMethod();
      // Get class name
      String className = methodBase.DeclaringType.Name;
      // Get method name
      String methodName = methodBase.Name;
      if (methodName == ".ctor") { methodName = "ctor"; }

      // Add to prefix string
      prefix += className + "." + methodName + ":  ";

      /* // Witch hunt
      if (Globals.Clock != null)
      {
        s += "\n";
        s += String.Format("{0}.Contacts.Count = {1}", Globals.Scene.Bodies[0].Name, Globals.Scene.Bodies[0].Contacts.Count);
      } */

      // [2]  Next, append log file

      // Write string
      using (StreamWriter writer = new StreamWriter(Log.logFile, true))
      {
        writer.WriteLine(prefix + s);
      }

      // [3]  Lastly, update counts

      // Increment lineCount
      lineCount++;

      // Check if lineCount exceeds lineLimit
      if (Log.lineCount >= Log.lineLimit)
      {
        // Increment fileCount
        Log.fileCount++;
        // Reset lineCount
        Log.lineCount = 0;
        // Update path
        Log.logFile = Log.logFolder + "log" + String.Format("{0:0000}", fileCount) + ".txt";
      }
    }

    // Go to sleep
    public static void GoToSleep()
    {
      // Log.Write("Log is now going to sleep...");
      Log.isSleeping = true;
    }

    // Wake up
    public static void WakeUp()
    {
      Log.isSleeping = false;
      // Log.Write("Log has now woken up!");
    }

    #endregion
  }
}
