using Microsoft.AspNetCore.Mvc;

namespace ForumDigitalParadise.ViewComponents
{
    public class LoginWrapperViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Factory.StartNew(() => { return View(new ForumDigitalParadise.Models.User.LoginRegisterViewModel()); });
        }
    }
}
