var images = {
    localId: [],
    savePath: ''
};
$(document).ready(function () {
    new AjaxUpload('chooseImage', {
        action: '/UploadImage/AjaxUpload',
        name: 'myImageFile',
        responseType: 'json',
        onSubmit: function (file, ext) {
            if (!(ext && /^(jpg|jpeg|png)$/.test(ext))) {
                alert("只能上传图片文件");
                return false;
            }
            this.setData({
            });
            this.disable();
        },
        onComplete: function (file, response) {
            this.enable();
            var data = eval("(" + response + ")");
            if (data.status == '1') {
                $('#divUploadImageFileStatus').html('');
                $("#hidImagePath").val(data.filePath);
                $("#txtImageName").val(data.fileName);
            }
            else {
                alert(data.note);
            }
        }
    });
    //    wx.error(function (res) {
    //        alert(res.errMsg);
    //    });
    //    wx.config({
    //        debug: false,
    //        appId: 'wxf8b4f85f3a794e77',
    //        timestamp: 1420774989,
    //        nonceStr: '2nDgiWM7gCxhL8v0',
    //        signature: '1f8a6552c1c99991fc8bb4e2a818fe54b2ce7687',
    //        jsApiList: [
    //        'checkJsApi',
    //        'chooseImage',
    //        'previewImage',
    //        'uploadImage'
    //      ]
    //    });
    //    // 拍照、本地选图
    //    document.querySelector('#chooseImage').onclick = function () {
    //        wx.chooseImage({
    //            success: function (res) {
    //                images.localId = res.localIds;
    //                if (images.localId.length == 0) {
    //                    alert('请先使用 chooseImage 接口选择图片');
    //                    return;
    //                }
    //                if (images.localId.length > 1) {
    //                    alert('请选择一张图片');
    //                    return;
    //                }
    //                var i = 0, length = images.localId.length;
    //                images.serverId = [];
    //                function upload() {
    //                    wx.uploadImage({
    //                        localId: images.localId[i],
    //                        success: function (res) {
    //                            i++;
    //                            images.serverId = res.serverId;
    //                            if (i < length) {
    //                                upload();
    //                            }
    //                            else {
    //                                $.get("/UploadImage/AjaxUpload?m=" + Math.random(), { MedioId: res.serverId }, function (data) {
    //                                    if (data != "") {
    //                                        var json = eval("(" + data + ")");
    //                                        images.savePath = json.savepath;
    //                                    }
    //                                });
    //                            }
    //                        },
    //                        fail: function (res) {
    //                            alert(JSON.stringify(res));
    //                        }
    //                    });
    //                }
    //                upload();
    //            }
    //        });
    //    };
    document.querySelector("#saveImagePath").onclick = function () {
        if ($("#txtImageName").val() == '') {
            alert('请先上传图片');
        }
        else {
            $.get("/UploadImage/SaveImagePath?m=" + Math.random(), { TpId: $("#hidTpId").val(), ImagePath: $("#hidImagePath").val() }, function (data) {
                if (data != "err") {
                    window.location.href = "/MyViewTaskProject";
                }
                else {
                    alert('保存出错');
                }
            });
        }

    };
})

