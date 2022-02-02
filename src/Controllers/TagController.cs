using AutoMapper;
using CashTrack.Models.ExpenseModels;
using CashTrack.Models.TagModels;
using CashTrack.Repositories.TagRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CashTrack.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        //private readonly ITagRepository _tagService;

        //public TagController(ITagRepository tagService)
        //{
        //    this._tagService = tagService;
        //}
    }
}
