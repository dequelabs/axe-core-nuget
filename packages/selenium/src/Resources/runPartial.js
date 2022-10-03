const cb = arguments[arguments.length - 1];
const args = Array.from(arguments);
const context = typeof args[0] === 'string' ? JSON.parse(args[0]) : args[0];
const options = JSON.parse(args[1]);
axe.runPartial(context || document, options)
    .then((results) => cb(JSON.stringify(results)));
