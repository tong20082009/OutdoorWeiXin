var viewTaskProject = {
    bHeight: 0,
    pHeight: 0,
    Initial: function () {
        viewTaskProject.bHeight = $("#main").height();
        viewTaskProject.pHeight = $(document).height();
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
        var city = $("#hidCity").val();
        $(".hCity td").each(function () {
            if ($(this).attr("id") == city) {
                $(this).addClass("on");
            }
        });
        var regionid = $(".hCity td.on").attr("id")
        if (regionid != null && regionid != undefined && regionid != '') {
            $.get("/ViewTaskProject/GetArea?m=" + Math.random(), { RegionId: regionid }, function (data) {
                if (data != "err") {
                    $(".hArea").html(data);
                    $(".hArea td").click(function () {
                        $(".hArea td").removeClass("on");
                        $(this).addClass("on")
                    })
                    var area = $("#hidArea").val();
                    $(".hArea td").each(function () {
                        if ($(this).find('a').text() == area) {
                            $(this).addClass("on");
                        }
                    });
                    viewTaskProject.InitList(1, 'param');
                }
            });
        }
        $(".hCity td").click(function () {
            $(".hCity td").removeClass("on");
            $(this).addClass("on");
            regionid = $(this).attr("id")
            $.get("/ViewTaskProject/GetArea?m=" + Math.random(), { RegionId: regionid }, function (data) {
                if (data != "err") {
                    $(".hArea").html(data);
                    $(".hArea td").click(function () {
                        $(".hArea td").removeClass("on");
                        $(this).addClass("on")
                    })
                }
            });
        })
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
        var mediaType = 'ALL';
        $("#ulMediaType li").each(function () {
            if ($(this).attr("class") == 'curr') {
                mediaType = $(this).attr("id");
            }
        });
        if (regionId != '' && regionId != undefined && regionId != null) {
            $.post("ViewTaskProject/ParamSearchList?m=" + Math.random, { RegionId: regionId, AreaName: areaName, ProjectName: $("#txtProjectName").val(), Type: mediaType, PageIndex: pageIndex, PageSize: pageSize }, function (data) {
                if (data != "err") {
                    $("#hfPageIndex").val(pageIndex);
                    if (type == 'param') {
                        $("center").height(200 + $(window).height());
                        $("#DivTaskProjectList").html(data);
                    }
                    else {
                        if (data != null && data != '') {
                            $("center").height($("center").height() + $(window).height());
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
        }
    },
    ReceiveTask: function (tid, tpid, status, node) {
        $.get("/ViewTaskProject/ReceiveTask?m=" + Math.random(), { Tid: tid, TpId: tpid, TaskStatus: status }, function (data) {
            if (data != "err") {
                if (data == "success") {
                    $(node).hide();
                    $(node).next().show();
                }
                else {
                    alert('该任务已被领取，请刷新页面');
                }
            }
        });
    },
    AreaSelect: function () {
        $(".fmSearc").click(function () {
            var bHeight = $("#main").height(),
			bWidth = $("body").width(),
		    pHeight = $(document).height();
            $("center").css({ "width": +bWidth + "px", "height": +viewTaskProject.bHeight + "px", "min-height": +viewTaskProject.pHeight + "px" });
            $("#main").addClass("main").animate({ "left": "80%" }).css({ "width": +bWidth + "px" });
            $(".hSenior").animate({ "left": "0" }).css({ "box-shadow": "0px 0px 3px #c4c4c4", "border-right": "solid 1px #dbdbdb", "height": +viewTaskProject.bHeight + "px", "min-height": +viewTaskProject.pHeight + "px" });
            $(".sBg").show();
        });
        $(".sBg").click(function () {
            $(".hSenior").animate({ "left": "-80%" });
            $(".hSenior").attr("style", "");
            $("#main").animate({ "left": "0" }); //.removeClass("main");
            $(".sBg").hide();
            var regionid = $(".hCity td.on").attr("id")
            if (regionid != undefined && regionid != '' && regionid != null) {
                viewTaskProject.InitList(1, 'param');
            }
        });
    }
};

$(function () {
    viewTaskProject.Initial();
    viewTaskProject.InitList(1, 'param');
    viewTaskProject.AreaSelect();
})

function SelectMediaType(type) {
    $("#ulMediaType li").each(function () {
        if ($(this).attr("id") == type) {
            $(this).addClass('curr');
        }
        else {
            $(this).removeClass('curr');
        }
    });
    viewTaskProject.InitList(1, 'param');
}