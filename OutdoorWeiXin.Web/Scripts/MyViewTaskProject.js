var myViewTaskProject = {
    Initial: function () {
        $("#txtProjectName").attr("maxlength", "50");
        $("#txtProjectName").focus(function () {
            if ($("#txtProjectName").val() == '请输入楼宇名称') {
                $("#txtProjectName").val('');
            }
        }).blur(function () {
            if ($("#txtProjectName").val() == '') {
                $("#txtProjectName").val('请输入楼宇名称');
            }
        });
        $("#btnSearch").click(function () {
            myViewTaskProject.InitList(1, 'param');
        });
        var current = 0;
        $("#imgPreview").click(function () {
            current = (current + 90) % 360;
            this.style.transform = 'rotate(' + current + 'deg)';
        });
        //滚动条效果
        $(window).bind("scroll", function () {
            var o = $('#DivMyTaskProjectList');
            if (o != null && o.length != 0) {
                //获取网页的完整高度(fix)  
                var contentheight = $(document).height();
                //获取浏览器（窗口）高度(fix)  
                var windowHeight = $(window).height();
                //??获取网页滚过的高度(dynamic)  
                var top = document.documentElement.scrollTop + document.body.scrollTop;
                //当 top+clientHeight = scrollHeight的时候就说明到底儿了  
                if (top >= (parseInt(contentheight) - windowHeight)) {
                    var pageIndex = Number($("#hfPageIndex").val()); //当前页数
                    myViewTaskProject.InitList(pageIndex + 1, 'noparam');
                }
            }
        });
    },
    InitList: function (pageIndex, type) {
        var windowHeight = $(window).height(); //当前窗口高度
        var dataRowHeight = 64; //数据行高度
        var pageSize = Math.round(windowHeight / dataRowHeight) + 1; //每页显示条数
        var mediaType = 'ALL', showType = 'In';
        $("#ulMediaType li").each(function () {
            if ($(this).attr("class") == 'curr') {
                mediaType = $(this).attr("id");
            }
        });
        $("#ulShowList li").each(function () {
            if ($(this).attr("class") == 'curr') {
                showType = $(this).attr("id");
            }
        });
        $.post("/MyViewTaskProject/ParamSearchList?m=" + Math.random, { ProjectName: $("#txtProjectName").val(), MediaType: mediaType, Type: showType, PageIndex: pageIndex, PageSize: pageSize }, function (data) {
            if (data != "err") {
                $("#hfPageIndex").val(pageIndex);
                if (type == 'param') {
                    $("#DivMyTaskProjectList").html(data);
                }
                else {
                    if (data != null) {
                        $("#DivMyTaskProjectList").append(data);
                    }
                }
                //点击效果
                $("#DivMyTaskProjectList dl dt").toggle(
                    function () {
                        $(this).find("div[n='Operating']").attr("class", "close");
                        $(this).parent('.l1').removeClass().attr('class', 'l1 curr');
                        $(this).closest("dl").find("dd").show();
                    },
                    function () {
                        $(this).find("div[n='Operating']").attr("class", "open");
                        $(this).parent('.curr').removeClass().attr('class', 'l1');
                        $(this).closest("dl").find("dd").hide();
                    });
            }
        });
    },
    ReturnTask: function (tpid) {
        $.get("/MyViewTaskProject/ReturnTask?m=" + Math.random(), { TpId: tpid }, function (data) {
            if (data != "err") {
                if (data == "success") {
                    myViewTaskProject.InitList(1, 'param');
                }
                else {
                    alert(data);
                }
            }
        });
    }
};

$(function () {
    myViewTaskProject.Initial();
    myViewTaskProject.InitList(1, 'noparam');
})

function PreviewImage(src) {
    $("#imgPreview").attr('src', 'http://' + src);
    jDivShow('divPreviewImage', '预览图片');
}

function SelectMediaType(type) {
    $("#ulMediaType li").each(function () {
        if ($(this).attr("id") == type) {
            $(this).addClass('curr');
        }
        else {
            $(this).removeClass('curr');
        }
    });
    myViewTaskProject.InitList(1, 'param');
}

function SelectShowList(type) {
    $("#ulShowList li").each(function () {
        if ($(this).attr("id") == type) {
            $(this).addClass('curr');
        }
        else {
            $(this).removeClass('curr');
        }
    });
    myViewTaskProject.InitList(1, 'param');
}