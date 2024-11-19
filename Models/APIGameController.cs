using Azure;
using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ServerGame106.Data;
using ServerGame106.DTO;
using ServerGame106.Service;
using ServerGame106.ViewModel;

namespace ServerGame106.Models
{
    [Route("api/[controller]")]
    [ApiController]


    public class APIGameController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        protected ResponseApi _response;
        private readonly UserManager<ApplicationUser> _userManager;
        public APIGameController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager
            )
        {
            _db = db;
            _response = new();
            _userManager = userManager;
        }
        [HttpGet("GetAllGameLevel")]

        public async Task<IActionResult> GetAllGameLevel()
        {
            try
            {
                var gameLevel = await _db.GameLevels.ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lấy dữ liệu thành công";
                _response.Data = gameLevel;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("GetAllQuestionGame")]

        public async Task<IActionResult> GetAllQuestionGame()
        {
            try
            {
                var questionsGame = await _db.Questions.ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lấy dữ liệu thành công";
                _response.Data = questionsGame;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("GetAllRegion")]

        public async Task<IActionResult> GetAllRegion()
        {
            try
            {
                var region = await _db.Regions.ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lấy dữ liệu thành công";
                _response.Data = region;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                // Trả về lỗi 400 khi dữ liệu không hợp lệ
                return BadRequest(new
                {
                    IsSuccess = false,
                    Notification = "Dữ liệu không hợp lệ",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                var user = new ApplicationUser
                {
                    Email = registerDTO.Email,
                    UserName = registerDTO.Email,
                    Name = registerDTO.Name,
                    Avatar = registerDTO.LinkAvatar,
                    RegionId = registerDTO.RegionId
                };

                var result = await _userManager.CreateAsync(user, registerDTO.Password);
                if (result.Succeeded)
                {
                    _response.IsSuccess = true;
                    _response.Notification = "Đăng ký thành công";
                    _response.Data = user;
                    return CreatedAtAction(nameof(Register), _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Đăng ký thất bại";
                    _response.Data = result.Errors;
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return StatusCode(500, _response);
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var email = loginRequest.Email;
                var password = loginRequest.Password;

                // Tìm người dùng bằng email
                var user = await _userManager.FindByEmailAsync(email);

                // Kiểm tra nếu người dùng tồn tại và mật khẩu đúng
                if (user != null && await _userManager.CheckPasswordAsync(user, password))
                {
                    _response.IsSuccess = true;
                    _response.Notification = "Đăng nhập thành công";
                    _response.Data = user; // Bạn có thể gửi thông tin cần thiết về người dùng ở đây
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Đăng nhập thất bại";
                    _response.Data = null;
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpGet("GetAllQuestionGameByLevel/{levelID}")]
        public async Task<IActionResult> GetAllQuestionGameByLevel(int levelID)
        {
            try
            {
                var questionGame = await _db.Questions.Where(x => x.levelId == levelID).ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lấy dữ liệu thành công";
                _response.Data = questionGame;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpPost("SaveResult")]
        public async Task<IActionResult> SaveResult(LevelResultDTO levelResult)
        {
            try
            {
                var levelResultSave = new LevelResult
                {
                    UserId = levelResult.UserID,
                    LevelId = levelResult.LevelID,
                    Score = levelResult.Score,
                    CompletionDate = DateOnly.FromDateTime(DateTime.Now) // Ghi nhận ngày hoàn thành hiện tại
                };

                // Thêm bản ghi vào cơ sở dữ liệu và lưu thay đổi
                await _db.LevelResults.AddAsync(levelResultSave);
                await _db.SaveChangesAsync();

                // Trả về phản hồi thành công
                _response.IsSuccess = true;
                _response.Notification = "Lưu kết quả thành công";
                _response.Data = levelResultSave; // Trả về thông tin kết quả đã lưu
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi thất bại
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("Rating/{idRegion}")]
        public async Task<IActionResult> Rating(int idRegion)
        {
            try
            {
                if (idRegion > 0)
                {
                    var nameRegion = await _db.Regions.Where(x => x.RegionId == idRegion)
                                                      .Select(x => x.Name)
                                                      .FirstOrDefaultAsync();

                    if (nameRegion == null)
                    {
                        _response.IsSuccess = false;
                        _response.Notification = "Không tìm thấy khu vực";
                        _response.Data = null;
                        return BadRequest(_response);
                    }

                    var userByRegion = await _db.Users.Where(x => x.RegionId == idRegion).ToListAsync();
                    var resultLevelByRegion = await _db.LevelResults
                                                       .Where(x => userByRegion.Select(x => x.Id).Contains(x.UserId))
                                                       .ToListAsync();

                    RatingVM ratingVM = new();
                    ratingVM.NameRegion = nameRegion;
                    ratingVM.userResultSums = new();

                    foreach (var item in userByRegion)
                    {
                        var sumScore = resultLevelByRegion.Where(x => x.UserId == item.Id).Sum(x => x.Score);
                        var sumLevel = resultLevelByRegion.Where(x => x.UserId == item.Id).Count();

                        UserResultSum userResultSum = new();
                        userResultSum.NameUser = item.Name;
                        userResultSum.SumScore = sumScore;
                        userResultSum.SumLevel = sumLevel;

                        ratingVM.userResultSums.Add(userResultSum);
                    }

                    _response.IsSuccess = true;
                    _response.Notification = "Lấy dữ liệu thành công";
                    _response.Data = ratingVM;

                    return Ok(_response);
                }
                else
                {
                    var user = await _db.Users.ToListAsync();
                    var resultLevel = await _db.LevelResults.ToListAsync();
                    string nameRegion = "Tất cả";
                    RatingVM ratingVM = new();
                    ratingVM.NameRegion = nameRegion;
                    ratingVM.userResultSums = new();
                    foreach (var item in user)
                    {
                        var sumScore = resultLevel.Where(x => x.UserId == item.Id).Sum(x => x.Score);
                        var sumLevel = resultLevel.Where(x => x.UserId == item.Id).Count();
                        UserResultSum userResultSum = new();
                        userResultSum.NameUser = item.Name;
                        userResultSum.SumScore = sumScore;
                        userResultSum.SumLevel = sumLevel;
                        ratingVM.userResultSums.Add(userResultSum);
                    }
                    _response.IsSuccess = true;
                    _response.Notification = "Lấy dữ liệu thành công";
                    _response.Data = ratingVM;
                    return Ok(resultLevel);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("GetUserInformation/{userID}")]
        public async Task<IActionResult> GetUserInformation(string userId)
        {
            try
            {
                var user = await _db.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Không tìm thấy người dùng";
                    _response.Data = null;
                    return BadRequest(_response);
                }
                UserInformationVM userInformationVM = new();
                userInformationVM.Name = user.Name;
                userInformationVM.Email = user.Email;
                userInformationVM.avatar = user.Avatar;
                userInformationVM.Region = await _db.Regions.Where(x => x.RegionId == user.RegionId).Select(x => x.Name).FirstOrDefaultAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lấy dữ liệu thành công";
                _response.Data = userInformationVM;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPut("ChangeUserPassword")]
        public async Task<IActionResult> ChangeUserPassword(ChangePasswordDTO changePasswordDTO)
        {

            try
            {
                var user = await _db.Users.Where(x => x.Id == changePasswordDTO.UserID).FirstOrDefaultAsync();
                if (user != null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Không tìm thấy người dùng";
                    _response.Data = null;
                    return BadRequest(_response);
                }
                var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
                if (result.Succeeded)
                {
                    _response.IsSuccess = true;
                    _response.Notification = "Đổi mật khẩu thành công";
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Đổi mật khẩu thất bại";
                    _response.Data = result.Errors;
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPut("UpdateUserInformation")]
        public async Task<IActionResult> UpdateUserInformation([FromForm] UserInformationDTO userInformationDTO)
        {
            try
            {
                var user = await _db.Users.Where(x => x.Id == userInformationDTO.UserID).FirstOrDefaultAsync();
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Không tìm thấy người dùng";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                user.Name = userInformationDTO.Name;
                user.RegionId = userInformationDTO.RegionID;

                if (userInformationDTO.Avatar != null)
                {
                    var fileExtension = Path.GetExtension(userInformationDTO.Avatar.FileName);
                    var fileName = $"{userInformationDTO.UserID}{fileExtension}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/avatars", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await userInformationDTO.Avatar.CopyToAsync(stream);
                    }

                    user.Avatar = fileName;
                }
                await _db.SaveChangesAsync();
                _response.IsSuccess = true;
                _response.Notification = "Cập nhật thông tin thành công";
                _response.Data = user;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpDelete("DeleteAccount/{userId}")]
        public async Task<IActionResult> DeleteAccount(string userId)
        {
            try
            {
                var user = await _db.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Không tìm thấy người dùng";
                    _response.Data = null;
                    return BadRequest(_response);
                }

                user.IsDeleted = true;
                await _db.SaveChangesAsync();
                _response.IsSuccess = true;
                _response.Notification = "Xóa người dùng thành công";
                _response.Data = user;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(Email);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Không tìm thấy người dùng";
                    _response.Data = null;
                    return BadRequest(_response);
                }
                Random random = new();
                string OTP = random.Next(100000, 999999).ToString();
                user.OTP = OTP;
                await _userManager.UpdateAsync(user);
                await _db.SaveChangesAsync();
                string subject = "Reset Password Game 106 – " + Email;
                string message = "Mã OTP của bạn là: " + OTP;
                await _emailService.SendEmailAsync(Email, subject, message);
                _response.IsSuccess = true;
                _response.Notification = "Gửi mã OTP thành công";
                _response.Data = "email sent to " + Email;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Lỗi";
                _response.Data = ex.Message;
                return BadRequest(_response);
            }
        }

    }
}

