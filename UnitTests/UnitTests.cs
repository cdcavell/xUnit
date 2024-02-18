using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Diagnostics;
using System.Reflection;
using Xunit.Abstractions;

namespace UnitTests
{
    public abstract class UnitTests : IDisposable
    {
        protected readonly ITestOutputHelper _testOutputHelper;

        private readonly HttpClient? _httpClient;
        private readonly string _className;
        private string _methodName;

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

        protected async Task<HttpResponseMessage> GetHttpResponse(string url)
        {
            if (_httpClient == null)
                throw new HttpRequestException("Http Client is null");

            _testOutputHelper.WriteLine($"{_className}.GetHttpResponse - Sending HttpGet Request");
            return await _httpClient.GetAsync(url);
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
