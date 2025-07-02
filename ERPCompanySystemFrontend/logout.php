<?php
try {
    // Clear session
    if (session_status() === PHP_SESSION_NONE) {
        session_start();
    }
    
    // Destroy session data
    $_SESSION = array();
    if (ini_get("session.use_cookies")) {
        $params = session_get_cookie_params();
        setcookie(session_name(), '', time() - 3600,
            $params["path"], $params["domain"],
            $params["secure"], $params["httponly"]
        );
    }
    
    // Clear JWT token from cookies
    setcookie('jwt', '', time() - 3600, '/');
    setcookie('refreshToken', '', time() - 3600, '/');
    
    // Destroy session
    session_destroy();
    
    // Redirect to login page
    header('Location: login.php');
    exit;
} catch (Exception $e) {
    error_log("Logout error: " . $e->getMessage());
    header('Location: error.php?code=500');
    exit;
}
?>
