using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MVC_Project.Extensions;
using MVC_Project.Models;
using MVC_Project.Services.Helper;
using Newtonsoft.Json;


namespace MVC_Project.Services.ClassSection
{
    public class ClassSectionService : IClassSectionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClassSectionService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _baseUrl = configuration.GetSection("Api:BaseUrl").Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<List<ClassSectionDto>>> GetClassSectionBySessionAsync(string session)
        {
            try
            {
                var userSession = _httpContextAccessor.HttpContext?.Session.GetObject<Models.Auth.UserSession>(Constants.SessionKeys.UserSession);
                var token = userSession?.Token;
                
                if (string.IsNullOrEmpty(token))
                {
                    return new ApiResponse<List<ClassSectionDto>> { IsSuccess = false, Message = "Unauthorized" };
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"{_baseUrl}api/ClassSection/by-session/{session}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<List<ClassSectionDto>>>(responseString);
                    return result;
                }
                
                return new ApiResponse<List<ClassSectionDto>> { IsSuccess = false, Message = "Failed to retrieve classes" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ClassSectionDto>> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<List<SectionDto>>> GetSectionsByClassIdAsync(int classId)
        {
            try
            {
                var userSession = _httpContextAccessor.HttpContext?.Session.GetObject<Models.Auth.UserSession>(Constants.SessionKeys.UserSession);
                var token = userSession?.Token;

                if (string.IsNullOrEmpty(token))
                {
                    return new ApiResponse<List<SectionDto>> { IsSuccess = false, Message = "Unauthorized" };
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"{_baseUrl}api/ClassSection/by-Classid/{classId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<List<SectionDto>>>(responseString);
                    return result;
                }

                return new ApiResponse<List<SectionDto>> { IsSuccess = false, Message = "Failed to retrieve sections" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<SectionDto>> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<List<SubjectDto>>> GetSubjectsByClassIdAsync(int classId)
        {
            try
            {
                var userSession = _httpContextAccessor.HttpContext?.Session.GetObject<Models.Auth.UserSession>(Constants.SessionKeys.UserSession);
                var token = userSession?.Token;

                if (string.IsNullOrEmpty(token))
                {
                    return new ApiResponse<List<SubjectDto>> { IsSuccess = false, Message = "Unauthorized" };
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"{_baseUrl}api/ClassSection/subject/{classId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<List<SubjectDto>>>(responseString);
                    return result;
                }
                
                return new ApiResponse<List<SubjectDto>> { IsSuccess = false, Message = "Failed to retrieve subjects" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<SubjectDto>> { IsSuccess = false, Message = ex.Message };
            }
        }
    }
}
