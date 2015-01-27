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
  // A collection of action methods
  public class Action
  {
    // =====
    #region Delegates

    // A class of action methods that return an integer-valued exit status.
    //   0 = Action is parallel but not yet complete
    //   1 = Action is parallel and complete
    //   2 = Action is serial but not yet complete
    //   3 = Action is serial and complete
    public delegate byte Method(ref List<Object> arguments);

    #endregion

    // =====
    #region Methods

    // Wait for some time
    public static byte Wait(ref List<Object> arguments)
    {
      // This method assumes the arguments have the following structure:
      // arguments[0] = (float)  Remaining wait time
      // arguments[1] = (float)  Initial wait time

      // Entry logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Entering method");
      #endif

      // Initialize result as 'serial but not yet complete'
      byte result = 2;

      // Get wait time
      float t = (float)arguments[0];

      // Check remaining time
      if (t <= 0)
      {
        // If time is zero, return 'serial and complete'
        result = 3;
        // Reset timer
        arguments[0] = arguments[1];
      }
      else
      {
        // Otherwise, update timer
        arguments[0] = (Object)(t - Globals.Clock.ElapsedGameTime);
      }

      // Exit logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Write a string to the console
    public static byte WriteToConsole(ref List<Object> arguments)
    {
      // This method assumes the arguments have the following structure:
      //   arguments[0] = (String)  A string to display

      // Entry logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Entering method");
      #endif

      // Display string
      Trace.WriteLine(String.Format("[{0}] {1}", Globals.Clock.TotalGameTime, arguments[0]));

      // Exit logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Exiting method");
      #endif

      // Return 'parallel and complete'
      return 1;
    }

    // Pause scene
    public static byte PauseScene(ref List<Object> arguments)
    {
      // This method assumes the arguments have the following structure:
      //   arguments[0] = (Scene)  A scene to pause

      // Entry logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Entering method");
      #endif

      // Pause scene
      ((Scene)arguments[0]).Pause();

      // Exit logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Exiting method");
      #endif

      // Return 'parallel and complete'
      return 1;
    }

    // Unpause scene
    public static byte UnpauseScene(ref List<Object> arguments)
    {
      // This method assumes the arguments have the following structure:
      //   arguments[0] = (Scene)  A scene to unpause

      // Entry logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Entering method");
      #endif

      // Pause scene
      ((Scene)arguments[0]).Unpause();

      // Exit logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Exiting method");
      #endif

      // Return 'parallel and complete'
      return 1;
    }

    // Set camera state
    public static byte SetCamera(ref List<Object> arguments)
    {
      // This method assumes the arguments have the following structure:
      //   arguments[0] = (Camera)  A camera to modify
      //   arguments[1] = (Vector2)  A new camera position
      //   arguments[2] = (float)  A new camera angle
      //   arguments[3] = (float)  A new camera zoom

      // Entry logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Entering method");
      #endif

      // Unbox arguments
      Camera camera = (Camera)arguments[0];
      Vector2 position = (Vector2)arguments[1];
      float angle = (float)arguments[2];
      float zoom = (float)arguments[3];

      // Update camera
      camera.Position = position;
      camera.Angle = angle;
      camera.Zoom = zoom;

      // Rebox arguments
      arguments[0] = ((Object)camera);

      // Exit logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Exiting method");
      #endif

      // Return 'parallel and complete'
      return 1;
    }

    // Increment camera state
    public static byte MoveCamera(ref List<Object> arguments)
    {
      // This method assumes the arguments have the following structure:
      //   arguments[0] = (Camera)  A camera to modify
      //   arguments[1] = (Vector2)  Camera position additive change
      //   arguments[2] = (float)  Camera angle additive change
      //   arguments[3] = (float)  Camera zoom multiplicative change

      // Entry logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Entering method");
      #endif

      // Unbox arguments
      Camera camera = (Camera)arguments[0];
      // Vector2 position = ((Wrapper<Vector2>)arguments[1]).Value;
      // float angle = ((Wrapper<float>)arguments[2]).Value;
      // float zoom = ((Wrapper<float>)arguments[3]).Value;
      Vector2 position = Globals.TestWrapper1.Value;
      float angle = Globals.TestWrapper2.Value;
      float zoom = Globals.TestWrapper3.Value;

      // Update camera
      camera.Position += position;
      camera.Angle += angle;
      camera.Zoom *= zoom;

      // Rebox arguments
      arguments[0] = ((Object)camera);

      // Exit logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Exiting method");
      #endif

      // Return 'parallel and complete'
      return 1;
    }

    // Move a position over time
    public static byte MoveTo(ref List<Object> arguments)
    {
      // This method assumes the arguments have the following structure:
      //   arguments[0] = (Vector2) Current position
      //   arguments[1] = (Vector2) Final position
      //   arguments[2] = (float) Remaining time
      //   arguments[3] = (float) Initial time

      // Entry logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Entering method");
      #endif

      // Unbox arguments
      Vector2 x1 = (Vector2)arguments[0];
      Vector2 x2 = (Vector2)arguments[1];
      float t = (float)arguments[2];
      float T = (float)arguments[3];
      // Trace.WriteLine(String.Format("x1 = {0}, x2 = {1}, t = {2}, T = {3}", x1, x2, t, T));

      // Initialize result as 'parallel but not complete'
      byte result = 0;
      // Declare next position
      Vector2 x;

      // Check remaining time
      if (t <= 0)
      {
        // If time is zero, snap into place
        x = x2;
        // Reset timer
        t = T;
        // Return 'parallel and complete'
        result = 1;
      }
      else
      {
        // Otherwise, get next position
        x = x1 + (x2 - x1) / (t / (float)Globals.Clock.TargetElapsedTime.TotalSeconds);
        // Update time
        t -= Globals.Clock.ElapsedGameTime;
      }

      // Box arguments
      arguments[0] = (Object)x;
      arguments[1] = (Object)x2;
      arguments[2] = (Object)t;
      arguments[3] = (Object)T;
      // Trace.WriteLine(String.Format("x = {0}, t = {1}", x, t));

      // Exit logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write("Entering method");
      #endif

      // Return result
      return result;
    }

    #endregion
  }
}
