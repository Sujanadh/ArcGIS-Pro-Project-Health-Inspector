# Developer Guide

Welcome to the APHI developer guide! This document explains how to set up your environment, build the project, and contribute new features like custom analyzers.

## Environment Setup
1. Install **Visual Studio 2022**.
2. Install **ArcGIS Pro 3.3** (or later).
3. Install the **ArcGIS Pro SDK for .NET** via the Visual Studio Installer (under "Other Toolsets") or download the VSIX from My Esri.
4. Ensure the **.NET 8.0 SDK** is installed.

## Building and Debugging
1. Open the solution file `APHI.sln` in Visual Studio.
2. Set the `APHI` project as the Startup Project.
3. The project is configured to launch ArcGIS Pro when you start debugging (F5).
4. If ArcGIS Pro fails to launch, verify the `Start Action` in the project properties points to your `ArcGISPro.exe` installation path.

## Adding a New Analyzer

To add a new health check to APHI, you need to implement the `IAnalyzer` interface.

1. Create a new class in the `Analyzers` folder.
2. Implement `IAnalyzer`:
   ```csharp
   public class MyCustomAnalyzer : IAnalyzer
   {
       public string Id => "MY_ANALYZER_01";
       public string Name => "My Custom Health Check";
       public string Category => "Map";
       public string Description => "Checks for a specific condition in maps.";
       public bool SupportsAutoFix => false;

       public async Task<AnalysisResult> AnalyzeAsync(ProjectContext context, CancellationToken token)
       {
           // Implement analysis logic here
           // Use the context to access Project, Maps, Layouts, etc.
           
           return new AnalysisResult {
               AnalyzerId = Id,
               Status = ResultStatus.Passed,
               Message = "All good!"
           };
       }
   }
   ```
3. Register the analyzer in the `AnalysisEngine` or Dependency Injection container (if used).

## Coding Conventions
- Use `async/await` for all operations involving the ArcGIS Pro API on the QueuedTask.
- Always use `QueuedTask.Run()` when interacting with ArcGIS Pro project items.
- Avoid blocking the main UI thread.
- Write unit tests for your analyzer logic where possible.
