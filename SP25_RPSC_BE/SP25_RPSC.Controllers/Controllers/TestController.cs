//using Microsoft.AspNetCore.Mvc;
//using SP25_RPSC.Data.Models;
//using SP25_RPSC.Data.Models.RefreshTokenModel.Request;
//using SP25_RPSC.Data.Models.ResultModel;
//using SP25_RPSC.Services.Service;
//using SP25_RPSC.Services.Service.RefeshTokenService;
//using System.Net;

//namespace SP25_RPSC.Controllers.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class TestController : ControllerBase
//    {
//        private readonly ICloudinaryStorageService _cloudinaryStorageService;

//        public TestController(ICloudinaryStorageService cloudinaryStorageService)
//        {
//            _cloudinaryStorageService = cloudinaryStorageService;
//        }
//        [HttpPost]
//        [Consumes("multipart/form-data")]
//        //public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
//        //{
//        //    try
//        //    {
//        //        var imageUrl = await _cloudinaryStorageService.UploadImageAsync(request.File);
//        //        return Ok(new { imageUrl });
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return BadRequest(new { error = ex.Message });
//        //    }
//        //}
//    //}
//}

