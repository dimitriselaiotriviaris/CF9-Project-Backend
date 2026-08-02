namespace CF9Project.Services
{
    public interface IApplicationService
    {
        IUserService UserService { get;  }
        ICompanyService CompanyService { get; }
        IGamerService GamerService { get; }
        // Other services can be added here as needed
    }
}
