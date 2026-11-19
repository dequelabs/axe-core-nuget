// a frame that blocks scripts skips event listeners and never fires timers, so axe.runPartial would hang in it
(function () {
    var ran = false;
    try {
        var target = document.createElement('div');
        target.addEventListener('axe', function () { ran = true; });
        var event = document.createEvent('Event');
        event.initEvent('axe', false, false);
        target.dispatchEvent(event);
    } catch (e) {
        return;
    }
    if (!ran) {
        throw new Error('Frame cannot run scripts');
    }
})();
