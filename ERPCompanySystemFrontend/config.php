<?php
// API Configuration
$base_url = 'http://localhost:5184/api';

// Database Configuration
$db_host = 'localhost';
$db_name = 'ERPCompanySystem';
$db_user = 'sa';
$db_pass = 'ERP@Company2025'; // Update with your actual SQL Server password

// Create database connection
try {
    $pdo = new PDO("sqlsrv:Server=$db_host;Database=$db_name", $db_user, $db_pass);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
} catch(PDOException $e) {
    // Store error in session instead of dying
    $_SESSION['db_error'] = "خطأ في الاتصال بقاعدة البيانات: " . $e->getMessage();
}

// Function to make API requests
function makeApiRequest($endpoint, $method = 'GET', $data = null) {
    global $base_url;
    
    $ch = curl_init();
    $url = $base_url . $endpoint;
    
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false); // For development
    curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, false); // For development
    
    if ($method === 'POST' || $method === 'PUT') {
        curl_setopt($ch, CURLOPT_POST, true);
        curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode($data));
        curl_setopt($ch, CURLOPT_HTTPHEADER, [
            'Content-Type: application/json'
        ]);
    }
    
    $response = curl_exec($ch);
    $http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    curl_close($ch);
    
    return [
        'response' => $response,
        'http_code' => $http_code
    ];
}
?>
