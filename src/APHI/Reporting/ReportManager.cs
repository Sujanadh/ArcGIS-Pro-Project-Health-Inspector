using System;
using System.IO;
using System.Threading.Tasks;
using APHI.Core.Models;

namespace APHI.Reporting
{
    /// <summary>
    /// Supported formats for the health report.
    /// </summary>
    public enum ReportFormat
    {
        /// <summary>HTML Format</summary>
        Html,
        /// <summary>CSV Format</summary>
        Csv,
        /// <summary>JSON Format</summary>
        Json,
        /// <summary>Plain Text Format</summary>
        Text
    }

    /// <summary>
    /// Interface for report generators.
    /// </summary>
    public interface IReportGenerator
    {
        /// <summary>
        /// Generates a report from the provided HealthReport model.
        /// </summary>
        /// <param name="report">The health report data.</param>
        /// <returns>The generated report content as a string.</returns>
        string Generate(HealthReport report);
    }

    /// <summary>
    /// Manages the generation and saving of health reports.
    /// </summary>
    public class ReportManager
    {
        /// <summary>
        /// Generates a report in the specified format and saves it to the given file path.
        /// </summary>
        /// <param name="report">The health report data.</param>
        /// <param name="outputPath">The file path where the report will be saved.</param>
        /// <param name="format">The desired format of the report.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SaveReportAsync(HealthReport report, string outputPath, ReportFormat format)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            if (string.IsNullOrWhiteSpace(outputPath)) throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));

            IReportGenerator generator = GetGenerator(format);
            string content = generator.Generate(report);

            string directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (StreamWriter writer = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8))
            {
                await writer.WriteAsync(content).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Returns the appropriate generator for the given format.
        /// </summary>
        /// <param name="format">The report format.</param>
        /// <returns>An instance of a report generator.</returns>
        private IReportGenerator GetGenerator(ReportFormat format)
        {
            switch (format)
            {
                case ReportFormat.Html:
                    return new HtmlReportGenerator();
                case ReportFormat.Csv:
                    return new CsvReportGenerator();
                case ReportFormat.Json:
                    return new JsonReportGenerator();
                case ReportFormat.Text:
                    return new TextReportGenerator();
                default:
                    throw new NotSupportedException($"Report format {format} is not supported.");
            }
        }
    }
}
