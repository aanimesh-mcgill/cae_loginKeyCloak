# Active Directory Login System - Setup Guide

## Prerequisites

Before setting up the system, ensure you have the following installed:

- **.NET 8 SDK** - Download from https://dotnet.microsoft.com/download
- **Node.js 18+** - Download from https://nodejs.org/
- **SQL Server** - Either SQL Server Express, LocalDB, or full SQL Server instance
- **Visual Studio 2022** or **Visual Studio Code** (recommended)

## Quick Setup Instructions

### 1. Database Setup

1. **Create Database:**
   ```sql
   CREATE DATABASE LoginSystemDB;
   GO
   ```

2. **Run Schema Script:**
   - Open SQL Server Management Studio or Azure Data Studio
   - Connect to your SQL Server instance
   - Open and execute `database/schema.sql`

### 2. Backend Setup

1. **Navigate to backend directory:**
   ```bash
   cd backend
   ```

2. **Update Configuration:**
   - Open `appsettings.json`
   - Update the connection string to point to your SQL Server instance
   - Update Active Directory settings (for production) or leave as-is for demo

3. **Install Dependencies:**
   ```bash
   dotnet restore
   ```

4. **Run the API:**
   ```bash
   dotnet run
   ```
   
   The API will be available at: `https://localhost:7001`
   Swagger documentation: `https://localhost:7001/swagger`

### 3. Frontend Setup

1. **Navigate to frontend directory:**
   ```bash
   cd frontend
   ```

2. **Install Dependencies:**
   ```bash
   npm install
   ```

3. **Start the React App:**
   ```bash
   npm start
   ```
   
   The React app will be available at: `http://localhost:3000`

## Demo Credentials

For testing purposes, the system includes demo credentials:

| Username | Password | Role | Permissions |
|----------|----------|------|-------------|
| admin | admin123 | Admin | Full system access |
| editor | editor123 | Editor | Content creation and editing |
| viewer | viewer123 | Viewer | Read-only access |

## Configuration Details

### Backend Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LoginSystemDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyHereThatShouldBeAtLeast32CharactersLong",
    "Issuer": "LoginSystemAPI",
    "Audience": "LoginSystemAPI",
    "ExpireMinutes": 60
  },
  "ActiveDirectory": {
    "Domain": "yourdomain.com",
    "LdapServer": "ldap://yourdomain.com:389"
  }
}
```

### Production Configuration

For production deployment:

1. **Update JWT Key:** Generate a secure random key (at least 32 characters)
2. **Update AD Settings:** Configure your actual Active Directory domain
3. **Database Connection:** Use a production SQL Server instance
4. **HTTPS:** Ensure HTTPS is enabled
5. **Environment Variables:** Use environment variables for sensitive configuration

## Architecture Overview

### Backend (.NET 8 Web API)
- **Entity Framework Core:** Data access layer with Code First approach
- **Repository Pattern:** Clean separation of data access logic
- **JWT Authentication:** Token-based security with role-based claims
- **Active Directory Integration:** LDAP authentication with group mapping
- **Dependency Injection:** Service layer architecture

### Frontend (React 18)
- **Context API:** Global state management for authentication
- **Protected Routes:** Role-based access control at the UI level
- **Axios:** HTTP client with automatic token management
- **Modern UI:** Clean, responsive design with role-based features

### Database (MSSQL)
- **Users Table:** Store AD user mappings and application roles
- **LoginHistory Table:** Audit trail for authentication events
- **Contents Table:** Sample content for demonstrating access control

## API Endpoints

### Authentication
- `POST /api/auth/login` - Authenticate user
- `POST /api/auth/logout` - Logout user
- `GET /api/auth/me` - Get current user info

### User Management (Admin Only)
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Content Management
- `GET /api/content` - Get all content (authenticated users)
- `GET /api/content/published` - Get published content
- `GET /api/content/{id}` - Get content by ID
- `POST /api/content` - Create content (Editor/Admin)
- `PUT /api/content/{id}` - Update content (Editor/Admin)
- `DELETE /api/content/{id}` - Delete content (Admin only)

## Role-Based Access Control

### Admin Role
- Full system access
- User management (create, read, update, delete)
- Content management (full CRUD)
- System administration

### Editor Role
- Content creation and editing
- View all content
- Cannot delete content or manage users

### Viewer Role
- Read-only access to published content
- Cannot create, edit, or delete content

## Security Features

- **Active Directory Authentication:** Secure credential validation
- **JWT Tokens:** Stateless authentication with short expiration
- **Role-Based Authorization:** Fine-grained access control
- **Audit Logging:** Login attempt tracking
- **HTTPS Enforcement:** Secure communication
- **Input Validation:** Protection against malicious input

## Troubleshooting

### Common Issues

1. **Database Connection Error:**
   - Verify SQL Server is running
   - Check connection string in appsettings.json
   - Ensure database exists

2. **CORS Errors:**
   - Verify frontend is running on http://localhost:3000
   - Check CORS configuration in Program.cs

3. **Authentication Failures:**
   - In development, use demo credentials
   - For production, verify AD domain settings
   - Check JWT configuration

4. **Frontend Build Errors:**
   - Ensure Node.js version is 18+
   - Clear npm cache: `npm cache clean --force`
   - Delete node_modules and reinstall

### Development Tips

1. **Use Swagger UI:** Available at `/swagger` for API testing
2. **Check Browser Console:** For frontend debugging
3. **Use SQL Server Profiler:** For database query debugging
4. **Enable Detailed Logging:** In appsettings.json for troubleshooting

## Deployment

### Backend Deployment
1. Build the application: `dotnet publish -c Release`
2. Deploy to IIS, Azure App Service, or container
3. Configure environment variables
4. Set up SSL certificate

### Frontend Deployment
1. Build the application: `npm run build`
2. Deploy to static hosting (Azure Static Web Apps, Netlify, etc.)
3. Configure API base URL for production

## Support

For issues or questions:
1. Check the troubleshooting section
2. Review the API documentation in Swagger
3. Check browser console and network tab
4. Review server logs

## License

This project is for educational and demonstration purposes. 