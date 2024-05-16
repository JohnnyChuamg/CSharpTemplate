using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.ResultContracts;

public class Result
{
	public ResultCode Code { get; set; }

	[JsonIgnore] public int HttpStatusCode => ResultHelper.ConvertHttpStatusCode(Code);

	public string? Message { get; set; }

	[JsonIgnore] public bool IsSuccess => ResultHelper.IsSuccess(Code);

	protected static Result SuccessResult { get; } = new Result(ResultCode.Success);


	protected static Result SuccessNoContentResult { get; } = new Result(ResultCode.SuccessNoContent);


	protected static Result SuccessCreatedResult { get; } = new Result(ResultCode.SuccessCreated);


	protected static Result AcceptedResult { get; } = new Result(ResultCode.Accepted);


	protected static Result InvalidInputResult { get; } = new Result(ResultCode.InvalidInput);


	protected static Result UnauthorizedResult { get; } = new Result(ResultCode.Unauthorized);


	protected static Result ForbiddenResult { get; } = new Result(ResultCode.Forbidden);


	protected static Result NotFoundResult { get; } = new Result(ResultCode.ResourceNotFound);


	protected static Result RequestTimeoutResult { get; } = new Result(ResultCode.RequestTimeout);


	protected static Result ConflictResult { get; } = new Result(ResultCode.ResourceAlreadyExists);


	protected static Result InternalServerErrorResult { get; } = new Result(ResultCode.InternalServerError);


	public Result()
	{
	}

	public Result(ResultCode code)
		: this(code, ResultHelper.GetEnumInfo(code).Description)
	{
	}

	public Result(ResultCode code, string? message)
	{
		Code = code;
		Message = message;
	}

	public static Result Create(ResultCode code)
		=> new Result(code);

	public static Result Create(ResultCode code, string? message)
		=> new Result(code, message);

	public static Task<Result> CreateAsync(ResultCode code)
		=> Task.FromResult(Create(code));

	public static Task<Result> CreateAsync(ResultCode code, string? message)
		=> Task.FromResult(Create(code, message));

	public static Result<T> Create<T>(ResultCode code)
		=> new(code, default(T));

	public static Result<T> Create<T>(ResultCode code, T value)
		=> new(code, value);

	public static Result<T> Create<T>(ResultCode code, string? message, T value)
		=> new(code, message, value);

	public static Result<T> Create<T>(ResultCode code, T value, Action<Paging> paging)
		=> new(code, value, paging);

	public static Result<T> Create<T>(ResultCode code, string? message, T value, Action<Paging> paging)
		=> new(code, message, value, paging);

	public static Task<Result<T>> CreateAsync<T>(ResultCode code)
		=> Task.FromResult(Create<T>(code, default));

	public static Task<Result<T>> CreateAsync<T>(ResultCode code, T value)
		=> Task.FromResult(Create(code, value));

	public static Task<Result<T>> CreateAsync<T>(ResultCode code, string? message, T value)
		=> Task.FromResult(Create(code, message, value));

	public static Task<Result<T>> CreateAsync<T>(ResultCode code, T value, Action<Paging> paging)
		=> Task.FromResult(Create(code, value, paging));

	public static Task<Result<T>> CreateAsync<T>(ResultCode code, string? message, T value, Action<Paging> paging)
		=> Task.FromResult(Create(code, message, value, paging));

	public static Result Success()
		=> SuccessResult;

	public static Result Success(string? message)
		=> Create(ResultCode.Success, message);

	public static Task<Result> SuccessAsync()
		=> Task.FromResult(Success());

	public static Task<Result> SuccessAsync(string? message)
		=> Task.FromResult(Success(message));

	public static Result<T> Success<T>()
		=> Result<T>.SuccessResult;

	public static Result<T> Success<T>(T value)
		=> Create(ResultCode.Success, value);

	public static Result<T> Success<T>(string? message, T value)
		=> Create(ResultCode.Success, message, value);

	public static Result<T> Success<T>(T value, Action<Paging> paging)
		=> Create(ResultCode.Success, value, paging);

