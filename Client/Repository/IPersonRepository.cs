using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDemoUdemy.Shared.DTO;
using BlazorDemoUdemy.Shared.Entity;

namespace BlazorDemoUdemy.Client.Repository{

    public interface IPersonRepository
    {
        Task CreatePerson (Person person);
        Task<PaginatedResponse<List<Person>>> GetPeople(PaginationDTO paginationDTO);
         Task<List<Person>> GetPeopleByName(string name);
         Task UpdatePerson(Person person);
         Task<Person> GetPeopleById(int id);
         Task DeletePerson(int id);
    }
}