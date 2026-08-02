using CF9Project.DTO;

namespace CF9Project.Services
{
    public interface ICompanyService
    {
        Task SignUpUserAsync(CompanySignupDTO request);
    }
}
