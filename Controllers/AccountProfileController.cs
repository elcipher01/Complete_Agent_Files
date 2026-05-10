using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NextHorizon.Services;

namespace NextHorizon.Controllers
{
    public class AccountProfileController : Controller
    {
        private readonly OrderService _orderService;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<object> _passwordHasher;

        public AccountProfileController(OrderService orderService, IConfiguration configuration)
        {
            _orderService = orderService;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<object>();
        }

        public IActionResult ProfileView()
        {
            return View();
        }

        public IActionResult UpdateProfile()
        {
            return View();
        }

        public IActionResult UploadActivity()
        {
            return View();
        }

        public IActionResult ShippingAddress()
        {
            return View();
        }

        public IActionResult PaymentMethods()
        {
            return View();
        }

        public IActionResult MyPurchases()
        {
            var orders = _orderService.GetUserPurchases();
            return View(orders);
        }

        [HttpGet]
        public IActionResult WorkspaceProfile()
        {
            var staffId = HttpContext.Session.GetInt32("StaffId");
            if (!staffId.HasValue || staffId.Value <= 0)
            {
                return RedirectToAction("AdminLogin", "Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeWorkspacePassword([FromBody] WorkspacePasswordChangeRequest request)
        {
            var staffId = HttpContext.Session.GetInt32("StaffId");
            if (!staffId.HasValue || staffId.Value <= 0)
            {
                return Unauthorized(new { success = false, message = "Session expired. Please login again." });
            }

            if (request is null
                || string.IsNullOrWhiteSpace(request.CurrentPassword)
                || string.IsNullOrWhiteSpace(request.NewPassword)
                || string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                return BadRequest(new { success = false, message = "All password fields are required." });
            }

            if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
            {
                return BadRequest(new { success = false, message = "New password and confirmation do not match." });
            }

            if (request.NewPassword.Length < 8)
            {
                return BadRequest(new { success = false, message = "New password must be at least 8 characters." });
            }

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return StatusCode(500, new { success = false, message = "Database connection is not configured." });
            }

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var getHashCommand = new SqlCommand(
                    """
                    SELECT u.password_hash
                    FROM users u
                    INNER JOIN staff_info s ON u.user_id = s.user_id
                    WHERE s.staff_id = @StaffId
                    """,
                    connection);
                getHashCommand.Parameters.AddWithValue("@StaffId", staffId.Value);

                var currentHash = await getHashCommand.ExecuteScalarAsync() as string;
                if (string.IsNullOrWhiteSpace(currentHash))
                {
                    return NotFound(new { success = false, message = "Unable to find account credentials." });
                }

                var verificationResult = _passwordHasher.VerifyHashedPassword(null!, currentHash, request.CurrentPassword);
                if (verificationResult != PasswordVerificationResult.Success
                    && verificationResult != PasswordVerificationResult.SuccessRehashNeeded)
                {
                    return BadRequest(new { success = false, message = "Current password is incorrect." });
                }

                var newHash = _passwordHasher.HashPassword(null!, request.NewPassword);
                var changeCommand = new SqlCommand("sp_ChangePassword", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                changeCommand.Parameters.AddWithValue("@StaffId", staffId.Value);
                changeCommand.Parameters.AddWithValue("@NewPasswordHash", newHash);

                await changeCommand.ExecuteNonQueryAsync();
                return Json(new { success = true, message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public sealed class WorkspacePasswordChangeRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
