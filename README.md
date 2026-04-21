This project consists of an ASP.NET Core Web API backend and a Blazor WebAssembly frontend. Follow these instructions to run the application locally on your machine.

### Prerequisites
1. Install the .NET 8 SDK (or newer).

2. Install an IDE like Visual Studio 2022 or Visual Studio Code (with the C# Dev Kit extension).

Obtain a free Gemini API Key from Google AI Studio.

Step 1: Clone the Repository
Download or clone the project repository to your local machine:

```
git clone https://github.com/eh44/OdeTutor.git
cd OdeTutor
```
Step 2: Configure Your API Key (Securely)
The backend requires a Gemini API key to evaluate student steps. To prevent accidentally committing your private key to version control, we use the .NET Secret Manager for local development.

Open your terminal and navigate to the Server project directory:

```
cd Server
```
Initialize User Secrets for the project:


```
dotnet user-secrets init
```
Set your Gemini API key (replace YOUR_API_KEY_HERE with your actual key):

```
dotnet user-secrets set "GeminiApiKey" "YOUR_API_KEY_HERE"
```
(Alternative: If you are not using Git, you can simply open Server/appsettings.Development.json and add "GeminiApiKey": "YOUR_API_KEY_HERE" to the JSON object).

Step 3: Build and Run the Application
Make sure you run both the Server and the Client
Ensure you are still in the Server directory.

Build and launch the application:

```
dotnet run
```
The terminal will output the local address where the app is hosted (typically http://localhost:5000 or https://localhost:5001).
