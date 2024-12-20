# User & Service Data API Application

An ASP.NET Core web application that provides user and service data through APIs with different access levels. This project demonstrates how to build a web application with a frontend, backend APIs, and data interactions. It includes functionality for downloading PowerShell scripts and a contact form that sends emails using Microsoft Graph.

## Live Demo

The application is available for public use at [http://api.cronotech.us](http://api.cronotech.us).

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Configuration](#configuration)
  - [Running the Application](#running-the-application)
- [Usage](#usage)
  - [Accessing the Frontend](#accessing-the-frontend)
  - [Downloading PowerShell Scripts](#downloading-powershell-scripts)
- [API Endpoints](#api-endpoints)
  - [User Data API](#user-data-api)
  - [Service Data API](#service-data-api)

## Features

- **User Data API**: Provides user data accessible via API endpoints.
- **Service Data API**: Provides service data with access level restrictions.
- **Frontend Interface**: Razor Pages frontend to interact with the data.
- **Downloadable Scripts**: Allows users to download PowerShell scripts for data generation.
- **Contact Form**: Users can send messages via a contact form, which sends emails using Microsoft Graph.
- **Data Filtering and Searching**: Integrated with DataTables for enhanced data interaction.

## Technologies Used

- **ASP.NET Core** (.NET 6 or later)
- **C#**
- **Razor Pages**
- **Microsoft Graph SDK**
- **Bootstrap 5** (Darkly Theme from Bootswatch)
- **jQuery**
- **DataTables**

## Getting Started

These instructions will help you set up the project on your local machine for development and testing purposes.

### Prerequisites

- **.NET SDK**: Install the [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later.
- **Visual Studio 2022** or **Visual Studio Code**: For development.
- **Azure Account**: Required if you plan to use Microsoft Graph for sending emails.
- **Git**: For cloning the repository.

### Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/killer6oose/API_Example.git
   ```

2. **Navigate to the Project Directory**

   ```bash
   cd API_Example
   ```

3. **Install Dependencies** (may not be required if Visual Studio takes care of it)

   ```bash
   dotnet restore
   ```

## Configuration

### App Settings

Create an `appsettings.json` file in the root of the project if it doesn't exist, and add the following configuration:

```json
{
  "AzureAd": {
    "ClientId": "your-client-id",
    "TenantId": "your-tenant-id",
    "ClientSecret": "your-client-secret",
    "SenderEmail": "your-email@example.com"
  },
  "Settings": {
    // Set the PublicUrl to your preferred URL instead of localhost:7189 for deployment
    "PublicUrl": "https://localhost:7189",
    "SupportEmail": "support@yourDomain.tld"
  }
}
```

- **ClientId**, **TenantId**, **ClientSecret**: Required for Microsoft Graph API integration. You can obtain these by registering an application in Azure Active Directory.
- **SenderEmail**: The email address used to send emails via Microsoft Graph.
- **PublicUrl**: Set to your preferred URL.

### Running the Application

You can run the application using the .NET CLI or Visual Studio.

#### Using .NET CLI

```bash
dotnet run
```

#### Using Visual Studio

Open the solution file (`.sln`) in Visual Studio. Set the startup project if not already set. Press **F5** to run the application in debug mode or **Ctrl+F5** to run without debugging.

The console should now show you what port to browse.

## Usage

### Accessing the Frontend

- **Home Page**: Navigate to `/` or `/Index` to view user data.
- **Service Data Page**: Navigate to `/ServiceData` to view service data.
- **Contact Page**: Navigate to `/Contact` to access the contact form.

### Downloading PowerShell Scripts

1. On the **Home** or **Service Data** pages, click the **Download Script** button in the navbar.
2. A modal will appear with information about the script.
3. Acknowledge the disclaimer by checking the checkbox.
4. Click **Download** to download the script.
5. Rename the file from `.txt` to `.ps1` to execute the script.

## API Endpoints

Feel free to view the [Swagger page](https://api.cronotech.us/swagger) for detailed API endpoints.

### User Data API

- **Get All User Data**

  ```bash
  GET /api/UserData
  ```

- **Get User Data by Access Level**

  ```bash
  POST /api/UserData/accesslevel
  ```

  **Request Body:**

  ```json
  {
    "RequesterAccessLevel": "Admin" // or "User", "Guest"
  }
  ```

### Service Data API

- **Get Service Data by Access Level**

  ```bash
  POST /api/ServiceData/accesslevel
  ```

  **Request Body:**

  ```json
  {
    "RequesterAccessLevel": "Admin" // or "User", "Guest"
  }
  ```

- **Generate Service Data Records**

  ```json
  POST /api/ServiceData/generate
  ```
