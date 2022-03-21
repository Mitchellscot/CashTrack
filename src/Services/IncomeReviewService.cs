using AutoMapper;
using CashTrack.Common.Exceptions;
using CashTrack.Data.Entities;
using CashTrack.Models.IncomeReviewModels;
using CashTrack.Repositories.Common;
using System.Threading.Tasks;

namespace CashTrack.Services.IncomeReviewService;


public interface IIncomeReviewService
{
    Task<IncomeReviewListItem> GetIncomeReviewByIdAsync(int id);
    Task<IncomeReviewResponse> GetIncomeReviewsAsync(IncomeReviewRequest request);
    Task<bool> UpdateIncomeReviewAsync(int id);
}

public class IncomeReviewService : IIncomeReviewService
{
    private readonly IRepository<IncomeReviewEntity> _repo;
    private readonly IMapper _mapper;

    public IncomeReviewService(IRepository<IncomeReviewEntity> repo, IMapper mapper) => (_repo, _mapper) = (repo, mapper);

    public async Task<IncomeReviewListItem> GetIncomeReviewByIdAsync(int id)
    {
        var singleIncome = await _repo.FindById(id);
        return _mapper.Map<IncomeReviewListItem>(singleIncome);
    }

    public async Task<IncomeReviewResponse> GetIncomeReviewsAsync(IncomeReviewRequest request)
    {
        var income = await _repo.FindWithPagination(x => x.IsReviewed == false, request.PageNumber, request.PageSize);
        var count = await _repo.GetCount(x => x.IsReviewed == false);

        return new IncomeReviewResponse(request.PageNumber, request.PageSize, count, _mapper.Map<IncomeReviewListItem[]>(income));
    }

    public async Task<bool> UpdateIncomeReviewAsync(int id)
    {
        //only sets it to reviewed... nothing else
        //might have bugs here... if so take off change tracker in repo
        var income = await _repo.FindById(id);
        if (income == null)
            throw new IncomeNotFoundException(id.ToString());

        income.IsReviewed = true;
        return await _repo.Update(income);
    }
}

public class IncomeReviewMapper : Profile
{
    public IncomeReviewMapper()
    {
        CreateMap<IncomeReviewEntity, IncomeReviewListItem>()
            .ForMember(x => x.Id, o => o.MapFrom(src => src.Id))
            .ForMember(x => x.Date, o => o.MapFrom(src => src.Date))
            .ForMember(x => x.Amount, o => o.MapFrom(src => src.Amount))
            .ForMember(x => x.Notes, o => o.MapFrom(src => src.Notes))
            .ForMember(x => x.SuggestedCategoryId, o => o.MapFrom(src => src.SuggestedCategory.Id))
            .ForMember(x => x.SuggestedCategory, o => o.MapFrom(src => src.SuggestedCategory.Name))
            .ForMember(x => x.SuggestedSourceId, o => o.MapFrom(src => src.SuggestedSource.Id))
            .ForMember(x => x.SuggestedSource, o => o.MapFrom(src => src.SuggestedSource.Name))
            .ReverseMap();
    }
}


