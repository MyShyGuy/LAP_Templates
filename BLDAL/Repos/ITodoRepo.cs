using DB_Models.DTO;
using DB_Models.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLDAL.Repos
{
    public interface ITodoRepo
    {
        public List<TodoDTO> GetTodoDTOs(User usr);
        public void SaveTodos(List<TodoDTO> todos);
        public void deleteTodos(List<TodoDTO> todos);
    }
}
