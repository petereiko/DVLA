using DVLA.Business.EmailModule;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.UI.Controllers
{
    public class TestController : Controller
    {
        private readonly IEmailService _emailService;

        public TestController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            _emailService.SendEmail("peterayebhere@gmail.com", "Test Email", "This is a test email from DVLA application.");
            return View();
        }
    }
}
