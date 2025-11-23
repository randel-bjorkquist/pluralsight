namespace DAL.Core.Results;

public abstract class Result
{
    public bool IsSuccess { get; protected set; }

    public MessageCollection Messages { get; protected set; } = new();  // Always non-null

    protected Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    // Factories (public)
    public static Result Success(MessageCollection? messages = null)
    {
        messages ??= [];
        return new SuccessResult(messages);
    }

    public static Result Failure(MessageCollection messages)
    {
        if (messages == null) 
            throw new ArgumentNullException(nameof(messages));
        
        if (!messages.HasErrors() && !messages.HasNotFounds())
            throw new ArgumentException("Any non-NotFound Failure requires at least one error.", nameof(messages));
        
        return new FailureResult(messages);
    }

    // Internal subclasses (accessible via factories)
    internal class SuccessResult : Result
    {
        public SuccessResult(MessageCollection messages) : base(true)
        {
            Messages = messages;
        }
    }

    internal class FailureResult : Result
    {
        public FailureResult(MessageCollection messages) : base(false)
        {
            Messages = messages;
        }
    }
}

public class Result<T> : Result 
{
    public new bool IsSuccess => base.IsSuccess;

    public T? Data { get; protected set; } //NOTE: null for failures/not-founds

    protected Result(bool isSuccess) : base(isSuccess) { }
    protected Result(Result<T> result) : base(result.IsSuccess) 
    {
        Data = result.Data;
        Messages = new MessageCollection(result.Messages);
    }

    #region Factories (public, handle null coalescing)

    public static Result<T> Success(T? value, MessageCollection? messages = null)
    {
        messages ??= [];
        return new SuccessResult<T>(value, messages); // Private subclass wrapper
    }

    public static Result<T> Success(MessageType type, T? value, string message, string? code = null)
    {
        return new SuccessResult<T>(value, FromType(type, message, code));
    }

    public static new Result<T> Failure(MessageCollection messages)
    {
        if (messages == null) 
            throw new ArgumentNullException(nameof(messages));

        if (!messages.HasErrors() && !messages.HasNotFounds())
            throw new ArgumentException("Any non-NotFound Failure requires at least one error.", nameof(messages));
        
        return new FailureResult<T>(messages);
    }

    public static Result<T> Failure(MessageType type, string message, string? code = null)
    {
        return new FailureResult<T>(FromType(type, message, code));
    }

    #endregion

    // Internal subclasses
    private class SuccessResult<TData> : Result<TData>
    {
        public SuccessResult(TData? data, MessageCollection messages) : base(true)
        {
            Data      = data;
            Messages  = messages;
        }
    }

    private class FailureResult<TData> : Result<TData>
    {
        public FailureResult(MessageCollection messages) : base(false)
        {
            Data      = default;
            Messages  = messages;
        }
    }

    private static MessageCollection FromType(MessageType type, string message, string? code = null)
    {
        var messages = new MessageCollection();

        switch(type)
        {
            case MessageType.NotFound:    messages.AddNotFound(message, code);    break;
            case MessageType.Success:     messages.AddSuccess(message, code);     break;
            case MessageType.Information: messages.AddInformation(message, code); break;
            case MessageType.Warning:     messages.AddWarning(message, code);     break;
            case MessageType.Error:       messages.AddError(message, code);       break;
            default:                      messages.AddError(message, code);       break;
        }

        return messages;
    }
}
