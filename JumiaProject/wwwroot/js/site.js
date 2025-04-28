
$(window).on('load',function () {
        $(".myOwlCarousel").owlCarousel({
            loop: true,
            margin: 10,
            nav: true,
            dots: false,
            responsive: {
                0: {
                    items: 1
                },
                600: {
                    items: 3
                },
                1000: {
                    items: 6
                }
            }
        });
    });

function updateCartCount() {
    $.get("/Cart/GetCartCount", function (data) {
        $("#cart-count").text(data);
    });
}
setInterval(updateCartCount, 10000);