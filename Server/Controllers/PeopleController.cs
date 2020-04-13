using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorDemoUdemy.Server.Helpers;
using BlazorDemoUdemy.Shared.DTO;
using BlazorDemoUdemy.Shared.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorDemoUdemy.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PeopleController : ControllerBase{
        private readonly ApplicationDbContext context;
        private readonly IFileStorageService fileStorageService;
        private readonly IMapper mapper;
        public PeopleController(ApplicationDbContext context, 
        IFileStorageService fileStorageService,
        IMapper mapper)
        {
            this.context = context;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Person>>> Get([FromQuery] PaginationDTO paginationDTO){
            var queryable = context.People.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable,paginationDTO.RecordsPerPage);
            return await queryable.Paginate(paginationDTO).ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Person>> Get(int id){
            var person = await context.People.FirstOrDefaultAsync(e=>e.Id==id);
            if (person==null){
                return NotFound();
            }
            return person;
        }

        [HttpGet("search/{searchText}")]
        public async Task<ActionResult<List<Person>>> FilterByName(string searchText){
            if (string.IsNullOrWhiteSpace(searchText)){
                return new List<Person>();
            }
            return await context.People.Where(e=>e.Name.Contains(searchText)).Take(5).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post (Person person){
            if (!string.IsNullOrEmpty(person.Picture)){
                var personPicture = Convert.FromBase64String(person.Picture);
                person.Picture = await fileStorageService.SaveFile(personPicture, "jpg", "people");
            }

            context.Add(person);
            await context.SaveChangesAsync();
            return person.Id;

        }

        [HttpPut]
        public async Task<ActionResult> Put(Person person){
            var personDB = await context.People.FirstOrDefaultAsync(e=>e.Id == person.Id);
            if (personDB == null) {return NotFound();} 
            personDB = mapper.Map(person, personDB);
            if (!string.IsNullOrWhiteSpace(person.Picture)){
                var personPicture = Convert.FromBase64String(person.Picture);
                personDB.Picture = await fileStorageService.EditFile(personPicture, "jpg", "people", personDB.Picture);
            }
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id){
            var person = await context.People.FirstOrDefaultAsync(e=> e.Id == id);
            if (person == null){
                return NotFound();
            }
            context.Remove(person);
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}