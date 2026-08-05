namespace APHI.Core.Models;

/// <summary>
/// Categories of issues that the health inspector can detect.
/// </summary>
public enum IssueCategory
{
    BrokenPath,
    DuplicateLayer,
    ProjectionInconsistency,
    EmptyDataset,
    LargeDataset,
    SpatialIndex,
    BrokenJoin,
    LabelIssue,
    SymbologyIssue,
    DefinitionQuery,
    NetworkPath,
    RelativePath,
    RasterOptimization,
    LayerNaming,
    Performance,
    Metadata
}
