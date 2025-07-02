<?php
session_start();
require_once 'config.php';
require_once 'includes/functions.php';

// توليد CSRF token إذا غائب
if (!isset($_SESSION['csrf_token'])) {
    $_SESSION['csrf_token'] = bin2hex(random_bytes(32));
}

// تهيئة PDO و base_url
/** @var PDO $pdo */
$pdo      = $GLOBALS['pdo'];
$base_url = rtrim($GLOBALS['base_url'], '/');

// متغير الخطأ
$error = '';

// دوال التحقق
function isIpBlocked(PDO $pdo) {
    $ip = $_SERVER['REMOTE_ADDR'];
    $stmt = $pdo->prepare("
        SELECT COUNT(*) 
          FROM ip_blocks 
         WHERE ip_address = ? 
           AND blocked_until > NOW()
    ");
    $stmt->execute([$ip]);
    return $stmt->fetchColumn() > 0;
}

function checkRateLimit(PDO $pdo) {
    $ip = $_SERVER['REMOTE_ADDR'];
    $stmt = $pdo->prepare("
        SELECT COUNT(*) 
          FROM login_attempts 
         WHERE ip_address = ? 
           AND attempt_time > DATE_SUB(NOW(), INTERVAL 5 MINUTE)
    ");
    $stmt->execute([$ip]);
    $attempts = $stmt->fetchColumn();
    if ($attempts >= 5) {
        // احظر IP لساعة
        $stmt = $pdo->prepare("
            INSERT INTO ip_blocks (ip_address, blocked_until) 
            VALUES (?, DATE_ADD(NOW(), INTERVAL 1 HOUR))
        ");
        $stmt->execute([$ip]);
        return true;
    }
    return false;
}

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    // 1. تحقق CSRF
    if (!isset($_POST['csrf_token']) || $_POST['csrf_token'] !== $_SESSION['csrf_token']) {
        $error = 'طلب غير صالح. حاول مرة أخرى.';
    }
    // 2. تحقق حظر IP
    elseif (isIpBlocked($pdo)) {
        $error = 'تم حظر IP الخاص بك. حاول مرة أخرى بعد ساعة.';
    }
    // 3. تحقق الحد الأقصى للمحاولات
    elseif (checkRateLimit($pdo)) {
        $error = 'كثرة محاولات تسجيل الدخول. حاول مرة أخرى بعد دقيقة.';
    }
    else {
        // 4. استقبال المدخلات
        $username = filter_input(INPUT_POST, 'username', FILTER_SANITIZE_STRING) ?? '';
        $password = $_POST['password'] ?? '';

        if (strlen($password) < 8) {
            $error = 'كلمة المرور يجب أن تكون 8 أحرف على الأقل.';
        } else {
            // 5. سجل محاولة
            $ip = $_SERVER['REMOTE_ADDR'];
            $stmt = $pdo->prepare("
                INSERT INTO login_attempts (ip_address, username, attempt_time) 
                VALUES (?, ?, NOW())
            ");
            $stmt->execute([$ip, $username]);

            // 6. طلب المصادقة للـ API
            list('response' => $resp, 'http_code' => $code) = makeApiRequest('/auth/login', 'POST', [
                'username' => $username,
                'password' => $password
            ]);

            if ($code === 200 && $data = json_decode($resp, true)) {
                if (!empty($data['token'])) {
                    // خزّن التوكن في كوكي
                    setcookie('jwt', $data['token'], [
                        'expires'  => time() + 3600,
                        'path'     => '/',
                        'secure'   => true,
                        'httponly' => true,
                        'samesite' => 'Strict'
                    ]);
                    // كوكي الريفرش إذا موجود
                    if (!empty($data['refreshToken'])) {
                        setcookie('refreshToken', $data['refreshToken'], [
                            'expires'  => time() + 7*24*3600,
                            'path'     => '/',
                            'secure'   => true,
                            'httponly' => true,
                            'samesite' => 'Strict'
                        ]);
                    }
                    $_SESSION['user'] = [
                        'username'   => $username,
                        'last_login' => time()
                    ];
                    header('Location: index.php');
                    exit;
                } else {
                    $error = 'استجابة غير متوقعة من الخادم.';
                }
            } else {
                $error = 'بيانات الاعتماد غير صحيحة أو خطأ بالخادم.';
            }
        }
    }
}

// عرض النموذج (GET أو بعد POST)
?><!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>نظام ERP - تسجيل الدخول</title>
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
  <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.7.2/font/bootstrap-icons.css" rel="stylesheet">
  <style>
    body { background: #f8f9fa; }
    .login-container { min-height: 100vh; display: flex; align-items: center; }
    .login-card { border: none; border-radius: .75rem; box-shadow: 0 .5rem 1rem rgba(0,0,0,0.15); }
  </style>
</head>
<body>
  <div class="container login-container">
    <div class="row justify-content-center w-100">
      <div class="col-md-6 col-lg-4">
        <div class="card login-card">
          <div class="card-body text-center p-4">
            <!-- الشعار -->
            <img src="<?= htmlspecialchars($base_url . '/images/office.jpg') ?>"
                 alt="شعار الشركة"
                 class="mb-4 rounded"
                 style="max-width:150px;">

            <h4 class="mb-4">نظام ERP - تسجيل الدخول</h4>

            <?php if ($error): ?>
              <div class="alert alert-danger alert-dismissible fade show" role="alert">
                <?= htmlspecialchars($error) ?>
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
              </div>
            <?php endif; ?>

            <form method="POST" class="needs-validation" novalidate>
              <input type="hidden" name="csrf_token" value="<?= htmlspecialchars($_SESSION['csrf_token']) ?>">

              <div class="mb-3 text-start">
                <label class="form-label" for="username">اسم المستخدم</label>
                <div class="input-group">
                  <span class="input-group-text"><i class="bi bi-person"></i></span>
                  <input type="text" id="username" name="username" class="form-control" required>
                  <div class="invalid-feedback">من فضلك أدخل اسم المستخدم</div>
                </div>
              </div>

              <div class="mb-4 text-start">
                <label class="form-label" for="password">كلمة المرور</label>
                <div class="input-group">
                  <span class="input-group-text"><i class="bi bi-lock"></i></span>
                  <input type="password" id="password" name="password" class="form-control"
                         required minlength="8">
                  <div class="invalid-feedback">الرجاء كلمة المرور (8 أحرف على الأقل)</div>
                </div>
              </div>

              <button type="submit" class="btn btn-primary w-100 mb-3">تسجيل الدخول</button>
              <a href="forgot-password.php" class="text-muted">هل نسيت كلمة المرور؟</a>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
  <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
  <script>
    (() => {
      'use strict';
      document.querySelectorAll('.needs-validation').forEach(form => {
        form.addEventListener('submit', event => {
          if (!form.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
          }
          form.classList.add('was-validated');
        });
      });
    })();
  </script>
</body>
</html>
