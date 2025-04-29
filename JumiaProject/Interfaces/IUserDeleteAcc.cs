namespace JumiaProject.Interfaces
{
    public interface IUserDeleteAcc
    {
        Task<bool> ValidatePassword(string email, string password);
        Task<bool> DeleteAccount(string email);
        Task RemoveUserDataByEmail(string email);
    }
}
