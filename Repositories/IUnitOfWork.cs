namespace CF9Project.Repositories
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IGamerRepository GamerRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        IGameRepository GameRepository { get; }

        Task<bool> SaveAsync();
    }
}
