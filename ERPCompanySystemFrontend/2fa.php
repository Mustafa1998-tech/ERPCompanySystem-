<?php
session_start();
require_once 'config.php';

// Check if user is logged in
if (!isset($_SESSION['user'])) {
    header('Location: login.php');
    exit;
}

// Function to generate QR code image
function generateQRCodeImage($qrCodeData)
{
    $data = base64_decode($qrCodeData);
    $image = imagecreatefromstring($data);
    ob_start();
    imagepng($image);
    $imageData = ob_get_clean();
    imagedestroy($image);
    return 'data:image/png;base64,' . base64_encode($imageData);
}
?>

<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>نظام ERP - تفعيل المصادقة الثنائية</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.1/font/bootstrap-icons.css" rel="stylesheet">
    <link href="css/style.css" rel="stylesheet">
</head>
<body>
    <?php include 'includes/header.php'; ?>

    <div class="container-fluid">
        <div class="row">
            <?php include 'includes/sidebar.php'; ?>
            
            <main class="col-md-9 ms-sm-auto col-lg-10 px-md-4">
                <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
                    <h1 class="h2">تفعيل المصادقة الثنائية</h1>
                </div>

                <div class="card">
                    <div class="card-body">
                        <div id="setupContainer">
                            <div class="text-center mb-4">
                                <h5>خطوات تفعيل المصادقة الثنائية</h5>
                                <ol class="list-unstyled">
                                    <li>1. تحميل تطبيق المصادقة (مثل Google Authenticator)</li>
                                    <li>2. مسح الكود المربع (QR Code) باستخدام التطبيق</li>
                                    <li>3. إدخال الكود المصادق من التطبيق</li>
                                </ol>
                            </div>

                            <div id="qrCodeContainer" class="text-center mb-4">
                                <!-- QR code will be populated by JavaScript -->
                            </div>

                            <div id="secretContainer" class="alert alert-info mb-4">
                                <strong>مهم:</strong> احتفظ بـ <span id="secretKey"></span> في مكان آمن. ستحتاج إليه إذا فقدت جهازك.
                            </div>

                            <div class="mb-4">
                                <label for="verificationCode" class="form-label">أدخل الكود المصادق من التطبيق</label>
                                <input type="text" class="form-control" id="verificationCode" required>
                            </div>

                            <div class="d-grid">
                                <button class="btn btn-primary" id="verifyButton">تحقق من الكود</button>
                            </div>
                        </div>

                        <div id="successContainer" class="alert alert-success d-none">
                            تم تفعيل المصادقة الثنائية بنجاح! من الآن فصاعداً، ستحتاج إلى إدخال الكود المصادق عند تسجيل الدخول.
                        </div>

                        <div id="errorContainer" class="alert alert-danger d-none">
                            <!-- Error message will be populated by JavaScript -->
                        </div>
                    </div>
                </div>
            </main>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="js/2fa.js"></script>
</body>
</html>
