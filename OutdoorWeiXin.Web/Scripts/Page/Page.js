Page = {
    Init: function (p1, s1, t1, d1, funcname) {
        var p = parseInt(p1);
        var s = parseInt(s1);
        var t = parseInt(t1);
        var c = Math.ceil(t / s);
        var d = d1;
        var str = "<div class=\"pageBox\">";
        if (p > 1) {
            str += "<a id=\"prevpage" + d + "\" page=\"" + (p - 1 > 0 ? p - 1 : 1) + "\" href=\"javascript:void(0)\" class=\"fast\">上一页</a>";
        } else {
            str += "<span class=\"fast\">上一页</span>";
        }
        if (c <= 7) {
            for (var i = 1; i <= c; i++) {
                str += "<a id=\"page" + i + "\" href=\"javascript:void(0)\" class=\"page\" page=\"" + i + "\">" + i + "</a>";
            }
        } else {
            str += "<a id=\"page1\" href=\"javascript:void(0);\" page=\"1\" class=\"page\">1</a>";
            if (p > 4) {
                if (p >= 10) {
                    if (p == c) {
                        str += "<a id=\"page" + (c - 5) + "\" class=\"more\" href=\"javascript:void(0);\" page=\"" + (c - 5) + "\">...</a>";
                    } else {
                        str += "<a id=\"page" + (p - 3) + "\" class=\"more\" href=\"javascript:void(0);\" page=\"" + (p - 3) + "\">...</a>";
                    }
                    
                } else {
                    str += "<a id=\"page" + (p - 3) + "\" class=\"more\" href=\"javascript:void(0);\" page=\"" + (p - 3) + "\">...</a>";
                }
            }
            var k = 0;
            if (p + 2 < c) {
                for (var j = -2; j > -3; j++) {
                    if (p + j > 1 && p + 2 <= c) {
                        str += "<a id=\"page" + (p + j) + "\"  class=\"page\" href=\"javascript:void(0);\" page=\"" + (p + j) + "\" >" + (p + j) + "</a>";
                        k++;
                    }
                    if (p < 4 && p + j >= 5 || p >= 4 && k >= 5) {
                        break;
                    }
                }
            } else {
                for (var l = c - 4; l <= c; l++) {
                    str += "<a id=\"page" + l + "\" class=\"page\" href=\"javascript:void(0);\" page=\"" + l + "\" >" + l + "</a>";
                    k++;
                }
            }
            if (p + 3 < c) {
                if (p <= 3) {
                    str += "<a id=\"page" + 6 + "\" class=\"more\" href=\"javascript:void(0);\" page=\"" + 6 + "\">...</a>";
                } else {
                    str += "<a id=\"page" + (p + 3) + "\" class=\"more\" href=\"javascript:void(0);\" page=\"" + (p + 3) + "\">...</a>";
                }
            }
            if (p + 2 < c) {
                str += "<a id=\"page" + c + "\" class=\"page\" href=\"javascript:void(0);\" page=\"" + c + "\" >" + c + "</a>";
            }
        }
        if (p != c&&c!=0) {
            str += "<a id=\"nextpage" + d + "\" page=\"" + (p + 1 > c ? c : p + 1) + "\" title=\"下页\" class=\"next\" href=\"javascript:void(0);\">下页 ></a>";
        } else {
            str += "<span class=\"next\">下一页</span>";
        }
        str += "<span class=\"oTotal\">到第 <input type=\"text\" id=\"txtpageno" + d + "\" class=\"txt inpPage\" /> 页</span>";
        str += "<button class=\"sBtn\" id=\"ago\">GO</button>";
        str += "<span class=\"zongshu\">总数：<em class=\"pageNb\">" + t + "</em> 条</span>";
        str += "</div>";
        $("#" + d).html(str);
        $("#page" + p).addClass("on");
        $("#txtpageno" + d).val(p);
        $("#txtpageno" + d).die().keyup(function () {
            var r = /^\+?[1-9][0-9]*$/;
            if (!r.test($("#txtpageno" + d).val())) {
                $("#txtpageno" + d).val("");
            } else if ($("#txtpageno" + d).val() > c) {
                $("#txtpageno" + d).val(c);
            }
        });

        $("a[id^=page]").each(function () {
            if ($(this).attr("class") != "on") {
                $(this).die().live("click", function () {
                    Page.CallBack(funcname, $(this).attr("page"));
                });
            } else {
                $(this).attr("style", "margin-right:5px;cursor:auto;");
            }
        });

        $("#ago").die().live("click", function () {
            Page.CallBack(funcname, $("#txtpageno" + d).val());
        });

        if (p > 1) {
            $("#prevpage" + d).die().live("click", function () {
                Page.CallBack(funcname, $(this).attr("page"));
            });
        } else {
            //$("#prevpage" + d).die();
            //$("#prevpage" + d).attr("style", "margin-right:5px;cursor:auto;");
        }

        if (p != c) {
            $("#nextpage" + d).die().live("click", function () {
                Page.CallBack(funcname, $(this).attr("page"));
            });
        } else {
            //$("#nextpage" + d).die();
            //$("#nextpage" + d).attr("style", "margin-right:5px;cursor:auto;");
        }
    },
    CallBack: function (funcname, pid) {
        eval(funcname + "(" + pid + ")");
    }
};
