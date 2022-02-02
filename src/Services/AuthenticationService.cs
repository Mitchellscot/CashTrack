using CashTrack.Data;
using CashTrack.Models.AuthenticationModels;
using System.Threading.Tasks;
using BCryptNet = BCrypt.Net.BCrypt;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CashTrack.Data.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using FluentValidation;
using CashTrack.Models.UserModels;
using CashTrack.Common;

namespace CashTrack.Services.AuthenticationService;

public interface IAuthenticationService
{
    Task<AuthenticationModels.Response> AuthenticateAsync(AuthenticationModels.Request model);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly AppSettings _appSettings;

    public AuthenticationService(AppDbContext context, IMapper mapper, ILogger<AuthenticationService> logger, IOptions<AppSettings> appSettings)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _appSettings = appSettings.Value;
    }
    public async Task<AuthenticationModels.Response> AuthenticateAsync(AuthenticationModels.Request model)
    {
        var user = await _context.Users.Where(u => u.first_name.ToUpper() == model.Name.ToUpper()).FirstOrDefaultAsync();
        if (user == null || !BCryptNet.Verify(model.Password, user.password_hash))
        {
            return null;
        }
        var response = _mapper.Map<AuthenticationModels.Response>(user) with { Token = GenerateJwtToken(user) };
        _logger.LogInformation($"{response.FirstName} logged in at {DateTime.Now}");
        return response;
    }
    internal string GenerateJwtToken(Users user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim("id", user.id.ToString()),
                    //if this is buggy in the future consider changing this from email to first name
                    new Claim(ClaimTypes.Name, user.email.ToString()),
                }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
public class AuthorizationProfile : Profile
{
    public AuthorizationProfile()
    {
        CreateMap<Users, AuthenticationModels.Response>()
            .ForMember(u => u.Id, o => o.MapFrom(src => src.id))
            .ForMember(u => u.FirstName, o => o.MapFrom(src => src.first_name))
            .ForMember(u => u.LastName, o => o.MapFrom(src => src.last_name))
            .ForMember(u => u.Email, o => o.MapFrom(src => src.email));

        //I'm putting the mapping for users here since I don't have a user service
        CreateMap<Users, UserModels.Response>()
            .ForMember(u => u.id, o => o.MapFrom(src => src.id))
            .ForMember(u => u.FirstName, o => o.MapFrom(src => src.first_name))
            .ForMember(u => u.LastName, o => o.MapFrom(src => src.last_name))
            .ForMember(u => u.Email, o => o.MapFrom(src => src.email));
    }
}