	public static Result<T> Success<T>(string? message, T value, Action<Paging> paging)
		=> Create(ResultCode.Success, message, value, paging);

	public static Task<Result<T>> SuccessAsync<T>()
		=> Task.FromResult(Result<T>.SuccessResult);

	public static Task<Result<T>> SuccessAsync<T>(T value)
		=> Task.FromResult(Create(ResultCode.Success, value));

	public static Task<Result<T>> SuccessAsync<T>(string? message, T value)
		=> Task.FromResult(Create(ResultCode.Success, message, value));

	public static Task<Result<T>> SuccessAsync<T>(T value, Action<Paging> paging)
		=> Task.FromResult(Create(ResultCode.Success, value, paging));

	public static Task<Result<T>> SuccessAsync<T>(string? message, T value, Action<Paging> paging)
		=> Task.FromResult(Create(ResultCode.Success, message, value, paging));

	public static Result SuccessCreated()
		=> SuccessCreatedResult;

	public static Result SuccessCreated(string? message)
		=> Create(ResultCode.SuccessCreated, message);

	public static Task<Result> SuccessCreatedAsync()
		=> Task.FromResult(SuccessCreatedResult);

	public static Task<Result> SuccessCreatedAsync(string? message)
		=> Task.FromResult(SuccessCreated(message));

	public static Result<T> SuccessCreated<T>()
		=> Result<T>.SuccessCreatedResult;

	public static Result<T> SuccessCreated<T>(T value)
		=> Create(ResultCode.SuccessCreated, value);

	public static Result<T> SuccessCreated<T>(string? message, T value)
		=> Create(ResultCode.SuccessCreated, message, value);

	public static Result<T> SuccessCreated<T>(T value, Action<Paging> paging)
		=> Create(ResultCode.SuccessCreated, value, paging);

	public static Result<T> SuccessCreated<T>(string? message, T value, Action<Paging> paging)
		=> Create(ResultCode.SuccessCreated, message, value, paging);

	public static Task<Result<T>> SuccessCreatedAsync<T>()
		=> Task.FromResult(Result<T>.SuccessCreatedResult);

	public static Task<Result<T>> SuccessCreatedAsync<T>(T value)
		=> Task.FromResult(SuccessCreated(value));

	public static Task<Result<T>> SuccessCreatedAsync<T>(string? message, T value)
		=> Task.FromResult(SuccessCreated(message, value));

	public static Task<Result<T>> SuccessCreatedAsync<T>(T value, Action<Paging> paging)
		=> Task.FromResult(SuccessCreated(value, paging));

	public static Task<Result<T>> SuccessCreatedAsync<T>(string? message, T value, Action<Paging> paging)
		=> Task.FromResult(SuccessCreated(message, value, paging));

	public static Result Accepted()
		=> AcceptedResult;

	public static Result Accepted(string? message)
		=> Create(ResultCode.Accepted, message);

	public static Task<Result> AcceptedAsync()
		=> Task.FromResult(AcceptedResult);

	public static Result<T> Accepted<T>()
		=> Result<T>.AcceptedResult;

	public static Result<T> Accepted<T>(string? message)
		=> new(ResultCode.Accepted, message);

	public static Task<Result<T>> AcceptedAsync<T>()
		=> Task.FromResult(Result<T>.AcceptedResult);

	public static Result SuccessNoContent()
		=> SuccessNoContentResult;

	public static Task<Result> SuccessNoContentAsync()
		=> Task.FromResult(SuccessNoContentResult);

	public static Result<T> SuccessNoContent<T>()
		=> Result<T>.SuccessNoContentResult;

	public static Task<Result<T>> SuccessNoContentAsync<T>()
		=> Task.FromResult(SuccessNoContent<T>());

	public static Result InvalidInput()
		=> InvalidInputResult;

	public static Result InvalidInput(string? message)
		=> Create(ResultCode.InvalidInput, message);

	public static Task<Result> InvalidInputAsync()
		=> Task.FromResult(InvalidInputResult);

