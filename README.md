# 📚 BookLink - Community Book Lending, Borrowing and Purchasing Platform

[![.NET Version](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/en-us/download)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-blue)](https://dotnet.microsoft.com/apps/aspnet)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE.txt)

BookLink is a comprehensive full-stack web application built with ASP.NET Core MVC that serves as both a book lending platform and marketplace.
It enables users to list their books for lending or selling, borrow books from other users, purchase books, and manage their personal book collections.
<br> Features secure authentication, Stripe payments, Full text searching, Pagination and admin dashboard.

<div align="center">
  <a href="https://github.com/hadialshaer"><strong>👤 View Profile</strong></a>
  <br />
  <br />
  <a href="https://github.com/hadialshaer/BookLink-WebApp-MVC/issues/new?labels=bug&template=bug_report.md">🐞 Report Bug</a>
  ·
  <a href="https://github.com/hadialshaer/BookLink-WebApp-MVC/issues/new?labels=enhancement&template=feature_request.md">✨ Request Feature</a>
</div>

![Azure](https://upload.wikimedia.org/wikipedia/commons/a/a8/Microsoft_Azure_Logo.svg)
<br>
<br>
Deployed to Microsoft Azure: https://booklink-webapp-mvc-dzbrdshae8d7gyb9.canadacentral-01.azurewebsites.net
<br>
## 📋 Table of Contents
- [Overview](#-overview)
- [Key Features](#-key-features)
- [System Architecture](#-system-architecture)
- [Technical Stack](#-technical-stack)
- [Installation Guide](#-installation-guide)
- [Project Structure](#-project-structure)
- [Functional Requirements](#-functional-requirements)
- [Database Design](#-database-design)
- [UML Diagrams](#-uml-diagrams)
- [Deployment](#-deployment)
- [Roadmap](#-roadmap)
- [Contributing](#-contributing)
- [Contact](#-contact)

## 🔍 Overview
BookLink is a comprehensive digital library and book marketplace platform that connects book enthusiasts, enabling them to browse, purchase, lend, and borrow books within a community-centered environment.<br>
This application is built using ASP.NET Core MVC with a multi-tier architecture that separates data access, business logic, and presentation layers for maintainability and scalability.<br>
BookLink implements role-based authorization with admin, member, and guest access levels, each with specific permissions and capabilities.<br>
The platform features an intuitive user interface built with Bootstrap 5, rich text editing capabilities via TinyMCE, secure payment processing through Stripe integration, and social authentication options for streamlined user registration and login.

## 🚀 Key Features
### Multi-Role System
- **Admin**: Books, Categories, Orders, and User management.
- **Member**: Book lending, borrowing, purchasing, and  Books, Borrowings, Account, and wishlist management.
- **Guest**: Browse books and limited viewing capabilities.

### Book Management
- Complete CRUD operations for books.
- Rich text editing with TinyMCE for book descriptions.
- Book categorization.
- Image upload and management.

### Lending System
- Request/approve/reject book borrowing.
- Duration tracking with automated reminders.
- Return book workflow.

### Wishlist Functionality
- Save books for future reference.
- Quick transfer from wishlist to cart or borrow request.

### E-Commerce Features
- Shopping cart with session management.
- Secure checkout process.
- Stripe payment integration.

### Advanced Search
- Full-text search with database indexing.
- Filter books by category, status, and availability.
- Sort results by various criteria.

### Email Notifications
- Order confirmations.
- Email account confirmations.

### Responsive UI
- Clean interface with Bootstrap 5 components.

### Social Authentication
- Google & Facebook login options.
- Secure user authentication workflows in addition to QR-Code token generation.
- Profile management.

### Admin Dashboard
- User management tools.
- Order and transaction oversight.
- Content management system.

### Advanced ASP.NET Core Features
- View Components and Partial Views.
- Authentication and Authorization.
- Repository Pattern.
- N-tier Architecture.
- Session Management.
- Pagination.
- Full-text Indexing.

## 🏗 System Architecture

BookLink follows a clean N-tier architecture pattern:

### Presentation Layer - `BookLink (MVC Project)`
- Areas for Admin, Member, and Guest roles.
- Views, Controllers, and ViewComponents.
- JavaScript, CSS, and static assets.

### Business Logic Layer - `BookLink.Models`
- Domain models.
- View models for UI representation.
- Business logic and validation.

### Data Access Layer - `BookLink.DataAccess`
- Entity Framework Core context.
- Repository pattern implementation.
- Database migrations.

### Utility Layer - `BookLink.Utility`
- Shared constants and helpers.
- Email services.
- Stripe integration utilities.

## 💻 Technical Stack

### Backend
- **ASP.NET Core 9.0 MVC**

### ORM
- **Entity Framework Core 9.0**

### Database
- **SQL Server 2024**

### Frontend
- **HTML5, CSS3, JavaScript**
- **Bootstrap 5**
- **jQuery**
- **Ajax**

### Authentication
- **ASP.NET Core Identity**

### Payment Processing
- **Stripe API**

### Rich Text Editing
- **TinyMCE**

### Email Service
- **SMTP integration**

### Social Authentication
- **Google, Facebook OAuth**

### Deployment
- **Microsoft Azure**

### 🔧 Installation Guide
## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) that includes Full-text indexing
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/)

## Setup Steps

1. **Clone the repository:**
    ```bash
    git clone https://github.com/hadialshaer/BookLink-WebApp-MVC.git
    cd Booklink-Webapp-MVC
    ```

2. **Update the connection string in `appsettings.json`:**
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER;Database=BookLink;Trusted_Connection=True;TrustServerCertificate=True"
    }
    ```

3. **Configure required services in `appsettings.json`:**
    ```json
    "TinyMCE": {
      "ApiKey": "YOUR_TINYMCE_API_KEY"
    },
    "EmailSettings": {
      "SenderEmail": "YOUR_EMAIL",
      "Password": "YOUR_EMAIL_PASSWORD",
      "SmtpServer": "smtp.gmail.com",
      "Port": 587,
      "SenderName": "Booklink"
    },
    "Stripe": {
      "SecretKey": "YOUR_STRIPE_SECRET_KEY",
      "PublishableKey": "YOUR_STRIPE_PUBLISHABLE_KEY"
    },
    "Authentication": {
      "Google": {
        "ClientId": "YOUR_GOOGLE_CLIENT_ID",
        "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
      },
      "Facebook": {
        "AppId": "YOUR_FACEBOOK_APP_ID",
        "AppSecret": "YOUR_FACEBOOK_APP_SECRET"
      }
    }
    ```

4. **Apply database migrations:**
    ```bash
    dotnet ef database update
    ```

    Or through Package Manager Console:
    ```bash
    Update-Database
    ```

5. **Build and run the project:**
    ```bash
    dotnet build
    dotnet run --project BookLink
    ```

### 📁 Project Structure
    ```
    BookLink.sln
    ├── BookLink/                           # Main MVC application
    │   ├── Areas/                          # Feature-specific areas
    │   │   ├── Admin/                      # Admin functionality
    │   │   ├── Guest/                      # Guest functionality
    │   │   ├── Identity/                   # User authentication
    │   │   └── Member/                     # Member functionality
    │   ├── ViewComponents/                 # Reusable UI components
    │   ├── Views/                          # Shared views
    │   └── wwwroot/                        # Static assets
    ├── BookLink.DataAccess/                # Data access layer
    │   ├── Data/                           # EF Core context
    │   ├── DbInitializer/                  # Database seeding
    │   ├── Migrations/                     # EF Core migrations
    │   └── Repository/                     # Repository pattern
    ├── BookLink.Models/                    # Business models
    │   └── ViewModels/                     # UI-specific models
    └── BookLink.Utility/                   # Helper classes
    ```
## 📋 Functional Requirements
The complete functional requirements document is available in the [Requirements.md file](./docs/Requirements.md).

## 🗄 DataBase Design
`For a detailed clear ER Diagram` => https://bit.ly/4i4V7kw

## 📊 UML Diagrams
Various UML diagrams are available to help understand the system architecture:
- **Activity Diagrams**: Transaction workflows for Borrow Book, Purchase Book and Ship Order
- **Use Case Diagram**: User interaction patterns<br>
`For a detailed clear Visualization Diagrams` => https://bit.ly/41pCTES

## 🚀 Deployment
BookLink is deployed on Microsoft Azure using the following services:

- **App Service**: Hosts the web application.
- **SQL Database**: Managed SQL Server instance.
- **Key Vault**: Secures connection strings and API keys.

## 🛣 Roadmap
Here are some planned enhancements for future releases:

- **Visual Search**: Integrate AI/ML tools (e.g., Google Vision API) to allow users to upload book cover images for instant identification.
- **Advanced Filters**: Add price ranges, location-based sorting, and book condition grading (e.g., “New,” “Used”).
- **AR Previews**: Enable users to visualize books on their shelves using augmented reality.
- **Book Clubs & Forums**: Create spaces for readers to discuss literature, host virtual events, and share recommendations.
- **Reviews & Ratings**: Let users rate and review books, sellers, and borrowers.
- **Reminders**: Send reminders for due dates or overdue books via email or push notifications.
- **Social Sharing**: Add referral rewards and social media integration for users to highlight their bookshelves or wish lists.
- **Carousel**: Use a carousel to highlight trending books, new arrivals, and user recommendations.
- **Automated Workflows**: Introduce a 72-hour timeout for unresponsive borrow requests and penalties for overdue returns (e.g., account restrictions).
- **Notifications**: Notify users when a book they are interested in becomes available.
- **Performance Optimization**: Implement loading animations and background processes to enhance speed and responsiveness.
- **AI-Driven Features**: Develop personalized recommendations based on user behavior and integrate a chatbot for real-time support.
- **Camera Integration**: Allow direct upload of book details via smartphone cameras.
- **Virtual Bookshelves**: Let users curate digital shelves with customizable themes.
- **Audiobook Previews**: Partner with platforms to offer audio snippets for pre-borrowing engagement.
- **Seasonal Campaigns**: Host themed events like “Back-to-School Book Drives” to boost engagement.
- **Newsletters**: Share curated reading lists, event updates, and exclusive offers.

## 🤝 Contributing
Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are greatly appreciated.

### Steps to Contribute:
1. **Fork the repository**.
2. **Create your feature branch**:  
   `git checkout -b feature/AmazingFeature`
3. **Commit your changes**:  
   `git commit -m 'Add some AmazingFeature'`
4. **Push to the branch**:  
   `git push origin feature/AmazingFeature`
5. **Open a Pull Request**.

## 📞 Contact
**Hadi Al-Shaer**

- GitHub: [@hadialshaer](https://github.com/hadialshaer)
- Email: [hadialshaer.dev@gmail.com](mailto:hadialshaer.dev@gmail.com)

⚠️ **Disclaimer**: This application integrates with external services like Stripe and email providers. Ensure API keys and sensitive data are handled securely and comply with each service’s terms of use.











