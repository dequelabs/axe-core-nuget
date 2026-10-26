// a frame that blocks scripts skips event listeners and never fires timers, so axe.runPartial would hang in it
(function () {
    var ran = false;
    var target = new EventTarget();
    target.addEventListener('axe', function () { ran = true; });
    target.dispatchEvent(new Event('axe'));
    if (!ran) {
        throw new Error('Frame cannot run scripts');
    }
})();
