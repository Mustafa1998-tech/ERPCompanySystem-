<?php
session_start();
require_once 'config.php';

// Check if user is logged in and has admin role
if (!isset($_SESSION['user']) || $_SESSION['user']['role'] !== 'Admin') {
    header('Location: login.php');
    exit;
}
?>

<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>نظام ERP - إدارة المستخدمين</title>
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
                    <h1 class="h2">إدارة المستخدمين</h1>
                    <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#addUserModal">
                        <i class="bi bi-person-plus"></i> إضافة مستخدم جديد
                    </button>
                </div>

                <div class="table-responsive">
                    <table class="table table-striped table-sm">
                        <thead>
                            <tr>
                                <th>اسم المستخدم</th>
                                <th>الاسم الكامل</th>
                                <th>البريد الإلكتروني</th>
                                <th>رقم الهاتف</th>
                                <th>الدور</th>
                                <th>تاريخ الإنشاء</th>
                                <th>آخر تسجيل دخول</th>
                                <th>الإجراءات</th>
                            </tr>
                        </thead>
                        <tbody id="usersTableBody">
                            <!-- Users will be populated by JavaScript -->
                        </tbody>
                    </table>
                </div>
            </main>
        </div>
    </div>

    <!-- Add User Modal -->
    <div class="modal fade" id="addUserModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">إضافة مستخدم جديد</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <form id="addUserForm">
                        <div class="mb-3">
                            <label for="username" class="form-label">اسم المستخدم</label>
                            <input type="text" class="form-control" id="username" required>
                        </div>
                        <div class="mb-3">
                            <label for="fullName" class="form-label">الاسم الكامل</label>
                            <input type="text" class="form-control" id="fullName" required>
                        </div>
                        <div class="mb-3">
                            <label for="email" class="form-label">البريد الإلكتروني</label>
                            <input type="email" class="form-control" id="email" required>
                        </div>
                        <div class="mb-3">
                            <label for="phoneNumber" class="form-label">رقم الهاتف</label>
                            <input type="tel" class="form-control" id="phoneNumber">
                        </div>
                        <div class="mb-3">
                            <label for="password" class="form-label">كلمة المرور</label>
                            <input type="password" class="form-control" id="password" required>
                        </div>
                        <div class="mb-3">
                            <label for="role" class="form-label">الدور</label>
                            <select class="form-select" id="role" required>
                                <option value="User">مستخدم</option>
                                <option value="Manager">مدير</option>
                                <option value="Admin">مدير النظام</option>
                            </select>
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إلغاء</button>
                    <button type="button" class="btn btn-primary" id="addUserBtn">إضافة</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Edit User Modal -->
    <div class="modal fade" id="editUserModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">تعديل مستخدم</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <form id="editUserForm">
                        <input type="hidden" id="editUserId">
                        <div class="mb-3">
                            <label for="editUsername" class="form-label">اسم المستخدم</label>
                            <input type="text" class="form-control" id="editUsername" required>
                        </div>
                        <div class="mb-3">
                            <label for="editFullName" class="form-label">الاسم الكامل</label>
                            <input type="text" class="form-control" id="editFullName" required>
                        </div>
                        <div class="mb-3">
                            <label for="editEmail" class="form-label">البريد الإلكتروني</label>
                            <input type="email" class="form-control" id="editEmail" required>
                        </div>
                        <div class="mb-3">
                            <label for="editPhoneNumber" class="form-label">رقم الهاتف</label>
                            <input type="tel" class="form-control" id="editPhoneNumber">
                        </div>
                        <div class="mb-3">
                            <label for="editPassword" class="form-label">كلمة المرور الجديدة (إذا كنت تريد تغييرها)</label>
                            <input type="password" class="form-control" id="editPassword">
                        </div>
                        <div class="mb-3">
                            <label for="editRole" class="form-label">الدور</label>
                            <select class="form-select" id="editRole" required>
                                <option value="User">مستخدم</option>
                                <option value="Manager">مدير</option>
                                <option value="Admin">مدير النظام</option>
                            </select>
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إلغاء</button>
                    <button type="button" class="btn btn-primary" id="editUserBtn">حفظ التغييرات</button>
                </div>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="js/users.js"></script>
</body>
</html>
