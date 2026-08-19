using System;
using System.Collections.Generic;
using System.Text;
using mobileFitnes.ApiService.DataObjects;
using Refit;

namespace mobileFitnes.ApiService;

public interface IApiEndpoints
{
    // --- AUTH ---
    [Post("/v1/auth/register")]
    Task<IApiResponse> Register([Body] RegisterReqDto req);

    [Post("/v1/auth/login")]
    Task<IApiResponse<TokensResDto>> Login([Body] LoginReqDto req);

    [Post("/v1/auth/refresh")]
    Task<IApiResponse<TokensResDto>> Refresh([Body] RefreshReqDto req);

    [Post("/v1/auth/logout")]
    Task<IApiResponse> Logout();

    // --- CLASSES ---
    [Get("/v1/classes")]
    Task<IApiResponse<List<ClassData>>> GetClasses();

    [Post("/v1/classes/{classId}/signup")]
    Task<IApiResponse> SignUpForClass(int classId);

    [Post("/v1/classes/{classId}/leave")]
    Task<IApiResponse> LeaveClass(int classId);
}

public record RegisterReqDto(
    string Email,
    string Password,
    string Username
);
public record LoginReqDto(
    string Email,
    string Password
);
public record RefreshReqDto(
    string RefreshToken
);

public record TokensResDto(string JwtToken, string RefreshToken);
public record SimpleResDto(string Message);