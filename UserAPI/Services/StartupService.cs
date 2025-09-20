namespace UserAPI.Services
{
    public class StartupService
    {
        public static void Register(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IUserService, UserService>();
        }
    }
}
