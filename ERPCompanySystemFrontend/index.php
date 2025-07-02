<?php
session_start();

// Base URL for API
$base_url = 'http://localhost:5184/api';

// Check if user is logged in
if (!isset($_SESSION['user'])) {
    header('Location: login.php');
    exit;
}
?>

<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>نظام ERP - لوحة التحكم</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.1/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        .sidebar {
            min-height: 100vh;
            background-color: #343a40;
            color: white;
        }
        .sidebar a {
            color: white;
            text-decoration: none;
        }
        .sidebar a:hover {
            background-color: #495057;
        }
    </style>
</head>
<body>
    <div class="container-fluid">
        <div class="row">
            <!-- Sidebar -->
            <nav class="col-md-3 col-lg-2 d-md-block sidebar collapse">
                <div class="position-sticky pt-3">
                    <h6 class="sidebar-heading d-flex justify-content-between align-items-center px-3 mt-4 mb-1 text-muted">
                        <span>النظام</span>
                    </h6>
                    <ul class="nav flex-column">
                        <li class="nav-item">
                            <a class="nav-link active" href="dashboard.php">
                                <i class="bi bi-speedometer2"></i>
                                لوحة التحكم
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="sales.php">
                                <i class="bi bi-cart3"></i>
                                المبيعات
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="purchases.php">
                                <i class="bi bi-box-seam"></i>
                                المشتريات
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="inventory.php">
                                <i class="bi bi-box"></i>
                                المخزون
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="clients.php">
                                <i class="bi bi-people"></i>
                                العملاء
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="reports.php">
                                <i class="bi bi-graph-up"></i>
                                التقارير
                            </a>
                        </li>
                    </ul>
                    <h6 class="sidebar-heading d-flex justify-content-between align-items-center px-3 mt-4 mb-1 text-muted">
                        <span>الإعدادات</span>
                    </h6>
                    <ul class="nav flex-column mb-2">
                        <li class="nav-item">
                            <a class="nav-link" href="settings.php">
                                <i class="bi bi-gear"></i>
                                الإعدادات
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="logout.php">
                                <i class="bi bi-box-arrow-right"></i>
                                تسجيل الخروج
                            </a>
                        </li>
                    </ul>
                </div>
            </nav>

            <!-- Main content -->
            <main class="col-md-9 ms-sm-auto col-lg-10 px-md-4">
                <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
                    <h1 class="h2">لوحة التحكم</h1>
                </div>

                <!-- Dashboard content -->
                <div class="row">
                    <div class="col-md-3 mb-4">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title">إجمالي المبيعات</h5>
                                <h2 class="card-text" id="totalSales">جاري التحميل...</h2>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3 mb-4">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title">المخزون منخفض</h5>
                                <h2 class="card-text" id="lowStock">جاري التحميل...</h2>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3 mb-4">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title">العملاء الجدد</h5>
                                <h2 class="card-text" id="newClients">جاري التحميل...</h2>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3 mb-4">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title">المشتريات الجديدة</h5>
                                <h2 class="card-text" id="newPurchases">جاري التحميل...</h2>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Recent sales chart -->
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">المبيعات الأخيرة</h5>
                        <canvas id="salesChart"></canvas>
                    </div>
                </div>
            </main>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="js/dashboard.js"></script>
</body>
</html>
