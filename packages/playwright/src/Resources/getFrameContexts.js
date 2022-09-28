(contextObj) => {
    const context = typeof contextObj === 'string' ? JSON.parse(contextObj) : contextObj;
    return JSON.stringify(axe.utils.getFrameContexts(context));
}
