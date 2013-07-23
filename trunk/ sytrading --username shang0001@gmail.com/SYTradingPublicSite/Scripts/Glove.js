// Enable flexslider2 for Home page
$('.flexslider').flexslider({
    animation: "slide"
});

$("#pageslide_openbar").pageslide({ href: '#', direction: "left", iframe: false, overlay: true });

if ($('p[class="hidden-phone"]').css("display") != "none") {
    // For detail page, enable colorbox
    $(".group1").colorbox({ rel: 'group1', height: "100%" });
}
else {
    // For glove list, make listview item take tap/click event
    $('.show-grid [class*="span"]').click(function () {
        location.href = $("a", this).attr("href");
    });

    $('.GlovePage').on('swipeleft', function (e) {
        //alert('show right');
        $.pageslide({ href: '#', direction: "left", iframe: false, overlay: true });
    }).on('swiperight', function (e) {
        //alert('close right');
        $.pageslide.close();
    }).on('movestart', function (e) {
        // If the movestart is heading off in an upwards or downwards
        // direction, prevent it so that the browser scrolls normally.
        if ((e.distX > e.distY && e.distX < -e.distY) ||
            (e.distX < e.distY && e.distX > -e.distY)) {
            e.preventDefault();
        }
    });
}