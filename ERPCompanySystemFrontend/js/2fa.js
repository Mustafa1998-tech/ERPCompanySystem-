document.addEventListener('DOMContentLoaded', function() {
    const token = sessionStorage.getItem('token');
    const setupContainer = document.getElementById('setupContainer');
    const successContainer = document.getElementById('successContainer');
    const errorContainer = document.getElementById('errorContainer');
    const verifyButton = document.getElementById('verifyButton');
    const verificationCode = document.getElementById('verificationCode');

    // Initialize 2FA setup
    async function setup2FA() {
        try {
            const response = await fetch(`${base_url}/api/twofactor/setup`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error('Failed to setup 2FA');
            }

            const data = await response.json();
            
            // Display QR code
            const qrCodeContainer = document.getElementById('qrCodeContainer');
            qrCodeContainer.innerHTML = `
                <img src="${data.qrCode}" alt="QR Code" class="img-fluid mb-3">
            `;

            // Display secret key
            document.getElementById('secretKey').textContent = data.secret;
        } catch (error) {
            showError('حدث خطأ أثناء تفعيل المصادقة الثنائية');
        }
    }

    // Verify 2FA code
    verifyButton.addEventListener('click', async () => {
        const code = verificationCode.value.trim();
        
        if (!code) {
            showError('الرجاء إدخال الكود المصادق');
            return;
        }

        try {
            const response = await fetch(`${base_url}/api/twofactor/verify`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({ token: code })
            });

            if (!response.ok) {
                throw new Error('Invalid verification code');
            }

            // Enable 2FA
            await fetch(`${base_url}/api/twofactor/enable`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({ token: code })
            });

            // Show success message and hide setup form
            setupContainer.style.display = 'none';
            successContainer.classList.remove('d-none');
        } catch (error) {
            showError('كود المصادقة غير صحيح. حاول مرة أخرى');
        }
    });

    // Show error message
    function showError(message) {
        errorContainer.textContent = message;
        errorContainer.classList.remove('d-none');
        
        // Hide error after 5 seconds
        setTimeout(() => {
            errorContainer.classList.add('d-none');
        }, 5000);
    }

    // Initial setup
    setup2FA();
});
