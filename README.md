# ERP Company System

A comprehensive Enterprise Resource Planning (ERP) solution built with ASP.NET Core Web API and PHP frontend. This system offers robust business management capabilities with enterprise-grade security features and a modern user experience.

![ERP Company System](docs/screenshots/dashboard.png)

## 🛡️ Security Architecture

### Authentication & Authorization
- **JWT-based Authentication**
  - Secure token generation and validation
  - Token expiration and refresh mechanism
  - Token blacklisting support
  - Role-based access control
  - Permission-based authorization

- **Multi-Factor Authentication (MFA)**
  - Time-based One-Time Passwords (TOTP)
  - QR code-based 2FA setup
  - Backup code generation
  - MFA enforcement policies
  - Session-based MFA state management

### Security Features
- **IP Security**
  - IP blocking based on failed login attempts
  - Rate limiting per IP address
  - IP whitelist/blacklist management
  - GeoIP-based restrictions
  - IP activity logging

- **Request Security**
  - Input validation and sanitization
  - CSRF protection
  - XSS prevention
  - SQL injection prevention
  - Content type validation
  - Request size limits
  - JSON schema validation

- **Session Security**
  - Secure session management
  - Session timeout configuration
  - Session hijacking prevention
  - Session cleanup mechanism
  - Session activity tracking

## Key Features

### Security Features
- **Multi-Factor Authentication (MFA)**
  - Two-Factor Authentication (2FA) with QR code generation
  - Time-based One-Time Passwords (TOTP)
  - IP blocking and rate limiting
  - Failed login attempt tracking
  - User activity logging

- **Authentication & Authorization**
  - JWT-based authentication
  - Role-based access control (Admin, Manager, User)
  - Session management
  - Request validation and sanitization
  - Secure password hashing

- **Request Security**
  - Rate limiting
  - IP blocking
  - Content type validation
  - Request size limits
  - JSON schema validation

### Backend (ASP.NET Core Web API)
- RESTful API endpoints
- Entity Framework Core with SQL Server
- Comprehensive API documentation (Swagger)
- Request/response logging
- Error handling middleware
- CORS configuration
- Background job processing

### Frontend (PHP)
- Modern, responsive UI with Bootstrap 5
- RTL support for Arabic language
- Real-time analytics with Chart.js
- Secure session management
- API integration with backend
- Responsive dashboard
- RTL support for all components

## 📊 Core Modules

### Business Management
1. **Authentication & User Management**
   - User registration and login with 2FA
   - Role-based access control
   - User activity tracking
   - Session management
   - Password management

2. **Clients Management**
   - CRUD operations for clients
   - Client credit management
   - Client balance tracking
   - Client categorization
   - Client history tracking

3. **Products Management**
   - Product catalog management
   - Stock quantity tracking
   - Product categories
   - Purchase and sale prices
   - Product adjustments

4. **Sales Management**
   - Sales order processing
   - Invoice generation
   - Discount management
   - Payment tracking
   - Sales analytics

5. **Purchases Management**
   - Purchase order processing
   - Supplier management
   - Purchase tracking
   - Payment management
   - Purchase analytics

6. **Inventory Management**
   - Stock movement tracking
   - Low stock alerts
   - Product adjustments
   - Stock history
   - Inventory reports

7. **Reports & Analytics**
   - Daily sales reports
   - Monthly sales trends
   - Inventory status
   - Product movement tracking
   - Custom report generation

8. **System Management**
   - User management
   - Role management
   - Audit logging
   - System settings
   - Backup management

## 🧪 Testing & Quality Assurance

### Backend Testing
1. Install test dependencies:
```bash
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package MSTest.TestAdapter
dotnet add package MSTest.TestFramework
dotnet add package Moq
dotnet add package FluentAssertions
```

#### Backend (ASP.NET Core) Testing
1. Install test dependencies:
```bash
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package MSTest.TestAdapter
dotnet add package MSTest.TestFramework
dotnet add package Moq
dotnet add package FluentAssertions
```

2. Run tests:
```bash
dotnet test ERPCompanySystem.Tests/ERPCompanySystem.Tests.csproj
```

#### Frontend (PHP) Testing
1. Install Composer if not already installed:
   - Download from https://getcomposer.org/download/
   - Run the installer

2. Install dependencies:
```bash
cd ERPCompanySystemFrontend
composer install
```

3. Run tests:
```bash
phpunit
```

### Core Modules
1. **Authentication & User Management**
   - User registration and login with 2FA
   - Role-based access control
   - User activity tracking
   - Session management
   - Password management

