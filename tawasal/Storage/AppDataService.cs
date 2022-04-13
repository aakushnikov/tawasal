using System.Net.Http.Json;
using tawasal.Model;

namespace tawasal.Storage
{
	public class AppDataService
	{
        const int TIMEOUT = 5;
        Dictionary<string, DateTime> _meta;
        
        UserServiceData _dataUsers;
        
        public event EventHandler OnGetUserData;

        Dictionary<string, UserPostServiceData> _dataUserPosts;
        public event EventHandler OnGetUserPostsData;

        readonly ILocalStorageService _localStorageService;

        public AppDataService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
            _dataUserPosts = new Dictionary<string, UserPostServiceData>();
            _meta = new Dictionary<string, DateTime>();
        }
        
        public UserServiceData GetUsers()
		{
            if (_dataUsers == null) GetUsersDataAsync();
            return _dataUsers;
        }

        protected async Task GetUsersDataAsync()
        {
            _meta = await _localStorageService.GetAsync<Dictionary<string, DateTime>>("timeouts");

            if (_dataUsers != null && _meta.ContainsKey("user") && _meta["user"] > DateTime.UtcNow) return;

            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.SendAsync(CreateRequest("user"));

            if (response.IsSuccessStatusCode)
            {
                _dataUsers = await response.Content.ReadFromJsonAsync<UserServiceData>();
                UpdateMeta("user");
                await _localStorageService.SetAsync("timeouts", _meta);

                if (OnGetUserData != null) OnGetUserData(this, EventArgs.Empty);
            }
        }

        public UserPostServiceData GetUserPosts(string id)
        {
            if (_dataUserPosts.Keys.Contains(id)) return _dataUserPosts[id];
            GetUserPostsAsync(id);
            return null;
        }

        protected async Task GetUserPostsAsync(string id)
		{
            _meta = await _localStorageService.GetAsync<Dictionary<string, DateTime>>("timeouts");
            if (_dataUserPosts.Keys.Contains(id) && _meta.ContainsKey(id) && _meta[id] > DateTime.UtcNow) return;

            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.SendAsync(CreateRequest($"user/{id}/post"));

            if (response.IsSuccessStatusCode)
            {
                _dataUserPosts.Add(id, await response.Content.ReadFromJsonAsync<UserPostServiceData>());
                UpdateMeta(id);
                await _localStorageService.SetAsync("timeouts", _meta);
                if (OnGetUserPostsData != null) OnGetUserPostsData(this, new PostEventArgs(id));
            }
        }

        HttpRequestMessage CreateRequest(string method)
		{
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://dummyapi.io/data/v1/{method}");
            request.Headers.Add("app-id", "625485766077ec0e2f5571d1");
            return request;
        }

        void UpdateMeta(string id)
		{
            if (_meta == null)
                _meta = new Dictionary<string, DateTime>();
            if (_meta.ContainsKey(id))
                _meta[id] = DateTime.UtcNow.AddMinutes(TIMEOUT);
            else
                _meta.Add(id, DateTime.UtcNow.AddMinutes(TIMEOUT));
        }
    }

    public class PostEventArgs : EventArgs
	{
        string _id;
        public string Id { get { return _id; } }
        public PostEventArgs(string id)
		{
            _id = id;
		}
	}
}
