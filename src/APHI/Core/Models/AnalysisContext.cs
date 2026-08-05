using System.Collections.Generic;
using System.Threading;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Mapping;

namespace APHI.Core.Models;

/// <summary>
/// Context passed to analyzers during the scanning process.
/// </summary>
public class AnalysisContext
{
    /// <summary>
    /// The current ArcGIS Pro project.
    /// </summary>
    public Project Project { get; set; }

    /// <summary>
    /// A list of maps within the project to analyze.
    /// </summary>
    public IReadOnlyList<Map> Maps { get; set; }

    /// <summary>
    /// The current settings for the health inspector.
    /// </summary>
    public ProjectSettings Settings { get; set; }

    /// <summary>
    /// Token to monitor for cancellation requests.
    /// </summary>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalysisContext"/> class.
    /// </summary>
    /// <param name="project">The ArcGIS Pro project.</param>
    /// <param name="maps">The maps in the project.</param>
    /// <param name="settings">The project settings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public AnalysisContext(Project project, IReadOnlyList<Map> maps, ProjectSettings settings, CancellationToken cancellationToken)
    {
        Project = project;
        Maps = maps;
        Settings = settings;
        CancellationToken = cancellationToken;
    }
}
