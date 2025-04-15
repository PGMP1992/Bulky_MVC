using Bulky.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Stripe;

namespace Bulky.Utility
{
    public class BookService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public BookService(HttpClient httpClient, ILogger<BookService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<OpenLibraryBook?> GetBookDetailsByISBNAsync(string isbn) // Open Library API
        //public async Task<GoogleAPi?> GetBookDetailsByISBNAsync(string isbn) // Google API
        {
            try
            {
                var url = $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&jscmd=data&format=json"; // Open Library API Original
                
                // --------------------------------------------------------------------------------
                //var url = $"https://www.googleapis.com/books/v1/volumes?q={isbn}"; // Google API
                // https://www.googleapis.com/books/v1/volumes?q=Lord+of+the+Rings This works on the Browser 
                
                // Open Library API Alternative Does not work
                //var url = $"https://openlibrary.org/search.json?title={isbn}&jscmd=data&format=json";  
                // ---------------------------------------------------------------------------------- 
                
                _logger.LogInformation($"Requesting Open Library API with URL: {url}");
                var response = await _httpClient.GetStringAsync(url);
                _logger.LogInformation($"Open Library API response: {response}");

                var data = JObject.Parse(response);
                var bookData = data[$"ISBN:{isbn}"];

                if (bookData == null)
                {
                    _logger.LogWarning($"No data found for ISBN: {isbn}");
                    return null;
                }

                // Open Library API response structure
                var book = new OpenLibraryBook
                {
                    Title = (string?)bookData["title"],
                    Author = bookData["authors"]?.FirstOrDefault()?["name"]?.ToString(),
                    Genre = bookData["subjects"]?.FirstOrDefault()?["name"]?.ToString(),
                    Pages = (int?)bookData["number_of_pages"] ?? 0,
                    PublishDate = (string?)bookData["publish_date"],
                    Cover = bookData["cover"]?.FirstOrDefault()?.ToString() //Added PM
                };


                // Google API response structure
                //var book = new GoogleAPi
                //{
                //    Title = (string?)bookData["title"],
                //    Author = bookData["authors"]?.FirstOrDefault()?["name"]?.ToString(),
                //    Description = (string)bookData["description"],
                //    Pages = (int?)bookData["pageCount"] ?? 0,
                //    PublishDate = (string?)bookData["publishedDate"],
                //    Category = bookData["categories"]?.FirstOrDefault()?.ToString(),
                //    Price = bookData["listPrice"]?.ToString()//Added PM
                //};

                _logger.LogInformation($"Deserialized Open Library API response: {book}");
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book details.");
                return null;
            }
        }
    }
}
