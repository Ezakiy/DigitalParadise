﻿using Microsoft.AspNetCore.Mvc;

namespace ForumDigitalParadise.ViewComponents
{
    public class SideNavViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Factory.StartNew(() => { return View(); });
        }
    }
}
