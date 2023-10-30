using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Errors.Model;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using TFAuto.DAL.Constant;
using TFAuto.DAL.Entities;
using TFAuto.Domain.ExtensionMethods;
using TFAuto.Domain.Services.Admin.DTO.Request;
using TFAuto.Domain.Services.Admin.DTO.Response;
using User = TFAuto.TFAuto.DAL.Entities.User;

namespace TFAuto.Domain.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<User> _repositoryUser;
        private readonly IRepository<Role> _repositoryRole;

        public AdminService(
            IRepository<User> repositoryUser,
            IRepository<Role> repositoryRole)
        {
            _repositoryUser = repositoryUser;
            _repositoryRole = repositoryRole;

        }

        public async ValueTask<GetUserResponse> ChangeUserRoleAsync(Guid userId, string userNewRole)
        {
            var user = await _repositoryUser.GetAsync(t => t.Id == userId.ToString()).FirstOrDefaultAsync();
            if (user is null)
                throw new ValidationException(ErrorMessages.USER_NOT_FOUND);

            var currentUserRole = await _repositoryRole.GetAsync(user.RoleId, nameof(Role));
            if (currentUserRole is null)
                throw new NotFoundException(ErrorMessages.ROLE_NOT_FOUND);

            var role = await _repositoryRole.GetAsync(t => t.RoleName == userNewRole).FirstOrDefaultAsync();
            if (role is null)
                throw new NotFoundException(ErrorMessages.ROLE_NOT_FOUND);

            if (currentUserRole == role)
                throw new Exception(ErrorMessages.ROLES_ARE_EQUAL);

            user.RoleId = role.Id;
            await _repositoryUser.UpdateAsync(user);

            var userInfo = new GetUserResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = role.RoleName
            };

            return userInfo;
        }

        public async ValueTask DeleteUserAsync(Guid userId)
        {
            var user = await _repositoryUser.GetAsync(t => t.Id == userId.ToString()).FirstOrDefaultAsync();

            if (user is null)
                throw new ValidationException(ErrorMessages.USER_NOT_FOUND);

            await _repositoryUser.DeleteAsync(user);
        }

        public async ValueTask<GetAllUsersResponse> GetAllUsersAsync(GetUsersPaginationRequest paginationRequest)
        {
            string queryUsers = await BuildQuery(paginationRequest);
            var users = await _repositoryUser.GetByQueryAsync(queryUsers);

            if (users == null)
                throw new NotFoundException(ErrorMessages.USERS_NOT_FOUND);

            var totalItems = users.Count();

            if (totalItems <= paginationRequest.Skip)
                throw new NotFoundException(ErrorMessages.USERS_NOT_FOUND);

            if ((totalItems - paginationRequest.Skip) < paginationRequest.Take)
                paginationRequest.Take = (totalItems - paginationRequest.Skip);

            var userList = users
                .Skip(paginationRequest.Skip)
                .Take(paginationRequest.Take)
                .Select(async user => await ConvertGetUserResponse(user))
                .WhenAll()
                .ToList();

            var allUsersResponse = new GetAllUsersResponse()
            {
                TotalItems = totalItems,
                Skip = paginationRequest.Skip,
                Take = paginationRequest.Take,
                OrderBy = paginationRequest.SortBy,
                Users = userList
            };

            return allUsersResponse;
        }

        private async ValueTask<string> BuildQuery(GetUsersPaginationRequest paginationRequest)
        {
            string baseQuery = $"SELECT * FROM c WHERE c.type = \"{nameof(User)}\"";
            StringBuilder queryBuilder = new StringBuilder(baseQuery);

            if (!paginationRequest.Text.IsNullOrEmpty())
            {
                queryBuilder.Append(" AND (");

                queryBuilder.Append(
                    $"CONTAINS(LOWER(c.{nameof(User.UserName).FirstLetterToLower()}), LOWER(\"{paginationRequest.Text}\")) " +
                    $"OR CONTAINS(LOWER(c.{nameof(User.Email).FirstLetterToLower()}), LOWER(\"{paginationRequest.Text}\"))");

                queryBuilder.Append(") ");
            }

            switch (paginationRequest.SortBy)
            {
                case SortOrderUsers.UserNameAscending:
                    queryBuilder.Append($" ORDER BY c.{nameof(User.UserName).FirstLetterToLower()} ASC");
                    break;

                case SortOrderUsers.UserNameDescending:
                    queryBuilder.Append($" ORDER BY c.{nameof(User.UserName).FirstLetterToLower()} DESC");
                    break;

                case SortOrderUsers.UserRole:

                    queryBuilder.Append($" AND c.roleId = \"{RoleId.USER}\"");
                    queryBuilder.Append($" ORDER BY c.{nameof(User.UserName).FirstLetterToLower()} ASC");
                    break;

                case SortOrderUsers.AuthorRole:
                    queryBuilder.Append($" AND c.roleId = \"{RoleId.AUTHOR}\"");
                    queryBuilder.Append($" ORDER BY c.{nameof(User.UserName).FirstLetterToLower()} ASC");
                    break;

                case SortOrderUsers.SuperAdminRole:
                    queryBuilder.Append($" AND c.roleId = \"{RoleId.SUPER_ADMIN}\"");
                    queryBuilder.Append($" ORDER BY c.userName ASC");
                    break;
            }

            return queryBuilder.ToString();
        }

        private async ValueTask<GetUserResponse> ConvertGetUserResponse(User user)
        {
            var role = await _repositoryRole.GetAsync(user.RoleId, nameof(Role));

            if (role is null)
                throw new NotFoundException(ErrorMessages.ROLE_NOT_FOUND);

            var userResponse = new GetUserResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = role.RoleName
            };

            return userResponse;
        }
    }
}