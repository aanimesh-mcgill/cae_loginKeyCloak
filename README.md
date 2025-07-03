# Active Directory Login System

A comprehensive login system built with React frontend, C# Web API backend, and MSSQL database that integrates with Microsoft Active Directory for authentication and role-based access control.

## Architecture

### Backend (.NET 8 Web API)
- **Entity Framework Core**: Data access layer
- **Repository Pattern**: Abstraction for data operations
- **JWT Authentication**: Token-based security
- **Active Directory Integration**: LDAP authentication
- **Dependency Injection**: Service layer architecture

### Frontend (React 18)
- **Role-based Access Control**: Different views for different user roles
- **JWT Token Management**: Secure authentication flow
- **Protected Routes**: Route-level access control
- **Modern UI**: Clean, responsive design

### Database (MSSQL)
- **User Management**: Store AD user mappings and roles
- **Login History**: Audit trail for authentication events
- **Role-based Permissions**: Application-specific access control

## Features

### User Roles
- **Admin**: Full system access, user management
- **Editor**: Content creation and editing capabilities
- **Viewer**: Read-only access to content

### Security Features
- Active Directory authentication
- JWT token-based sessions
- Role-based access control
- Login audit logging
- Secure password handling (no plain text storage)

## Project Structure

```
cae_login/
├── backend/                 # C# Web API
│   ├── Controllers/        # API endpoints
│   ├── Models/            # Entity Framework models
│   ├── Services/          # Business logic services
│   ├── Repositories/      # Data access layer
│   ├── Interfaces/        # Service contracts
│   └── Program.cs         # Application entry point
├── frontend/              # React application
│   ├── src/
│   │   ├── components/    # React components
│   │   ├── pages/         # Page components
│   │   ├── services/      # API service layer
│   │   └── hooks/         # Custom React hooks
│   └── package.json
└── database/              # Database scripts
    └── schema.sql         # MSSQL schema
```

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- SQL Server (LocalDB or full instance)
- Active Directory access

### Backend Setup
1. Navigate to `backend/` directory
2. Update `appsettings.json` with your AD domain and JWT settings
3. Run database migrations: `dotnet ef database update`
4. Start the API: `dotnet run`

### Frontend Setup
1. Navigate to `frontend/` directory
2. Install dependencies: `npm install`
3. Update API base URL in `src/services/api.js`
4. Start the app: `npm start`

### Database Setup
1. Create MSSQL database
2. Run `database/schema.sql` to create tables
3. Update connection string in `backend/appsettings.json`

## Configuration

### Active Directory Settings
Update `appsettings.json`:
```json
{
  "ActiveDirectory": {
    "Domain": "yourdomain.com",
    "LdapServer": "ldap://yourdomain.com:389"
  }
}
```

### JWT Settings
```json
{
  "Jwt": {
    "Key": "your-super-secret-key-here",
    "Issuer": "your-api",
    "Audience": "your-api",
    "ExpireMinutes": 60
  }
}
```

## API Endpoints

### Authentication
- `POST /api/auth/login` - Authenticate user
- `POST /api/auth/logout` - Logout user
- `GET /api/auth/me` - Get current user info

### User Management
- `GET /api/users` - Get all users (Admin only)
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user (Admin only)

### Content (Role-based)
- `GET /api/content` - Get content (All roles)
- `POST /api/content` - Create content (Editor/Admin)
- `PUT /api/content/{id}` - Update content (Editor/Admin)
- `DELETE /api/content/{id}` - Delete content (Admin only)

## Security Considerations

- All passwords are validated against Active Directory
- JWT tokens have short expiration times
- Role-based access control at API and UI levels
- Login attempts are logged for audit purposes
- HTTPS should be used in production

## Development Notes

- The system uses Entity Framework Code First approach
- Repository pattern provides clean separation of concerns
- Interface-based design allows for easy testing and maintenance
- React hooks provide clean state management
- Protected routes ensure proper access control

## License

This project is for educational and demonstration purposes. 