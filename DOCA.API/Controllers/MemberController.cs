using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Request.Member;
using DOCA.API.Payload.Response.Account;
using DOCA.API.Payload.Response.Member;
using DOCA.API.Services.Interface;
using DOCA.API.Validators;
using Microsoft.AspNetCore.Mvc;

namespace DOCA.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.ApiEndpoint)]
public class MemberController : BaseController<MemberController>
{
    private readonly IUserService _userService;
    public MemberController(ILogger<MemberController> logger, IUserService userService) : base(logger)
    {
        _userService = userService;
    }
    
    [HttpGet(ApiEndPointConstant.Member.MemberInformation)]
    [ProducesResponseType(typeof(MemberResponse), statusCode: StatusCodes.Status200OK)]
    [CustomAuthorize(RoleEnum.Member, RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> GetMemberInformationAsync()
    {
        var member = await _userService.GetMemberInformationAsync();
        return Ok(member);
    }
    [HttpPatch(ApiEndPointConstant.Member.MemberEndpoint)]
    [ProducesResponseType(typeof(UserResponse), statusCode: StatusCodes.Status200OK)]
    [CustomAuthorize(RoleEnum.Member, RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> UpdateMemberAsync( UpdateMemberRequest request)
    {
        var response = await _userService.UpdateMemberAsync(request);
        if (response == null)
        {
            _logger.LogError($"Update member failed with {request.Username}");
            return Problem(MessageConstant.User.UpdateFail);
        }
        _logger.LogInformation($"Update member successful with {request.Username}");
        return Ok(response);
    }
}