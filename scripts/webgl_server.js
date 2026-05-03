const http = require('http');
const fs = require('fs');
const path = require('path');

const PORT = 8080;
const BUILD_DIR = path.join(__dirname, '..', 'Canicross', 'Builds', 'WebGL');

const MIME_TYPES = {
    '.html': 'text/html',
    '.js': 'application/javascript',
    '.wasm': 'application/wasm',
    '.data': 'application/octet-stream',
    '.json': 'application/json',
    '.png': 'image/png',
    '.jpg': 'image/jpeg',
    '.css': 'text/css',
    '.unityweb': 'application/octet-stream',
    '.ico': 'image/x-icon'
};

function serveFile(res, filePath) {
    const ext = path.extname(filePath).toLowerCase();
    const contentType = MIME_TYPES[ext] || 'application/octet-stream';

    fs.readFile(filePath, (err, data) => {
        if (err) {
            res.writeHead(404);
            res.end('Not found');
            return;
        }
        res.writeHead(200, {
            'Content-Type': contentType,
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Headers': '*'
        });
        res.end(data);
    });
}

const server = http.createServer((req, res) => {
    let filePath = path.join(BUILD_DIR, req.url === '/' ? 'index.html' : req.url);
    
    // Handle gzip/unityweb files
    if (!fs.existsSync(filePath) && fs.existsSync(filePath + '.unityweb')) {
        filePath += '.unityweb';
    }
    if (!fs.existsSync(filePath) && fs.existsSync(filePath + '.gz')) {
        filePath += '.gz';
    }

    if (fs.existsSync(filePath) && fs.statSync(filePath).isFile()) {
        serveFile(res, filePath);
    } else {
        res.writeHead(404);
        res.end('Not found: ' + req.url);
    }
});

server.listen(PORT, '0.0.0.0', () => {
    console.log(`======================================`);
    console.log(`  Canicross WebGL Server`);
    console.log(`======================================`);
    console.log(`  Serving: ${BUILD_DIR}`);
    console.log(`  URL: http://localhost:${PORT}`);
    console.log(`  Network: http://0.0.0.0:${PORT}`);
    console.log(`======================================`);
    console.log(`  To play on mobile:`);
    console.log(`  1. Find your computer IP: ipconfig`);
    console.log(`  2. Open http://<IP>:${PORT} on phone`);
    console.log(`======================================`);
});
