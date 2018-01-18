$(document).ready(function () {
    //$('.header-menu').html('');    /*clear html content on home page*/          
    window.onscroll = function () { scrollFunction() };

});

function scrollFunction() {

    var currentPageYoffset = window.pageYOffset;
    var headerMenuYoffset = $(".header-menu").offset().top;

    if (currentPageYoffset > headerMenuYoffset) {
        $("#sticker").addClass("sticky");
    } else {
        $("#sticker").removeClass("sticky");
    }
}
