namespace tawasal.Storage
{
	public class SecurityToken
	{
		public string UserName { get; set; }
		public Guid Token { get; set; }
		public DateTime ExpiredAt { get; set; }
	}
}
