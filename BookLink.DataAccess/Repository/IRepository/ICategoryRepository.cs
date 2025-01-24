using BookLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.DataAccess.Repository.IRepository
{
	public interface ICategoryRepository : IRepository<Category> // Category is a model
	{
		void Update(Category category);
		
	}
}
