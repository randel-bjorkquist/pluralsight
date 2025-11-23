using DAL.Core.Utilities;
using System.Collections;

namespace DAL.Core.Results;

public class MessageCollection : ICollection<Message>
{
  private readonly List<Message> _messages = new();
    
  public MessageCollection() { }
  public MessageCollection(MessageCollection messages) { _messages.AddRange(messages); }

  public int Count => _messages.Count;
  public bool IsReadOnly => false;
    
  public MessageType HighestSeverity => _messages.Any() 
                                      ? _messages.Max(m => m.Type) 
                                      : MessageType.Information;
    
  #region Single Add Method(s)

  public void Add(Exception ex, string? code = null)
  {
    if (ex == null)
      throw new ArgumentNullException(nameof(ex));
        
      Add(Message.Error(ex.Message, code));
  }
    
  public void Add(Message message)
  {
    if (message == null)
      throw new ArgumentNullException(nameof(message));

    _messages.Add(message);
  }

  //-----------------------------------------------------------
      
  public void AddNotFound(Exception ex, string? code = null)
    => AddNotFound(ex.Message, code);
    
  public void AddNotFound(string text, string? code = null)
    => Add(Message.NotFound(text, code));
    
  //-----------------------------------------------------------
    
  public void AddError(Exception ex, string? code = null)      
    => AddError(ex.Message, code);
    
  public void AddError(string text, string? code = null)
    => Add(Message.Error(text, code));
    
  //-----------------------------------------------------------

  public void AddInformation(Exception ex, string? code = null)
    => AddInformation(ex.Message, code);
    
  public void AddInformation(string text, string? code = null)
    => Add(Message.Information(text, code));

  //-----------------------------------------------------------

  public void AddSuccess(Exception ex, string? code = null)
    => AddSuccess(ex.Message, code);
      
  public void AddSuccess(string text, string? code = null)
    => Add(Message.Success(text, code));

  //-----------------------------------------------------------
      
  public void AddWarning(Exception ex, string? code = null)
    => AddWarning(ex.Message, code);
    
  public void AddWarning(string text, string? code = null)
    => Add(Message.Warning(text, code));

  #endregion Single Add Method(s)
    
  #region Range & Bulk Add Method(s)
    
  public void AddRange(MessageCollection? collection)
  {   
    if (collection.IsNullOrEmpty())
      return;

    _messages.AddRange(collection!);
  }

  public void AddRange(IEnumerable<Message>? messages)
  {
    if (messages == null)
      return;

    _messages.AddRange(messages);
  }
    
  //-----------------------------------------------------------
    
  public void AddNotFound(IEnumerable<string> errorTexts, string? code = null)
  {
    foreach (var text in errorTexts ?? Enumerable.Empty<string>())
    {
      AddNotFound(text, code);
    }
  }
    
  public void AddErrors(IEnumerable<string> errorTexts, string? code = null)
  {
    foreach (var text in errorTexts ?? Enumerable.Empty<string>())
    {
      AddError(text, code);
    }
  }

  public void AddInformations(IEnumerable<string> informationTexts, string? code = null)
  {
    foreach (var text in informationTexts ?? Enumerable.Empty<string>())
    {
      AddInformation(text, code);
    }
  }

  public void AddSuccesses(IEnumerable<string> successTexts, string? code = null)
  {
    foreach (var text in successTexts ?? Enumerable.Empty<string>())
    {
      AddSuccess(text, code);
    }
  }

  public void AddWarnings(IEnumerable<string> warningTexts, string? code = null)
  {
    foreach (var text in warningTexts ?? Enumerable.Empty<string>())
    {
      AddWarning(text, code);
    }
  }
    
  #endregion Range & Bulk Add Method(s)
    
  #region Queries Property(s) and Method(s)

  public bool HasErrors()
    => _messages.Any(m => m.Type == MessageType.Error);

  public bool HasInformations()
    => _messages.Any(m => m.Type == MessageType.Information);

  public bool HasNotFounds()
    => _messages.Any(m => m.Type == MessageType.NotFound);

  public bool HasSuccesses()
    => _messages.Any(m => m.Type == MessageType.Success);

  public bool HasWarnings()
    => _messages.Any(m => m.Type == MessageType.Warning);

  //-----------------------------------------------------------

  public IEnumerable<Message> Errors
    => _messages.Where(m => m.Type == MessageType.Error);

  public IEnumerable<Message> Informations
    => _messages.Where(m => m.Type == MessageType.Information);

  public IEnumerable<Message> NotFounds
    => _messages.Where(m => m.Type == MessageType.NotFound);

  public IEnumerable<Message> Successes
    => _messages.Where(m => m.Type == MessageType.Success);

  public IEnumerable<Message> Warnings 
    => _messages.Where(m => m.Type == MessageType.Warning);

  #endregion Queries Property(s) and Method(s)
    
  #region ICollection Implementation

  public void Clear()
    => _messages.Clear();
      
  public bool Contains(Message item)
    => _messages.Contains(item);
      
  public void CopyTo(Message[] array, int arrayIndex)
    => _messages.CopyTo(array, arrayIndex);
      
  public bool Remove(Message item)
    => _messages.Remove(item);

  public IEnumerator<Message> GetEnumerator()
    => _messages.GetEnumerator();
    
  IEnumerator IEnumerable.GetEnumerator()
    => GetEnumerator();

  #endregion ICollection Implementation
    
  public override string ToString() => _messages!.Join(Environment.NewLine) ?? string.Empty;
}
