using FluentValidation;

namespace ToDoRepositoryPattern.DTOs.DTOValidations;

public class ToDoCreateDTOValidator : AbstractValidator<ToDoCreateDTO>
{
	public ToDoCreateDTOValidator()
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
