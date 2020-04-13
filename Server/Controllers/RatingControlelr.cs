using System;
using System.Threading.Tasks;
using BlazorDemoUdemy.Shared.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorDemoUdemy.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RatingController : ControllerBase
    {

        private readonly ApplicationDbContext context;
        public UserManager<IdentityUser> userManager { get; }

        public RatingController(ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Rate(MovieRating movieRating){
            var user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
            var userId = user.Id;
            var currentRating = await context.MovieRatings
            .FirstOrDefaultAsync(x=> x.MovieId == movieRating.MovieId &&
               x.UserId == userId);
            
            if (currentRating == null){
                movieRating.UserId = userId;
                movieRating.RatingDate = DateTime.Today;
                context.Add(movieRating);
                await context.SaveChangesAsync();
            } else {
                currentRating.Rate = movieRating.Rate;
                await context.SaveChangesAsync();
            }
            return NoContent();

        }

    }
}