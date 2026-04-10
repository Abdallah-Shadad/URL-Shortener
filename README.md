# URL Shortener API

A production-ready RESTful API that shortens long URLs, handles redirects, and protects each user's links with JWT authentication. Built with ASP.NET Core, Entity Framework Core, and SQL Server.

## Tech Stack

- ASP.NET Core 9
- Entity Framework Core + SQL Server
- JWT Bearer Authentication
- BCrypt password hashing
- AutoMapper
- Swagger UI

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server (LocalDB works fine)
- Visual Studio 2022 or VS Code

### Setup

1. Clone the repository
```bash
git clone https://github.com/Abdallah-Shadad/UrlShortenerAPI.git
cd UrlShortenerAPI
```

2. Set up user secrets
```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=UrlShortenerDB;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Key" "your-super-secret-key-minimum-32-characters"
dotnet user-secrets set "Jwt:Issuer" "UrlShortenerAPI"
dotnet user-secrets set "Jwt:Audience" "UrlShortenerAPIUsers"
dotnet user-secrets set "AppSettings:BaseUrl" "https://localhost:<YOUR_PORT>" // put your port
```

3. Apply migrations
```bash
dotnet ef database update
```

4. Run the project
```bash
dotnet run
```

5. Open Swagger at `https://localhost:{PORT}/swagger`  // put your port

## API Endpoints

### Auth

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | /api/auth/register | Register a new user | No |
| POST | /api/auth/login | Login and receive JWT token | No |

### URLs

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | /api/urls | Get all URLs for current user | Yes |
| GET | /api/urls/{id} | Get a specific URL | Yes |
| POST | /api/urls | Shorten a new URL | Yes |
| PUT | /api/urls/{id} | Update a URL | Yes |
| DELETE | /api/urls/{id} | Delete a URL | Yes |

### Redirect

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | /{code} | Redirect to original URL | No |

## Authentication

This API uses JWT Bearer tokens. After login or register, include the token in every protected request:

```
Authorization: Bearer YOUR_TOKEN_HERE
```

## Project Structure

```
UrlShortenerAPI/
├── Controllers/
│   ├── BaseController.cs
│   ├── AuthController.cs
│   ├── UrlsController.cs
│   └── RedirectController.cs
├── Data/
│   └── AppDbContext.cs
├── DTOs/
│   ├── CreateUrlDto.cs
│   ├── UpdateUrlDto.cs
│   └── UrlResponseDto.cs
├── Mappings/
│   └── UrlMappingProfile.cs
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
├── Models/
│   ├── User.cs
│   └── ShortenedUrl.cs
├── Repositories/
│   ├── IUrlRepository.cs
│   └── UrlRepository.cs
├── Services/
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── IUrlService.cs
│   └── UrlService.cs
└── Program.cs
```

## How It Works

1. User registers and receives a JWT token
2. User sends a long URL to `POST /api/urls`
3. API generates a unique 8-character code using Base64 encoding
4. Code is stored in the database linked to the user
5. API returns the short URL reconstructed from BaseUrl + Code
6. Anyone can visit `/{code}` and get redirected to the original URL
7. Expired URLs return a clear error message

## Security

- Passwords hashed with BCrypt
- JWT tokens expire after 7 days
- IDOR protection — users cannot access each other's URLs
- Expired URLs rejected at redirect time
- Secrets stored in user-secrets locally, environment variables in production
- CORS restricted to allowed origins in production

## Future Enhancements

- Analytics — track clicks per URL with timestamps
- Custom codes — let users choose their own short code
- URL deduplication — return existing code if same URL shortened twice
- Rate limiting — prevent abuse
- Admin dashboard
