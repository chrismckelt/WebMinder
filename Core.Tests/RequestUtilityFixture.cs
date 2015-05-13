using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Routing;
using NSubstitute;
using WebMinder.Core;
using Xunit;

namespace Boomer.Web.Tests.Utilities
{
    public class RequestUtilityFixture
    {
        HttpRequestBase _httpRequest;

        private const string XForwardedFor = "X_FORWARDED_FOR";
        private const string MalformedIpAddress = "MALFORMED";
        private const string DefaultIpAddress = "0.0.0.0";
        private const string GoogleIpAddress = "74.125.224.224";
        private const string MicrosoftIpAddress = "65.55.58.201";
        private const string Private24Bit = "10.0.0.0";
        private const string Private20Bit = "172.16.0.0";
        private const string Private16Bit = "192.168.0.0";
        private const string PrivateLinkLocal = "169.254.0.0";


        public RequestUtilityFixture()
        {
            var routeData = new RouteData();
            _httpRequest = Substitute.For<HttpRequestBase>();
            _httpRequest.Url.Returns(new Uri("http://www.google.com"));
            _httpRequest.UserHostAddress.Returns("1.2.3.4");
            _httpRequest.ServerVariables.Returns(new NameValueCollection { { "X_FORWARDED_FOR", "1.7.8.9" } });
            _httpRequest.UserAgent.Returns("Mozilla/5.0 (iPhone; CPU iPhone OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3");
            var context = Substitute.For<HttpContextBase>();
            context.Request.Returns(_httpRequest);
            context.Request.RequestContext.RouteData.Returns(routeData);

        }


        [Fact]
        public void PublicIpAndNullXForwardedFor_Returns_CorrectIp()
        {
            // Arrange
            _httpRequest.UserHostAddress.Returns(GoogleIpAddress);
            _httpRequest.ServerVariables[XForwardedFor] = string.Empty;

            // Act
            var ip = RequestUtility.GetClientIpAddress(_httpRequest);

            // Assert
            Assert.Equal(ip, GoogleIpAddress);
        }


        [Fact]
        public void MalformedUserHostAddress_Returns_DefaultIpAddress()
        {
            // Arrange
            _httpRequest.UserHostAddress.Returns(MalformedIpAddress);
            _httpRequest.ServerVariables[XForwardedFor] = null;

            // Act
            var ip = RequestUtility.GetClientIpAddress(_httpRequest);

            // Assert
            Assert.Equal(ip, DefaultIpAddress);
        }

        [Fact]
        public void MalformedXForwardedFor_Returns_DefaultIpAddress()
        {
            // Arrange
            _httpRequest.UserHostAddress.Returns(GoogleIpAddress);
            _httpRequest.ServerVariables[XForwardedFor] = (MalformedIpAddress);

            // Act
            var ip = RequestUtility.GetClientIpAddress(_httpRequest);

            // Assert
            Assert.Equal(ip, DefaultIpAddress);
        }

        [Fact]
        public void SingleValidPublicXForwardedFor_Returns_XForwardedFor()
        {
            // Arrange
            _httpRequest.UserHostAddress.Returns(GoogleIpAddress);
            _httpRequest.ServerVariables[XForwardedFor] = (MicrosoftIpAddress);

            // Act
            var ip = RequestUtility.GetClientIpAddress(_httpRequest);

            // Assert
            Assert.Equal(ip, MicrosoftIpAddress);
        }

        [Fact]
        public void MultipleValidPublicXForwardedFor_Returns_LastXForwardedFor()
        {
            // Arrange
            _httpRequest.UserHostAddress.Returns(GoogleIpAddress);
            _httpRequest.ServerVariables[XForwardedFor] = (GoogleIpAddress + "," + MicrosoftIpAddress);

            // Act
            var ip = RequestUtility.GetClientIpAddress(_httpRequest);

            // Assert
            Assert.Equal(ip, MicrosoftIpAddress);
        }

        [Fact]
        public void SinglePrivateXForwardedFor_Returns_UserHostAddress()
        {
            // Arrange
            _httpRequest.UserHostAddress.Returns(GoogleIpAddress);
            _httpRequest.ServerVariables[XForwardedFor] = (Private24Bit);

            // Act
            var ip = RequestUtility.GetClientIpAddress(_httpRequest);

            // Assert
            Assert.Equal(ip, GoogleIpAddress);
        }

        [Fact]
        public void MultiplePrivateXForwardedFor_Returns_UserHostAddress()
        {
            // Arrange
            _httpRequest.UserHostAddress.Returns(GoogleIpAddress);
            const string privateIpList = Private24Bit + "," + Private20Bit + "," + Private16Bit + "," + PrivateLinkLocal;
            _httpRequest.ServerVariables[XForwardedFor] = (privateIpList);

            // Act
            var ip = RequestUtility.GetClientIpAddress(_httpRequest);

            // Assert
            Assert.Equal(ip, GoogleIpAddress);
        }

        [Fact]
        public void MultiplePublicXForwardedForWithPrivateLast_Returns_LastPublic()
        {
            // Arrange
            _httpRequest.UserHostAddress.Returns(GoogleIpAddress);
            const string privateIpList = Private24Bit + "," + Private20Bit + "," + MicrosoftIpAddress + "," + PrivateLinkLocal;
            _httpRequest.ServerVariables[XForwardedFor] = (privateIpList);

            // Act
            var ip = RequestUtility.GetClientIpAddress(_httpRequest);

            // Assert
            Assert.Equal(ip, MicrosoftIpAddress);
        }
    }
}
