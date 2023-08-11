namespace LinkWeb.Modules
{
    public abstract class ResponseData<T>
    {
        public int Code { get; init; } = 0;
        public string Message { get; set; } = "";
        public T? Data { get; set; }
    }

    public class RegisterResponse : ResponseData<object>
    {
        public RegisterResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }

    public class RegisterKeyResponse : ResponseData<RegisterKeyResponseData>
    {
        public RegisterKeyResponse(string taskId, string salt)
        {
            Data = new(taskId, salt);
        }
    }

    public class RegisterKeyResponseData
    {
        public string TaskId { get; set; }
        public string Salt { get; set; }

        public RegisterKeyResponseData(string taskId, string salt)
        {
            TaskId = taskId;
            Salt = salt;
        }
    }
    
    public class LoginResponse : ResponseData<LoginResponseData>
    {
        public LoginResponse(int code, string message, string token)
        {
            Code = code;
            Message = message;
            Data = new(token);
        }
    }

    public class LoginResponseData
    {
        public string Token { get; set; } = "_error_";

        public LoginResponseData(string token)
        {
            Token = token;
        }
    }

    public class LoginKeyResponse : ResponseData<LoginKeyResponseData>
    {

        public LoginKeyResponse(int code, string message, string taskId, string salt)
        {
            Code = code;
            Message = message;
            Data = new(taskId, salt);
        }
    }

    public class LoginKeyResponseData
    {
        public string TaskId { get; set; }
        public string Salt { get; set; }

        public LoginKeyResponseData(string taskId, string salt)
        {
            TaskId = taskId;
            Salt = salt;
        }
    }
    public class VerifySessionResponse : ResponseData<object>
    {
        public VerifySessionResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
