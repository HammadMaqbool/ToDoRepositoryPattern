using FluentValidation;

namespace ToDoRepositoryPattern.DTOs.DTOValidations;

public class ToDoUpdateDTOValidator : AbstractValidator<ToDoUpdateDTO>
{
	public ToDoUpdateDTOValidator()
	{
		RuleFor(x => x.Title)
			.MinimumLength(10)
			.MaximumLength(50)
			.NotEmpty();

		RuleFor(x => x.Description)
			.MinimumLength(10)
			.MaximumLength(50)
			.NotEmpty();

	}
}
