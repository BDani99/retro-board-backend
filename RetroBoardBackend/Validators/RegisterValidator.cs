using FluentValidation;
using RetroBoardBackend.Constans;
using RetroBoardBackend.Dtos;

namespace RetroBoardBackend.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Password)
                .Equal(x => x.ConfirmPassword)
                .WithErrorCode(ErrorMessages.PASSWORD_MISMATHCH);
        }
    }
}
