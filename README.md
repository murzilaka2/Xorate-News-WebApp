Xorate
Xorate is a small news and rating platform built with ASP.NET Core MVC, Entity Framework Core (EF Core), and Blazor Components. The platform allows users to publish articles, create reviews, and interact with content through comments, likes, and social media sharing.

🚀 Features

📜 Publish news articles using a built-in text editor.

⭐ Create short rating reviews for various topics.

💬 User interactions:

Post comments on articles and reviews.

Like and rate content.

Share content on social media.



📁 Project Structure

Xorate/
│
├── Components/         # Blazor components for interactive UI parts
├── Controllers/        # MVC controllers managing routes and requests
├── Data/               # Database context and configurations (EF Core)
├── Helpers/            # Utility and helper classes
├── Interfaces/         # Interfaces for repositories and services (DI)
├── Models/             # Domain models: News, Reviews, Comments, Users, Ratings
├── Repository/         # Data access layer (Repository pattern)
├── ViewComponents/     # Reusable UI elements (e.g., recent articles)
├── ViewModels/         # View models for shaping data for views
├── Views/              # Razor views for rendering pages
├── wwwroot/            # Static files (CSS, JS, images)
│
├── appsettings.json    # App configuration (database, settings)
└── Program.cs          # Application entry point

🛠️ Technologies Used

ASP.NET Core MVC – server-side logic and routing.
Entity Framework Core (EF Core) – database access and ORM.
Blazor Components – dynamic and interactive UI elements.
Bootstrap/Custom CSS/JS – responsive and modern design.
