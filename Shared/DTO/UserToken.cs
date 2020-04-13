using System;

namespace BlazorDemoUdemy.Shared.DTO{
    public class UserToken{
        public string Token { get; set; }
        public DateTime Expiration { get; set; }

    }
}