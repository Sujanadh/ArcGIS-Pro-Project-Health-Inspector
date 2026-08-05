# Configuration Guide

APHI is highly configurable, allowing you to tailor the health checks to your organization's specific needs.

## Accessing Settings
Click the **Options** button (gear icon) on the Health Inspector dockpane to open the configuration dialog.

## General Settings
| Setting | Description | Default | Valid Range |
|---------|-------------|---------|-------------|
| **Auto-Run on Open** | Run analysis automatically when a project opens. | False | True/False |
| **Theme** | Dockpane UI theme (Light, Dark, System). | System | Light, Dark, System |

## Analyzer Settings

### Performance Thresholds
| Setting | Description | Default | Valid Range |
|---------|-------------|---------|-------------|
| **Slow Service Warning** | Response time threshold for web services before warning. | 2000 ms | 500 - 10000 ms |
| **Max GDB Size** | File Geodatabase size threshold for bloat warning. | 500 MB | 100 - 5000 MB |
| **Max Label Count** | Maximum number of labels to render before warning. | 1000 | 100 - 10000 |

### Path Limits
| Setting | Description | Default | Valid Range |
|---------|-------------|---------|-------------|
| **Path Length Warning** | Warn when file paths exceed this length. | 240 chars | 100 - 260 chars |

### Analyzers to Run
You can enable or disable specific analyzers. Disabling analyzers you don't need can speed up the overall analysis process.
- [x] Broken Data Links
- [x] Unused Layers
- [x] Empty Groups
- [x] Coordinate System Mismatches
- [x] ... (All 20 analyzers can be toggled)

## Report Output
| Setting | Description | Default |
|---------|-------------|---------|
| **Default Export Path** | Default directory for saving reports. | `C:\Users\[User]\Documents\APHI_Reports` |
| **Include Timestamp** | Append date/time to report filenames. | True |
