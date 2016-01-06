/*
* Written by:     haocy
* function:       对话框效果js库
* Created Date:   2011-11-29
*/
if (typeof (effect) == 'undefined') effect = { author: 'haocy', version: '1.0.0' };
effect.Dialog = {
    show: function (msg) {
        return art.dialog({
            id: 'Show',
            icon: 'succeed',
            fixed: true,
            lock: true,
            resize: false,
            content: msg,
            ok: true
        });
    },
    alert: function (msg, fCallback) {
        return art.dialog({
            id: 'Alert',
            icon: 'warning',
            fixed: true,
            lock: true,
            resize: false,
            content: msg,
            ok: true,
            close: fCallback
        });
    },
   alert_error: function (msg, fCallback) {
        return art.dialog({
            id: 'Alert',
            icon: 'error',
            fixed: true,
            lock: true,
            resize: false,
            content: msg,
            ok: true,
            close: fCallback
        });
    },
   alert_succeed: function (msg, fCallback) {
        return art.dialog({
            id: 'Alert',
            icon: 'succeed',
            fixed: true,
            lock: true,
            resize: false,
            content: msg,
            ok: true,
            close: fCallback
        });
    },   
    confirm: function (msg, confirmCallback, cancelCallback) {
        return art.dialog({
            id: 'Confirm',
            icon: 'question',
            fixed: true,
            lock: true,
            resize: false,
            content: msg,
            ok: function (here) {
                return confirmCallback.call(this, here);
            },
            cancel: function (here) {
                return cancelCallback && cancelCallback.call(this, here);
            }
        });
    }
};

function jDivShow(obj, txtTitle, closeCallback) {
    var auiId = obj + '_aui';
    if (art.dialog.list[auiId] == undefined) {
        if ($("#" + obj).length > 0) {
            art.dialog({
                id: auiId,
                title: txtTitle,
                fixed: true,
                lock: true,
                resize: false,
                padding: '0px',
                content: $('#' + obj)[0],
                close: closeCallback
            });
        }
    }
    else {
        art.dialog.list[auiId].title(txtTitle);
        art.dialog.list[auiId].show();
    }
}

function jDivHide(obj) {
    var curDialog = art.dialog.list[obj + '_aui'];
    if (curDialog != null) {
        curDialog.hide();
    }
}
