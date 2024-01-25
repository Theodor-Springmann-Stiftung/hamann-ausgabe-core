import { resolve } from 'path';
import { defineConfig } from 'vite';

export default defineConfig({
	build: {
		lib: {
			entry: resolve(__dirname, 'wwwroot/js/main.js'),
			name: 'HaWeb',
			fileName: 'scripts'
		},
        outDir: resolve(__dirname, 'wwwroot/dist/'),
	}
});