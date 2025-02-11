using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.Models
{
	public class Location
	{
		public int Id { get; set; }
		[Required(ErrorMessage =("Please choose your location"))]
		public string Name { get; set; }
		public string Address { get; set; }
	}
}
