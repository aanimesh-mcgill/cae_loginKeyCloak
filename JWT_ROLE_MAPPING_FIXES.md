# JWT Role Mapping and Authorization Fixes

## Summary of Changes

This document outlines the fixes implemented to ensure the backend always relies on JWT roles for authorization and properly handles the `ClaimTypes.NameIdentifier` issue.

## Key Issues Fixed

### 1. **Always Use JWT Roles for Authorization**
- **Problem**: The code was sometimes using database user roles instead of JWT roles for authorization decisions.
- **Solution**: Created `JwtClaimsHelper` utility class to consistently extract and use JWT roles.
- **Implementation**: All POST endpoints now use `[Authorize(Roles = "...")]` which relies on JWT roles mapped in `Program.cs`.

### 2. **Fixed ClaimTypes.NameIdentifier Issues**
- **Problem**: The code expected `ClaimTypes.NameIdentifier` (sub claim) to be a valid GUID, but Keycloak might not provide it as a GUID.
- **Solution**: Created robust user ID extraction that tries multiple strategies:
  1. Try `ClaimTypes.NameIdentifier` (sub claim)
  2. Try "sub" claim directly
  3. Try "preferred_username" if it's a GUID
- **Implementation**: `JwtClaimsHelper.GetUserId()` method handles all cases gracefully.

## Files Modified

### Backend Changes

1. **`backend/Utils/JwtClaimsHelper.cs`** (NEW)
   - Utility class for JWT claims extraction
   - Methods for getting user ID, username, email, roles
   - Role checking and logging utilities

2. **`backend/Controllers/DocumentsController.cs`**
   - Updated `CreateDocument` and `UpdateDocument` methods
   - Now uses JWT roles for authorization
   - Uses JWT username for audit trail
   - Simplified user creation logic

3. **`backend/Controllers/ContentController.cs`**
   - Updated `GetAllContent`, `CreateContent`, and `UpdateContent` methods
   - Removed complex user creation logic from GET endpoints
   - Now uses JWT roles for authorization
   - Uses JWT username for audit trail

4. **`backend/Controllers/TestController.cs`** (NEW)
   - Test endpoints to verify JWT role mapping
   - Endpoints for different role combinations
   - Claims inspection endpoint

### Frontend Changes

1. **`frontend/src/pages/TestPage.js`** (NEW)
   - Test page to verify JWT role mapping
   - Tests role-based access to different endpoints
   - Shows JWT claims and role information

2. **`frontend/src/App.js`**
   - Added route for `/test` page

## How to Test the Fixes

### 1. **Backend Testing**

Start the backend and test the new endpoints:

```bash
cd backend
dotnet run
```

Test endpoints:
- `GET /api/test/claims` - Shows all JWT claims and role mapping
- `GET /api/test/admin-only` - Requires Admin role
- `GET /api/test/editor-or-admin` - Requires Editor or Admin role
- `GET /api/test/any-role` - Requires any role

### 2. **Frontend Testing**

Start the frontend and navigate to the test page:

```bash
cd frontend
npm start
```

Navigate to: `http://localhost:3000/test`

### 3. **Testing Steps**

1. **Login with different users** having different roles:
   - Admin user
   - Editor user  
   - Viewer user

2. **Check JWT Claims**:
   - Verify roles are correctly mapped from `realm_access.roles`
   - Verify user ID extraction works
   - Verify username and email extraction

3. **Test Role-Based Access**:
   - Admin user should access all endpoints
   - Editor user should access Editor+ endpoints
   - Viewer user should only access basic endpoints

4. **Test POST Endpoints**:
   - Try creating documents/content with different user roles
   - Verify authorization works correctly
   - Check audit trail uses JWT username

## Key Benefits

### 1. **Consistent Authorization**
- All authorization decisions now use JWT roles
- No more database role dependency for access control
- Real-time role changes (no need to sync database)

### 2. **Robust User ID Handling**
- Multiple fallback strategies for user ID extraction
- Works with different Keycloak configurations
- Graceful handling of missing or invalid claims

### 3. **Better Debugging**
- Comprehensive logging of JWT claims
- Clear error messages for missing claims
- Test endpoints for verification

### 4. **Simplified Code**
- Removed complex user creation logic from GET endpoints
- Centralized JWT claims handling
- Cleaner, more maintainable code

## Troubleshooting

### Common Issues

1. **401 Unauthorized on POST endpoints**
   - Check if JWT token is valid and not expired
   - Verify user has the required roles in Keycloak
   - Check backend logs for role mapping errors

2. **User ID extraction fails**
   - Check if `sub` claim exists in JWT
   - Verify `preferred_username` is available
   - Use test endpoints to inspect claims

3. **Role mapping not working**
   - Verify `realm_access.roles` exists in JWT
   - Check role names match exactly (case-sensitive)
   - Ensure Audience mapper is configured in Keycloak

### Debug Commands

1. **Check JWT claims**:
   ```bash
   curl -H "Authorization: Bearer YOUR_TOKEN" http://localhost:5000/api/test/claims
   ```

2. **Test role access**:
   ```bash
   curl -H "Authorization: Bearer YOUR_TOKEN" http://localhost:5000/api/test/admin-only
   ```

3. **Check backend logs**:
   - Look for `JwtClaimsHelper.LogAllClaims` entries
   - Check for role mapping in `OnTokenValidated` event

## Best Practices

1. **Always use JWT roles for authorization** - Never rely on database roles for access control
2. **Use the JwtClaimsHelper** - Centralized claims handling prevents inconsistencies
3. **Log JWT claims for debugging** - Use `JwtClaimsHelper.LogAllClaims` in development
4. **Test with different user roles** - Verify authorization works for all role combinations
5. **Keep tokens fresh** - Use automatic token refresh for seamless UX

## Next Steps

1. **Test thoroughly** with different user roles and scenarios
2. **Monitor logs** for any authorization issues
3. **Consider adding more test endpoints** for specific business logic
4. **Update documentation** for team members
5. **Consider implementing role-based UI components** using JWT roles 