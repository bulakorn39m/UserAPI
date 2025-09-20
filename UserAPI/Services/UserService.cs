using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using UserAPI.Models;

namespace UserAPI.Services
{
    public interface IUserService
    {
        Task<(List<UserDo>? Result, ProblemDetails? Problem)> GetAllUsers();
        Task<(UserDo? Result, ProblemDetails? Problem)> GetUserById(long userId);
        Task<(UserDo? Result, ProblemDetails? Problem)> AddUser(UserDo user);
        Task<(UserDo? Result, ProblemDetails? Problem)> UpdateUser(UserDo user);
        Task<(bool Result, ProblemDetails? Problem)> DeleteUser(long userId);
    }

    public class UserService : IUserService
    {
        private readonly ConcurrentDictionary<long, UserDo> _usersStore;
        private long _nextId;
        private string notFounMsg = "Users not found";
        private string duplicateMsg = "Duplicate user";

        public UserService(ConcurrentDictionary<long, UserDo> usersStore)
        {
            _usersStore = usersStore;
            _nextId = _usersStore.Count > 0 ? _usersStore.Keys.Max() + 1 : 1;
        }

        public async Task<(List<UserDo>? Result, ProblemDetails? Problem)> GetAllUsers()
        {
            if (!_usersStore.Any())
            {
                return (null, CreateProblem(StatusCodes.Status204NoContent, notFounMsg, notFounMsg));
            }

            var result = _usersStore.Values.ToList();

            return (result, null);
        }

        public async Task<(UserDo? Result, ProblemDetails? Problem)> GetUserById(long userId)
        {
            var result = _usersStore.Values.FirstOrDefault(f => f.Id == userId);
            if (result == null)
            {
                return (null, CreateProblem(StatusCodes.Status404NotFound, notFounMsg, notFounMsg));
            }


            return (result, null);
        }

        public async Task<(UserDo? Result, ProblemDetails? Problem)> AddUser(UserDo user)
        {
            var problem = ValidateObject(user);
            if (problem != null)
                return (null, problem);

            if(!_usersStore.Values.Any(t => t.Username == user.Username && t.Email == t.Email))
            {
                user.Id = _nextId++;
                _usersStore.TryAdd(user.Id.Value, user);
                return (user, null);
            }

            return (null, CreateProblem(StatusCodes.Status400BadRequest, duplicateMsg, duplicateMsg));
        }

        public async Task<(UserDo? Result, ProblemDetails? Problem)> UpdateUser(UserDo user)
        {
            var problem = ValidateObject(user);
            if (problem != null)
                return (null, problem);

            if (user.Id != null)
            {
                if (_usersStore.ContainsKey(user.Id.Value))
                {
                    _usersStore[user.Id.Value] = user;
                    return (user, null);
                }
            }

            return (null, CreateProblem(StatusCodes.Status404NotFound, notFounMsg, notFounMsg));
        }

        public async Task<(bool Result, ProblemDetails? Problem)> DeleteUser(long userId)
        {
            if (_usersStore.ContainsKey(userId))
            {
                if (_usersStore.TryRemove(userId, out _))
                {
                    return (true, null);
                }

            }

            return (false, CreateProblem(StatusCodes.Status404NotFound, notFounMsg, notFounMsg));
        }

        #region private method
        private ProblemDetails? CreateProblem(int status, string title, string detail)
        {

            return new ProblemDetails()
            {
                Status = status,
                Title = title,
                Detail = detail,
            };
        }

        private ProblemDetails? ValidateObject(object entity)
        {
            List<string?> errors = Validate(entity);
            if (errors.Count > 0)
            {
                return CreateProblem(
                    status: StatusCodes.Status400BadRequest,
                    title: "NotValid.Header",
                    detail: string.Format("{0} is required", string.Join(",", errors)));
            }

            return null;
        }

        private List<string?> Validate(object entity)
        {
            List<string?> errors = new List<string?>();

            if (entity != null)
            {
                var results = new List<ValidationResult>();
                var context = new ValidationContext(entity);
                if (Validator.TryValidateObject(entity, context, results, true) == false)
                {
                    foreach (var r in results)
                    {
                        errors.Add(r.ErrorMessage);
                    }
                }

                foreach (var prop in entity.GetType().GetProperties())
                {
                    if (prop.PropertyType.IsGenericType
                        && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        System.Collections.IList? list = prop.GetValue(entity, null) as System.Collections.IList;
                        if (list != null)
                        {
                            foreach (var l in list)
                            {
                                errors.AddRange(Validate(l));
                            }
                        }
                    }
                }
            }

            return errors;
        }
        #endregion
    }
}
