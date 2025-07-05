# Database Entity Relationship Diagram

## Overview
This document contains the Entity Relationship Diagram (ERD) for the Keycloak Login System database schema.

## ERD Diagram

```mermaid
erDiagram
    User {
        GUID Id PK
        string AdUsername UNIQUE
        string Email
        string DisplayName
        string Role
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
        datetime LastLogin
    }
    Document {
        GUID Id PK
        string Title
        string Content
        string CreatedBy
        datetime CreatedAt
        string UpdatedBy
        datetime UpdatedAt
    }
    Content {
        GUID Id PK
        string Title
        string Body
        bool IsPublished
        string CreatedBy
        datetime CreatedAt
        string UpdatedBy
        datetime UpdatedAt
    }
    User ||--o{ Document : creates
    User ||--o{ Content : creates
```

## Entity Descriptions

### User Entity
- **Id**: Primary key (GUID)
- **AdUsername**: Unique username from Keycloak/AD (UNIQUE constraint)
- **Email**: User's email address
- **DisplayName**: User's display name
- **Role**: User's role (Admin, Editor, Viewer)
- **IsActive**: Whether the user account is active
- **CreatedAt**: When the user was first created
- **UpdatedAt**: When the user was last updated (nullable in database)
- **LastLogin**: When the user last logged in (nullable in database)

### Document Entity
- **Id**: Primary key (GUID)
- **Title**: Document title
- **Content**: Document content/body
- **CreatedBy**: Username of who created the document
- **CreatedAt**: When the document was created
- **UpdatedBy**: Username of who last updated the document (nullable in database)
- **UpdatedAt**: When the document was last updated (nullable in database)

### Content Entity
- **Id**: Primary key (GUID)
- **Title**: Content title
- **Body**: Content body
- **IsPublished**: Whether the content is published
- **CreatedBy**: Username of who created the content
- **CreatedAt**: When the content was created
- **UpdatedBy**: Username of who last updated the content (nullable in database)
- **UpdatedAt**: When the content was last updated (nullable in database)

## Relationships

- **User → Document**: One-to-many relationship where a user can create multiple documents
- **User → Content**: One-to-many relationship where a user can create multiple content items

## Design Notes

1. **Audit Trail**: All entities include `CreatedAt` and `UpdatedAt` timestamps for audit purposes
2. **User References**: `CreatedBy` and `UpdatedBy` fields store usernames rather than foreign keys for simplicity and audit trail clarity
3. **Soft Deletes**: Consider implementing soft deletes if needed (add `IsDeleted` boolean field)
4. **Indexing**: Consider adding indexes on frequently queried fields like `AdUsername`, `Role`, `IsPublished`
5. **Constraints**: The `AdUsername` field has a UNIQUE constraint to prevent duplicate users

## Migration Notes

When making schema changes:
1. Create a new migration: `dotnet ef migrations add MigrationName`
2. Update the database: `dotnet ef database update`
3. Test the migration on a copy of production data first
4. Consider data migration scripts for existing data if needed 
