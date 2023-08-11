using LinkWeb.Modules;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LinkWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        [HttpPost("Register")]
        public JsonResult Register([FromBody] RegisterRequest model)
        {
            if (ModBase.IsRegisteredTaskExpired(model.TaskId ?? ""))
            {
                return Json(new RegisterResponse(1, "任务请求已过期或不存在"));
            }
            if (ModBase.Database.GetUserByUsername(model.Username ?? "") != null)
            {
                return Json(new RegisterResponse(2, "用户名已存在"));
            }
            try
            {
                ModBase.Database.InsertUserData(model.Username ?? "", ModBase.GetRegisteredTaskSalt(model.TaskId ?? ""), model.Hash ?? "");
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new RegisterResponse(3, "在操作数据库时发生了错误"));
            }
            return Json(new RegisterResponse(0, "成功创建账户"));
        }

        [HttpGet("RegisterKey")]
        public JsonResult RegisterKey()
        {
            var data = ModBase.GenerateRegisterData();
            return Json(new RegisterKeyResponse(data["TaskId"], data["Salt"]));
        }

        [HttpPost("Login")]
        public JsonResult Login([FromBody] LoginRequest model)
        {
            if (ModBase.IsLoginTaskExpired(model.TaskId ?? ""))
            {
                return Json(new LoginResponse(1, "任务请求已过期或不存在", ""));
            }
            var userData = ModBase.Database.GetUserByUsername(model.Username ?? "");
            if (userData == null)
            {
                return Json(new LoginResponse(2, "用户名或密码不正确", ""));
            }
            if (model.Hash == userData[3].ToString())
            {
                var token = ModBase.GetTokenOfUsername(model.Username ?? "");
                CookieOptions options = new()
                {
                    Expires = DateTime.UtcNow.AddDays(14) // 设置过期时间
                };
                Response.Cookies.Append("token", token, options);
                return Json(new LoginResponse(0, "登录成功", token));
            }
            return Json(new LoginResponse(3, "用户名或密码不正确", ""));
        }

        [HttpPost("LoginKey")]
        public JsonResult LoginKey([FromBody] LoginKeyRequest model)
        {
            var data = ModBase.GenerateLoginData();
            var saltData = ModBase.Database.GetUserByUsername(model.Username ?? "");
            if (saltData == null)
            {
                return Json(new LoginKeyResponse(1, "用户名或密码不正确", "", ""));
            }
            return Json(new LoginKeyResponse(0, "", data["TaskId"], saltData[2].ToString()!));
        }

        [HttpPost("VerifySession")]
        public JsonResult VerifySession([FromBody] VerifySessionRequest model)
        {
            if (ModBase.IsTokenExpired(model.Token ?? ""))
            {
                return Json(new VerifySessionResponse(1, "Token已过期"));
            } else
            {
                return Json(new VerifySessionResponse(0, "Token未过期"));
            }
        }
    }

    public class RegisterRequest
    {
        public string? Username { get; set; }
        public string? Hash { get; set; }
        public string? TaskId { get; set; }
    }


    public class LoginRequest
    {
        public string? TaskId { get; set; }
        public string? Username { get; set; }
        public string? Hash { get; set; }
    }

    public class LoginKeyRequest
    {
        public string? Username { get; set; }
    }

    public class VerifySessionRequest
    {
        public string? Token { get; set; }
    }
}
