/*<Copyright> Cross-Tab </Copyright>
<ProjectName>SICT</ProjectName>
<FileName> pagehelpers.js </FileName>
<CreatedOn>15 Jan 2015</CreatedOn>*/
var pageHelper = {
    stickyPosition: {
        top: {
            RIGHT: "top-right",
            LEFT: "top-left",
            CENTER: "top-center"
        }
    },
    stickyTypes: {
        SUCCESS: "st-success",
        ERROR: "st-error",
        INFO: "st-info",
    },
    removeSmokeSignal: function () {
        try {
            var smokeelement = $('.smoke-base.smoke-visible');
            if (smokeelement) {
                smokeelement[0].id;
                var regex = /[+-]?\d+\.\d+/g;
                smoke.destroy('signal', Math.abs(smokeelement[0].id.match(regex)[0]));
            }
        } catch (e) { console.log(e); }
    },
    addSmokeSignal: function (message, newTimeOut) {
        try {
            var defaultValue = undefined === newTimeOut ? 2 * 60 * 1000 : newTimeOut;
            smoke.signal(message, defaultValue);
        } catch (e) { console.log(e); }
    },
    clearStickies: function () {
        //	Close the previous sticky elements
        try {
            $('.sticky-queue.top-right').remove();
        } catch (e) { console.log(e); }
    },
    notify: function (msg, position, type) {
        var me_ = this;
        if (me_.stickyTypes.ERROR === type)
            $.sticky(msg, { autoclose: false, position: position, type: type });
        else if (msg && position && type)
            $.sticky(msg, { autoclose: 5000, position: position, type: type });
        else
            $.sticky(msg, { autoclose: 2000, position: pageHelper.stickyPosition.top.RIGHT, type: me_.stickyTypes.INFO });
    },
    clearAllPopUps: function () {
        pageHelper.removeSmokeSignal();
        pageHelper.clearStickies();
    }
};