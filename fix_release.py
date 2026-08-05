import re

with open('.github/workflows/release.yml', 'r') as f:
    content = f.read()

# Update Verify step
content = content.replace('"src/APHI/bin/Release"', '"src/APHI/bin"')
content = content.replace('"$outputDir/ProjectHealthInspector.esriAddinX"', '"$outputDir/ProjectHealthInspector.esriAddinX"') # Same because outputDir is now bin
# Update files in Create GitHub Release
content = content.replace('src/APHI/bin/Release/ProjectHealthInspector.esriAddinX', 'src/APHI/bin/ProjectHealthInspector.esriAddinX')

with open('.github/workflows/release.yml', 'w') as f:
    f.write(content)

print("release.yml fixed")
