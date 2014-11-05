using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Should;
using System.Web.Mvc;
using Moq.Mvc;
using System.Web;
using System.Web.SessionState;
using Moq;
using Should.Core;

namespace Harbour.RedisTempData.Test
{
    public class DefaultTempDataUserProviderTests
    {
        private const string cookieName = "aid";

        private readonly ControllerContext context = new ControllerContext();
        private readonly HttpContextMock httpContext = new HttpContextMock();
        private readonly Mock<ISessionIDManager> sessionIdManager = new Mock<ISessionIDManager>();
        private readonly HttpCookieCollection requestCookies = new HttpCookieCollection();
        private readonly HttpCookieCollection responseCookies = new HttpCookieCollection();

        public DefaultTempDataUserProviderTests()
        {
            httpContext.Setup(ctx => ctx.Session).Returns((HttpSessionStateBase)null);
            httpContext.Setup(ctx => ctx.Request.Cookies).Returns(requestCookies);
            httpContext.Setup(ctx => ctx.Response.Cookies).Returns(responseCookies);
            httpContext.Setup(ctx => ctx.Items).Returns(new Dictionary<object, object>());
            sessionIdManager.Setup(m => m.Validate(It.IsAny<string>())).Returns(true);
            context.HttpContext = httpContext.Object;
        }

        [Fact]
        void uses_the_current_session_id_if_the_session_is_available()
        {
            httpContext.Setup(ctx => ctx.Session.SessionID).Returns("session12345");
            var sut = new DefaultTempDataUserProvider(cookieName, sessionIdManager.Object);

            var result = sut.GetUser(context);

            result.ShouldEqual("session12345");
        }

        [Fact]
        void uses_the_request_anonymous_id_module_value_if_it_is_available()
        {
            httpContext.Setup(ctx => ctx.Request.AnonymousID).Returns("anon12345");
            var sut = new DefaultTempDataUserProvider(cookieName, sessionIdManager.Object);

            var result = sut.GetUser(context);

            result.ShouldEqual("anon12345");
        }

        [Fact]
        void when_the_user_is_not_authenticated_and_no_cookie_exists_it_issues_and_uses_a_new_cookie()
        {
            SetupSessionIds("a-1", "b-2", "c-3");
            var sut = new DefaultTempDataUserProvider(cookieName, sessionIdManager.Object);

            var result = sut.GetUser(context);

            result.ShouldEqual("a-1");
            var cookie = GetCookie(responseCookies);
            cookie.ShouldNotBeNull();
            cookie.Value.ShouldEqual("a-1");
        }

        [Fact]
        void when_the_user_is_not_authenticated_but_a_cookie_already_exists_it_uses_the_existing_cookie()
        {
            var sut = new DefaultTempDataUserProvider(cookieName, sessionIdManager.Object);
            AddCookie(requestCookies, "existing-value");

            var result = sut.GetUser(context);

            result.ShouldEqual("existing-value");
        }

        [Fact]
        void when_the_user_is_authenticated_it_uses_the_user_name()
        {
            var sut = new DefaultTempDataUserProvider(cookieName, sessionIdManager.Object);
            UseAuthenticatedUser("john.doe");

            var result = sut.GetUser(context);

            result.ShouldEqual("john.doe");
        }

        [Fact]
        void when_the_user_was_previously_anonymous_with_anonymous_id_module_and_is_now_authenticated_it_uses_the_anonymous_id()
        {
            var sut = new DefaultTempDataUserProvider(cookieName, sessionIdManager.Object);
            httpContext.Setup(x => x.Request.AnonymousID).Returns("anon12345");
            UseAuthenticatedUser("current-user");

            var result = sut.GetUser(context);

            result.ShouldEqual("anon12345");
        }

        [Fact]
        void when_the_user_was_previously_anonymous_with_custom_cookie_and_is_now_authenticated_it_uses_and_deletes_the_old_cookie()
        {
            var sut = new DefaultTempDataUserProvider(cookieName, sessionIdManager.Object);
            AddCookie(requestCookies, "cookie-value");
            UseAuthenticatedUser("current-user");

            var result = sut.GetUser(context);

            result.ShouldEqual("cookie-value");
            var cookie = GetCookie(responseCookies);
            cookie.ShouldNotBeNull();
            cookie.Expires.ShouldEqual(DateTime.UtcNow.AddYears(-1), DatePrecision.Hour);
        }

        [Fact]
        void multiple_calls_with_the_same_context_uses_the_same_value()
        {
            SetupSessionIds("a-1", "b-2", "c-3");
            var sut = new DefaultTempDataUserProvider(cookieName, sessionIdManager.Object);

            var result1 = sut.GetUser(context);
            var result2 = sut.GetUser(context);
            var result3 = sut.GetUser(context);

            result1.ShouldEqual("a-1");
            result2.ShouldEqual("a-1");
            result3.ShouldEqual("a-1");
        }
        
        private void UseAuthenticatedUser(string userName)
        {
            httpContext.Setup(c => c.Request.IsAuthenticated).Returns(true);
            httpContext.Setup(c => c.User.Identity.Name).Returns(userName);
        }

        private void SetupSessionIds(params string[] sessionIds)
        {
            sessionIdManager.Setup(m => m.CreateSessionID(httpContext.Object.ApplicationInstance.Context))
                .ReturnsInOrder(sessionIds);
        }

        private HttpCookie GetCookie(HttpCookieCollection cookies)
        {
            return cookies[cookieName];
        }

        private HttpCookie AddCookie(HttpCookieCollection cookies, string value)
        {
            var cookie = new HttpCookie(cookieName, value);
            cookies.Add(cookie);
            return cookie;
        }
    }
}
