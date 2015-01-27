using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Engine2D
{
  public struct DenseMatrix
  {
    // =====
    #region Variables

    // A list of elements
    private float[] elements;
    // The total number of rows
    private int m;
    // The total number of columns
    private int n;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      // Initialize string
      String result = "\n [";

      // Loop through elements
      for (int i = 0; i < this.M; i++)
      {
        // Add new line
        if (i > 0) { result += "\n  "; }

        for (int j = 0; j < this.N; j++)
        {
          // Update result
          result += this.GetCell(i, j).ToString("+0000.0000;-0000.0000; 0000.0000") + ", ";
        }
      }

      // Remove last comma
      result = result.Substring(0, result.Length - 2);
      // Close bracket
      result += " ]";

      // Return result
      return result;
    }

    // elements accessor
    public float[] Elements
    {
      get
      {
        return this.elements;
      }
      set
      {
        this.elements = value;
      }
    }

    // The number of rows
    public int M
    {
      get
      {
        return this.m;
      }
    }
    // The number of columns
    public int N
    {
      get
      {
        return this.n;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public DenseMatrix(int m, int n, float[] elements)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      #region Error checking
      #if IS_ERROR_CHECKING

      // Declare error message
      String s;

      #region  [1]  Positive dimensionality

      if (n <= 0 || m <= 0)
      {
        // Create error message
        s = "Invalid arguments\n";
        s += "Matrix dimensions m and n must be positive integers!\n";
        s += String.Format("m = {0}, n = {1}", m, n);

        // Throw exception
        throw new ArgumentException(s);
      }

      #endregion

      #region [2]  Array size violation

      // Check for list count violation
      if (elements.Length != m * n)
      {
        // Create error message
        s = "Invalid arguments\n";
        s += "Array must have m * n elements!\n";
        s += String.Format("elements.Count = {0}, m = {1], n = {2}", elements.Length, m, n);

        // Throw exception
        throw new ArgumentException(s);
      }

      #endregion

      #endif
      #endregion

      // Set instance variables
      this.m = m;
      this.n = n;
      this.elements = elements;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Get cell value
    public float GetCell(int i, int j)
    {
      #region Error checking
      #if IS_ERROR_CHECKING

      #region [1]  Check range of i

      if (i < 0 || i > this.M - 1)

      {
        // Create error message
        String s = "Invalid arguments\n";
        s += "Index i must be between 0 and (m - 1)!\n";
        s += String.Format("i = {0}, m = {1}", i, m);

        // Throw exception
        throw new ArgumentException(s);
      }

      #endregion

      #region [2]  Check range of j

      if (j < 0 || j > this.N - 1)
      {
        // Create error message
        String s = "Invalid arguments\n";
        s += "Index j must be between 0 and (n - 1)!\n";
        s += String.Format("j = {0}, n = {1}", j, n);

        // Throw exception
        throw new ArgumentException(s);
      }

      #endregion

      #endif
      #endregion

      // Get value
      return this.Elements[this.N * i + j];
    }

    // Set cell value
    public void SetCell(int i, int j, float value)
    {
      #region Error checking
      #if IS_ERROR_CHECKING

      #region [1]  Check range of i

      if (i < 0 || i > this.M - 1)

      {
        // Create error message
        String s = "Invalid arguments\n";
        s += "Index i must be between 0 and (m - 1)!\n";
        s += String.Format("i = {0}, m = {1}", i, m);

        // Throw exception
        throw new ArgumentException(s);
      }

      #endregion

      #region [2]  Check range of j

      if (j < 0 || j > this.N - 1)
      {
        // Create error message
        String s = "Invalid arguments\n";
        s += "Index j must be between 0 and (n - 1)!\n";
        s += String.Format("j = {0}, n = {1}", j, n);

        // Throw exception
        throw new ArgumentException(s);
      }

      #endregion

      #endif
      #endregion

      // Set value
      this.Elements[this.N * i + j] = value;
    }

    // Swap rows i and j
    public void SwapRow(int i, int x)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      #region Error checking
      #if IS_ERROR_CHECKING

      #region [1]  Check range of i

      if (i < 0 || i > this.M - 1)

      {
        // Create error message
        String s = "Invalid arguments\n";
        s += "Index i must be between 0 and (m - 1)!\n";
        s += String.Format("i = {0}, m = {1}", i, m);

        // Throw exception
        throw new ArgumentException(s);
      }

      #endregion

      #region [2]  Check range of x

      if (x < 0 || i > this.M - 1)
      {
        // Create error message
        String s = "Invalid arguments\n";
        s += "Index x must be between 0 and (m - 1)!\n";
        s += String.Format("x = {0}, m = {1}", x, m);

        // Throw exception
        throw new ArgumentException(s);
      }

      #endregion

      #endif
      #endregion

      // Loop through columns
      for (int j = 0; j < this.N; j++)
      {
        // Remember 'old' ijCell
        float oldCell = this.GetCell(i, j);

        // Replace 'old' ijCell with xjCell
        this.SetCell(i, j, this.GetCell(x, j));

        // Replace xjCell with 'old' ijCell
        this.SetCell(x, j, oldCell);
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Add a multiple of a row to another
    public void AddRow(int i, int x, float k)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      #region Error checking
      #if IS_ERROR_CHECKING

      #region [1]  Check range of i

      if (i < 0 || i > this.M - 1)

      {
        // Create error message
        String s = "Invalid arguments\n";
        s += "Index i must be between 0 and (m - 1)!\n";
        s += String.Format("i = {0}, m = {1}", i, m);

        // Throw exception
        throw new ArgumentException(s);
      }

      #endregion

      #region [2]  Check range of x

      if (x < 0 || i > this.M - 1)
      {
        // Create error message
        String s = "Invalid arguments\n";
        s += "Index x must be between 0 and (m - 1)!\n";
        s += String.Format("x = {0}, m = {1}", x, m);

        // Throw exception
        throw new ArgumentException(s);
      }

      #endregion

      #endif
      #endregion

      // Loop through columns
      for (int j = 0; j < this.N; j++)
      {
        // Increment ijCell by k times xjCell
        this.SetCell(i, j, this.GetCell(i, j) + k * this.GetCell(x, j));
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Multiply each element in a row by a scalar
    public void ScaleRow(int i, float k)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      #region Error checking
      #if IS_ERROR_CHECKING

      #region [1]  Check range of i

      if (i < 0 || i > this.M - 1)

      {
        // Create error message
        String s = "Invalid arguments\n";
        s += "Index i must be between 0 and (m - 1)!\n";
        s += String.Format("i = {0}, m = {1}", i, m);

        // Throw exception
        throw new ArgumentException(s);
      }

      #endregion

      #endif
      #endregion

      // Loop through columns
      for (int j = 0; j < this.N; j++)
      {
        // Increment ijCell by k times xjCell
        this.SetCell(i, j, k * this.GetCell(i, j));
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    } 

    // Transform this matrix to row echelon form
    public void ToRowEchelonForm()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize pivot diagonal
      int d = 0;
      // Initialize pivot 'horizontal shift from diagonal'
      int s = 0;

      // Pivot entry point
      pivot:

      // Loop through pivot positions
      while (d < this.M && d + s < this.N)
      {
        // Test logging
        // Trace.WriteLine(String.Format("d = {0}, s = {1}, matrix = {2}", d, s, this));

        // Get current pivot cell
        float pivotCell = this.GetCell(d, d + s);

        // Check if pivot cell is non-zero
        if (Math.Round(pivotCell, 6) != 0)
        {
          // If so, good!  We can pivot on this diagonal.  We will loop through
          // all rows below d and subtract the appropriate multiple of row d.

          // Loop through 'below' rows
          for (int i = d + 1; i < this.M; i++)
          {
            // Get idsCell
            float idsCell = this.GetCell(i, d + s);

            // If non-zero, subtract the appropriate multiple of row d
            if (Math.Round(idsCell, 6) != 0) { this.AddRow(i, d, -idsCell / pivotCell); }
          }
        }
        else
        {
          // If not, we must find a 'below' row, let us call it i, such that
          // idsCell is non-zero.  If such a row exists, then we swap rows i and
          // d and proceed similar to case 1.

          // Loop through 'below' rows
          for (int i = d + 1; i < this.M; i++)
          {
            // Check if idsCell is non-zero
            if (Math.Round(this.GetCell(i, d + s), 6) != 0)
            {
              // If so, swap rows i and d and repeat from the pivot step
              this.SwapRow(i, d); goto pivot;
            }
          }

          // If we've made it this far, then no such i exists.  In this case,
          // we take a 'horizontal step to the right, away from the diagonal'
          // and try to find a non-zero pivot cell.
          s++; goto pivot;
        }

        // Increment pivot diagonal
        d++;
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Transform this matrix to reduced row echelon form
    public void ToReducedRowEchelonForm()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // First, go to row echelon form
      this.ToRowEchelonForm();

      // Next, we use back-substitution

      // Loop through rows, from bottom to top
      for (int i = this.M - 1; i >= 0; i--)
      {
        // Test logging
        // Trace.WriteLine("i = " + i + this);

        // Loop through columns, from left to right
        for (int j = 0; j < this.N; j++)
        {
          // Get ijCell
          float ijCell = this.GetCell(i, j);

          // If zero, skip
          if (Math.Round(ijCell, 6) == 0) { continue; }

          // If we've made it this far, then we've found the leftmost non-zero
          // element in row i.  We will scale this row such that the leftmost
          // element becomes equal to 1.
          this.ScaleRow(i, 1f / ijCell);

          // We will loop through all 'above' rows and subtract the appropriate
          // multiple of this row
          for (int x = i - 1; x >= 0; x--)
          {
            // Get xjCell
            float xjCell = this.GetCell(x, j);

            // If non-zero, multiply by appropriate multiple of row i
            if (Math.Round(xjCell, 6) != 0) { this.AddRow(x, i, -xjCell); }
          }

          // Proceed to the next row
          break;
        }
      }


      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion
  }
}
