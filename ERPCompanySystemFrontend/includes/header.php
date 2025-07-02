<?php
// Security checks
if (!isset($_SESSION['user'])) {
    header('Location: login.php');
    exit;
}

// Validate JWT token
$jwt = $_COOKIE['jwt'] ?? '';
if (empty($jwt)) {
    header('Location: login.php');
    exit;
}

// Check session timeout (30 minutes)
if (isset($_SESSION['user']['last_login']) && 
    time() - $_SESSION['user']['last_login'] > 1800) {
    session_destroy();
    header('Location: login.php');
    exit;
}
?>

<header class="navbar navbar-dark sticky-top bg-dark flex-md-nowrap p-0 shadow">
    <a class="navbar-brand col-md-3 col-lg-2 me-0 px-3 fs-6" href="index.php">نظام ERP</a>
    <button class="navbar-toggler position-absolute d-md-none collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="navbar-nav">
        <div class="nav-item text-nowrap">
            <span class="nav-link px-3 text-white">
                <i class="bi bi-person-badge"></i>
                <?php echo htmlspecialchars($_SESSION['user']['username']); ?>
            </span>
        </div>
        <div class="nav-item text-nowrap">
            <a class="nav-link px-3 text-white" href="logout.php" onclick="return confirm('هل أنت متأكد من تسجيل الخروج؟');">
                <i class="bi bi-box-arrow-right"></i>
                تسجيل الخروج
            </a>
        </div>
    </div>
</header>

<script>
// Add CSRF token to all AJAX requests
document.addEventListener('DOMContentLoaded', function() {
    var csrfToken = '<?php echo $_SESSION['csrf_token']; ?>';
    var xhr = new XMLHttpRequest();
    xhr.open('GET', 'api/check-session.php', true);
    xhr.setRequestHeader('X-CSRF-Token', csrfToken);
    xhr.send();
});
</script>
