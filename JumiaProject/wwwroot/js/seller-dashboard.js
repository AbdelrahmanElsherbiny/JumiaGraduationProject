document.addEventListener('DOMContentLoaded', function() {
    const wrapper = document.getElementById('wrapper');
    const sidebar = document.getElementById('sidebar');
    const toggleBtn = document.getElementById('sidebarToggleBtn');
    const pageContent = document.getElementById('page-content-wrapper');

    // Check for saved state
    const sidebarState = localStorage.getItem('sidebarState');
    if (sidebarState === 'collapsed') {
        sidebar.classList.add('collapsed');
        wrapper.classList.add('collapsed');
    }

    // Toggle sidebar
    toggleBtn.addEventListener('click', function(e) {
        e.preventDefault();
        sidebar.classList.toggle('collapsed');
        wrapper.classList.toggle('collapsed');
        
        // Save state
        localStorage.setItem('sidebarState', 
            sidebar.classList.contains('collapsed') ? 'collapsed' : 'expanded'
        );
    });

    // Handle responsive behavior
    function handleResize() {
        if (window.innerWidth <= 768) {
            sidebar.classList.add('collapsed');
            wrapper.classList.add('collapsed');
        } else {
            // Restore saved state on desktop
            const savedState = localStorage.getItem('sidebarState');
            if (savedState === 'collapsed') {
                sidebar.classList.add('collapsed');
                wrapper.classList.add('collapsed');
            } else {
                sidebar.classList.remove('collapsed');
                wrapper.classList.remove('collapsed');
            }
        }
    }

    // Initial check
    handleResize();

    // Listen for window resize
    window.addEventListener('resize', handleResize);

    // Add active class to current nav item
    const currentPath = window.location.pathname;
    document.querySelectorAll('.nav-link').forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
        }
    });
});