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
        $.post("MyViewTaskProject/ParamSearchList?m=" + Math.random, { ProjectName: $("#txtProjectName").val(), Type: $("#hidType").val(), PageIndex: pageIndex, PageSize: pageSize }, function (data) {
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
    }
};

$(function () {
    myViewTaskProject.InitList(1, 'noparam');
    myViewTaskProject.Initial();
})