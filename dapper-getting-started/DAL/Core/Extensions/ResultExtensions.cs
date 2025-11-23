using DAL.Core.Results;
using DAL.Core.Utilities;

namespace DAL.Core.Extensions;

public static class ResultExtensions
{
  #region OnSuccess / OnFailure (Side Effects)

  /// <summary>
  /// What it does: If the Result<T> is a success (and Data is non-null), it executes the provided 
  /// action (Action<T>) with the success data as input. It does nothing on failure. Returns the original 
  /// Result<T> unchanged, enabling chaining.
  /// 
  /// Why it's useful: Allows injecting side effects (e.g., logging, caching, UI updates) only on success 
  /// paths without breaking the chain. Keeps functional flows pure while adding imperative behaviors 
  /// where needed—avoids wrapping everything in if-statements.
  /// </summary>
  /// <example>
  /// 
  /// var result = GetUserById(123);  // Success: User { Id=123, Name="Alice" }
  /// 
  /// result.OnSuccess(user => CacheUser(user))   // Caches the user only if success
  ///       .OnFailure(msgs => LogErrors(msgs));  // Hypothetical next step
  /// 
  /// RESULT NOTE:
  ///   result remains the same; caching happened as side effect.
  /// </example>
  public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> onSuccess)
  {
    if (result.IsSuccess && result.Data is not null)
    {
      onSuccess(result.Data);
    }

    return result;
  }

  /// <summary>
  /// What it does: If the Result<T> is a failure and has messages, it executes the provided action 
  /// (Action<MessageCollection>) with the messages as input. It does nothing on success. Returns 
  /// the original Result<T> unchanged, enabling chaining.
  /// 
  /// Why it's useful: Similar to OnSuccess, but for failure-side effects like logging errors, 
  /// sending alerts, or auditing—without halting the chain. Ensures errors are handled explicitly 
  /// andconsistently across operations.
  /// </summary>
  /// <example>
  /// 
  /// var result = GetUserById(999);  // Failure: Messages { "User not found" }
  /// 
  /// result.OnFailure(msgs => LogErrors(msgs))   // Logs the messages only if failure
  ///       .OnSuccess(user => CacheUser(user));  // WILL NOT RUN here ...
  /// 
  /// RESULT NOTE:
  ///   result remains the same; logging happened as side effect.
  /// </example>
  public static Result<T> OnFailure<T>(this Result<T> result, Action<MessageCollection> onFailure)
  {
    if (!result.IsSuccess && result.Messages.HasItems())
    {
      onFailure(result.Messages);
    }

    return result;
  }

  /// <summary>
  /// What it does: Same as the base overload, but passes both the messages and the highest severity 
  /// (MessageType) from the collection to the action (Action<MessageCollection, MessageType>). Only 
  /// triggers on failure with messages.
  /// 
  /// Why it's useful: Provides context about the failure's "level" (e.g., Error vs. Warning) in the 
  /// side effect, allowing nuanced handling—like alerting for Errors but just logging Warnings. Builds 
  /// on the base without redundancy.
  /// </summary>
  /// <example>
  /// 
  /// result.OnFailure((msgs, severity) => { 
  ///   if(severity == MessageType.Error) 
  ///       AlertAdmin(msgs); // Alerts only for high severity
  ///   else 
  ///       LogSoftIssue(msgs); 
  /// });
  /// 
  /// RESULT NOTE:
  ///   result unchanged; alert triggered due to Error presence.
  /// </example>
  public static Result<T> OnFailure<T>(this Result<T> result, Action<MessageCollection, MessageType> onFailure)
  {
    if (!result.IsSuccess && result.Messages.HasItems())
    {
      onFailure(result.Messages, result.Messages.HighestSeverity);
    }

    return result;
  }

  #endregion

  #region Map / Bind / ToSingle (Transformations)

  /// <summary>
  /// Transforms the success data to a new value if successful; returns unchanged on failure.
  /// </summary>
  public static Result<TResult> Map<T, TResult>(this Result<T> result, Func<T, TResult> mapper)
  {
    if (result.IsSuccess && result.Data is not null)
    {
      return Result<TResult>.Success(mapper(result.Data), result.Messages);
    }

    return Result<TResult>.Failure(new MessageCollection(result.Messages));
  }

  /// <summary>
  /// What it does: If the input Result<T> is a success, it applies a function (Func<T, Result<TResult>>) that returns 
  /// another Result (so the inner function can itself succeed or fail). On success, it returns the inner result. On 
  /// any failure (input or inner), it merges messages into a single failure Result<TResult> (copying the original 
  /// messages + any new ones from the inner failure). It preserves messages from the bound result if it succeeds.
  /// 
  /// Why it's useful: Enables chaining operations where each step might fail independently, without nesting if-statements. 
  /// It's like a "flatMap" in functional langs—keeps the "railway" going straight on success, but collects all errors if 
  /// it derails. (Contrast with Map, which assumes the mapper always succeeds and just transforms the data.)
  /// </summary>
  /// <example> 
  /// 
  /// //Assume: GetUserById returns Result<User>, ValidateUser returns Result<User> (or failure with validation msgs)
  /// var userResult = GetUserById(123);  // Success: User { Id=123, Email="test@example.com" }
  /// 
  /// RESULT NOTE: 
  ///   validatedResult is Failure, with merged Messages: any from GetUserById (none) + "Invalid email". If ValidateUser 
  ///   succeeded, you'd get its Data and Messages.
  /// </example>
  public static Result<TResult> Bind<T, TResult>(this Result<T> result, Func<T, Result<TResult>> binder)
  {
    if (result.IsSuccess && result.Data is not null)
    {
      var boundResult = binder(result.Data);

      if (!boundResult.IsSuccess)
      {
          // Merge messages immutably: Create new collection with original + bound
          var mergedMessages = new MessageCollection(result.Messages);
          mergedMessages.AddRange(boundResult.Messages);

          return Result<TResult>.Failure(mergedMessages);
      }

      return boundResult; // Preserve bound's messages (already includes any new ones)
    }

    return Result<TResult>.Failure(new MessageCollection(result.Messages)); // Copy for immutability
  }

  /// <summary>
  /// What it does: If the result is a failure with errors, it replaces the entire Messages collection with a single new error 
  /// message generated by the mapper function (Func<MessageCollection, string>). It ignores non-errors and doesn't touch 
  /// successes. Returns a new Result<T> (immutable).
  /// 
  /// Why it's useful: Failures often accumulate detailed messages during chaining, but for end-users or logs, you might want a
  /// concise summary (e.g., "3 validation errors occurred" instead of listing all). It's a lightweight way to "squash" errors 
  /// without losing the original during internal processing.
  /// </summary>
  /// <example>
  /// 
  /// // Failure with Messages: ["Email invalid", "Age too low", "Name missing"]
  /// var result = SomeOperation().MapFailure(msgs => $"Validation failed: {msgs.Errors.Count()} issues");
  /// 
  /// RESULT NOTE: 
  ///   result.Messages now has just one: Message.Error("Validation failed: 3 issues"). Original detailed message are lost 
  ///   here—use before this if you need them elsewhere.
  /// </example>
  public static Result<T> MapFailure<T>(this Result<T> result, Func<MessageCollection, string> errorMapper)
  {
    if (!result.IsSuccess && result.Messages.HasErrors())
    {
      var newError        = errorMapper(result.Messages);
      var updatedMessages = new MessageCollection();
              
      updatedMessages.AddError(newError);
              
      return Result<T>.Failure(updatedMessages);
    }

    return result;
  }

  /// <summary>
  /// Converts a <see cref="Result{IEnumerable{T}}"/> to a single-item <see cref="Result{T}"/>.
  /// </summary>
  /// <typeparam name="T">The model type within the collection.</typeparam>
  /// <param name="result">The source result containing a collection of models.</param>
  /// <param name="message">
  /// Optional custom message when the collection is null or empty.
  /// Defaults to "{typeof(T).Name} not found."
  /// </param>
  /// <returns>
  /// A <see cref="Result{T}"/> containing the single item (if found) or a failure result 
  /// with <see cref="MessageType.NotFound"/> if empty or null.
  /// </returns>
  /// <remarks>
  /// • Preserves messages from the source result.  
  /// • Returns a failure if the collection is empty, null, or the source was already a failure.  
  /// • Maintains consistency with the Result Pattern across services.
  /// </remarks>
  public static Result<T?> ToSingle<T>(this Result<IEnumerable<T>> result, string? message = null)
  {
    var messages = new MessageCollection(result.Messages);

    // Pass through failures directly ..
    if(!result.IsSuccess || result.Data is null)
      return Result<T?>.Failure(messages);

    // Try to extract one ...
    //var item = result.Data.FirstOrDefault();
    var item = result.Data.SingleOrDefault();

    if(item is null && !messages.HasNotFounds())
    {
      messages.AddNotFound(message ?? $"{typeof(T).Name} record not found.", "NOT_FOUND");

      return Result<T?>.Failure(messages);
    }

    return Result<T?>.Success(item, messages);
  }

  #endregion

  #region Match (Pattern Matching)

  /// <summary>
  /// What it does: Like a switch on success/failure. Provides handlers: one for success (Func<T, TResult>, 
  /// gets the data) and one for failure (Func<MessageCollection, TResult>, gets the messages; overload adds
  /// MessageType for severity). Executes the right one and returns the result of that handler (any type 
  /// TResult).
  /// 
  /// Why it's useful: Avoids if-else boilerplate for "do X on success, Y on failure." It's exhaustive (forces 
  /// handling both cases) and composable—great for converting Results to other types (e.g., HTTP responses, UI
  /// states) without throwing.
  /// </summary>
  /// <example>
  /// 
  /// var result = GetUserById(999);  // Failure: "User not found"
  /// 
  /// string response = result.Match( onSuccess: user => $"Welcome, {user.Name}!",
  ///                                 onFailure: msgs => $"Error: {msgs.Errors.First().Text}" ); // Or overload: (msgs, severity) => ...
  /// 
  /// RESULT NOTE: 
  ///   response = "Error: User not found"  // If success, it'd be "Welcome, Alice!"
  /// </example>
  public static TResult Match<T, TResult>(this Result<T> result,
                                          Func<T, TResult> onSuccess,
                                          Func<MessageCollection, TResult> onFailure)
  {
    return result.IsSuccess && result.Data is not null
            ? onSuccess(result.Data)
            : onFailure(result.Messages);
  }

  /// <summary>
  /// Overload: Includes highest severity in failure handler.
  /// </summary>
  public static TResult Match<T, TResult>(this Result<T> result,
                                          Func<T, TResult> onSuccess,
                                          Func<MessageCollection, MessageType, TResult> onFailure)
  {
    return result.IsSuccess && result.Data is not null
            ? onSuccess(result.Data)
            : onFailure(result.Messages, result.Messages.HighestSeverity);
  }

  #endregion

  #region Convenience (e.g., ToResult, EnsureSuccess)

  /// <summary>
  /// What it does: If success, returns the Data (non-null). If failure, throws an 
  /// InvalidOperationException with the first error message's text (or "Unknown failure"). 
  /// It's a terminal method—no chaining after.
  /// 
  /// Why it's useful: In mixed paradigms (functional chaining then imperative code), this lets 
  /// you "unwrap" safely and crash early with context if something went wrong. It's like ! in 
  /// Rust or get() in Option<T>—use sparingly, only when you must have the data and failures 
  /// are truly exceptional.
  /// </summary>
  /// <example>
  /// 
  /// var result = GetUserById(123).Bind(ProcessUser);  // Assume success: User { ... }
  /// var user   = result.EnsureSuccess();              // Returns the User; throws if failure 
  ///                                                   // with e.g., "User not found"
  /// 
  /// RESULT NOTE:
  ///   Now use 'user' imperatively: Console.WriteLine(user.Name);  // No more Result handling needed.
  /// </example>
  public static T EnsureSuccess<T>(this Result<T> result)
  {
    if (!result.IsSuccess)
    {
      var errorMsg = result.Messages
                           .Errors
                           .FirstOrDefault()?.Text ?? "Unknown failure";
              
      throw new InvalidOperationException(errorMsg);
    }

    return result.Data!;
  }

  /// <summary>
  /// What it does: Converts a generic Result<T> to a non-generic Result (loses the Data type). 
  /// On success, it's Result.Success with the messages; on failure, Result.Failure with messages. 
  /// Data is discarded.
  /// 
  /// Why it's useful: Sometimes you need a uniform Result (no generics) for APIs, logging, or mixing 
  /// with non-generic flows (e.g., a handler that takes any Result). Since your pattern has both 
  /// Result and Result<T>, this bridges them without duplicating code. (You're spot-on questioning 
  /// it—if everything returns Result<T>, you might rarely need this; it's more for legacy/uniformity.)
  /// </summary>
  /// <example>
  /// 
  /// Result<T> typedResult = GetUserById(123);       // Has Data: User
  /// Result genericResult  = typedResult.ToResult(); // Now just Result, with Messages but no Data
  /// 
  /// RESULT NOTE:
  ///   Use genericResult in a method expecting Result: LogResult(genericResult); 
  ///   Logs messages regardless of T.
  /// </example>
  public static Result ToResult<T>(this Result<T> result)
      => result.IsSuccess ? Result.Success(result.Messages)
                          : Result.Failure(result.Messages);

  #endregion
}
