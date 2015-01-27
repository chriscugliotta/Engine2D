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
  // A collection of condition methods
  public class Condition
  {
    // =====
    #region Delegates

    public delegate bool Method(List<Object> arguments);

    #endregion

    // =====
    #region Methods

    // True
    public static bool Always(List<Object> arguments)
    {
      return true;
    }

    #endregion
  }
}
