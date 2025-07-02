document.addEventListener('DOMContentLoaded', function() {
    const token = sessionStorage.getItem('token');
    const usersTableBody = document.getElementById('usersTableBody');
    const addUserBtn = document.getElementById('addUserBtn');
    const editUserBtn = document.getElementById('editUserBtn');

    // Fetch users
    async function fetchUsers() {
        try {
            const response = await fetch(`${base_url}/api/users`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error('Failed to fetch users');
            }

            const users = await response.json();
            displayUsers(users);
        } catch (error) {
            showError('حدث خطأ أثناء جلب المستخدمين');
        }
    }

    // Display users in table
    function displayUsers(users) {
        usersTableBody.innerHTML = '';
        users.forEach(user => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${user.username}</td>
                <td>${user.fullName}</td>
                <td>${user.email}</td>
                <td>${user.phoneNumber || '-'}</td>
                <td>${user.role}</td>
                <td>${user.createdAt}</td>
                <td>${user.lastLogin || '-'}</td>
                <td>
                    <button class="btn btn-sm btn-primary edit-user-btn" data-id="${user.id}">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm btn-danger delete-user-btn" data-id="${user.id}">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            `;
            usersTableBody.appendChild(row);
        });

        // Add event listeners to edit buttons
        document.querySelectorAll('.edit-user-btn').forEach(button => {
            button.addEventListener('click', () => editUser(button.dataset.id));
        });

        // Add event listeners to delete buttons
        document.querySelectorAll('.delete-user-btn').forEach(button => {
            button.addEventListener('click', () => deleteUser(button.dataset.id));
        });
    }

    // Add user
    addUserBtn.addEventListener('click', async () => {
        const formData = {
            username: document.getElementById('username').value,
            fullName: document.getElementById('fullName').value,
            email: document.getElementById('email').value,
            phoneNumber: document.getElementById('phoneNumber').value,
            password: document.getElementById('password').value,
            role: document.getElementById('role').value
        };

        try {
            const response = await fetch(`${base_url}/api/users`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(formData)
            });

            if (!response.ok) {
                throw new Error('Failed to add user');
            }

            // Close modal and refresh users
            const modal = bootstrap.Modal.getInstance(document.getElementById('addUserModal'));
            modal.hide();
            fetchUsers();
            showSuccess('تم إضافة المستخدم بنجاح');
        } catch (error) {
            showError('حدث خطأ أثناء إضافة المستخدم');
        }
    });

    // Edit user
    async function editUser(userId) {
        try {
            const response = await fetch(`${base_url}/api/users/${userId}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error('Failed to fetch user');
            }

            const user = await response.json();
            document.getElementById('editUserId').value = user.id;
            document.getElementById('editUsername').value = user.username;
            document.getElementById('editFullName').value = user.fullName;
            document.getElementById('editEmail').value = user.email;
            document.getElementById('editPhoneNumber').value = user.phoneNumber;
            document.getElementById('editRole').value = user.role;

            const editModal = new bootstrap.Modal(document.getElementById('editUserModal'));
            editModal.show();
        } catch (error) {
            showError('حدث خطأ أثناء جلب بيانات المستخدم');
        }
    }

    editUserBtn.addEventListener('click', async () => {
        const userId = document.getElementById('editUserId').value;
        const formData = {
            username: document.getElementById('editUsername').value,
            fullName: document.getElementById('editFullName').value,
            email: document.getElementById('editEmail').value,
            phoneNumber: document.getElementById('editPhoneNumber').value,
            password: document.getElementById('editPassword').value,
            role: document.getElementById('editRole').value
        };

        try {
            const response = await fetch(`${base_url}/api/users/${userId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(formData)
            });

            if (!response.ok) {
                throw new Error('Failed to update user');
            }

            // Close modal and refresh users
            const modal = bootstrap.Modal.getInstance(document.getElementById('editUserModal'));
            modal.hide();
            fetchUsers();
            showSuccess('تم تحديث بيانات المستخدم بنجاح');
        } catch (error) {
            showError('حدث خطأ أثناء تحديث بيانات المستخدم');
        }
    });

    // Delete user
    async function deleteUser(userId) {
        if (!confirm('هل أنت متأكد من حذف هذا المستخدم؟')) {
            return;
        }

        try {
            const response = await fetch(`${base_url}/api/users/${userId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error('Failed to delete user');
            }

            fetchUsers();
            showSuccess('تم حذف المستخدم بنجاح');
        } catch (error) {
            showError('حدث خطأ أثناء حذف المستخدم');
        }
    }

    // Show success message
    function showSuccess(message) {
        const alert = document.createElement('div');
        alert.className = 'alert alert-success alert-dismissible fade show';
        alert.innerHTML = `${message} <button type="button" class="btn-close" data-bs-dismiss="alert"></button>`;
        document.querySelector('main').insertBefore(alert, document.querySelector('main').firstChild);
        setTimeout(() => alert.remove(), 3000);
    }

    // Show error message
    function showError(message) {
        const alert = document.createElement('div');
        alert.className = 'alert alert-danger alert-dismissible fade show';
        alert.innerHTML = `${message} <button type="button" class="btn-close" data-bs-dismiss="alert"></button>`;
        document.querySelector('main').insertBefore(alert, document.querySelector('main').firstChild);
        setTimeout(() => alert.remove(), 3000);
    }

    // Initial load
    fetchUsers();
});
