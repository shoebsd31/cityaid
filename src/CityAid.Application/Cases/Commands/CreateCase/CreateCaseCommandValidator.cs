using FluentValidation;

namespace CityAid.Application.Cases.Commands.CreateCase;

public class CreateCaseCommandValidator : AbstractValidator<CreateCaseCommand>
{
    public CreateCaseCommandValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty()
            .Length(3)
            .WithMessage("City code must be exactly 3 characters");

        RuleFor(x => x.Team)
            .NotEmpty()
            .Must(team => team == "AL" || team == "BE")
            .WithMessage("Team must be either 'AL' (Alpha) or 'BE' (Beta)");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Title is required and cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Budget)
            .GreaterThan(0)
            .When(x => x.Budget.HasValue)
            .WithMessage("Budget must be greater than 0");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be before or equal to end date");
    }
}