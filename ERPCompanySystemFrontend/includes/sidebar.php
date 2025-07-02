<?php
if (!isset($_SESSION['user'])) {
    header('Location: login.php');
    exit;
}
?>

<nav id="sidebarMenu" class="col-md-3 col-lg-2 d-md-block bg-light sidebar collapse">
    <div class="position-sticky pt-3">
        <ul class="nav flex-column">
            <li class="nav-item">
                <a class="nav-link <?php echo basename($_SERVER['PHP_SELF']) === 'index.php' ? 'active' : ''; ?>" href="index.php">
                    <i class="bi bi-speedometer2"></i>
                    لوحة التحكم
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link <?php echo basename($_SERVER['PHP_SELF']) === 'clients.php' ? 'active' : ''; ?>" href="clients.php">
                    <i class="bi bi-people"></i>
                    العملاء
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link <?php echo basename($_SERVER['PHP_SELF']) === 'products.php' ? 'active' : ''; ?>" href="products.php">
                    <i class="bi bi-box"></i>
                    المنتجات
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link <?php echo basename($_SERVER['PHP_SELF']) === 'sales.php' ? 'active' : ''; ?>" href="sales.php">
                    <i class="bi bi-cart-plus"></i>
                    المبيعات
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link <?php echo basename($_SERVER['PHP_SELF']) === 'purchases.php' ? 'active' : ''; ?>" href="purchases.php">
                    <i class="bi bi-cart-check"></i>
                    المشتريات
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link <?php echo basename($_SERVER['PHP_SELF']) === 'inventory.php' ? 'active' : ''; ?>" href="inventory.php">
                    <i class="bi bi-boxes"></i>
                    المخزون
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link <?php echo basename($_SERVER['PHP_SELF']) === 'reports.php' ? 'active' : ''; ?>" href="reports.php">
                    <i class="bi bi-graph-up"></i>
                    التقارير
                </a>
            </li>
            <?php if ($_SESSION['user']['role'] === 'Admin'): ?>
            <li class="nav-item">
                <a class="nav-link <?php echo basename($_SERVER['PHP_SELF']) === 'users.php' ? 'active' : ''; ?>" href="users.php">
                    <i class="bi bi-people-fill"></i>
                    إدارة المستخدمين
                </a>
            </li>
            <?php endif; ?>
        </ul>
    </div>
</nav>
