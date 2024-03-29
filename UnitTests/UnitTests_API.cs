﻿using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using System.Reflection;
using UnitTests.PolicyEvaluators;
using Xunit.Abstractions;

namespace UnitTests
{
    public class UnitTests_API(ITestOutputHelper testOutputHelper) : UnitTests(testOutputHelper, new UnitTestPolicyEvaluator())
    {
        [Theory]
        [InlineData("Test/Hello")]
        public async Task GetAsync(string url)
        {
            // Arrange
            StandardOut($"Arranging");

            // Act
            StandardOut($"Acting");
            HttpResponseMessage response = await GetHttpRequest(url);

            // Assert
            StandardOut($"Asserting");
            string responseContent = await response.Content.ReadAsStringAsync();
        }
    }
}
