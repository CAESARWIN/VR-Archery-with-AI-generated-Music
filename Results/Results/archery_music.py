import asyncio
import threading
import sys
from playwright.async_api import async_playwright

trigger_points = {15, 10, 5}
last_count = [None]

prompts = [
    "cheerful and playful orchestral music with soft drums and light melodies",
    "cinematic background music with steady rhythm and a sense of challenge",
    "intense cinematic buildup with fast percussion and escalating strings",
    "fast-paced, dramatic orchestral music with rapid strings, intense percussion, and a sense of urgency"
]

async def mute_logic(page, active_index):
    buttons = await page.query_selector_all("button.muteButton")

    target_btn = buttons[active_index]
    icon = await target_btn.query_selector("i.google-symbols")
    text = await icon.inner_text()
    if "no_sound" in text:
        await target_btn.click()
        await asyncio.sleep(0.3)

    for idx, btn in enumerate(buttons):
        if idx != active_index:
            icon = await btn.query_selector("i.google-symbols")
            text = await icon.inner_text()
            if "volume_mute" in text:
                await btn.click()
    print(f"Switched to prompt {active_index + 1}, others muted.")

async def generate_all_prompts(page):
    delete_icons = await page.query_selector_all("i.google-symbols >> text=delete")
    for icon in delete_icons:
        await icon.click()
    print(f"Cleared {len(delete_icons)} style cards")

    for prompt in prompts:
        input_box = await page.query_selector("#addPrompt")
        await input_box.fill(prompt)
        add_btn = await page.query_selector("i.google-symbols >> text=add_circle")
        await add_btn.click()
        await asyncio.sleep(0.5)

    await mute_logic(page, 0)

    play_btn = await page.query_selector("#playButton")
    await play_btn.click()
    print("Started playback")

def stdin_reader(loop, page):
    for line in sys.stdin:
        line = line.strip()
        if not line:
            continue
        try:
            count = int(line)
            print(f"Remaining arrows: {count}")

            if count in trigger_points and last_count[0] not in trigger_points:
                idx = {15:1, 10:2, 5:3}.get(count, 0)
                asyncio.run_coroutine_threadsafe(mute_logic(page, idx), loop)

            last_count[0] = count
        except ValueError:
            print(f"Invalid input: {line}")

async def connect_to_musicfx():
    playwright = await async_playwright().start()
    browser = await playwright.chromium.connect_over_cdp("http://localhost:9222")
    context = browser.contexts[0]
    for page in context.pages:
        if "music-fx-dj" in page.url:
            print("Connected to MusicFX DJ")
            return page
    return None

async def main():
    page = await connect_to_musicfx()
    if not page:
        return

    await generate_all_prompts(page)

    loop = asyncio.get_running_loop()
    threading.Thread(target=stdin_reader, args=(loop, page), daemon=True).start()

    while True:
        await asyncio.sleep(1)

if __name__ == "__main__":
    asyncio.run(main())
