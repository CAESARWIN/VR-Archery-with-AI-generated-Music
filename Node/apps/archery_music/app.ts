import { ApplicationController } from '../../components/application';
import { ArcheryMusicService } from '../../services/archery_music/service';
import path from 'path';
import { NetworkId } from 'ubiq';
import { fileURLToPath } from 'url';

class ArcheryMusicApplication extends ApplicationController {
    constructor(configPath: string) {
        super(configPath);
    }

    start(): void {
        // STEP 1: Register all services needed by the application
        this.registerComponents();
        this.log(`Services registered: ${Object.keys(this.components).join(', ')}`);

        // STEP 2: Set up message passing and processing
        this.definePipeline();
        this.log('Pipeline defined');

        // STEP 3: Connect to the room
        this.joinRoom();
    }

    registerComponents(): void {
        // 注册你自定义的 archery_music 服务
        this.components.arrowService = new ArcheryMusicService(this.scene);
    }

    definePipeline(): void {
        // 监听子进程（如 Python）返回的数据
        this.components.arrowService.on('data', (data: string) => {
            this.log(`Arrow service output: ${data}`);

            // 可选：将数据广播回 Unity（如果你想）
            this.scene.send(new NetworkId(120), {
                result: data.toString()
            });
        });
    }
}

// 如果当前是入口文件，则启动程序
if (fileURLToPath(import.meta.url) === path.resolve(process.argv[1])) {
    const configPath = './config.json';
    const __dirname = path.dirname(fileURLToPath(import.meta.url));
    const absConfigPath = path.resolve(__dirname, configPath);
    const app = new ArcheryMusicApplication(absConfigPath);
    app.start();
}

export { ArcheryMusicApplication };
