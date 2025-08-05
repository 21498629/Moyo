using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Moyo.Models;
using Moyo.View_Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ITOKEN SERVICE
builder.Services.AddScoped<ITokenService, TokenService>();

// IREPOSITORY
builder.Services.AddScoped<IRepository, Repository>();


builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    // PASSWORD RESTRICTIONS
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 10;
})
  //.AddRoles<Role>()
  .AddEntityFrameworkStores<AppDbContext>()
  .AddDefaultTokenProviders();
  //.AddSignInManager<SignInManager<User>>()
  //.AddRoleManager<RoleManager<Role>>()
  //.AddUserManager<UserManager<User>>();

// JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        ),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

// AUTHORIZATION POLICIES
builder.Services.AddAuthorization(options =>
{
    // Match either "scope" (space-delimited) or "scp" (Azure style)
    options.AddPolicy("scope:api.read", policy => policy.RequireAssertion(ctx =>
        ctx.User.HasClaim(c => (c.Type == "scope" || c.Type == "scp") &&
                               c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                      .Contains("api.read"))));

    options.AddPolicy("scope:api.write", policy => policy.RequireAssertion(ctx =>
        ctx.User.HasClaim(c => (c.Type == "scope" || c.Type == "scp") &&
                               c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                      .Contains("api.write"))));

    // Example policy that requires both scope and a role
    options.AddPolicy("orders.manage", policy => policy.RequireAssertion(ctx =>
        ctx.User.IsInRole("Admin") &&
        ctx.User.HasClaim(c => (c.Type == "scope" || c.Type == "scp") &&
                               c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                      .Contains("api.write"))));
});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MOYO API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

var app = builder.Build();

var scope = app.Services.CreateScope();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

string[] roles = { "Admin", "User", "Vendor" };

foreach (var roleName in roles)
{
    if (!await roleManager.RoleExistsAsync(roleName))
    {
        await roleManager.CreateAsync(new IdentityRole<int> { Name = roleName, NormalizedName = roleName.ToUpper() });
    }
}

   // SEED ADMIN USER

    /*var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    var adminEmail = "admin@example.com";
    var adminPassword = "Admin@123";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var user = new User
        {
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = adminEmail,
            NormalizedEmail = adminEmail.ToUpper(),
            Name = "Admin",
            Surname = "User",
            Address = "System",
            PhoneNumber = "0000000000",
            EmailConfirmed = true,
            UserRole = 1
        };

        var result = await userManager.CreateAsync(user, adminPassword);

        if (result.Succeeded)
        {
            var addRoleResult = await userManager.AddToRoleAsync(user, "Admin");

            if (!addRoleResult.Succeeded)
            {
                foreach (var error in addRoleResult.Errors)
                    Console.WriteLine($"Error assigning Admin role: {error.Description}");
            }
        }
        else
        {
            foreach (var error in result.Errors)
                Console.WriteLine($"Error creating admin user: {error.Description}");
        }
    }
}*/


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
