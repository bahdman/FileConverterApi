using FileConverter.Repositories.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace FileConverter.Controllers{
    [ApiController]
    [Route("api")]
    [EnableCors("AllowAnyOrigin")]
    public class FileController : ControllerBase{

        private readonly IFile _service;

        public FileController(IFile service)
        {
            _service = service;
        }


        [HttpPost("UploadFile")]
        public async Task<IActionResult> File(IFormFile file)
        {
            var fileExtension = System.IO.Path.GetExtension(file.FileName);
           
            if(fileExtension == ".docx" || fileExtension == ".pdf" || fileExtension == ".doc")
            {
                var response = _service.ConvertFile(file);

                if (response == null)
                {
                    return BadRequest("Unable to process document");
                }

                byte [] bytes = System.IO.File.ReadAllBytes(response.RefinedPath);
                System.Threading.Thread.Sleep(1000);

                System.IO.File.Delete(response.FilePath);
                System.IO.File.Delete(response.RefinedPath);
                return File(bytes, response.FileType, response.FileName);
            }           
                        
            return StatusCode(400, "Invalid file upload");
        }

        [HttpGet("TestApi")]
        public ActionResult Test()
        {
            return Ok("Api is a active");
        }
        
    }
}