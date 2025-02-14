using System.ComponentModel.DataAnnotations;


namespace BookLink.Models
{
	public class Location
	{
		public int Id { get; set; }
		[Required(ErrorMessage =("Please choose your location"))]
		public string Name { get; set; }
	}
}