	public static Task<Result> InvalidInputAsync(string? message)
		=> Task.FromResult(InvalidInput(message));

	public static Result<T> InvalidInput<T>()
		=> Result<T>.InvalidInputResult;

	public static Result<T> InvalidInput<T>(string? message)
		=> new(ResultCode.InvalidInput, message);

	public static Task<Result<T>> InvalidInputAsync<T>()
		=> Task.FromResult(InvalidInput<T>());

	public static Task<Result<T>> InvalidInputAsync<T>(string? message)
		=> Task.FromResult(InvalidInput<T>(message));

	public static Result Unauthorized()
		=> UnauthorizedResult;

	public static Result Unauthorized(string? message)
		=> Create(ResultCode.Unauthorized, message);

	public static Task<Result> UnauthorizedAsync()
		=> Task.FromResult(UnauthorizedResult);

	public static Task<Result> UnauthorizedAsync(string? message)
		=> Task.FromResult(Unauthorized(message));

	public static Result<T> Unauthorized<T>()
		=> Result<T>.UnauthorizedResult;

	public static Result<T> Unauthorized<T>(string? message)
		=> new(ResultCode.Unauthorized, message);

	public static Task<Result<T>> UnauthorizedAsync<T>()
		=> Task.FromResult(Unauthorized<T>());

	public static Task<Result<T>> UnauthorizedAsync<T>(string? message)
		=> Task.FromResult(Unauthorized<T>(message));

	public static Result Forbidden()
		=> ForbiddenResult;

	public static Result Forbidden(string? message)
		=> Create(ResultCode.Forbidden, message);

	public static Task<Result> ForbiddenAsync()
		=> Task.FromResult(ForbiddenResult);

	public static Task<Result> ForbiddenAsync(string? message)
		=> Task.FromResult(Forbidden(message));

	public static Result<T> Forbidden<T>()
		=> Result<T>.ForbiddenResult;

	public static Result<T> Forbidden<T>(string? message)
		=> new(ResultCode.Forbidden, message);

	public static Task<Result<T>> ForbiddenAsync<T>()
		=> Task.FromResult(Forbidden<T>());

	public static Task<Result<T>> ForbiddenAsync<T>(string? message)
		=> Task.FromResult(Forbidden<T>(message));

	public static Result NotFound()
		=> NotFoundResult;

	public static Result NotFound(string? message)
		=> Create(ResultCode.ResourceNotFound, message);

	public static Task<Result> NotFoundAsync()
		=> Task.FromResult(NotFound());

	public static Task<Result> NotFoundAsync(string? message)
		=> Task.FromResult(NotFound(message));

	public static Result<T> NotFound<T>()
		=> Result<T>.NotFoundResult;

	public static Result<T> NotFound<T>(string? message)
		=> new(ResultCode.ResourceNotFound, message);

	public static Task<Result<T>> NotFoundAsync<T>()
		=> Task.FromResult(Result<T>.NotFoundResult);

	public static Task<Result<T>> NotFoundAsync<T>(string? message)
		=> Task.FromResult(new Result<T>(ResultCode.ResourceNotFound, message));

	public static Result RequestTimeout()
		=> RequestTimeoutResult;

	public static Result RequestTimeout(string? message)
		=> new(ResultCode.RequestTimeout, message);

	public static Task<Result> RequestTimeoutAsync()
		=> Task.FromResult(RequestTimeoutResult);

	public static Task<Result> RequestTimeoutAsync(string? message)
		=> Task.FromResult(new Result(ResultCode.RequestTimeout, message));

	public static Result<T> RequestTimeout<T>()
		=> Result<T>.RequestTimeoutResult;

	public static Result<T> RequestTimeout<T>(string? message)
		=> new(ResultCode.RequestTimeout, message);

	public static Task<Result<T>> RequestTimeoutAsync<T>()
		=> Task.FromResult(Result<T>.RequestTimeoutResult);

