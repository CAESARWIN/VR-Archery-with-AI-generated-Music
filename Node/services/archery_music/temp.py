import asyncio
import threading
import sys
from playwright.async_api import async_playwright

"C:\Program Files\Google\Chrome\Application\chrome.exe" ^
--remote-debugging-port=9222 ^
--user-data-dir="C:\temp\musicfx-profile"

# 触发生成音乐的箭矢数
trigger_points = {20, 15, 10, 5}
last_count = [None]

# 不同箭矢数对应的提示词
prompts = {
    20: "bright epic orchestral opening",
    15: "tense electronic countdown",
    10: "dark suspenseful music with deep bass",
    5: "slow dramatic strings with ambient textures"
}

# 用于与 MusicFX 交互的协程
async def generate_music(prompt, page):
    try:
        # 清除风格卡片
        delete_icons = await page.query_selector_all("i.google-symbols >> text=delete")
        for icon in delete_icons:
            await icon.click()
        print(f"Cleared {len(delete_icons)} style cards")

        # 输入提示词
        input_box = await page.query_selector("#addPrompt")
        if input_box:
            await input_box.fill(prompt)
            print(f"Entered prompt: {prompt}")
        else:
            print("Input box not found")
            return

        # 点击添加按钮
        add_btn = await page.query_selector("i.google-symbols >> text=add_circle")
        if add_btn:
            await add_btn.click()
            print("Clicked add button")
        else:
            print("Add button not found")
            return

        await asyncio.sleep(1)

        # 点击播放
        play_btn = await page.query_selector("#playButton")
        if play_btn:
            await play_btn.click()
            print("Clicked play button")
        else:
            print("Play button not found")

    except Exception as e:
        print(f"Error generating music: {e}")

# 启动 playwright 并连接 MusicFX 页面
async def connect_to_musicfx():
    playwright = await async_playwright().start()
    browser = await playwright.chromium.connect_over_cdp("http://localhost:9222")
    context = browser.contexts[0]
    for page in context.pages:
        if "music-fx-dj" in page.url:
            print("Connected to MusicFX DJ page")
            return page
    print("MusicFX DJ page not found")
    return None

# 在线程中监听 stdin，并在合适的节点触发生成音乐
def stdin_reader(loop, page):
    for line in sys.stdin:
        line = line.strip()
        if not line:
            continue
        try:
            count = int(line)
            print(f"Remaining arrows: {count}")

            if count in trigger_points and last_count[0] not in trigger_points:
                prompt = prompts.get(count, "dynamic orchestral music")
                print(f"Triggering music generation for count {count}")
                asyncio.run_coroutine_threadsafe(generate_music(prompt, page), loop)

            last_count[0] = count
        except ValueError:
            print(f"Invalid input: {line}")

# 主函数：启动连接并运行事件循环
async def main():
    page = await connect_to_musicfx()
    if not page:
        return

    # ★ 初始播放音乐（无需等待箭矢数量）
    initial_prompt = "bright epic orchestral opening"
    print(f"Auto-playing initial music: {initial_prompt}")
    await generate_music(initial_prompt, page)

    # 保持箭矢监听
    loop = asyncio.get_running_loop()
    thread = threading.Thread(target=stdin_reader, args=(loop, page), daemon=True)
    thread.start()

    while True:
        await asyncio.sleep(1)


if __name__ == "__main__":
    asyncio.run(main())
