const httpServer = require('http-server');

const server = httpServer.createServer({
  root: 'node_modules/axe-test-fixtures/fixtures',
  before: [
    (req, res) => {
      if (req.url.startsWith('/csp-sandbox/')) {
        req.url = req.url.slice('/csp-sandbox'.length);
        res.setHeader('Content-Security-Policy', 'sandbox allow-same-origin');
      }
      res.emit('next');
    }
  ]
});

server.server.on('error', err => {
  if (err.code !== 'EADDRINUSE') {
    throw err;
  }
});
server.listen(8080);
