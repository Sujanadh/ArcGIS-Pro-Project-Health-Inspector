using System;
using System.Collections.Generic;
using System.Linq;
using APHI.Core.Models;

namespace APHI.Analysis;

/// <summary>
/// Calculates the overall health score of an ArcGIS Pro Project based on a list of health issues.
/// </summary>
public class HealthScoreCalculator
{
    /// <summary>
    /// Calculates the overall project health score and grade.
    /// </summary>
    /// <param name="issues">The list of detected health issues.</param>
    /// <param name="settings">The project settings, which may contain category weights (mocked as null/default here).</param>
    /// <returns>A tuple containing the numeric score [0,100] and the text grade.</returns>
    public (int Score, string Grade) CalculateScore(List<HealthIssue> issues, object settings = null)
    {
        int totalScore = 100;
        
        // Dictionary to track deductions per category so we can cap them at 20
        var categoryDeductions = new Dictionary<IssueCategory, int>();

        foreach (var category in Enum.GetValues(typeof(IssueCategory)).Cast<IssueCategory>())
        {
            categoryDeductions[category] = 0;
        }

        foreach (var issue in issues)
        {
            int deduction = issue.Severity switch
            {
                IssueSeverity.Critical => 10,
                IssueSeverity.High => 5,
                IssueSeverity.Medium => 2,
                IssueSeverity.Low => 1,
                IssueSeverity.Information => 0,
                _ => 0
            };

            categoryDeductions[issue.Category] += deduction;
        }

        foreach (var kvp in categoryDeductions)
        {
            // Cap deduction per category at 20
            int cappedDeduction = Math.Min(kvp.Value, 20);
            totalScore -= cappedDeduction;
        }

        // Clamp final score
        totalScore = Math.Clamp(totalScore, 0, 100);

        string grade = GetGrade(totalScore);

        return (totalScore, grade);
    }

    /// <summary>
    /// Converts a numeric score to a grade string.
    /// </summary>
    /// <param name="score">The numeric score [0,100].</param>
    /// <returns>The grade string.</returns>
    public string GetGrade(int score)
    {
        if (score >= 90) return "Excellent";
        if (score >= 75) return "Good";
        if (score >= 60) return "Fair";
        if (score >= 40) return "Needs Attention";
        return "Poor";
    }

    /// <summary>
    /// Calculates performance metrics based solely on performance-category issues.
    /// </summary>
    /// <param name="performanceIssues">List of performance issues.</param>
    /// <returns>An object representing performance metrics (if needed).</returns>
    public object CalculatePerformanceScore(List<HealthIssue> performanceIssues)
    {
        // Compute an aggregate or summary object. Returning generic object as PerformanceMetrics model might vary.
        int score = 100;
        foreach (var issue in performanceIssues.Where(i => i.Category == IssueCategory.Performance))
        {
            if (issue.Severity == IssueSeverity.High) score -= 15;
            else if (issue.Severity == IssueSeverity.Medium) score -= 5;
            else if (issue.Severity == IssueSeverity.Low) score -= 2;
        }

        return new 
        {
            OverallPerformanceScore = Math.Clamp(score, 0, 100),
            IssueCount = performanceIssues.Count
        };
    }
}
