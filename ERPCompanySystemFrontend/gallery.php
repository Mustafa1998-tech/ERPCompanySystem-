<?php
session_start();
require_once 'config.php';
require_once 'includes/header.php';

// Security check
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
    <title>معرض الصور - نظام ERP</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.7.2/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        .gallery-card {
            transition: transform 0.3s ease;
            height: 100%;
        }
        .gallery-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        }
        .gallery-card img {
            height: 200px;
            object-fit: cover;
        }
        .gallery-card .card-body {
            padding: 1rem;
        }
        .gallery-card .card-title {
            font-size: 1.2rem;
            margin-bottom: 0.5rem;
        }
        .gallery-card .card-text {
            color: #6c757d;
        }
    </style>
</head>
<body>
    <div class="container-fluid">
        <div class="row mt-4">
            <div class="col-12">
                <h2 class="text-center mb-4">معرض الصور</h2>
                <div class="row g-4">
                    <!-- صورة مكتب عمل احترافي -->
                    <div class="col-md-6 col-lg-3">
                        <div class="card gallery-card">
                            <img src="images/office.jpg" 
                                 class="card-img-top" alt="مكتب عمل احترافي">
                            <div class="card-body">
                                <h5 class="card-title">مكتب عمل احترافي</h5>
                                <p class="card-text">صورة مكتب احترافي من Pexels (ID 374074).</p>
                            </div>
                        </div>
                    </div>

                    <!-- صورة برمجية -->
                    <div class="col-md-6 col-lg-3">
                        <div class="card gallery-card">
                            <img src="images/code.jpg" 
                                 class="card-img-top" alt="برمجة وتطوير">
                            <div class="card-body">
                                <h5 class="card-title">برمجة وتطوير</h5>
                                <p class="card-text">صورة برمجة من Unsplash (ID 1518770660439).</p>
                            </div>
                        </div>
                    </div>

                    <!-- صورة تخطيط مبيعات -->
                    <div class="col-md-6 col-lg-3">
                        <div class="card gallery-card">
                            <img src="images/sales.jpg" 
                                 class="card-img-top" alt="تخطيط مبيعات">
                            <div class="card-body">
                                <h5 class="card-title">تخطيط مبيعات</h5>
                                <p class="card-text">صورة مبيعات من Pexels (ID 3183150).</p>
                            </div>
                        </div>
                    </div>

                    <!-- صورة إدارة شركات -->
                    <div class="col-md-6 col-lg-3">
                        <div class="card gallery-card">
                            <img src="images/desk.jpg" 
                                 class="card-img-top" alt="إدارة شركات">
                            <div class="card-body">
                                <h5 class="card-title">إدارة شركات</h5>
                                <p class="card-text">صورة مكتب عصري من Pexels (ID 271639).</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
