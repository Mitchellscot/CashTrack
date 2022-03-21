using AutoMapper;
using CashTrack.Data.Entities;
using CashTrack.Models.UserModels;
using CashTrack.Repositories.UserRepository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CashTrack.Services.UserService;

public interface IUserService
{
    Task<UserModels.Response> GetUserByIdAsync(int id);
    Task<UserModels.Response[]> GetAllUsersAsync();
    Task<UserModels.Response> CreateUserAsync(UserModels.AddEditUser request);
}
public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;
    private readonly UserManager<UserEntity> _userManager;
    private readonly SignInManager<UserEntity> _signInManager;

    public UserService(IUserRepository userRepo, IMapper mapper, UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager) => (_userRepo, _mapper, _userManager, _signInManager) = (userRepo, mapper, userManager, signInManager);

    public async Task<UserModels.Response> CreateUserAsync(UserModels.AddEditUser request)
    {
        var newUser = new UserEntity()
        {
            Email = request.Email,
            UserName = request.FirstName,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));

        var user = await _userManager.FindByEmailAsync(request.Email);

        await _signInManager.RefreshSignInAsync(user);

        return _mapper.Map<UserModels.Response>(user);
    }

    public async Task<UserModels.Response[]> GetAllUsersAsync()
    {
        var users = await _userRepo.Find(x => true);
        return _mapper.Map<UserModels.Response[]>(users);
    }

    public async Task<UserModels.Response> GetUserByIdAsync(int id)
    {
        var user = await _userRepo.FindById(id);
        return _mapper.Map<UserModels.Response>(user);
    }
}
public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<UserEntity, UserModels.Response>()
            .ForMember(u => u.id, o => o.MapFrom(src => src.Id))
            .ForMember(u => u.FirstName, o => o.MapFrom(src => src.FirstName))
            .ForMember(u => u.LastName, o => o.MapFrom(src => src.LastName))
            .ForMember(u => u.Email, o => o.MapFrom(src => src.Email));
    }
}
