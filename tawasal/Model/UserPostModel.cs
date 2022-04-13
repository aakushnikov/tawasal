namespace tawasal.Model
{

	public class UserPostServiceData
	{
		public UserPostPreviewModel[] data { get; set; }
		public int total { get; set; }
		public int page { get; set; }
		public int limit { get; set; }
	}

	public class UserPostPreviewModel
	{
		public string id { get; set; }
		public string image { get; set; }
		public int likes { get; set; }
		public string[] tags { get; set; }
		public string text { get; set; }
		public DateTime publishDate { get; set; }
		public UserPreviewModel owner { get; set; }
	}

}
