using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Web;
using tawasal.Storage;

namespace tawasal.Model
{

	public class LoginModel: ComponentBase
	{
		[Inject] ILocalStorageService localStorageService { get; set; }
		[Inject] NavigationManager navigationManager { get; set; }
		[Inject] AuthenticationStateProvider authenticationStateProvider { get; set; }
		public LoginViewModel loginData { get; set; }
		public LoginModel()
		{
			this.loginData = new LoginViewModel();
		}
		
		protected async Task loginAsync()
		{
			var token = new SecurityToken
			{
				Token = Guid.NewGuid(),
				UserName = this.loginData.UserName,
				ExpiredAt = DateTime.UtcNow.AddHours(1),
			};
			await localStorageService.SetAsync(nameof(SecurityToken), token);
			authenticationStateProvider.GetAuthenticationStateAsync();

			NameValueCollection pairs = HttpUtility.ParseQueryString(navigationManager.Uri.Substring(navigationManager.Uri.IndexOf("?")));
			string url = pairs["returnUrl"];

			navigationManager.NavigateTo(pairs.Count == null ? "/" : url, true);
		}

	}

	public class LoginViewModel
	{
		[Required]
		[StringLength(10, ErrorMessage = "Too long login")]
		public string UserName { get; set; }

		[Required]
		[StringLength(10, ErrorMessage = "Too long password")]
		public string Password { get; set; }
	}
}
