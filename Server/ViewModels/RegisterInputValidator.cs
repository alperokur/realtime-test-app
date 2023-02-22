public class RegisterInputValidator : AbstractValidator<RegisterInput>
{
    private readonly DatabaseContext _dbContext;
    public RegisterInputValidator(IDbContextFactory<DatabaseContext> dbContextFactory)
    {
        _dbContext = dbContextFactory.CreateDbContext();

        RuleFor(t => t.Username).NotEmpty().MaximumLength(16).MinimumLength(3)
            .Must(IsUniqueUsername).WithMessage("This username is used!");
    }

    private bool IsUniqueUsername(string username)
    {
        return !_dbContext.Accounts.Any(t => t.Username == username);
    }
}