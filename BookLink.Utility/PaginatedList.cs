using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.Utility
{
	public class PaginatedList<T> : List<T>
	{
		public int PageIndex { get; }
		public int TotalPages { get; }
		public int PageSize { get; }

		public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
		{
			PageIndex = pageIndex;
			PageSize = pageSize;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);

			this.AddRange(items);
		}

		public bool HasPreviousPage => PageIndex > 1;
		public bool HasNextPage => PageIndex < TotalPages;

		public static PaginatedList<T> Create(IQueryable<T> source, int pageIndex, int pageSize)
		{
			var count = source.Count();
			var items = source
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			return new PaginatedList<T>(items, count, pageIndex, pageSize);
		}
	}
}
