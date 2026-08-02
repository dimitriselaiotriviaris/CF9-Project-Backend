using AutoMapper;

using CF9Project.DTO;
using CF9Project.Exceptions;
using CF9Project.Models;
using CF9Project.Repositories;
using CF9Project.Security;


namespace CF9Project.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEncryptionUtil _encryptionUtil;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CompanyService> logger, IEncryptionUtil encryptionUtil)
        {
            _encryptionUtil = encryptionUtil;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task SignUpUserAsync(CompanySignupDTO request)
        {
            Company company = _mapper.Map<Company>(request);
            User user = _mapper.Map<User>(request);

            try
            {
                User? existingUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(user.Username);

                if (existingUser != null)
                {
                    throw new EntityAlreadyExistsException("User", "User with username " +
                        existingUser.Username + " already exists");
                }

                user.Company = company;
                user.Password = _encryptionUtil.Encrypt(user.Password);
                await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.CompanyRepository.AddAsync(company);

                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Company {Company} signed up successfully.", company);        // ToDo toString in Company
            }
            catch (EntityAlreadyExistsException ex)
            {
                _logger.LogError("Error signing up company {Company}. {Message}", company, ex.Message);
                throw;
            }
        }
    }
}
