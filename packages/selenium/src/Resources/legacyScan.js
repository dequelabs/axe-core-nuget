var callback = arguments[arguments.length - 1];

var context = typeof arguments[0] === 'string' ? JSON.parse(arguments[0]) : arguments[0];
context = context || document;

var options = JSON.parse(arguments[1]);
const thenFunc = (results) => {
    callback(results);
};
const catchFunc = (err) => {
    callback({ error: err.message });
}
axe.run(context, options).then(thenFunc).catch(catchFunc);
