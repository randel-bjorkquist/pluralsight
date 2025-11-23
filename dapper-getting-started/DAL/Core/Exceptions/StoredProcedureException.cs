namespace DAL.Core.Exceptions;

/// <summary>
/// Custom exception for stored procedure execution failures.
/// Wraps underlying errors (e.g., SqlException) with SP-specific details.
/// </summary>
public class StoredProcedureException : Exception
{
  /// <summary> The stored procedure name (if known). </summary>
  public string? StoredProcedureName { get; }

  /// <summary> The return value indicating the error (e.g., non-zero code). </summary>
  public int ReturnValue { get; }

  /// <summary> Initializes a new instance of the <see cref="StoredProcedureException"/> class. </summary>
  public StoredProcedureException() { }

  /// <summary> Initializes a new instance with a message. </summary>
  /// <param name="message">The error message.</param>
  public StoredProcedureException(string message) : base(message) { }

  /// <summary> Initializes a new instance with a message and inner exception. </summary>
  /// <param name="message">The error message.</param>
  /// <param name="innerException">The underlying exception (e.g., SqlException).</param>
  public StoredProcedureException(string message, Exception innerException) 
    : base(message, innerException) { }

  /// <summary> Initializes a new instance with SP details. </summary>
  /// <param name="storedProcedureName">The name of the stored procedure.</param>
  /// <param name="returnValue">The non-zero return value from the SP.</param>
  /// <param name="message">Optional custom message.</param>
  public StoredProcedureException(string storedProcedureName, int returnValue, string? message = null)
    : base(message ?? $"Stored procedure '{storedProcedureName}' returned error code {returnValue}.")
  {
      StoredProcedureName = storedProcedureName;
      ReturnValue         = returnValue;
  }
}