namespace bowling_backend_persistence_tests;

class Constants
{
    public const string SqlPassword = "Kakud*Ocebe536";
    public const string ServerConnectionString = $"Server=localhost,63000;User Id=sa;Password={SqlPassword};TrustServerCertificate=true";
    public const string DatabaseConnectionString = $"{ServerConnectionString};Database=bowling";
}