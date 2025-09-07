import { ServiceController } from '../../components/service';
import { NetworkScene } from 'ubiq';
import path from 'path';
import { fileURLToPath } from 'url';
import { MessageReader } from '../../components/message_reader';

class ArcheryMusicService extends ServiceController {
    private reader: MessageReader;

    constructor(scene: NetworkScene, name = 'ArcheryMusic') {
        super(scene, name);


        const __dirname = path.dirname(fileURLToPath(import.meta.url));


        this.registerChildProcess('arrow-handler', 'python', [
            '-u',
            path.join(__dirname, './archery_music.py'),
        ]);


        this.reader = new MessageReader(scene, 120);


        this.reader.on('data', (msg) => {
            const raw = msg.message.toString();
            console.log('Received raw message from Unity:', raw);

            try {
                const json = JSON.parse(raw);

                if (json.type === 'ArrowCount' && typeof json.arrowsLeft === 'number') {
                    this.log(`Arrows left:: ${json.arrowsLeft}`);


                    this.sendToChildProcess('arrow-handler', `${json.arrowsLeft}\n`);
                } else {
                    this.log(`Unknown message type: ${json.type}`, 'warning');
                }
            } catch (err) {
                this.log(`Failed to parse JSON message: ${(err as Error).message}`, 'error');
            }
        });
    }
}

export { ArcheryMusicService };
