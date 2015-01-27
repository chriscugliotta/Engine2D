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
  // A class of action methods that return an integer-valued exit status.
  //   0 = Action is parallel but not yet complete
  //   1 = Action is parallel and complete
  //   2 = Action is serial but not yet complete
  //   3 = Action is serial and complete
  // public delegate byte Method(ref List<Object> arguments);

  // A collection of user-customized methods
  public class Custom
  {
    #region Methods

    // 'Next frame'
    // Respond to button1 press
    public static void Button1RespondToPress(Button button)
    {
      // Trace.WriteLine(String.Format("[{0}]  Button1 pressed!", Globals.Clock.TotalGameTimeSpan));
    }
    // Respond to button1 release
    public static void Button1RespondToRelease(Button button)
    {
      // Trace.WriteLine(String.Format("[{0}]  Button1 released!", Globals.Clock.TotalGameTimeSpan));
      Globals.TriggerManager.Triggers[0].IsSleeping = false;
    }

    // 'Pause'
    // Respond to button2 press
    public static void Button2RespondToPress(Button button)
    {
      // Trace.WriteLine(String.Format("[{0}]  Button2 pressed!", Globals.Clock.TotalGameTimeSpan));
    }
    // Respond to button2 release
    public static void Button2RespondToRelease(Button button)
    {
      // Trace.WriteLine(String.Format("[{0}]  Button2 released!", Globals.Clock.TotalGameTimeSpan));
      if (Globals.Scene.IsPaused)
      {
        Globals.Scene.Unpause();
        button.Text = "Pause";
      }
      else
      {
        Globals.Scene.Pause();
        button.Text = "Unpause";
      }
    }

    // 'Up'
    // Respond to button3 press
    public static void Button3RespondToPress(Button button)
    {
      Globals.TestWrapper1 = new Wrapper<Vector2>(new Vector2(0.0f, -10.0f / Globals.Camera.Zoom));
      Globals.TestWrapper2 = new Wrapper<float>(0.0f);
      Globals.TestWrapper3 = new Wrapper<float>(1.0f);
      //Trace.WriteLine("Globals.TestWrapper1 = " + Globals.TestWrapper1);
      Globals.TriggerManager.Triggers[1].IsSleeping = false;
    }
    // Respond to button3 release
    public static void Button3RespondToRelease(Button button)
    {
      Globals.TriggerManager.Triggers[1].IsSleeping = true;
    }

    // 'Down'
    // Respond to button4 press
    public static void Button4RespondToPress(Button button)
    {
      Globals.TestWrapper1 = new Wrapper<Vector2>(new Vector2(0.0f, 10.0f / Globals.Camera.Zoom));
      Globals.TestWrapper2 = new Wrapper<float>(0.0f);
      Globals.TestWrapper3 = new Wrapper<float>(1.0f);
      Globals.TriggerManager.Triggers[1].IsSleeping = false;
    }
    // Respond to button4 release
    public static void Button4RespondToRelease(Button button)
    {
      Globals.TriggerManager.Triggers[1].IsSleeping = true;
    }

    // 'Left'
    // Respond to button5 press
    public static void Button5RespondToPress(Button button)
    {
      Globals.TestWrapper1 = new Wrapper<Vector2>(new Vector2(-10.0f / Globals.Camera.Zoom, 0.0f));
      Globals.TestWrapper2 = new Wrapper<float>(0.0f);
      Globals.TestWrapper3 = new Wrapper<float>(1.0f);
      Globals.TriggerManager.Triggers[1].IsSleeping = false;
    }
    // Respond to button5 release
    public static void Button5RespondToRelease(Button button)
    {
      Globals.TriggerManager.Triggers[1].IsSleeping = true;
    }

    // 'Right'
    // Respond to button6 press
    public static void Button6RespondToPress(Button button)
    {
      Globals.TestWrapper1 = new Wrapper<Vector2>(new Vector2(10.0f / Globals.Camera.Zoom, 0.0f));
      Globals.TestWrapper2 = new Wrapper<float>(0.0f);
      Globals.TestWrapper3 = new Wrapper<float>(1.0f);
      Globals.TriggerManager.Triggers[1].IsSleeping = false;
    }
    // Respond to button6 release
    public static void Button6RespondToRelease(Button button)
    {
      Globals.TriggerManager.Triggers[1].IsSleeping = true;
    }

    // 'Zoom in'
    // Respond to button7 press
    public static void Button7RespondToPress(Button button)
    {
      Globals.TestWrapper1 = new Wrapper<Vector2>(new Vector2(0.0f, 0.0f));
      Globals.TestWrapper2 = new Wrapper<float>(0.0f);
      Globals.TestWrapper3 = new Wrapper<float>(1.05f);
      Globals.TriggerManager.Triggers[1].IsSleeping = false;
    }
    // Respond to button7 release
    public static void Button7RespondToRelease(Button button)
    {
      Globals.TriggerManager.Triggers[1].IsSleeping = true;
    }

    // 'Zoom out'
    // Respond to button8 press
    public static void Button8RespondToPress(Button button)
    {
      Globals.TestWrapper1 = new Wrapper<Vector2>(new Vector2(0.0f, 0.0f));
      Globals.TestWrapper2 = new Wrapper<float>(0.0f);
      Globals.TestWrapper3 = new Wrapper<float>(0.95f);
      Globals.TriggerManager.Triggers[1].IsSleeping = false;
    }
    // Respond to button8 release
    public static void Button8RespondToRelease(Button button)
    {
      Globals.TriggerManager.Triggers[1].IsSleeping = true;
    }

    // 'Rotate CW'
    // Respond to button9 press
    public static void Button9RespondToPress(Button button)
    {
      Globals.TestWrapper1 = new Wrapper<Vector2>(new Vector2(0.0f, 0.0f));
      Globals.TestWrapper2 = new Wrapper<float>(0.01f);
      Globals.TestWrapper3 = new Wrapper<float>(1.0f);
      Globals.TriggerManager.Triggers[1].IsSleeping = false;
    }
    // Respond to button9 release
    public static void Button9RespondToRelease(Button button)
    {
      Globals.TriggerManager.Triggers[1].IsSleeping = true;
    }

    // 'Rotate CCW'
    // Respond to button10 press
    public static void Button10RespondToPress(Button button)
    {
      Globals.TestWrapper1 = new Wrapper<Vector2>(new Vector2(0.0f, 0.0f));
      Globals.TestWrapper2 = new Wrapper<float>(-0.01f);
      Globals.TestWrapper3 = new Wrapper<float>(1.0f);
      Globals.TriggerManager.Triggers[1].IsSleeping = false;
    }
    // Respond to button9 release
    public static void Button10RespondToRelease(Button button)
    {
      Globals.TriggerManager.Triggers[1].IsSleeping = true;
    }

    // 'Reset'
    // Respond to button11 press
    public static void Button11RespondToPress(Button button)
    {
      // Do nothing
    }
    // Respond to button11 release
    public static void Button11RespondToRelease(Button button)
    {
      // Reset camera state
      Globals.Camera.Position = Vector2.Zero;
      Globals.Camera.Angle = 0.0f;
      Globals.Camera.Zoom = 1.0f;
    }

    // 'Log'
    // Respond to button12 press
    public static void Button12RespondToPress(Button button)
    {
      // Do nothing
    }
    // Respond to button12 release
    public static void Button12RespondToRelease(Button button)
    {
      if (Log.IsSleeping)
      {
        Log.WakeUp();
        button.Text = "Disable Log";
      }
      else
      {
        Log.GoToSleep();
        button.Text = "Enable Log";
      }
    }

    // 'Spool'
    // Respond to button13 press
    public static void Button13RespondToPress(Button button)
    {
      // Do nothing
    }
    // Respond to button13 release
    public static void Button13RespondToRelease(Button button)
    {
      if (Globals.TestBool)
      {
        Globals.TestBool = false;
        button.Text = "Enable Spl";
      }
      else
      {
        Globals.TestBool = true;
        button.Text = "Disable Spl";
      }
    }

    // 'Restart'
    // Respond to button14 press
    public static void Button14RespondToPress(Button button)
    {
      // Do nothing
    }
    // Respond to button14 release
    public static void Button14RespondToRelease(Button button)
    {
      Globals.InitializeScene();
    }


    #endregion
  }
}
