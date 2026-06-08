using Microsoft.AspNetCore.Mvc;
using Sklep.Infrastructure.Data;

namespace Sklep.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Pay(int orderId)
        {
            var payment = _context.Payments
                .FirstOrDefault(p => p.OrderId == orderId);

            if (payment == null)
                return NotFound();

            return View(payment);
        }

        [HttpPost]
        public IActionResult Confirm(int paymentId)
        {
            var payment = _context.Payments
                .FirstOrDefault(p => p.Id == paymentId);

            if (payment == null)
                return NotFound();

            payment.Status = "Opłacona";
            payment.PaidAt = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction(
                "Success",
                new { orderId = payment.OrderId });
        }

        public IActionResult Success(int orderId)
        {
            return View(model: orderId);
        }
    }
}