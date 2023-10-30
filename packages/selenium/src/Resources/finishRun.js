const cb = arguments[arguments.length - 1];
const partialResults = JSON.parse(window.partialResults).flat()
const options = JSON.parse(arguments[0])

const catchFunc = (err) => {
    callback({ error: err.message }, res);
}
axe.finishRun(partialResults, options)
    .then((results) => cb(results))
    .catch(catchFunc);
