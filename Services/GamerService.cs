using AutoMapper;
using CF9Project.Core;
using CF9Project.Data;
using CF9Project.DTO;
using CF9Project.Models;
using CF9Project.Repositories;
using Serilog;
using System;

namespace CF9Project.Services
{
    public class GamerService : IGamerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<GamerService> logger = new LoggerFactory().AddSerilog().CreateLogger<GamerService>();

        public GamerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedGamersAsync(int pageNumber, int pageSize)
        {
            var result = await unitOfWork.GamerRepository.GetPaginatedUsersGamersAsync(pageNumber, pageSize);

            var dtoResult = new PaginatedResult<UserReadOnlyDTO>()
            {
                Data = result.Data.Select(u => new UserReadOnlyDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    UserRole = u.Role.Name
                }).ToList(),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
            logger.LogInformation("Retrieved {Count} users-students", dtoResult.Data.Count);
            return dtoResult;
        }
    }
}