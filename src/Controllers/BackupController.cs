using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using CashTrack.Services.ExportService;
using CashTrack.Common;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace CashTrack.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class BackupController : ControllerBase
    {
        private readonly IExportService _exportService;
        private readonly IWebHostEnvironment _env;
        public BackupController(IExportService exportService, IWebHostEnvironment env) => (_exportService, _env) = (exportService, env);
        [HttpGet("raw")]
        public async Task<ActionResult> GetRawData()
        {
            if (!_env.EnvironmentName.Equals(CashTrackEnv.Docker))
                return Ok();

            try
            {
                var filePath = await _exportService.ExportData(0, false);
                byte[] fileBytes = FileUtilities.GetFileBytesAndDeleteFile(filePath);

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Zip, Path.GetFileName(filePath) + ".zip");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("readable")]
        public async Task<ActionResult> GetReadableData()
        {
            if (!_env.EnvironmentName.Equals(CashTrackEnv.Docker))
                return Ok();

            try
            {
                var filePath = await _exportService.ExportData(0, true);
                byte[] fileBytes = FileUtilities.GetFileBytesAndDeleteFile(filePath);

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Zip, Path.GetFileName(filePath) + ".zip");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
