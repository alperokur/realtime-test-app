public class LoginInputValidator : AbstractValidator<LoginInput>
{
    public LoginInputValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username can not be empty!");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password can not be empty!");
    }
}