namespace LinkWeb.Modules
{
    public static class ModBase
    {
        private static readonly Random _random = new();
        private const string _characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public readonly static DatabaseHelper Database = new(DbConfiguration.GetConnectionString());
        /// <summary>
        /// 注册队列：5分钟过期
        /// </summary>
        private static readonly Dictionary<string, long> _registerTasks = new();
        private static readonly Dictionary<string, long> _loginTasks = new();
        private static readonly Dictionary<string, string> _registerTaskData = new();
        private static readonly Dictionary<string, string> _tokenList = new();

        private static byte[] _aesKey = Array.Empty<byte>();
        private static byte[] _aesIv = Array.Empty<byte>();
        private static AesHelper? helper;

        public static void Initialize()
        {
            (_aesKey, _aesIv) = AesHelper.GenerateKeyAndIV();
            helper = new(_aesKey, _aesIv);
        }

        /// <summary>
        /// 读取config
        /// </summary>
        /// <param name="key">格式为Section:Key</param>
        /// <returns></returns>
        public static string GetConfig(string key)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddIniFile("config.ini")
                .Build();
            return configuration[key];
        }

        /// <summary>
        /// 包含大小写字母以及数字
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomString(int length)
        {
            char[] randomChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomChars[i] = _characters[_random.Next(_characters.Length)];
            }

            return new string(randomChars);
        }

        /// <summary>
        /// 生成注册的数据
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GenerateRegisterData()
        {
            var tid = GenerateRandomString(16);
            var salt = GenerateRandomString(32);
            _registerTasks.Add(tid, DateTime.Now.Ticks);
            _registerTaskData.Add(tid, salt);
            return new Dictionary<string, string>
            {
                { "TaskId", tid },
                { "Salt", salt }
            };
        }

        /// <summary>
        /// 生成登录的数据
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GenerateLoginData()
        {
            var tid = GenerateRandomString(16);
            _loginTasks.Add(tid, DateTime.Now.Ticks);
            return new Dictionary<string, string>
            {
                { "TaskId", tid }
            };
        }

        /// <summary>
        /// 注册任务是否过期
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static bool IsRegisteredTaskExpired(string taskId)
        {
            if (!_registerTasks.TryGetValue(taskId, out var task))
            {
                return true;
            }
            long ticksInFiveMinutes = TimeSpan.FromMinutes(5).Ticks;
            if (task + ticksInFiveMinutes > DateTime.Now.Ticks)
            {
                return false;
            } else
            {
                _registerTasks.Remove(taskId);
                return true;
            }
        }

        /// <summary>
        /// 登录任务是否过期
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static bool IsLoginTaskExpired(string taskId)
        {
            if (!_loginTasks.TryGetValue(taskId, out var task))
            {
                return true;
            }
            long ticksInFiveMinutes = TimeSpan.FromMinutes(5).Ticks;
            if (task + ticksInFiveMinutes > DateTime.Now.Ticks)
            {
                return false;
            } else
            {
                _loginTasks.Remove(taskId);
                return true;
            }
        }

        public static bool IsTokenExpired(string token)
        {
            try
            {
                var timeString = helper?.Decrypt(token);
                if (!long.TryParse(timeString!.Remove(timeString.Length - 16, 16), out var time))
                {
                    return true;
                }
                return time < DateTime.Now.Ticks;
            } catch
            {
                return true;
            }
        }

        /// <summary>
        /// 获取任务的对应盐值 Tips:使用后会删除对应的任务
        /// </summary>
        /// <param name="taskId"></param>
        public static string GetRegisteredTaskSalt(string taskId)
        {
            _registerTaskData.TryGetValue(taskId, out var salt);
            if (salt == null)
            {
                return "";
            }
            _registerTaskData.Remove(taskId);
            _registerTasks.Remove(taskId);
            return salt;
        }

        public static string GetTokenOfUsername(string username)
        {
            var token = helper?.Encrypt((DateTime.Now.Ticks + TimeSpan.FromDays(14).Ticks).ToString() + GenerateRandomString(16));
            if (_tokenList.ContainsKey(username))
                _tokenList.Remove(username);
            _tokenList.Add(username, token ?? "");
            return token ?? "";
        }
    }
}
