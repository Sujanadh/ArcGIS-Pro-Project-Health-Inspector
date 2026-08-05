# Troubleshooting Guide

This document covers common issues you might encounter while using or installing APHI.

## Installation Issues

**Issue:** Add-in does not appear in ArcGIS Pro after installation.
**Solution:**
1. Verify the Add-in is enabled in ArcGIS Pro: Go to **Project** > **Add-In Manager**. Look for APHI and ensure it's not disabled.
2. Check your ArcGIS Pro version. APHI requires version 3.3 or higher.
3. Ensure the `.esriAddinX` file was installed in the correct well-known folder (usually `C:\Users\[User]\Documents\ArcGIS\AddIns\ArcGISPro`).

**Issue:** "Unsupported ArcGIS Pro version" during installation.
**Solution:** Update your ArcGIS Pro installation to 3.3 or later.

## Analysis Issues

**Issue:** Analysis takes a very long time to complete.
**Solution:**
1. Projects with hundreds of map frames or web layers can take longer to analyze.
2. Try disabling the "Network Drive Latency" and "Slow Web Services" analyzers in Options, as these perform actual network calls which can block.

**Issue:** Auto-Fix fails to fix a broken link.
**Solution:**
Auto-Fix relies on heuristic searches in known folder connections. If the missing data is not located within any registered folder connections in the project, Auto-Fix cannot locate it. Manually repair the link and add the folder connection.

## General Issues

**Issue:** Dockpane is completely blank or throws an exception.
**Solution:**
This usually indicates a .NET dependency issue. Ensure the .NET 8 Desktop Runtime is fully installed and up to date on your system.

**Issue:** Report generation fails.
**Solution:**
Check if the destination directory is read-only or if a previous report file is currently open in another application (like Excel). Change the output path in the configuration settings.
