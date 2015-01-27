using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Engine2D
{
  // A pointer to a value type
  public class Pointer<T>
  {
    // =====
    #region Variables

    // The get acccessor we're pointing to
    private Func<T> accessor;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return this.Value.ToString();
    }

    // value accessor
    public T Value
    {
      get
      {
        return this.accessor();
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Pointer(Func<T> accessor)
    {
      this.accessor = accessor;
    }

    #endregion
  }
}
