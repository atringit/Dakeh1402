




$( function() {

    $('a[href="#"]').click(function(e){
        e.preventDefault();
    });

    

    // MobileMenu
    //=====================================================================

    $('.main-header__mobile-menu-toggle a').click(function(){
        if($('.mobile-menu').hasClass('.mobile-menu--open'))
        {
            $('.mobile-menu').removeClass('mobile-menu--open');
        }
        else
        {
            $('.mobile-menu').addClass('mobile-menu--open');
        }
    });

    $('.mobile-menu__close-menu').click(function(){
        $('.mobile-menu').removeClass('mobile-menu--open');
    });
    $('.main-header__mobile-menu-toggle a,.mobile-menu').click(function(event){
        event.stopPropagation();
    });
    $(document).click(function(){
        $('.mobile-menu').removeClass('mobile-menu--open');
    });

    $('.mobile-menu nav > ul > li').each(function() {
        $(this).has('ul').addClass('has-child').prepend('<i class="dk-arrow-down"></i>')
    });
    $('.mobile-menu nav > ul > li').on('click', '>i', function() {
        if ($(this).parent().hasClass('open')) {
            $(this).parent().removeClass('open').find('>ul').slideUp()
        } else {
            $('.mobile-menu nav > ul > li > ul').slideUp().parent().removeClass('open');
            $(this).parent().addClass('open').find('>ul').slideDown()
        }
    });


    // Tabs
    //=====================================================================

    $(document).on('click','.tabs li[data-tab]',function(){

        tab = $(this).data('tab');
        $(this).addClass('active')
        .siblings()
        .removeClass('active');

        $(this).closest('.tabs')
        .find('.tab-content[data-tabtarget="'+tab+'"]')
        .addClass('active')
        .siblings()
        .removeClass('active');
    });


    // Toaster Alert
    //=====================================================================

    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        onOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    });

   


    // Detail page Slider
    //=====================================================================

    if($('.detail__slider').length > 0)
    {
        var detail_image_thumb = new Swiper('.detail__slider__thumb .swiper-container', {
            direction: 'vertical',
            slidesPerView: 5,
            spaceBetween: 15,
            watchSlidesVisibility: true,
            watchSlidesProgress: true,
        });

        let width = $(window).outerWidth();
        if(width >= 768)
        {
            detail_image_thumb.changeDirection('vertical');
            detail_image_thumb.update();
        }
        else
        {
            detail_image_thumb.changeDirection('horizontal');
            detail_image_thumb.update();
        }

        $(window).resize(function(){
            width = $(window).outerWidth();
            if(width >= 768)
            {
                detail_image_thumb.changeDirection('vertical');
                detail_image_thumb.update();
            }
            else
            {
                detail_image_thumb.changeDirection('horizontal');
                detail_image_thumb.update();
            }
        })



        var detail_image = new Swiper('.detail__slider__main .swiper-container', {
            simulateTouch:false,
            thumbs: {
                swiper: detail_image_thumb
            },
            effect: 'fade',
            fadeEffect: {
                crossFade: 1
            },
        });
    }


    var cats = new Swiper('.cats .swiper-container', {
        slidesPerView: 'auto',
        spaceBetween: 10,
        freeMode: 1,
    });

    
    // Custom upload button
    //=====================================================================

    function readURL(input) {

        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $(input).parent().addClass('form__box__upload__image__set-val');
                $(input).siblings().attr('src',e.target.result);
            }
            reader.readAsDataURL(input.files[0]);
        }
    }
    $(document).on('change','.form__box__upload__image input[type=file]',function(){
        readURL(this);
    }); 
    $(document).on('click','.form__box__upload__image i',function(){
        $(this).parent().remove()
    });

    var review_image = `
    <div class="form__box__upload__image">
        <input type="file" name="image[]">
        <img src="">
        <i class="dk-delete"></i>
        <span>انتخاب محتوا</span>
    </div>`;

    $(document).on('click','.form__box__upload__add',function(){
        $(this).before(review_image);
    });



    // Login Countdown
    //=====================================================================

    var t = "2022/01/01";
    $('#count_down').countdown(t)
    .on('update.countdown', function(event) {
        $(this).html(event.strftime('%-M:%S'));
    })
    .on('finish.countdown', function(event) {
        // when count down finished
    });

    if($('.zoom')){
          $('.zoom').magnify();
    }

    //=============================//
    // r.dadkhah.tehrani@gmail.com //
    //=============================//
});