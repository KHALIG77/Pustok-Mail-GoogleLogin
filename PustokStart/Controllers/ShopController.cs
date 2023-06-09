﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PustokStart.DAL;
using PustokStart.ViewModels;

namespace PustokStart.Controllers
{
    public class ShopController : Controller
    {
        private readonly PustokContext _context;

        public ShopController(PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? genreId,List<int> authorId=null,double? minPrice=null,double? maxPrice=null,string sort=null)
        {
            ShopViewModel shopVM=new ShopViewModel()
            {
                Authors=_context.Authors.Include(x => x.Books).ToList(),
                Genres=_context.Genres.Include(x=>x.Books).ToList(),
                Tags=_context.Tags.ToList(),

        };
            var query = _context.Books.Include(x => x.BookImages.Where(x => x.PosterStatus != null)).Include(x => x.Author).AsQueryable();

			if (genreId != null)
            {
                query=query.Where(x=>x.GenreId==genreId);

            }

			if (authorId.Count>0)
			{
				query = query.Where(x => authorId.Contains(x.AuthorId));

			}
            if(minPrice!=null && maxPrice!=null) 
            {
                query = query.Where(x => x.SalePrice >=(decimal) minPrice && x.SalePrice <= (decimal)maxPrice);
            }
            
                switch (sort)
                {
                    case "AToZ":
                        query = query.OrderBy(x => x.Name);
                        break;
                    case "ZToA":
						query = query.OrderByDescending(x => x.Name);

						break;
                    case "LowToHigh":
                        query=query.OrderBy(x => x.SalePrice);
						break;
                    case "HighToLow":
                        query = query.OrderByDescending(x => x.SalePrice);
						break;
                }
            
			shopVM.Books = query.ToList();
			ViewBag.MaxPriceLimit = _context.Books.Any() ? _context.Books.Max(x => x.SalePrice) : 0;
		
			ViewBag.SortList = new List<SelectListItem>
            {
				new SelectListItem{ Value="AToZ",Text="Sort By Name (A-Z)",Selected=sort=="AToZ"},
				new SelectListItem{ Value="ZToA",Text="Sort By Name (A-Z)",Selected=sort=="ZToA"},
				new SelectListItem{ Value="LowToHigh",Text="Sort By Price(LOW-HIGH)",Selected=sort=="LowToHigh"},
				new SelectListItem{ Value="HighToLow",Text="Sort By Price (HIGH-LOW)",Selected=sort=="HighToLow"}


			};
			ViewBag.GenreId=genreId;
            ViewBag.AuthorIds=authorId;  
            ViewBag.MinPrice=minPrice??0;  
            ViewBag.MaxPrice=maxPrice??ViewBag.MaxPriceLimit;
            ViewBag.Sort = sort; 
        
         
            return View(shopVM);
        }
    }
}
