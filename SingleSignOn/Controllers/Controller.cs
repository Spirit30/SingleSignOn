using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SingleSignOn.Data;
using SingleSignOn.Data.DTO;
using SingleSignOn.Data.Factory;
using SingleSignOn.Data.Model;
using SingleSignOn.Data.Storage;
using SingleSignOn.Logic;
using SingleSignOn.Logic.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SingleSignOn.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Controller : ControllerBase
    {
        readonly ILogger<Controller> _logger;
        readonly Context _context;

        public Controller(ILogger<Controller> logger, Context context)
        {
            _logger = logger;
            _context = context;
        }

        #region PUT

        /// <summary>
        /// Request Code to given email.
        /// </summary>
        /// <returns>Success</returns>
        [HttpPut("code")]
        public async Task<IActionResult> Code([FromBody] MailSenderDTO dto)
        {
            if (!MailSender.IsValidEmail(dto.Email, out string error))
            {
                return BadRequest(error);
            }

            //------------------------------

            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            string code = CodeGenerator.Generate();

            if (user == null)
            {
                if (!Auth.IsValidName(dto.Name, _context, out error))
                {
                    return BadRequest(error);
                }

                user = UserFactory.Create(_context.Users.Count() + 1, dto.Email, dto.Name, code);
                _context.Users.Add(user);
            }
            else
            {
                user.Code = code;
                user.CodeTimestamp = DateTime.UtcNow.Ticks;
            }

            await _context.SaveChangesAsync();

            //------------------------------

            await MailSender.Send(dto.Email, code);
            return Ok();
        }

        /// <summary>
        /// Exchange Code from user email for Token.
        /// </summary>
        /// <returns>Access Token</returns>
        [HttpPut("token")]
        public async Task<IActionResult> Token([FromBody] CodeDTO dto)
        {
            if (!MailSender.IsValidEmail(dto.Email, out string error))
            {
                return BadRequest(error);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.Code != dto.Code)
            {
                return Unauthorized("Invalid Code.");
            }

            var period = DateTime.UtcNow - new DateTime(user.CodeTimestamp);

            if (period.TotalMinutes > Const.CODE_MINUTES_ALIVE)
            {
                return Unauthorized("Code is expired.");
            }

            //------------------------------

            user.AccessToken = Guid.NewGuid().ToString();
            user.BearerToken = Guid.NewGuid().ToString();

            user.AccessTokenTimestamp = DateTime.UtcNow.Ticks;

            await _context.SaveChangesAsync();

            //------------------------------

            return Ok(new TokensDTO 
            { 
                AccessToken = user.AccessToken,
                BearerToken = user.BearerToken
            });
        }

        /// <summary>
        /// Exchange Code from user ThirdPartyId for Token.
        /// </summary>
        /// <returns>Access Token</returns>
        [HttpPut("thirdparty/token")]
        public async Task<IActionResult> Token([FromBody] ThirdPartyIdDTO dto)
        {
            string thirdPartyId = $"{dto.Provider}:{dto.Id}";

            var user = _context.Users.FirstOrDefault(u => u.ThirdPartyId == thirdPartyId);

            if (user == null)
            {
                user = UserFactory.Create(_context.Users.Count() + 1, thirdPartyId, dto.Name);
                _context.Users.Add(user);

                await _context.SaveChangesAsync();
            }

            //------------------------------

            user.AccessToken = Guid.NewGuid().ToString();
            user.BearerToken = Guid.NewGuid().ToString();

            user.AccessTokenTimestamp = DateTime.UtcNow.Ticks;

            await _context.SaveChangesAsync();

            //------------------------------

            return Ok(new TokensDTO
            {
                AccessToken = user.AccessToken,
                BearerToken = user.BearerToken
            });
        }

        /// <summary>
        /// Change Name.
        /// </summary>
        /// <returns>Ok</returns>
        [HttpPut("name")]
        public async Task<IActionResult> ChangeName([FromBody] ChangeNameDTO dto)
        {
            if (!Auth.ExcangeTokenToUser(dto.AccessToken, _context, out User user, out string error))
            {
                return Unauthorized(error);
            }

            if (!Auth.IsValidName(dto.Name, _context, out error))
            {
                return BadRequest(error);
            }

            //------------------------------

            user.DisplayName = dto.Name;

            await _context.SaveChangesAsync();

            //------------------------------

            return Ok();
        }

        /// <summary>
        /// Change Avatar.
        /// </summary>
        /// <returns>Ok</returns>
        [HttpPut("avatar")]
        public IActionResult ChangeAvatar([FromBody] ChangeAvatarDTO dto)
        {
            if (!Auth.ExcangeTokenToUser(dto.AccessToken, _context, out User user, out string error))
            {
                return Unauthorized(error);
            }

            //------------------------------

            AvatarStorage.Save(user.DisplayName, dto.AvatarBase64);

            //------------------------------

            return Ok();
        }

        /// <summary>
        /// Write CustomData.
        /// </summary>
        /// <returns>Ok</returns>
        [HttpPut("custom")]
        public IActionResult WriteCustomData([FromBody] CustomDataDTO dto)
        {
            if (!Auth.ExcangeTokenToUser(dto.AccessToken, _context, out User user, out string error))
            {
                return Unauthorized(error);
            }

            //------------------------------

            CustomDataStorage.Save(user.DisplayName, dto.CustomData);

            //------------------------------

            return Ok();
        }

        /// <summary>
        /// Refresh Access Token.
        /// </summary>
        /// <returns>Tokens.</returns>
        [HttpPut("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] TokensDTO dto)
        {
            if (!Auth.ExcangeTokenToUser(dto.AccessToken, _context, out User user, out string error, false))
            {
                return Unauthorized(error);
            }

            //------------------------------

            user.AccessToken = Guid.NewGuid().ToString();
            user.BearerToken = Guid.NewGuid().ToString();

            user.AccessTokenTimestamp = DateTime.UtcNow.Ticks;

            await _context.SaveChangesAsync();

            //------------------------------

            return Ok(new TokensDTO
            {
                AccessToken = user.AccessToken,
                BearerToken = user.BearerToken
            });
        }

        #endregion

        #region GET

        /// <summary>
        /// Exchange Token from user email for User.
        /// </summary>
        /// <returns>Public User Profile</returns>
        [HttpGet("user/{accessToken}")]
        public IActionResult GetUser(string accessToken)
        {
            if (!Auth.ExcangeTokenToUser(accessToken, _context, out User user, out string error))
            {
                return Unauthorized(error);
            }

            var userDTO = new UserDTO()
            {
                DisplayName = user.DisplayName,
                AvatarBase64 = AvatarStorage.LoadOrDefault(user.DisplayName),
                CustomData = CustomDataStorage.LoadOrDefault(user.DisplayName)
            };

            return Ok(userDTO);
        }

        [HttpGet]
        public IActionResult Get()
        {
            return GetVersion();
        }

        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            return Ok("Version: 0.3.0");
        }

        #endregion
    }
}
