$(function () {
    Login.Initial();
    Login.Submit();
});
var Login = {
    Initial: function () {
        $("#txtCusName").attr("maxlength", "20").attr("placeholder", "请输入用户名").focus(function () {
            $("#divError").html("");
        });
        $("#txtPassword").attr("maxlength", "16").attr("placeholder", "请输入密码").focus(function () {
            $("#divError").html("");
        }).keyup(function () {
            if ($.trim($("#txtCusName").val()) != '' && $.trim($("#txtPassword").val()) != '') {
                $("#submitLogin").attr("style", "color:#fff");
            }
            else {
                $("#submitLogin").attr("style", "color:#4dc2f1");
            }
        });
        //解决IE下不支持placeholder
        if ($.browser.msie) {
            $(":input[placeholder]").each(function () {
                $(this).placeholder();
            });
        }
        $("#txtCusName").focus();
    },
    Submit: function () {
        $("#submitLogin").click(function () {
            var userName = $.trim($("#txtCusName").val());
            var userPassword = $.trim($("#txtPassword").val());
            var isStatus = true;
            if (userName == "" || userName == "请输入用户名") {
                $("#divError").html("请填写用户名");
                isStatus = false;
                return false;
            }
            if (userPassword == "" || userPassword == "请输入密码") {
                $("#divError").html("请填写密码");
                isStatus = false;
                return false;
            }
            if ((userName == "" || userName == "请输入用户名") && (userPassword == "" || userPassword == "请输入密码")) {
                $("#divError").html("请填写用户名");
                isStatus = false;
                return false;
            }
            if (isStatus) {
                $.post("/CustomerBind/Bind?m=" + Math.random(), { CusName: userName, PassWord: userPassword }, function (data) {
                    if (data != "err") {
                        if (data == "success") {
                            window.location.reload();
                        }
                        else {
                            $("#divError").html(data);
                        }
                    }
                });
            }
            return false;
        });
    }
};