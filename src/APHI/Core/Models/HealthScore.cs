using System.Collections.Generic;

namespace APHI.Core.Models;

/// <summary>
/// Represents the calculated health score for a project.
/// </summary>
public class HealthScore
{
    /// <summary>
    /// The overall score from 0 to 100.
    /// </summary>
    public int OverallScore { get; set; } = 100;

    /// <summary>
    /// The letter grade for the project (e.g., A, B, C).
    /// </summary>
    public string Grade => GetGrade();

    /// <summary>
    /// Scores broken down by issue category.
    /// </summary>
    public Dictionary<IssueCategory, int> CategoryScores { get; set; } = new Dictionary<IssueCategory, int>();

    /// <summary>
    /// The weighted scores used to calculate the overall score.
    /// </summary>
    public Dictionary<IssueCategory, double> WeightedScores { get; set; } = new Dictionary<IssueCategory, double>();

    /// <summary>
    /// Gets the string representation of the grade based on the overall score.
    /// </summary>
    /// <returns>A descriptive grade string.</returns>
    public string GetGrade()
    {
        if (OverallScore >= 90) return "Excellent";
        if (OverallScore >= 80) return "Good";
        if (OverallScore >= 70) return "Fair";
        if (OverallScore >= 60) return "Needs Attention";
        return "Poor";
    }
}
