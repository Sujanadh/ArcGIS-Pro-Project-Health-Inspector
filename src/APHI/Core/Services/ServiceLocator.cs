using System;
using System.Collections.Generic;
using APHI.Core.Models;
using APHI.Utilities;

namespace APHI.Core.Services;

/// <summary>
/// Simple service locator for managing dependencies in the ArcGIS Pro add-in environment.
/// </summary>
public class ServiceLocator
{
    private static ServiceLocator? _instance;
    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    /// <summary>
    /// Gets the singleton instance of the ServiceLocator.
    /// </summary>
    public static ServiceLocator Current => _instance ??= new ServiceLocator();

    private ServiceLocator()
    {
        InitializeServices();
    }

    /// <summary>
    /// Initializes and registers all core services.
    /// </summary>
    private void InitializeServices()
    {
        // 1. Basic utilities
        var logManager = new LogManager();
        Register(logManager);

        var settings = ProjectSettings.Load();
        Register(settings);

        // 2. Engines
        var analysisEngine = new AnalysisEngine(logManager);
        // Ensure standard analyzers are registered
        analysisEngine.RegisterAllAnalyzers();
        Register(analysisEngine);

        // 3. Orchestrators
        var projectScanner = new ProjectScanner(analysisEngine, logManager);
        Register(projectScanner);

        // Note: ReportManager and AutoFixEngine could also be instantiated and registered here.
    }

    /// <summary>
    /// Registers a service instance.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    /// <param name="service">The service instance.</param>
    public void Register<T>(T service) where T : class
    {
        _services[typeof(T)] = service;
    }

    /// <summary>
    /// Resolves a service instance by type.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    /// <returns>The service instance, or null if not registered.</returns>
    public T? GetInstance<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out var service))
        {
            return service as T;
        }
        return null;
    }
}