	public static Task<Result<T>> RequestTimeoutAsync<T>(string? message)
		=> Task.FromResult(new Result<T>(ResultCode.RequestTimeout, message));

	public static Result Conflict()
		=> ConflictResult;

	public static Result Conflict(string? message)
		=> new(ResultCode.ResourceAlreadyExists, message);

	public static Task<Result> ConflictAsync()
		=> Task.FromResult(ConflictResult);

	public static Task<Result> ConflictAsync(string? message)
		=> Task.FromResult(new Result(ResultCode.ResourceAlreadyExists, message));

	public static Result<T> Conflict<T>()
		=> Result<T>.ConflictResult;

	public static Result<T> Conflict<T>(string? message)
		=> new(ResultCode.ResourceAlreadyExists, message);

	public static Task<Result<T>> ConflictAsync<T>()
		=> Task.FromResult(Result<T>.ConflictResult);

	public static Task<Result<T>> ConflictAsync<T>(string? message)
		=> Task.FromResult(new Result<T>(ResultCode.ResourceAlreadyExists, message));

	public static Result InternalServerError()
		=> InternalServerErrorResult;

	public static Result InternalServerError(string? message)
		=> new(ResultCode.InternalServerError, message);

	public static Task<Result> InternalServerErrorAsync()
		=> Task.FromResult(InternalServerErrorResult);

	public static Task<Result> InternalServerErrorAsync(string? message)
		=> Task.FromResult(new Result(ResultCode.InternalServerError, message));

	public static Result<T> InternalServerError<T>()
		=> Result<T>.InternalServerErrorResult;

	public static Result<T> InternalServerError<T>(string? message)
		=> new(ResultCode.InternalServerError, message);

	public static Task<Result<T>> InternalServerErrorAsync<T>()
		=> Task.FromResult(Result<T>.InternalServerErrorResult);

	public static Task<Result<T>> InternalServerErrorAsync<T>(string? message)
		=> Task.FromResult(new Result<T>(ResultCode.InternalServerError, message));
}

public class Result<T> : Result
{
	public T? Content { get; set; }

	public Paging? Paging { get; set; }

	internal new static Result<T> SuccessResult { get; } = new Result<T>(ResultCode.Success);


	internal new static Result<T> SuccessNoContentResult { get; } = new Result<T>(ResultCode.SuccessNoContent);


	internal new static Result<T> SuccessCreatedResult { get; } = new Result<T>(ResultCode.SuccessCreated);


	internal new static Result<T> AcceptedResult { get; } = new Result<T>(ResultCode.Accepted);


	internal new static Result<T> InvalidInputResult { get; } = new Result<T>(ResultCode.InvalidInput);


	internal new static Result<T> UnauthorizedResult { get; } = new Result<T>(ResultCode.Unauthorized);


	internal new static Result<T> ForbiddenResult { get; } = new Result<T>(ResultCode.Forbidden);


	internal new static Result<T> NotFoundResult { get; } = new Result<T>(ResultCode.ResourceNotFound);


	internal new static Result<T> RequestTimeoutResult { get; } = new Result<T>(ResultCode.RequestTimeout);


	internal new static Result<T> ConflictResult { get; } = new Result<T>(ResultCode.ResourceAlreadyExists);


	internal new static Result<T> InternalServerErrorResult { get; } = new Result<T>(ResultCode.InternalServerError);

	public Result(ResultCode code)
		: this(code, ResultHelper.GetEnumInfo(code).Description)
	{
	}

	public Result(ResultCode code, string? message)
	{
		Code = code;
		Message = message;
	}

	public Result(ResultCode code, T content)
		: this(code)
	{
		Content = content;
	}

	public Result(ResultCode code, T content, Action<Paging>? paging = null)
		: this(code)
	{
		Content = content;
		if (paging == null) return;
		Paging = new Paging();
		paging(Paging);
	}

	public Result(ResultCode code, string? message, T content, Action<Paging>? paging = null)
		: this(code, message)
	{
		Content = content;
		if (paging == null) return;
		Paging = new Paging();
		paging(Paging);
	}
}
