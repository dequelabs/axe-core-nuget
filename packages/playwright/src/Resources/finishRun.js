([partialResultsStr, optionsStr]) => {
    const partialResults = JSON.parse(partialResultsStr).flat();
    const options = JSON.parse(optionsStr)

    const catchFunc = (err) => {
        return { error: err.message };
    }
    return axe.finishRun(partialResults, options)
        .catch(catchFunc);
}
