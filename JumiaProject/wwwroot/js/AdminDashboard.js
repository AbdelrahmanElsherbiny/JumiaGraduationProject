// Function to check if Chart.js is loaded and initialize charts
function initializeAllCharts() {
    // First, check if Chart is defined
    if (typeof Chart === 'undefined') {
        console.error('Chart.js is not loaded. Please make sure Chart.js is properly included before running this script.');
        // Try to load Chart.js dynamically
        loadChartJS(function () {
            // Retry initialization after loading
            initializeChartsAfterLoad();
        });
        return;
    }

    initializeChartsAfterLoad();
}

// Function to dynamically load Chart.js if it's not already loaded
function loadChartJS(callback) {
    console.log('Attempting to load Chart.js dynamically...');
    var script = document.createElement('script');
    script.src = 'https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.9.1/chart.min.js';
    script.onload = callback;
    script.onerror = function () {
        console.error('Failed to load Chart.js dynamically.');
    };
    document.head.appendChild(script);
}

// Function to initialize all charts after ensuring Chart.js is loaded
function initializeChartsAfterLoad() {
    // Check if each chart canvas exists before creating it
    if (document.getElementById('monthlySalesChart')) {
        createMonthlySalesChart();
    }

    if (document.getElementById('categoryDistributionChart')) {
        createCategoryDistributionChart();
    }

    if (document.getElementById('topSellingProductsChart')) {
        createTopSellingProductsChart();
    }

    if (document.getElementById('orderStatusChart')) {
        createOrderStatusChart();
    }

    if (document.getElementById('mostViewedProductsChart')) {
        createMostViewedProductsChart();
    }
}

// Call this function immediately to initialize charts
initializeAllCharts();

// Chart creation functions
function createMonthlySalesChart() {
    const ctx = document.getElementById('monthlySalesChart').getContext('2d');
    const labels = Object.keys(dashboardData.monthlySales);
    const data = Object.values(dashboardData.monthlySales);

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Sales ($)',
                data: data,
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 2,
                pointRadius: 4,
                pointBackgroundColor: 'rgba(54, 162, 235, 1)',
                tension: 0.3
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return '$' + value;
                        }
                    }
                }
            },
            plugins: {
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return '$' + context.parsed.y;
                        }
                    }
                }
            }
        }
    });
}

function createCategoryDistributionChart() {
    const ctx = document.getElementById('categoryDistributionChart').getContext('2d');
    const labels = Object.keys(dashboardData.categoryDistribution);
    const data = Object.values(dashboardData.categoryDistribution);

    // Generate colors
    const backgroundColors = generateColors(labels.length);

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: backgroundColors,
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right',
                    labels: {
                        boxWidth: 12
                    }
                }
            }
        }
    });
}

function createTopSellingProductsChart() {
    const ctx = document.getElementById('topSellingProductsChart').getContext('2d');
    const products = dashboardData.topSellingProducts;

    // Limit to top 7 products for better visualization
    const limitedProducts = products.slice(0, 10);

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: limitedProducts.map(p => p.name),
            datasets: [{
                label: 'Units Sold',
                data: limitedProducts.map(p => p.sales),
                backgroundColor: 'rgba(75, 192, 192, 0.7)',
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            indexAxis: 'y',
            scales: {
                x: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Units Sold'
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        afterLabel: function (context) {
                            const index = context.dataIndex;
                            return 'Price: $' + limitedProducts[index].price.toFixed(2);
                        }
                    }
                }
            }
        }
    });
}

function createOrderStatusChart() {
    const ctx = document.getElementById('orderStatusChart').getContext('2d');
    const orderStatusData = dashboardData.recentOrderStatus;

    // Generate colors based on status
    const backgroundColors = orderStatusData.map(status => {
        switch (status.status) {
            case 'Delivered': return 'rgba(40, 167, 69, 0.7)';
            case 'Pending': return 'rgba(255, 193, 7, 0.7)';
            case 'Cancelled': return 'rgba(220, 53, 69, 0.7)';
            default: return 'rgba(108, 117, 125, 0.7)';
        }
    });

    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: orderStatusData.map(s => s.status),
            datasets: [{
                data: orderStatusData.map(s => s.count),
                backgroundColor: backgroundColors,
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right'
                }
            }
        }
    });
}

