const cb = arguments[arguments.length - 1];
const partialResults = JSON.parse(arguments[0]).flat();
const options = JSON.parse(arguments[1])
axe.finishRun(partialResults, options)
    .then((results) => cb(results));
