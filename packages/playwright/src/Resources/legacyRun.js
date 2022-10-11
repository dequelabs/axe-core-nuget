([contextStr, optionsStr]) => {
    const context = contextStr ? JSON.parse(contextStr) : null;
    const options = JSON.parse(optionsStr);
    return axe.run(context || document, options);
}
