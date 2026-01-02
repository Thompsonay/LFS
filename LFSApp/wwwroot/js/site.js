let cart = JSON.parse(localStorage.getItem('luxeCart')) || [];

// Slider functionality
let currentSlideIndex = 0;
const slides = document.querySelectorAll('.slide');
const dots = document.querySelectorAll('.dot');
const slider = document.querySelector('.slider');
// container that holds the slider (may not exist on every page)
const sliderContainer = document.querySelector('.slider-container');
let slideInterval;


// Slider Functions
function showSlide(index) {
    // Remove active class from all slides and dots
    if (slides && slides.length) slides.forEach(slide => slide.classList.remove('active'));
    if (dots && dots.length) dots.forEach(dot => dot.classList.remove('active'));

    // Ensure index is within bounds
    if (index >= slides.length) currentSlideIndex = 0;
    if (index < 0) currentSlideIndex = slides.length - 1;

    // Move slider (only if slider element exists)
    if (slider) slider.style.transform = `translateX(-${currentSlideIndex * 25}%)`;

    // Add active class to current slide and dot
    if (slides && slides[currentSlideIndex]) slides[currentSlideIndex].classList.add('active');
    if (dots && dots[currentSlideIndex]) dots[currentSlideIndex].classList.add('active');
}

function changeSlide(direction) {
    currentSlideIndex += direction;
    showSlide(currentSlideIndex);
    resetSlideShow();
}

function currentSlide(index) {
    currentSlideIndex = index - 1;
    showSlide(currentSlideIndex);
    resetSlideShow();
}

function nextSlide() {
    currentSlideIndex++;
    if (currentSlideIndex >= slides.length) {
        currentSlideIndex = 0;
    }
    showSlide(currentSlideIndex);
}

function startSlideShow() {
    slideInterval = setInterval(nextSlide, 4000); // Change slide every 5 seconds
}

function resetSlideShow() {
    clearInterval(slideInterval);
    startSlideShow();
}



// Touch/swipe support for mobile
let startX = 0;
let isDragging = false;

if (sliderContainer) {
    sliderContainer.addEventListener('touchstart', (e) => {
        startX = e.touches[0].clientX;
        isDragging = true;
        clearInterval(slideInterval);
    });

    sliderContainer.addEventListener('touchmove', (e) => {
        if (!isDragging) return;
        e.preventDefault();
    });

    sliderContainer.addEventListener('touchend', (e) => {
        if (!isDragging) return;
        isDragging = false;

        const endX = e.changedTouches[0].clientX;
        const diffX = startX - endX;

        if (Math.abs(diffX) > 50) { // Minimum swipe distance
            if (diffX > 0) {
                changeSlide(1); // Swipe left - next slide
            } else {
                changeSlide(-1); // Swipe right - previous slide
            }
        } else {
            startSlideShow();
        }
    });
}

const sidebar = document.getElementById("sidebar");
const overlay = document.getElementById("sidebarOverlay");
const toggleBtn = document.getElementById("sidebarToggle");
const mainContent = document.getElementById('mainContent');

// Gracefully handle pages where sidebar or toggle may not exist
if (toggleBtn) {
    // Toggle function handles both mobile (slide-in) and desktop (collapse) behaviors
    const toggleSidebar = () => {
        const isMobile = window.matchMedia('(max-width: 768px)').matches;

        if (isMobile) {
            // Mobile: slide-in using 'show' class and display overlay
            sidebar && sidebar.classList.toggle('show');
            overlay && overlay.classList.toggle('show');
            const expanded = sidebar && sidebar.classList.contains('show');
            toggleBtn.setAttribute('aria-expanded', expanded ? 'true' : 'false');
        } else {
            // Desktop: collapse/expand the sidebar (width change) and adjust main content
            sidebar && sidebar.classList.toggle('collapsed');
            mainContent && mainContent.classList.toggle('expanded');
            // aria-expanded reflects that the sidebar is currently expanded
            const expanded = sidebar && !sidebar.classList.contains('collapsed');
            toggleBtn.setAttribute('aria-expanded', expanded ? 'true' : 'false');
        }
    };

    toggleBtn.addEventListener('click', (e) => {
        e.preventDefault();
        toggleSidebar();
    });

    // Overlay click should close slide-in sidebar on mobile
    overlay && overlay.addEventListener('click', () => {
        if (sidebar && sidebar.classList.contains('show')) {
            sidebar.classList.remove('show');
        }
        if (overlay && overlay.classList.contains('show')) {
            overlay.classList.remove('show');
        }
        toggleBtn.setAttribute('aria-expanded', 'false');
    });

    // Close mobile sidebar with Escape key
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            if (sidebar && sidebar.classList.contains('show')) {
                sidebar.classList.remove('show');
            }
            if (overlay && overlay.classList.contains('show')) {
                overlay.classList.remove('show');
            }
            toggleBtn.setAttribute('aria-expanded', 'false');
        }
    });
}



   
    
