using System.Net;
using System.Text.Json;
using MoodleSdk;
using MoodleSdk.Core;
using Xunit;
using Moq;
using Moq.Protected;

namespace MoodleSdk.Tests;

public class MoodleClientTests
{
    private readonly MoodleOptions _options;

    public MoodleClientTests()
    {
        _options = new MoodleOptions
        {
            BaseUrl = new Uri("https://moodle.mine.edu.ng/"),
            DefaultToken = "my_token"
        };
    }

    [Fact]
    public async Task ExecuteAsync_WithSimpleParameters_EncodesCorrectly()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = new MoodleClient(httpClient, _options, Enumerable.Empty<MoodleSdk.Hooks.IMoodleClientHook>());

        var request = new MoodleRequest
        {
            Function = "core_user_get_users",
            Parameters = new Dictionary<string, object>
            {
                { "criteria[0][key]", "id" },
                { "criteria[0][value]", "123" }
            }
        };

        await client.ExecuteAsync<object>(request);

        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri!.Query.Contains("wsfunction=core_user_get_users") &&
                req.RequestUri.Query.Contains("criteria%5B0%5D%5Bkey%5D=id") &&
                req.RequestUri.Query.Contains("criteria%5B0%5D%5Bvalue%5D=123")
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithPOST_UsesFormUrlEncodedContent()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = new MoodleClient(httpClient, _options, Enumerable.Empty<MoodleSdk.Hooks.IMoodleClientHook>());

        var request = new MoodleRequest
        {
            Function = "core_user_create_users",
            Method = HttpMethod.Post,
            Parameters = new Dictionary<string, object>
            {
                { "users[0][username]", "jdoe" }
            }
        };

        await client.ExecuteAsync<object>(request);

        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.Content is FormUrlEncodedContent
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task ExecuteAsync_WhenMoodleReturnsError_ReturnsFailureResult()
    {
        var errorJson = "{\"exception\":\"moodle_exception\",\"errorcode\":\"invalidparameter\",\"message\":\"Invalid parameter value detected\"}";
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(errorJson)
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var client = new MoodleClient(httpClient, _options, Enumerable.Empty<MoodleSdk.Hooks.IMoodleClientHook>());

        var result = await client.ExecuteAsync<object>(new MoodleRequest { Function = "test" });

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("invalidparameter", result.Error.ErrorCode);
    }
}
