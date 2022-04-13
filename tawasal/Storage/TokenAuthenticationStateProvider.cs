using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace tawasal.Storage
{
	public class TokenAuthenticationStateProvider : AuthenticationStateProvider
	{
		readonly ILocalStorageService _localStorageService;
		public TokenAuthenticationStateProvider(ILocalStorageService localStorageService)
		{
			_localStorageService = localStorageService;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = await _localStorageService.GetAsync<SecurityToken>(nameof(SecurityToken));

			if (token == null) return GetAnonymus();
			if (token.ExpiredAt < DateTime.UtcNow) return GetAnonymus();

			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, token.UserName),
				new Claim(ClaimTypes.Expired, token.ExpiredAt.ToString()),
				new Claim(ClaimTypes.Role, "Administrator"),
			};
			token.ExpiredAt = DateTime.UtcNow.AddHours(1);
			await _localStorageService.SetAsync(nameof(SecurityToken), token);

			ClaimsIdentity identity = new ClaimsIdentity(claims, "Password");
			ClaimsPrincipal principal = new ClaimsPrincipal(identity);

			return await Task.FromResult(new AuthenticationState(principal));
		}

		public void NotifyAuthenticationStateChanged()
		{
			base.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		static AuthenticationState GetAnonymus()
		{
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}
	}
}
