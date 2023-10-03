using FileConverter.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileConverter.Repositories.Services{
    public interface IFile
    {
        ApiResponse ConvertFile(IFormFile file);
    }
}
