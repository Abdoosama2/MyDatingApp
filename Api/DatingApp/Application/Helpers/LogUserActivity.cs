using DatingApp.Api.Extensions;
using DatingApp.Infrastructer.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Application.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

            var memberId = context.HttpContext.User.GetMemberId();

            var dbContext = resultContext.HttpContext.RequestServices
                .GetRequiredService<AppDbContext>();

            await dbContext.Members.Where(x => x.Id == memberId).ExecuteUpdateAsync(setter => setter.SetProperty
            (x => x.LastActive, DateTime.UtcNow));
        }
    }
}
