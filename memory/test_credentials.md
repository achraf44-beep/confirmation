# Wassel Test Credentials

## Admin Account
- **Email:** admin@wassel.dz
- **Password:** Admin123!
- **Role:** Owner (full access)

## Agent Account
- **Email:** agent@wassel.dz
- **Password:** Agent123!
- **Role:** Agent (orders read/write)

## API Authentication
All protected endpoints require a Bearer token obtained from login:

```bash
# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@wassel.dz","password":"Admin123!"}'

# Use token
curl -X GET http://localhost:5000/api/orders \
  -H "Authorization: Bearer <token>"
```

## Database
- **Connection String:** Server=(localdb)\\MSSQLLocalDB;Database=WasselDb;Trusted_Connection=True;TrustServerCertificate=True
- **Docker:** Server=sqlserver;Database=WasselDb;User Id=sa;Password=Wassel@2024!

## Integration API Keys (to be configured)
- GOOGLE_SHEETS_CREDENTIALS_PATH: /app/integrations/credentials/google_sheets_credentials.json
- FACEBOOK_ACCESS_TOKEN: (user provided)
- WHATSAPP_ACCESS_TOKEN: (user provided)
