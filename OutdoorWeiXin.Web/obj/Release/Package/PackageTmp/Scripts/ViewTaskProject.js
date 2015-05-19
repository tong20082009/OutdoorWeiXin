var viewTaskProject = {
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
            viewTaskProject.InitList(1, 'param');
        });
        //滚动条效果
        $(window).bind("scroll", function () {
            var o = $('#DivTaskProjectList');
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
                    viewTaskProject.InitList(pageIndex + 1, 'noparam');
                }
            }
        });
    },
    InitList: function (pageIndex, type) {
        var windowHeight = $(window).height(); //当前窗口高度
        var dataRowHeight = 64; //数据行高度
        var pageSize = Math.round(windowHeight / dataRowHeight) + 1; //每页显示条数
        var regionId = $(".hCity td.on").attr("id");
        var areaName = $(".hArea td.on a").text();
        $.post("ViewTaskProject/ParamSearchList?m=" + Math.random, { RegionId: regionId, AreaName: areaName, ProjectName: $("#txtProjectName").val(), PageIndex: pageIndex, PageSize: pageSize }, function (data) {
            if (data != "err") {
                $("#hfPageIndex").val(pageIndex);
                if (type == 'param') {
                    $("#DivTaskProjectList").html(data);
                }
                else {
                    if (data != null) {
                        $("#DivTaskProjectList").append(data);
                    }
                }
                //点击效果
                $("#DivTaskProjectList dl dt").toggle(
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
    ReceiveTask: function (tpid, node) {
        $.get("/ViewTaskProject/ReceiveTask?m=" + Math.random(), { TpId: tpid }, function (data) {
            if (data != "err") {
                $(node).hide();
            }
        });
    },
    AreaSelect: function () {
        $(".fmSearc").click(function () {
            var bHeight = $("#main").height(),
			bWidth = $("body").width(),
		    pHeight = $(document).height();
            $("center").css({ "width": +bWidth + "px", "height": +bHeight + "px", "min-height": +pHeight + "px" });
            $("#main").addClass("main").animate({ "left": "80%" }).css({ "width": +bWidth + "px" });
            $(".hSenior").animate({ "left": "0" }).css({ "box-shadow": "0px 0px 3px #c4c4c4", "border-right": "solid 1px #dbdbdb", "height": +bHeight + "px", "min-height": +pHeight + "px" });
            $(".sBg").show();
        });
        $(".sBg").click(function () {
            $(".hSenior").animate({ "left": "-80%" });
            $(".hSenior").attr("style", "");
            $("#main").animate({ "left": "0" }); //.removeClass("main");
            $(".sBg").hide();
        });
    }
};

$(function () {
    viewTaskProject.Initial();
    viewTaskProject.AreaSelect();
    $(".hCity td").click(function () {
        $(".hCity td").removeClass("on");
        $(this).addClass("on")
        var regionId = $(".hCity td.on").attr("id");
        $.get("/ViewTaskProject/GetArea?m=" + Math.random(), { RegionId: regionId }, function (data) {
            if (data != "err") {
                $(".hArea").html(data);
                $(".hArea td").click(function () {
                    $(".hArea td").removeClass("on");
                    $(this).addClass("on")
                })
            }
        });
    })
})