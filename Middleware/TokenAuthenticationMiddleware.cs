namespace WebApplication1.Middleware
{
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;

    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secretKey = "your_secret_key_here"; // Replace with your actual secret key

        public TokenAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var token = context.Request.Headers["Authorization"].ToString().Split(" ")[1];

            if (!ValidateToken(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await _next(context);
        }

        private bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

    public static class TokenAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenAuthenticationMiddleware>();
        }
    }

}
