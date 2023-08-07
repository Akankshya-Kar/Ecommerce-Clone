using E_Commerce.Data;
using E_Commerce.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.WebControls.WebParts;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;



namespace E_Commerce.Models.Home
{
    public class HomeIndexViewModel
    {
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();

        ECommerceEntities context = new ECommerceEntities();

        public IPagedList<Tbl_Product> ListOfProducts { get; set; }
        public List<Tbl_Category> ListOfCategories { get; set; }

        public HomeIndexViewModel()
        {
            ListOfCategories = _unitOfWork.DBEntity.Tbl_Category.ToList();
        }

        public HomeIndexViewModel CreateModel(string search, int pageSize, int? page)
        {
            SqlParameter[] para = new SqlParameter[] {
                new SqlParameter("@search",search??(object)DBNull.Value)
            };
            IPagedList<Tbl_Product> data = context.Database.SqlQuery<Tbl_Product>("GetBySearch @search", para).ToList().ToPagedList(page ?? 1, pageSize);
            return new HomeIndexViewModel
            {
                ListOfProducts = data
            };
    }
    }
}