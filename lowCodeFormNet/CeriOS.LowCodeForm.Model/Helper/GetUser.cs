using Microsoft.AspNetCore.Http;

namespace CeriOS.LowCodeForm.Model.Helper
{
    public static class GetUser
    {
        public static string GetUserIdByHttpContext(IHttpContextAccessor _httpContextAccessor)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var obj = httpContext.User.Claims.ToList();
            var userId = "";
            foreach (var item in obj)
            {
                if (item.Type == "ptid")
                {
                    userId = item.Value;
                }
            }
            return userId;
        }
    }
}
