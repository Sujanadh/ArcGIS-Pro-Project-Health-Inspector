using System;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using APHI.Core.Services;
using APHI.Utilities;

namespace APHI
{
    /// <summary>
    /// The main add-in module for the Project Health Inspector.
    /// </summary>
    internal class Module1 : Module
    {
        private static Module1 _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static Module1 Current => _this ??= (Module1)FrameworkApplication.FindModule("APHI_Module");

        /// <summary>
        /// The service locator for the add-in.
        /// </summary>
        public ServiceLocator Services { get; private set; }

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro loads this module.
        /// </summary>
        /// <returns>A boolean indicating success</returns>
        protected override bool Initialize()
        {
            try
            {
                // Initialize the service locator and dependencies
                Services = ServiceLocator.Current;
                
                // Initialize logging
                var logManager = Services.GetInstance<LogManager>();
                logManager?.LogInfo("APHI Module initializing...");
                
                return base.Initialize();
            }
            catch (Exception ex)
            {
                // Core initialization failed
                System.Diagnostics.Debug.WriteLine($"APHI Module initialization failed: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        protected override void Uninitialize()
        {
            try
            {
                var logManager = Services?.GetInstance<LogManager>();
                logManager?.LogInfo("APHI Module uninitializing...");
            }
            catch
            {
                // Ignore exceptions during shutdown
            }
            finally
            {
                base.Uninitialize();
            }
        }
        #endregion
    }
}
