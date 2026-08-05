# Known Limitations

We strive to make APHI as robust as possible, but there are certain limitations in the current architecture and the ArcGIS Pro API that users should be aware of.

## 1. Enterprise Geodatabase Connections
Analysis of Enterprise Geodatabases (SDE) is limited to checking connection string validity and basic versioning state. Deep analysis of SDE performance, index fragmentation, or database-side locks is outside the scope of APHI.

## 2. Auto-Fix Safety
While Auto-Fix creates a save point before executing, extremely large projects may experience a slight delay during the rollback process if an error occurs. 

## 3. Web Service Ping
The "Slow Web Service" analyzer checks the REST endpoint response time. It does not perform a deep load test or evaluate the rendering speed of complex map services. It acts as a basic "health check" ping.

## 4. Locked Files
If a layer references a file (e.g., a shapefile) that is exclusively locked by another application, APHI may not be able to fully read its metadata, potentially leading to incomplete analysis results for that specific layer.

## 5. Script Tools and ModelBuilder
APHI analyzes the maps, layers, and layouts of a project. It currently does not parse or evaluate the efficiency of custom Python scripts or ModelBuilder models stored in the project's default toolbox.

## 6. Layout element drawing
Determining if layout elements are perfectly aligned or overlapping is based on bounding boxes. Complex, non-rectangular symbols might have bounding boxes that overlap even if the visual elements do not.
