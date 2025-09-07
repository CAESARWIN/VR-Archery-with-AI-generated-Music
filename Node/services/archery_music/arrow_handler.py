# arrow_handler.py
import sys
# "C:\Program Files\Google\Chrome\Application\chrome.exe" ^
# --remote-debugging-port=9222 ^
# --user-data-dir="C:\temp\musicfx-profile"
print("[Python] Arrow handler started. Waiting for arrow count...")

for line in sys.stdin:
    line = line.strip()
    if not line:
        continue
    try:
        count = int(line)
        print(f"[Python] Remaining arrows: {count}")
    except ValueError:
        print(f"[Python] Invalid input: {line}")
