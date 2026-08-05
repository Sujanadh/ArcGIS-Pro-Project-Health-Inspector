import re
with open('src/APHI/Analysis/LabelAnalyzer.cs', 'r') as f:
    lines = f.readlines()

for i, line in enumerate(lines):
    if line.strip() == '});' and lines[i-1].strip() == '}':
        # wait this is too brittle. Let's just remove the 5th bracket.
        pass

# Find "});"
# then there should be exactly 3 brackets before "});" not 4.
idx = -1
for i, line in enumerate(lines):
    if line.strip() == '});' and 'QueuedTask.Run' not in lines[i-1]:
        if lines[i-1].strip() == '}' and lines[i-2].strip() == '}' and lines[i-3].strip() == '}':
            idx = i - 1
            break

if idx != -1:
    print(f"Removing line {idx}: {lines[idx]}")
    lines.pop(idx)
    with open('src/APHI/Analysis/LabelAnalyzer.cs', 'w') as f:
        f.writelines(lines)
else:
    print("Could not find the extra bracket.")
