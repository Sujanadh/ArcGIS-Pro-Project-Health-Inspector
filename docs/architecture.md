# Architecture Overview

This document describes the high-level architecture of the ArcGIS Pro Project Health Inspector (APHI).

## System Architecture

APHI is built as an ArcGIS Pro Add-in using the ArcGIS Pro SDK for .NET and WPF for the UI. It follows the MVVM (Model-View-ViewModel) design pattern.

```mermaid
classDiagram
    class AnalysisEngine {
        +RunAnalyzers()
        +Cancel()
    }
    class IAnalyzer {
        <<interface>>
        +Category: string
        +Name: string
        +Analyze(ProjectContext): AnalysisResult
    }
    class AutoFixEngine {
        +ApplyFix(AnalysisResult)
        +Rollback()
    }
    class ReportGenerator {
        +ExportHtml()
        +ExportCsv()
        +ExportJson()
    }
    class DockpaneViewModel {
        +HealthScore: int
        +Results: List~AnalysisResult~
        +StartAnalysisCommand: ICommand
    }
    
    AnalysisEngine --> IAnalyzer : executes
    DockpaneViewModel --> AnalysisEngine : triggers
    DockpaneViewModel --> AutoFixEngine : invokes
    DockpaneViewModel --> ReportGenerator : invokes
```

## Component Flow

```mermaid
graph LR
    A[ArcGIS Pro UI] --> B[Dockpane View]
    B --> C[Dockpane ViewModel]
    C --> D[Analysis Engine]
    D --> E[Data Analyzers]
    D --> F[Map Analyzers]
    D --> G[Layout Analyzers]
    E --> H[Results Collection]
    F --> H
    G --> H
    H --> I[Scoring Engine]
    I --> C
```

## Auto-Fix Workflow

The Auto-Fix workflow ensures safety by providing preview, confirmation, and rollback capabilities.

```mermaid
sequenceDiagram
    actor User
    participant ViewModel
    participant AutoFixEngine
    participant ArcGISProAPI
    
    User->>ViewModel: Click Auto-Fix
    ViewModel->>AutoFixEngine: PreviewFix(Result)
    AutoFixEngine-->>ViewModel: FixDetails
    ViewModel-->>User: Show Confirmation Dialog
    User->>ViewModel: Confirm
    ViewModel->>AutoFixEngine: ExecuteFix(Result)
    AutoFixEngine->>ArcGISProAPI: Create Save Point
    AutoFixEngine->>ArcGISProAPI: Apply Changes
    alt Success
        AutoFixEngine-->>ViewModel: Success
    else Error
        AutoFixEngine->>ArcGISProAPI: Rollback to Save Point
        AutoFixEngine-->>ViewModel: Error Message
    end
    ViewModel-->>User: Update UI
```

## Analyzers Pipeline

The Analysis Engine runs analyzers asynchronously, aggregating results as they complete. Each analyzer implements the `IAnalyzer` interface and operates on a read-only snapshot of the project state to avoid cross-thread UI blocking.
