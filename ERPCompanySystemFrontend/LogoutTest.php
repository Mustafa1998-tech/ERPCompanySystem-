<?php
declare(strict_types=1);

use PHPUnit\Framework\TestCase;

class LogoutTest extends TestCase
{
    private $session;
    
    protected function setUp(): void
    {
        parent::setUp();
        $this->session = $_SESSION;
        $_SESSION = [];
    }
    
    protected function tearDown(): void
    {
        parent::tearDown();
        $_SESSION = $this->session;
    }
    
    public function testLogoutDestroysSession()
    {
        // Arrange
        $_SESSION['user_id'] = 123;
        $_SESSION['role'] = 'admin';
        
        // Act
        require_once __DIR__ . '/../logout.php';
        
        // Assert
        $this->assertEmpty($_SESSION);
    }
    
    public function testLogoutRedirectsToLogin()
    {
        // Arrange
        $_SERVER['HTTP_HOST'] = 'localhost';
        $_SERVER['REQUEST_URI'] = '/logout.php';
        
        ob_start(); // Start output buffering
        
        // Act
        require_once __DIR__ . '/../logout.php';
        
        // Assert
        $headers = headers_list();
        $this->assertContains('Location: login.php', $headers);
    }
}
