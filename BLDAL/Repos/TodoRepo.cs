using DB_Models.DTO;
using DB_Models.Models;
using DB_Models.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLDAL.Repos
{
    public class TodoRepo : ITodoRepo
    {
        private readonly AppDBContext dbc;
        public void deleteTodos(List<TodoDTO> todos)
        {
            throw new NotImplementedException();
        }

        public List<TodoDTO> GetTodoDTOs(User usr)
        {
            return dbc.Todos.Where(t => t.UserID == usr.UserID).Select(t => new TodoDTO
            {
                Title = t.Title,
                Description = t.Description,
                IsDone = t.IsDone,
                CreatedAt = t.CreatedAt,
                DueDate = t.DueDate,
                User = t.User
            }).ToList();
        }

        public void SaveTodoDesc(List<TodoDTO> todos)
        {
            throw new NotImplementedException();
        }

        public void SaveTodos(List<TodoDTO> todos)
        {
            throw new NotImplementedException();
        }
    }
}