2. **Clients Management**
   - CRUD operations for clients
   - Client credit management
   - Client balance tracking
   - Client categorization
   - Client history tracking

3. **Products Management**
   - Product catalog management
   - Stock quantity tracking
   - Product categories
   - Purchase and sale prices
   - Product adjustments

4. **Sales Management**
   - Sales order processing
   - Invoice generation
   - Discount management
   - Payment tracking
   - Sales analytics

5. **Purchases Management**
   - Purchase order processing
   - Supplier management
   - Purchase tracking
   - Payment management
   - Purchase analytics

6. **Inventory Management**
   - Stock movement tracking
   - Low stock alerts
   - Product adjustments
   - Stock history
   - Inventory reports

7. **Reports & Analytics**
   - Daily sales reports
   - Monthly sales trends
   - Inventory status
   - Product movement tracking
   - Custom report generation

8. **System Management**
   - User management
   - Role management
   - Audit logging
   - System settings
   - Backup management   - Role-based access control
   - Secure password hashing

2. **Clients Management**
   - CRUD operations for clients
   - Client credit management
   - Client balance tracking

3. **Products Management**
   - Product catalog management
   - Stock quantity tracking
   - Product categories
   - Purchase and sale prices

4. **Sales Management**
   - Sales order processing
   - Invoice generation
   - Discount management
   - Payment tracking

5. **Purchases Management**
   - Purchase order processing
   - Supplier management
   - Purchase tracking
   - Payment management

6. **Inventory Management**
   - Stock movement tracking
   - Low stock alerts
   - Product adjustments
   - Stock history

- Visual Studio 2022 or .NET CLI
- Windows Server 2016 or later
- Minimum 4GB RAM
- Minimum 2 CPU cores

### Frontend Requirements
- PHP 8.1 or later
- Composer (for future PHP dependencies)
- Web server (IIS, Apache, or PHP built-in server)
- Modern web browser (Chrome, Firefox, Safari)
- Minimum 2GB RAM
- Minimum 1 CPU core

## Installation Guide

### Backend Installation
1. Clone the repository:
```bash
git clone https://github.com/yourusername/ERPCompanySystem.git
```

2. Install required NuGet packages:
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.Extensions.Caching.Memory
dotnet add package Swashbuckle.AspNetCore
dotnet add package AspNetCoreRateLimit
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.IdentityModel.Tokens
dotnet add package System.Security.Cryptography.QRCode
dotnet add package System.Drawing.Common
```

3. Configure database connection in `appsettings.json`:
```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=your_server;Database=ERPCompanySystem;User Id=your_user;Password=your_password;"
    },
    "JwtSettings": {
        "Secret": "your_secret_key_here",
        "Issuer": "ERPCompanySystem",
        "Audience": "ERPCompanySystem"
    }
}
```

4. Run database migrations:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

5. Start the backend:
```bash
dotnet run
```

### Frontend Installation
1. Clone the repository:
```bash
git clone https://github.com/yourusername/ERPCompanySystem.git
```

2. Configure the frontend:
   - Edit `config.php` with your backend API URL
   - Set up database connection if needed
   - Configure session settings

3. Start the PHP development server:
```bash
php -S localhost:8000
```

## Usage Instructions

### Accessing the Application
1. Start the backend service first
2. Start the frontend server
3. Access the application at `http://localhost:8000`
4. Use the default admin credentials:
   - Username: admin
   - Password: admin123

### API Documentation
Access Swagger UI at: `https://localhost:5184/swagger`

## Security Features

### Authentication & Authorization
- JWT-based authentication
- Multi-Factor Authentication (MFA)
- Role-based access control
- Session management
- Password hashing (SHA256)

### Request Security
- Rate limiting (5 requests per minute)
- IP blocking
- Content type validation
- Request size limits
- JSON schema validation

### Data Security
- Secure database connections
- Encrypted data storage
- Regular backups
- Audit logging
- Activity tracking

## Performance Optimization

### Backend Optimization
- Caching implementation
- Database indexing
- Async operations
- Connection pooling
- Background job processing

### Frontend Optimization
- Minified assets
- Browser caching
- Lazy loading
- Responsive design
- Optimized images

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

### Code Style Guidelines
- Follow C# and PHP coding standards
- Use meaningful variable names
- Add comments for complex logic
- Write unit tests
- Follow security best practices

## License
This project is licensed under the MIT License - see the LICENSE file for details.

## Support
For support, please:
1. Check the documentation
2. Search existing issues
3. Open a new issue if needed
4. Contact the development team

### Troubleshooting
- Check logs for errors
- Verify configuration
- Check system requirements
- Review security settings
- Test API endpoints
