# Sample Data

This directory is intended to house sample ArcGIS Pro projects (`.aprx` files) and associated mock data (File Geodatabases, shapefiles) used for testing the APHI analyzers.

## Creating Sample Projects for Testing

When creating sample projects to test the add-in:

1. **Keep it small**: Do not commit multi-gigabyte datasets. Use small, clipped areas of interest.
2. **Intentional Errors**: Create specific projects designed to fail certain analyzers (e.g., `BrokenLinksProject.aprx` containing layers pointing to non-existent paths).
3. **Document the Expected Result**: In the commit message or a local text file, note what the Health Score *should* be for the sample project.

*(Note: Actual `.aprx` and `.gdb` files are ignored in Git via `.gitignore` to prevent repository bloat unless explicitly tracked using Git LFS).*
