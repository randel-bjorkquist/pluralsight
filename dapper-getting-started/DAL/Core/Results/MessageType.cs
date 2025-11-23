namespace DAL.Core.Results;

public enum MessageType : short
{
    NotFound    = 0,  // Neutral : Expected absence       (e.g., "No record found")
    Success     = 1,  // Lowest  : Positive Confirmations (e.g., "Created successfully")
    Information = 2,  // Low     : Neutral Information    (e.g., "Entity created successfully")
    Warning     = 3,  // Medium  : Non-blocking cautions  (e.g., "PhaId ignored for non-scoped op")
    Error       = 4,  // Highest : Blocking errors        (e.g., "Invalid PhaId")
}
