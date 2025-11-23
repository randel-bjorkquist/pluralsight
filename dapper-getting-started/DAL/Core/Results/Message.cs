namespace DAL.Core.Results;

public class Message
{
    public MessageType Type   { get; set; }
    public string Text        { get; set; } = string.Empty;
    public string? Code       { get; set; } //Optional Code (e.g., "VALIDATION_PHAID")    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Factories for convenience (immutable-ish via new)
    public static Message NotFound(string text, string? code = null, DateTime? now = null)
    {
        text ??= string.Empty;
        return new() { Type = MessageType.NotFound
                      ,Text = $"{(text.StartsWith("NOT FOUND:") ? text.Substring(text.IndexOf(":") + 2).Trim() : text?.Trim() ?? string.Empty)}"
                      ,Code = code 
                      ,Timestamp = now ?? DateTime.UtcNow };
    }

    public static Message Error(string text, string? code = null, DateTime? now = null)
    {
        text ??= string.Empty;
        return new() { Type = MessageType.Error
                      ,Text = $"{(text.StartsWith("ERROR:") ? text.Substring(text.IndexOf(":") + 2).Trim() : text?.Trim() ?? string.Empty)}"
                      ,Code = code 
                      ,Timestamp = now ?? DateTime.UtcNow };
    }

    public static Message Information(string text, string? code = null, DateTime? now = null)
    { 
        text ??= string.Empty;
        return new() { Type = MessageType.Information
                      ,Text = $"{(text.StartsWith("INFORMATION:") ? text.Substring(text.IndexOf(":") + 2).Trim() : text?.Trim() ?? string.Empty)}"
                      ,Code = code 
                      ,Timestamp = now ?? DateTime.UtcNow };
    }

    public static Message Success(string text, string? code = null, DateTime? now = null)
    { 
        text ??= string.Empty;
        return new() { Type = MessageType.Success
                      ,Text = $"{(text.StartsWith("SUCCESS:") ? text.Substring(text.IndexOf(":") + 2).Trim() : text?.Trim() ?? string.Empty)}"
                      ,Code = code 
                      ,Timestamp = now ?? DateTime.UtcNow };
    }

    public static Message Warning(string text, string? code = null, DateTime? now = null)
    {
        text ??= string.Empty;
        return new() { Type = MessageType.Warning
                      ,Text = $"{(text.StartsWith("WARNING:") ? text.Substring(text.IndexOf(":") + 2).Trim() : text?.Trim() ?? string.Empty)}"
                      ,Code = code 
                      ,Timestamp = now ?? DateTime.UtcNow };
    }
      
    public override string ToString() 
    {
        var prefix    = Type.ToString().ToUpper();
        var text      = Text.StartsWith(prefix) ? Text.Substring(Text.IndexOf(":") + 2) : Text;
        var code      = Code ?? "N/A";
        var timestamp = $"{Timestamp:yyyy-MM-dd HH:mm:ss}";

        return $"{prefix}: {text} ({code}) at {timestamp}";
    }

    //public override string ToString() 
    //  => $"{Text} ({Code ?? "N/A"}) at {Timestamp:yyyy-MM-dd HH:mm:ss}";
    //  => $"{Type.ToString().ToUpper()}: {(Text.StartsWith(Type.ToString().ToUpper()) ? Text[(Text.IndexOf(":") + 2)..] : Text)} ({Code ?? "N/A"}) at {Timestamp:yyyy-MM-dd HH:mm:ss}";
}
