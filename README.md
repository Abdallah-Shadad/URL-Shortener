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

---
> *Note: This repository primarily focuses on robust Backend architecture. 
  The included Frontend is an AI-assisted prototype designed purely to demonstrate and consume the API's capabilities in a real-world scenario.*

## 🖥️ Frontend Application

The repository includes a fully functional, premium Single Page Application (SPA) built to interact seamlessly with the API. 

### Frontend Features
- **Modern UI/UX:** Clean aesthetic with soft shadows, smooth transitions, and a mobile-responsive layout.
- **Vanilla Power:** Built entirely without heavy frameworks (No React, No Angular) to demonstrate strong core web fundamentals.
- **Dynamic State Management:** Handles JWT persistence, login/register toggling, and UI state securely on the client side.
- **Instant Copy:** Utilizes the modern `navigator.clipboard` API for one-click short URL copying.
- **Toast Notifications:** Custom-built sliding toast system for real-time user feedback (success/error handling).
- **Graceful Error Handling:** Catches ASP.NET Core validation errors and displays them neatly to the user instead of generic failure messages.

### Frontend Tech Stack
- HTML5 (Semantic Structure)
- CSS3 (CSS Variables, Flexbox, Custom Animations)
- Vanilla JavaScript (ES6+, Async/Await, Fetch API)
- FontAwesome 6 (Icons)
- Google Fonts (Inter)

### Running the Frontend Locally

Since the frontend is built with vanilla web technologies, it requires no node modules, build steps, or bundlers.

1. Ensure backend API is running (e.g., on `https://localhost:7174`).
2. Navigate to the `Frontend` folder.
3. Open `app.js` and verify the `BACKEND_URL` matches your local API port:
   ```javascript
     const BACKEND_URL = 'https://localhost:7174'; // Update this if your port differs
   ```
