using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Xunit.Abstractions;

namespace UnitTests
{
    public abstract class UnitTests : IDisposable
    {
        protected readonly ITestOutputHelper _testOutputHelper;

        private readonly string _className;
        private string _methodName;
        private HttpClient? _httpClient;

        public UnitTests(ITestOutputHelper testOutputHelper, IPolicyEvaluator? policyEvaluator = null)
        {
            _testOutputHelper = testOutputHelper;
            _className = this.GetType().Name.Trim();
            _methodName = string.Empty;

            if (policyEvaluator != null)
            {
                UnitTestWebApplicationFactory<Program> factory = new(policyEvaluator);
                _httpClient = factory.CreateClient();
                _httpClient.Timeout = TimeSpan.FromMinutes(10);
                _testOutputHelper.WriteLine($"{_className}.Constuctor - Created Http Client");
            }
        }
        
        private void SetMethodName()
        {
            _methodName = string.Empty;
            StackTrace stackTrace = new();

            if (stackTrace != null)
            {
                List<StackFrame> stackFrames = [.. stackTrace.GetFrames()];
                int index = stackFrames.TakeWhile(x => x.GetILOffset() != StackFrame.OFFSET_UNKNOWN).Count();

                if (index < stackFrames.Count)
                    _methodName = (stackTrace.GetFrame(index - 1)?.GetMethod()?.Name ?? string.Empty).Trim();
                else
                {
                    int count = stackFrames.Where(x => x.GetILOffset() == StackFrame.OFFSET_UNKNOWN).Count();
                    if (count > 0)
                        _methodName = (stackTrace.GetFrame(index - 1)?.GetMethod()?.Name ?? string.Empty).Trim();
                }
            }
        }

        protected void StandardOut(string message)
        {
            string writeMessage = $"{_className}";

            if (string.IsNullOrEmpty(_methodName))
                SetMethodName();

            if (!string.IsNullOrEmpty(_methodName))
                writeMessage += $".{_methodName}";

            writeMessage += $" - ";
            writeMessage += message.Trim();

            _testOutputHelper.WriteLine(writeMessage);
        }

        protected void SetHttpBearerToken(string token)
        {
            if (_httpClient == null)
                throw new HttpRequestException("Http Client is null");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _testOutputHelper.WriteLine($"{_className}.SetHttpBearerToken - Set Http Client Bearer Token");
        }

        protected async Task<HttpResponseMessage> GetHttpRequest(string url)
        {
            if (_httpClient == null)
                throw new HttpRequestException("Http Client is null");

            _testOutputHelper.WriteLine($"{_className}.GetHttpRequest - Sending HttpGet Request");
            HttpRequestMessage httpRequest = new(HttpMethod.Get, url);
            HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequest);
            httpResponse.EnsureSuccessStatusCode();

            return httpResponse;
        }

        protected async Task<HttpResponseMessage> PostFormUrlEncodedHttpRequest(string url, Dictionary<string, string> content)
        {
            if (_httpClient == null)
                throw new HttpRequestException("Http Client is null");

            _testOutputHelper.WriteLine($"{_className}.PostFormUrlEncodedHttpRequest - Sending HttpPost Request");
            HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(content)
            };
            HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequest);
            httpResponse.EnsureSuccessStatusCode();

            return httpResponse;
        }

        protected async Task<HttpResponseMessage> PostJsonHttpRequest(string url, Object content)
        {
            if (_httpClient == null)
                throw new HttpRequestException("Http Client is null");

            _testOutputHelper.WriteLine($"{_className}.PostJsonHttpRequest - Sending HttpPost Request");
            HttpRequestMessage httpRequest = new(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
            };
            HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequest);
            httpResponse.EnsureSuccessStatusCode();

            return httpResponse;
        }

        public void Dispose()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
                _testOutputHelper.WriteLine($"{_className}.Dispose - Disposed Http Client");
            }
        }
    }
}
