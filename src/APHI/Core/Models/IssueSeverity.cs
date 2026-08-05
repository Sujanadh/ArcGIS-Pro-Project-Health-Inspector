namespace APHI.Core.Models;

/// <summary>
/// Represents the severity level of a health issue found in an ArcGIS Pro project.
/// </summary>
public enum IssueSeverity
{
    /// <summary>
    /// Critical issues that severely impact project functionality (e.g., broken data links).
    /// </summary>
    Critical = 0,
    
    /// <summary>
    /// High severity issues that degrade performance or usability significantly.
    /// </summary>
    High = 1,
    
    /// <summary>
    /// Medium severity issues that represent poor practices or moderate performance hits.
    /// </summary>
    Medium = 2,
    
    /// <summary>
    /// Low severity issues that have minimal impact but could be improved.
    /// </summary>
    Low = 3,
    
    /// <summary>
    /// Informational notices about the project configuration.
    /// </summary>
    Information = 4
}
