using System.Threading.Tasks;
using APHI.Core.Models;

namespace APHI.Core.Interfaces;

/// <summary>
/// Interface for report generators.
/// </summary>
public interface IReportGenerator
{
    /// <summary>
    /// Gets the format of the report (e.g., "HTML", "JSON").
    /// </summary>
    string Format { get; }

    /// <summary>
    /// Gets the file extension for the report file (e.g., ".html").
    /// </summary>
    string FileExtension { get; }

    /// <summary>
    /// Generates a report based on the provided health report.
    /// </summary>
    /// <param name="report">The health report data.</param>
    /// <param name="outputDirectory">The directory to save the report.</param>
    /// <returns>The path to the generated report file.</returns>
    Task<string> GenerateAsync(HealthReport report, string outputDirectory);
}
