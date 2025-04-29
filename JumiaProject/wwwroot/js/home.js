document.addEventListener('DOMContentLoaded', function () {
    const scrollToTopBtn = document.getElementById('scroll-to-top');

    window.addEventListener('scroll', () => {
        if (window.pageYOffset > 200) {
            scrollToTopBtn.style.opacity = "1";
            scrollToTopBtn.style.visibility = "visible";
            scrollToTopBtn.style.transform = "translateY(0) scale(1)";
        } else {
            scrollToTopBtn.style.opacity = "0";
            scrollToTopBtn.style.visibility = "hidden";
            scrollToTopBtn.style.transform = "translateY(50px) scale(0.8)";
        }
    });

    scrollToTopBtn.addEventListener('click', (e) => {
        e.preventDefault();
        window.scrollTo({ top: 0, behavior: 'smooth' });
    });
});