using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemoUdemy.Client.Helpers;
using BlazorDemoUdemy.Shared.DTO;
using BlazorDemoUdemy.Shared.Entity;

namespace BlazorDemoUdemy.Client.Repository{

    public class PersonRepository: IPersonRepository
    {
        private readonly IHttpService httpService;
        private string url = "api/people";
        public PersonRepository(IHttpService httpService)
        {
            this.httpService = httpService;
        }
        public async Task CreatePerson(Person person){
            var response = await httpService.Post(url, person);
            if (!response.Success){
                throw new ApplicationException(await response.GetBody());
            }
            
        }

        public async Task<PaginatedResponse<List<Person>>> GetPeople(PaginationDTO paginationDTO){
           return await httpService.GetHelper<List<Person>>(url, paginationDTO);
        }

         public async Task<List<Person>> GetPeopleByName(string name){
            var response = await httpService.Get<List<Person>>($"{url}/search/{name}");
            if (!response.Success){
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task<Person> GetPeopleById(int id){
            return await httpService.GetHelper<Person>($"{url}/{id}");
        }

        public async Task UpdatePerson(Person person){
            var response = await httpService.Put(url, person);
            if (!response.Success){
                throw new ApplicationException(await response.GetBody());
            }            
        }

        public async Task DeletePerson(int id)
        {
            var response = await httpService.Delete($"{url}/{id}");
            if (!response.Success){
                throw new ApplicationException(await response.GetBody());
            }
        }
    }
}