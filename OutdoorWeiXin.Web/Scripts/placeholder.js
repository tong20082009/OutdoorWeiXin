/*
*  解决IE不支持HTML5表单属性placeholder的解决办法
* placeholder 属性提供可描述输入字段预期值的提示信息（hint）。
* 该提示会在输入字段为空时显示，并会在字段获得焦点时消失。
* 调用方法：
* if($.browser.msie) { 
* $(":input[placeholder]").each(function(){
* $(this).placeholder();
* });
* }
*/
(function ($) {
    $.fn.placeholder = function(options) {
        var defaults = {
            pColor: "#ccc",
            pActive: "#999",
            pFont: "14px",
            activeBorder: "#080",
            posL: 8,
            zIndex: "99"
        },
            opts = $.extend(defaults, options);
        //
        return this.each(function() {
            if ("placeholder" in document.createElement("input")) return;
            $(this).parent().css("position", "relative");
            var isIE = $.browser.msie,
                version = $.browser.version;

            //不支持placeholder的浏览器
            var $this = $(this),
                msg = $this.attr("placeholder"),
                iH = $this.outerHeight(),
                iW = $this.outerWidth(),
                iX = $this.position().left,
                iY = $this.position().top,
                oInput = $("<label>", {
                    "class": "test",
                    "text": msg,
                    "css": {
                        "position": "absolute",
                        "left": iX + "px",
                        "top": iY + "px",
                        "width": iW - opts.posL + "px",
                        "padding-left": opts.posL + "px",
                        "height": iH + "px",
                        "line-height": iH + "px",
                        "color": opts.pColor,
                        "font-size": opts.pFont,
                        "z-index": opts.zIndex,
                        "cursor": "text"
                    }
                }).insertBefore($this);
            //初始状态就有内容
            var value = $this.val();
            if (value.length > 0) {
                oInput.hide();
            }
            ;

            //
            $this.on("focus", function() {
                var value = $(this).val();
                if (value.length > 0) {
                    oInput.hide();
                }
                oInput.css("color", opts.pActive);
                //

                if (isIE && version < 9) {
                    var myEvent = "propertychange";
                } else {
                    var myEvent = "input";
                }

                $(this).on(myEvent, function() {
                    var value = $(this).val();
                    if (value.length == 0) {
                        oInput.show();
                    } else {
                        oInput.hide();
                    }
                });

            }).on("blur", function() {
                var value = $(this).val();
                if (value.length == 0) {
                    oInput.css("color", opts.pColor).show();
                }
            });
            //
            oInput.on("click", function() {
                $this.trigger("focus");
                $(this).css("color", opts.pActive);
            });
            //
            $this.filter(":focus").trigger("focus");
        });
    };
})(jQuery)