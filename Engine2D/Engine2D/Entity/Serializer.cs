using System;
using System.Diagnostics;

namespace Engine2D
{
  // A mechanism for managing object serialization
  public class Serializer
  {
    // =====
    #region Variables

    // An array of serialized objects
    private EngineObject[] objects;

    #endregion


    // =====
    #region Properties

    // objects accessor
    public EngineObject[] Objects
    {
      get
      {
        return this.objects;
      }
      set
      {
        this.objects = value;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public Serializer()
    {
      this.objects = new EngineObject[0];
    }

    #endregion


    // =====
    #region Methods

    #endregion
  }
}
