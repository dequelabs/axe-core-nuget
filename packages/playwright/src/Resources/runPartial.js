([contextObj, optionsStr]) => {
    const context = typeof contextObj === 'string' ? JSON.parse(contextObj) : contextObj;
    const options = JSON.parse(optionsStr);
    return axe.runPartial(context || document, options)
        .then(res => {window.res = res; return res;})
        .then((results) => JSON.stringify(results));
}
