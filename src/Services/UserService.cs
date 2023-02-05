using AutoMapper;
using CashTrack.Common.Extensions;
using CashTrack.Data.Entities;
using CashTrack.Models.UserModels;
using CashTrack.Repositories.UserRepository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CashTrack.Services.UserService;

public interface IUserService
{
    Task<User> GetUserByIdAsync(int id);
    Task<User[]> GetAllUsersAsync();
    Task<User> CreateUserAsync(AddEditUser request);
    Task<bool> UpdatePasswordAsync(ChangePassword request);
    Task<bool> UpdateUsernameAsync(ChangeUsername request);
    Task<bool> UpdateLastImportDate(int userId);
}
public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;
    private readonly UserManager<UserEntity> _userManager;
    private readonly SignInManager<UserEntity> _signInManager;

    public UserService(IUserRepository userRepo, IMapper mapper, UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager) => (_userRepo, _mapper, _userManager, _signInManager) = (userRepo, mapper, userManager, signInManager);

    public async Task<User> CreateUserAsync(AddEditUser request)
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

        return _mapper.Map<User>(user);
    }
    public async Task<bool> UpdatePasswordAsync(ChangePassword request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            throw new ArgumentException(nameof(request.Username));

        if (string.IsNullOrEmpty(request.OldPassword) || string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.ConfirmPassword))
            throw new ArgumentNullException("There is a password missing");

        if (request.NewPassword != request.ConfirmPassword)
            throw new ArgumentException(nameof(request.ConfirmPassword));

        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!result.Succeeded)
            return false;

        return true;
    }
    public async Task<bool> UpdateUsernameAsync(ChangeUsername request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            throw new ArgumentException(nameof(request.Username));

        if (string.IsNullOrEmpty(request.NewUsername) || string.IsNullOrEmpty(request.ConfirmUsername))
            throw new ArgumentNullException("There is a password missing");

        if (!request.NewUsername.IsEqualTo(request.ConfirmUsername))
            throw new ArgumentException(nameof(request.ConfirmUsername));
        user.UserName = request.NewUsername;
        user.NormalizedUserName = request.NewUsername.ToUpper();
        var passwordCheck = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordCheck)
            return false;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return false;
        await _signInManager.RefreshSignInAsync(user);
        return true;
    }

    public async Task<User[]> GetAllUsersAsync()
    {
        var users = await _userRepo.Find(x => true);
        return _mapper.Map<User[]>(users);
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        var user = await _userRepo.FindById(id);
        return _mapper.Map<User>(user);
    }

    public async Task<bool> UpdateLastImportDate(int userId)
    {
        var user = await _userRepo.FindById(userId);
        if (user == null)
            throw new ArgumentException(nameof(user));

        user.LastImport = DateTime.Now;
        var update = await _userRepo.Update(user);
        return update > 0;
    }
}
public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<UserEntity, User>()
            .ForMember(u => u.Id, o => o.MapFrom(src => src.Id))
            .ForMember(u => u.FirstName, o => o.MapFrom(src => src.FirstName))
            .ForMember(u => u.LastName, o => o.MapFrom(src => src.LastName))
            .ForMember(u => u.Email, o => o.MapFrom(src => src.Email));
    }
}
