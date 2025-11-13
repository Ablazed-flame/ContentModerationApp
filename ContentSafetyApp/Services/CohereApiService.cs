using ContentModerationApp.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace ContentModerationApp.Services
{
    public class CohereApiService
    {
        
          
            private readonly HttpClient _httpClient;
            
            public CohereApiService(HttpClient httpClient)
            {
                
                _httpClient = httpClient;
              
            }

            public async Task<string> AnalyzeTextAsync(string inputText)
            {
            //string prompt = "You are a content moderation system.\r\nI will give you a text. \r\nBased on this description, tell me what could be wrong with the image according to common moderation categories:\r\n- Violence or blood\r\n- Sexual or explicit content\r\n- Hate or offensive content\r\n- Other disturbing or inappropriate elements\r\n\r\nAnd please just only write summary in a paragraph no need of points and you dont have to suggest you have to assert like you are giving reasons why this got blocked\r\nyour work here is only to gaive a description of what ir wrong with the image based on the description provided.\r\nYou need not to specify what it breaks just give the description of what is wrong with the image based on the description provided.\r\nBe concise and objective. \r\nAnd dont write as you are suggesting just assert that yes it contains this which is inappropriate content.\r\nIf the image seems safe, just say 'No issues detected.'";

            string prompt = "You are a content moderation system. I will give you a text. Based ont he text write a brief summary of what could be wrong with this text according to common moderation categories of Violence or blood, Sexual or explicit content, Hate or offensive content, racial content, language and profanity. And please just only write summary in a paragraph no need of points and you dont have to suggest you have to assert like you are giving reasons why this got blocked. If it has nor problems just write No problems found";
                var requestBody = new
                {
                    model = "command-a-03-2025",
                    messages = new[]
                    {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = prompt + "\r\nText: " + inputText }
                    }
                }
            }
                };
                string jsonPayload = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("v2/chat", content);
                response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
                var cohereResponse = JsonSerializer.Deserialize<CohereChatResponse>(jsonResponse);
            var reply = cohereResponse?.Message?.Content
                .FirstOrDefault(c => c.Type == "text")?
                .Text;
            return reply;
        }
        public async Task<string> AnalyzeImageAsync(string imagePath)
            {
                if (!File.Exists(imagePath))
                    throw new FileNotFoundException($"Image not found: {imagePath}");


                string base64DataUrl = ImageFileToBase64DataUrl(imagePath);
                if (string.IsNullOrEmpty(base64DataUrl))
                    return "Error: Could not encode image to Base64.";

             string prompt = "Give me a detailed explicit description of this image. You are a content moderation system.\r\nI will give you a text description of an image (generated from dense captions). \r\nBased on this description, tell me what could be wrong with the image according to common moderation categories:\r\n- Violence or blood\r\n- Sexual or explicit content\r\n- Hate or offensive content\r\n- Other disturbing or inappropriate elements\r\n\r\nAnd please just only write summary in a paragraph no need of points and you dont have to suggest you have to assert like you are giving reasons why this got blocked\r\nyour work here is only to gaive a description of what ir wrong with the image based on the description provided.\r\nYou need not to specify what it breaks just give the description of what is wrong with the image based on the description provided.\r\nBe concise and objective. \r\nAnd dont write as you are suggesting just assert that yes it contains this which is inappropriate content.\r\nIf the image seems safe, just say 'No issues detected.'";
            //string prompt = "Give explicit description of this image. You are an content moderator system. I will give you the image tell me what wrong is present in it so that it got blocked by the content moderator system. Briefly explain details.";
                var requestBody = new
                {
                    // Use the latest vision-capable model
                    model = "command-a-vision-07-2025",
                    messages = new[]
                    {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        // The text prompt (instructions)
                        new { type = "text", text = prompt },
                        // The image object using the Base64 Data URL
                        new
                        {
                            type = "image_url",
                            image_url = new { url = base64DataUrl }
                        }
                    }
                }
            }
                };

                string jsonPayload = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // 3. Send the HTTP Request

                var response = await _httpClient.PostAsync("v2/chat", content);
                response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("Cohere API Response:" + jsonResponse);
                var cohereResponse = JsonSerializer.Deserialize<CohereChatResponse>(jsonResponse);

            var reply = cohereResponse?.Message?.Content
                .FirstOrDefault(c => c.Type == "text")?
                .Text;

            return reply;
            }

            private static string ImageFileToBase64DataUrl(string filePath)
            {
                try
                {
                    byte[] imageBytes = File.ReadAllBytes(filePath);
                    string base64String = System.Convert.ToBase64String(imageBytes);

                    // Simple approach to infer MIME type from extension
                    string mimeType = Path.GetExtension(filePath).ToLowerInvariant() switch
                    {
                        ".png" => "image/png",
                        ".jpg" or ".jpeg" => "image/jpeg",
                        ".gif" => "image/gif",
                        ".webp" => "image/webp",
                        _ => "image/jpeg"
                    };

                    return $"data:{mimeType};base64,{base64String}";
                }
                catch (IOException e)
                {
                    // Log this error properly in a real application
                    System.Console.WriteLine($"Error reading file for Base64: {e.Message}");
                    return null;
                }
            }
        
    }
}
