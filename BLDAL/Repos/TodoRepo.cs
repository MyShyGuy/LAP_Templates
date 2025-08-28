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

        public TodoRepo(AppDBContext dbc)
        {
            this.dbc = dbc;
        }
        public void deleteTodos(List<TodoDTO> todos)
        {
            var toDelete = todos
                .Where(t => t.IsDone) // nur erledigte Todos
                .ToList();

            foreach (var dto in toDelete)
            {
                var entity = dbc.Todos
                    .FirstOrDefault(t =>
                        t.Title == dto.Title &&
                        t.CreatedAt == dto.CreatedAt &&
                        t.UserID == dto.User.UserID);

                if (entity != null)
                {
                    dbc.Todos.Remove(entity);
                }
            }
            dbc.SaveChanges();
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
            var TodosTOSave = new List<TodoItem>();
            foreach (var todo in todos)
            {
                var todomodel = new TodoItem
                {
                    Title = todo.Title,
                    Description = todo.Description,
                    IsDone = todo.IsDone,
                    CreatedAt = todo.CreatedAt,
                    DueDate = todo.DueDate,
                    UserID = todo.User.UserID,
                    User = todo.User
                };
                var entity = dbc.Todos
                            .FirstOrDefault(t =>
                                t.Title == todo.Title &&
                                t.CreatedAt == todo.CreatedAt &&
                                t.UserID == todo.User.UserID);
                if (entity == null)
                {
                    TodosTOSave.Add(todomodel);
                }
                else
                {
                    // vorhandenes Todo aktualisieren
                    entity.Title = todo.Title;
                    entity.Description = todo.Description;
                    entity.IsDone = todo.IsDone;
                    entity.DueDate = todo.DueDate;
                }
            }
            dbc.Todos.AddRange(TodosTOSave);
            dbc.SaveChanges();
        }
    }
}