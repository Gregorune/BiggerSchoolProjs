using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Headers;

namespace mobileFitnes.ApiService;

public class RefitInterceptor : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider;

    public RefitInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // REQUEST INTERCEPTOR
        var jwt = await SecureStorage.Default.GetAsync("jwt");
        if (!string.IsNullOrEmpty(jwt))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        }

        // SEND REQUEST
        var response = await base.SendAsync(request, cancellationToken);

        // RESPONSE INTERCEPTOR
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized &&
            !request.RequestUri!.AbsolutePath.Contains("/auth/refresh"))
        {
            var refreshToken = await SecureStorage.Default.GetAsync("refresh");
            if (string.IsNullOrEmpty(refreshToken)) return response;

            var isRefreshed = await TryRefreshToken(refreshToken);

            if (isRefreshed)
            {
                var newJwt = await SecureStorage.Default.GetAsync("jwt");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newJwt);
                return await base.SendAsync(request, cancellationToken);
            }
            else
            {
                SecureStorage.Default.RemoveAll();
                await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("//login"));
            }
        }

        return response;
    }

    private async Task<bool> TryRefreshToken(string refreshToken)
    {
        try
        {
            var authApi = _serviceProvider.GetRequiredService<IApiEndpoints>();
            var result = await authApi.Refresh(new RefreshReqDto(refreshToken));

            if (result != null)
            {
                await SecureStorage.Default.SetAsync("jwt", result.Content!.JwtToken);
                await SecureStorage.Default.SetAsync("refresh", result.Content!.RefreshToken);
                return true;
            }
        }
        catch
        {
            return false;
        }
        return false;
    }
}