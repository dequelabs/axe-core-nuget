const context = typeof arguments[0] === 'string' ? JSON.parse(arguments[0]) : arguments[0];
return JSON.stringify(axe.utils.getFrameContexts(context));