function createMostViewedProductsChart() {
    const ctx = document.getElementById('mostViewedProductsChart').getContext('2d');
    const viewedProducts = dashboardData.mostViewedProducts;

    // Limit to top 7 products for better visualization
    const limitedProducts = viewedProducts.slice(0, 10);

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: limitedProducts.map(p => p.name),
            datasets: [{
                label: 'Views',
                data: limitedProducts.map(p => p.views),
                backgroundColor: 'rgba(153, 102, 255, 0.7)',
                borderColor: 'rgba(153, 102, 255, 1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            indexAxis: 'y',
            scales: {
                x: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Number of Views'
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    });
}
function createMonthlySalesChart() {
    const ctx = document.getElementById('monthlySalesChart').getContext('2d');
    const multiYearData = dashboardData.multiYearMonthlySales;

    // Get all years and sort them
    const years = Object.keys(multiYearData).map(Number).sort((a, b) => a - b);
    const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    // Prepare datasets for each year
    const datasets = years.map(year => {
        const yearData = multiYearData[year];
        const data = monthNames.map(month => yearData[month] || 0);

        // Generate a distinct color for each year
        const hue = (year % 5) * 72; // Spread hues around color wheel
        const color = `hsla(${hue}, 70%, 50%, 0.7)`;
        const borderColor = `hsla(${hue}, 70%, 40%, 1)`;

        return {
            label: year.toString(),
            data: data,
            backgroundColor: color,
            borderColor: borderColor,
            borderWidth: 2,
            pointRadius: 3,
            tension: 0.3
        };
    });

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: monthNames,
            datasets: datasets
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return '$' + value.toLocaleString();
                        }
                    },
                    title: {
                        display: true,
                        text: 'Sales Amount'
                    }
                },
                x: {
                    title: {
                        display: true,
                        text: 'Month'
                    }
                }
            },
            plugins: {
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return context.dataset.label + ': $' + context.parsed.y.toLocaleString();
                        }
                    }
                },
                legend: {
                    position: 'top',
                    labels: {
                        boxWidth: 12,
                        padding: 20
                    }
                }
            },
            interaction: {
                mode: 'nearest',
                intersect: false
            }
        }
    });
}
// Add this to AdminDashboard.js
document.querySelectorAll('.chart-card-header .btn-group button').forEach(btn => {
    btn.addEventListener('click', function () {
        // Update active state
        document.querySelectorAll('.chart-card-header .btn-group button').forEach(b => {
            b.classList.remove('active');
        });
        this.classList.add('active');

        // Reload data with new year range
        const years = parseInt(this.dataset.years);
        loadMultiYearSalesData(years);
    });
});

async function loadMultiYearSalesData(years) {
    try {
        const response = await fetch(`/Admin/GetMultiYearSalesData?years=${years}`);
        const data = await response.json();

        // Update the chart
        updateMonthlySalesChart(data);
    } catch (error) {
        console.error('Error loading multi-year sales data:', error);
    }
}

function updateMonthlySalesChart(multiYearData) {
    // Similar to createMonthlySalesChart but updates existing chart
    const chart = Chart.getChart('monthlySalesChart');
    if (!chart) return;

    const years = Object.keys(multiYearData).map(Number).sort((a, b) => a - b);
    const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    chart.data.datasets = years.map(year => {
        const yearData = multiYearData[year];
        const data = monthNames.map(month => yearData[month] || 0);
        const hue = (year % 5) * 72;
        const color = `hsla(${hue}, 70%, 50%, 0.7)`;
        const borderColor = `hsla(${hue}, 70%, 40%, 1)`;

        return {
            label: year.toString(),
            data: data,
            backgroundColor: color,
            borderColor: borderColor,
            borderWidth: 2,
            pointRadius: 3,
            tension: 0.3
        };
    });

    chart.update();
}

// Helper function to generate colors for charts
function generateColors(count) {
    const colorPalette = [
        'rgba(255, 99, 132, 0.7)',    // Red
        'rgba(54, 162, 235, 0.7)',    // Blue
        'rgba(255, 206, 86, 0.7)',    // Yellow
        'rgba(75, 192, 192, 0.7)',    // Green
        'rgba(153, 102, 255, 0.7)',   // Purple
        'rgba(255, 159, 64, 0.7)',    // Orange
        'rgba(199, 199, 199, 0.7)',   // Gray
        'rgba(83, 102, 255, 0.7)',    // Indigo
        'rgba(255, 99, 255, 0.7)',    // Pink
        'rgba(99, 255, 132, 0.7)',    // Light Green
    ];

    let colors = [];
    for (let i = 0; i < count; i++) {
        colors.push(colorPalette[i % colorPalette.length]);
    }

    return colors;
}
