using FluentValidation;
using MediatR;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next(cancellationToken);

        // ← construct failure response dynamically for Response<T> return types
        var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));

        var responseType = typeof(TResponse);

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Response<>))
        {
            var innerType = responseType.GetGenericArguments()[0];
            var failureMethod = typeof(Response<>)
                .MakeGenericType(innerType)
                .GetMethod(nameof(Response<object>.Failure));

            return (TResponse)failureMethod!.Invoke(null, [errorMessage])!;
        }

        throw new ValidationException(failures);
    }
}