var jQueryLoaderOptions = null;
(function ($) {
    $.loader = function (option) {
        switch (option) {
            case 'close':
                if (jQueryLoaderOptions) {
                    if ($("#" + jQueryLoaderOptions.id)) {
                        $("#" + jQueryLoaderOptions.id + ", #" + jQueryLoaderOptions.background.id).remove()
                    }
                }
                return;
                break;
            case 'setContent':
                if (jQueryLoaderOptions) {
                    if ($("#" + jQueryLoaderOptions.id)) {
                        if ($.loader.arguments.length == 2) {
                            $("#" + jQueryLoaderOptions.id).html($.loader.arguments[1])
                        } else {
                            if (console) {
                                console.error("setContent method must have 2 arguments $.loader('setContent', 'new content');")
                            } else {
                                alert("setContent method must have 2 arguments $.loader('setContent', 'new content');")
                            }
                        }
                    }
                }
                return;
                break;
            default:
                var options = $.extend({
                    // content: "<div>Loading ...</div>",
                    className: 'loader',
                    id: 'jquery-loader',
                    height: 10,
                    // width: 120,
                    zIndex: 30000,
                    background: {
                        opacity: 0.4,
                        id: 'jquery-loader-background'
                    }
                }, option)
        }
        jQueryLoaderOptions = options;
        var maskHeight;
        if (window.outerHeight) {
            maskHeight = window.outerHeight;
        } else {
            maskHeight = document.body.clientHeight;
        }
        var maskWidth = $(window).width();
        var appendElement = option.appendElementId || "body";
        var spanContent = option.loaderContent || 'Loading...';
        var bgDiv = $('<div id="' + options.background.id + '"/>');
        bgDiv.css({
            zIndex: options.zIndex,
            position: 'absolute',
            top: '0px',
            left: '0px',
            width: $(appendElement).width() || $(document).width(),
            height: $(appendElement).height() || $(document).height(),
            opacity: options.background.opacity
        });
        bgDiv.appendTo(appendElement);
        if (jQuery.bgiframe) {
            bgDiv.bgiframe()
        }
        var div = $('<div id="' + options.id + '" class="' + options.className + '"><span id="loader_content" style="font-size:9px;color:#299efe;">' + spanContent + '</span></div>');
        div.css({
            zIndex: options.zIndex + 1,
            width: options.width,
            height: options.height
        });
        div.appendTo(appendElement);
        if (appendElement == "body") {
            div.center();
        }
        else {
            div.center(appendElement);
        }
        $(options.content).appendTo(appendElement);
    };
    $.fn.center = function (appendElement) {
        var element = appendElement || window;
        this.css("position", "absolute");
        this.css("top", ($(element).height() - this.outerHeight()) / 2 + $(element).scrollTop() + "px");
        this.css("left", ($(element).width() - this.outerWidth()) / 2 + $(element).scrollLeft() + "px");
        return this;
    }
})(jQuery);