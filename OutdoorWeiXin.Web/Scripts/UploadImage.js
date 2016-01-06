var UploadImage = {
    longitude: '',
    latitude: ''
}
$(document).ready(function () {
    new AjaxUpload('chooseImage', {
        action: '/UploadImage/AjaxUpload',
        name: 'myImageFile',
        responseType: 'json',
        onSubmit: function (file, ext) {
            if (!(ext && /^(jpg|jpeg|png)$/.test(ext.toLowerCase()))) {
                alert("只能上传图片文件");
                return false;
            }
            this.setData({
                'TpId': $("#hidTpId").val(),
                'longitude': UploadImage.longitude,
                'latitude': UploadImage.latitude
            });
            $("#chooseImage").text('上传中...');
            this.disable();
        },
        onComplete: function (file, response) {
            this.enable();
            var data = eval("(" + response + ")");
            if (data.status == '1') {
                $('#divUploadImageFileStatus').html('');
                $("#hidImagePath").val(data.filePath);
                $("#txtImageName").val(data.fileName);
                $("#chooseImage").text('上传');
                window.location.href = "/MyViewTaskProject/Index";
            }
            else {
                alert(data.note);
            }
        }
    });

    wx.config({
        debug: false,
        appId: 'wxa542374b0e19a985',
        timestamp: 1420774989,
        nonceStr: '2nDgiWM7gCxhL8v0',
        signature: '1f8a6552c1c99991fc8bb4e2a818fe54b2ce7687',
        jsApiList: [
        'getLocation'
      ]
    });
    wx.getLocation({
        success: function (res) {
            UploadImage.latitude = res.latitude; // 纬度，浮点数，范围为90 ~ -90
            UploadImage.longitude = res.longitude; // 经度，浮点数，范围为180 ~ -180。
            var speed = res.speed; // 速度，以米/每秒计
            var accuracy = res.accuracy; // 位置精度
        }
    });
})

