$(document).ready(function () {
    let isShow = false;
    const showLeftMenu = () =>{
        isShow = true;
        $(".odd-left-menu").addClass("odd-left-menu-show");
        $(".odd-left-menu").removeClass("odd-left-menu-hide");
    };

    const hideLeftMenu = () =>{
        isShow = false;
        $(".odd-left-menu").addClass("odd-left-menu-hide");
        $(".odd-left-menu").removeClass("odd-left-menu-show");
    };

    $(".nav-toggle-botton").click(function () {
        if (isShow) {
            hideLeftMenu();
        }
        else {
            showLeftMenu();
        }
    });
});


