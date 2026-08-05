using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using APHI.Models;

namespace APHI.Reporting
{
    /// <summary>
    /// Generates a JSON format health report.
    /// </summary>
    public class JsonReportGenerator : IReportGenerator
    {
        /// <summary>
        /// Generates a JSON report from the provided HealthReport model.
        /// </summary>
        /// <param name="report">The health report data.</param>
        /// <returns>The generated JSON content.</returns>
        public string Generate(HealthReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return JsonSerializer.Serialize(report, options);
        }
    }
}
