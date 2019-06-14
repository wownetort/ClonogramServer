﻿using System;
using System.Threading.Tasks;
using Clonogram.Services;
using Clonogram.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clonogram.Controllers
{
    [Authorize]
    public class PhotosController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly IHashtagService _hashtagService;

        public PhotosController(IPhotoService photoService, IHashtagService hashtagService)
        {
            _photoService = photoService;
            _hashtagService = hashtagService;
        }
        public async Task<IActionResult> GetPhotosByHashtag(string hashtag)
        {
            var photos = await _hashtagService.GetPhotos(hashtag);
            return Ok(photos);
        }
        public async Task<IActionResult> GetLikesCount(string id)
        {
            var guid = Guid.Parse(id);
            var likesCount = await _photoService.GetLikesCount(guid);
            return Ok(likesCount);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(PhotoView photo)
        {
            var file = HttpContext.Request.Form.Files[0];
            photo.UserId = HttpContext.User.Identity.Name;

            await _photoService.Upload(file, photo);

            return Ok();
        }

        public async Task<IActionResult> GetById(string id)
        {
            var guid = Guid.Parse(id);
            var photoView = await _photoService.GetById(guid);
            return Ok(photoView);
        }

        [HttpPut]
        public async Task<IActionResult> Update(PhotoView photoView)
        {
            try
            {
                photoView.UserId = HttpContext.User.Identity.Name;
                await _photoService.Update(photoView);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var userId = Guid.Parse(HttpContext.User.Identity.Name);
                var photoId = Guid.Parse(id);
                await _photoService.Delete(userId, photoId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Like(string photoId)
        {
            var photoGuid = Guid.Parse(photoId);
            var userId = Guid.Parse(HttpContext.User.Identity.Name);

            await _photoService.Like(userId, photoGuid);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveLike(string photoId)
        {
            var photoGuid = Guid.Parse(photoId);
            var userId = Guid.Parse(HttpContext.User.Identity.Name);

            await _photoService.RemoveLike(userId, photoGuid);

            return Ok();
        }
    }
}
