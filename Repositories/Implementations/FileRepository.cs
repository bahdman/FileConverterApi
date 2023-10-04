using System;
using FileConverter.Repositories.Services;
using Microsoft.AspNetCore.Mvc;
using Spire.Doc;
using Freeware;
using FileConverter.Models;

namespace FileConverter.Repositories.Implementations
{
    public class FileRepository : IFile{
        public ApiResponse ConvertFile(IFormFile file)
        {
            try{

                var path = string.Empty;
                var fileName = string.Empty;
                var response = new ApiResponse();


                var extension = Path.GetExtension(file.FileName);
                if(extension == ".pdf")
                {
                    var pathToDoc = SaveFileToDir(file);
                    path = PdfToWord(pathToDoc.Result);
                    // PdfToWord(file);

                    fileName = Path.GetFileName(path);
                    if(fileName == null || fileName == string.Empty)
                    {
                        return null;
                    }

                    response =  new ApiResponse()
                    {
                        FilePath = path,
                        FileType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                        FileName = fileName
                    };                

                    return response;
                }  

                path = SaveFileToDir(file).Result;
                var docPath = WordToPdf(path, file.FileName);

                var docFileName = Path.GetFileName(docPath);
                if(docFileName == null || docFileName == string.Empty)
                {
                    return null;
                }

                response =  new ApiResponse()
                {
                    FilePath = docPath,
                    FileType = "application/pdf",
                    FileName = docFileName
                };  
                return response;          
            
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                var response = new ApiResponse();
                return response;
            }            
        }




        public string PdfToWord(string filePath)
        {
            try{
                // Pdf2Docx converter = new();
                var pathToSave = filePath.Replace("Document", "Converted");
                var savePath = pathToSave.Replace(".pdf", ".docx");

                if(!Directory.Exists("Converted"))
                {
                    Directory.CreateDirectory("Converted");
                }

                SautinSoft.PdfFocus instance = new SautinSoft.PdfFocus();
                
                instance.OpenPdf(filePath);
                instance.ToWord(savePath);
                instance.ClosePdf();///////

                return savePath;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
            
        }


        private static async Task<string> SaveFileToDir(IFormFile file) //ATM, this stores file in a folder and returns the filepath
        {
            try{
                if(file.Length > 0)
                {
                    var basePath = Path.Combine("Document");
                    var basePathExists = Directory.Exists(basePath);

                    // var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(basePath, file.FileName);
                    
                    if(!basePathExists)
                    {
                        Directory.CreateDirectory(basePath);           
                    }

                    
                    if(!File.Exists(filePath))
                    {
                        FileStream stream = new(filePath, FileMode.Create);
                        await file.CopyToAsync(stream);
                        stream.Dispose();
                        return filePath;
                    }  

                    return filePath;  

                }

                return "failed";

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
            
        }


        public string WordToPdf(string filePath, string fileName) // Converts docx to pdfs
        {
            
            try{
                Document document = new ();
                document.LoadFromFile(filePath);
                var basePath = Path.Combine("Converted");
                var ne = filePath.Replace("Document", "Converted");
                var re = filePath.Replace(".docx", ".pdf");

                if(!Directory.Exists("Converted"))
                {
                    Directory.CreateDirectory("Converted");
                }

                var savePath = Path.Combine(basePath, fileName.Replace(".docx", ".pdf"));

                document.SaveToFile(savePath, FileFormat.PDF);
                document.Dispose();

                return savePath;

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }                 
        }

    }
}