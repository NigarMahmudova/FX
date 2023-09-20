// prodct details slider active
$('.product-large-slider').slick({
    fade: true,
    arrows: false,
    asNavFor: '.pro-nav'
});


// product details slider nav active
$('.pro-nav').slick({
    slidesToShow: 4,
    asNavFor: '.product-large-slider',
    arrows: false,
    focusOnSelect: true
});

// image zoom effect
$('.img-zoom').zoom();