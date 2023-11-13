using System.Diagnostics;
using System.Net;
using FluentAssertions;
using Xunit;

namespace beers_of_belgium.bob_backend.Tests;

public class UsersEndpointTests : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly Process? _backendProcess;

    public UsersEndpointTests()
    {
        _httpClient = new();
        _backendProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = @"run --project ..\..\..\..\beers-of-belgium.bob-backend",
            UseShellExecute = true
        });

        _backendProcess.Should().NotBeNull();
        _backendProcess!.HasExited.Should().BeFalse();
    }
    
    [Fact]
    public async Task GetUsers_IsCalledWithoutToken_ReturnsStatus401()
    {
        var response = await _httpClient.GetAsync("http://localhost:5031/users");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public void Dispose()
    {
        _backendProcess?.Kill();
    }
}