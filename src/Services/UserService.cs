//using AutoMapper;
//using CashTrack.Data.Entities;
//using CashTrack.Models.UserModels;
//using CashTrack.Repositories.UserRepository;
//using System.Threading.Tasks;

//namespace CashTrack.Services.UserService;

//public interface IUserService
//{
//    Task<UserModels.Response> GetUserByIdAsync(int id);
//    Task<UserModels.Response[]> GetAllUsersAsync();
//}
//public class UserService : IUserService
//{
//    private readonly IUserRepository _userRepo;
//    private readonly IMapper _mapper;

//    public UserService(IUserRepository userRepo, IMapper mapper) => (_userRepo, _mapper) = (userRepo, mapper);

//    public async Task<UserModels.Response[]> GetAllUsersAsync()
//    {
//        var users = await _userRepo.Find(x => true);
//        return _mapper.Map<UserModels.Response[]>(users);
//    }

//    public async Task<UserModels.Response> GetUserByIdAsync(int id)
//    {
//        var user = await _userRepo.FindById(id);
//        return _mapper.Map<UserModels.Response>(user);
//    }
//}
//public class UserMapperProfile : Profile
//{
//    public UserMapperProfile()
//    {
//        CreateMap<Users, UserModels.Response>()
//            .ForMember(u => u.id, o => o.MapFrom(src => src.id))
//            .ForMember(u => u.FirstName, o => o.MapFrom(src => src.first_name))
//            .ForMember(u => u.LastName, o => o.MapFrom(src => src.last_name))
//            .ForMember(u => u.Email, o => o.MapFrom(src => src.email));
//    }
//}
