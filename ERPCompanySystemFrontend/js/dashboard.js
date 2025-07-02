document.addEventListener('DOMContentLoaded', function() {
    // Get token from session
    const token = sessionStorage.getItem('token');
    
    // Fetch dashboard data
    fetchDashboardData();
    fetchSalesData();
});

async function fetchDashboardData() {
    try {
        const response = await fetch(`http://${window.location.hostname}:5184/api/reports/DailySales`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error('Failed to fetch dashboard data');
        }

        const data = await response.json();
        document.getElementById('totalSales').textContent = data.TotalSales.toFixed(2);
        document.getElementById('newClients').textContent = data.Products.length;
        document.getElementById('newPurchases').textContent = data.Products.length;
    } catch (error) {
        console.error('Error fetching dashboard data:', error);
    }
}

async function fetchSalesData() {
    try {
        const response = await fetch(`http://${window.location.hostname}:5184/api/reports/MonthlySales`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error('Failed to fetch sales data');
        }

        const data = await response.json();
        createSalesChart(data.DailySales);
    } catch (error) {
        console.error('Error fetching sales data:', error);
    }
}

function createSalesChart(data) {
    const ctx = document.getElementById('salesChart').getContext('2d');
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.map(d => d.Date),
            datasets: [{
                label: 'إجمالي المبيعات',
                data: data.map(d => d.TotalSales),
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: 'إجمالي المبيعات اليومية'
                }
            },
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}
