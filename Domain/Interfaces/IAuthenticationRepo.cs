using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAuthenticationRepo
    {
        Task<Response> Register(RegisterModel model, string clientlink);
        Task<Response> checkEmailExist(RegisterModel model);
        Task<Response> checkUserExist(RegisterModel model);
        Task<Response> Login(LoginModel model);
        Task<JsonToken> CreateToken(LoginModel model);
    }
}
