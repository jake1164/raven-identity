﻿using System.Web;
using System.Web.Mvc;

namespace Web.MVC.Full
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
