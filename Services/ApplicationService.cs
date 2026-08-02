namespace CF9Project.Services
{
    public class ApplicationService : IApplicationService
    {
        public IUserService UserService { get; }
        public ICompanyService CompanyService { get; }
        public IGamerService GamerService { get; }

        public ApplicationService(IUserService userService, 
            ICompanyService companyService, IGamerService gamerService)
        {
            UserService = userService;
            CompanyService = companyService;
            GamerService = gamerService;
        }
    }
}
