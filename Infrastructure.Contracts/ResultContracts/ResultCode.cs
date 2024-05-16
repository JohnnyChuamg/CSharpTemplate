using System.ComponentModel;

namespace Infrastructure.Contracts.ResultContracts;

public enum ResultCode
{
    [Description("Success / OK")]
    Success = 200000,
    [Description("Success Created")]
    SuccessCreated = 201000,
    [Description("The request had been accepted for processing, but the processing has not been completed")]
    Accepted = 202000,
    [Description("Success No Content")]
    SuccessNoContent = 204000,
    [Description("Bad Request (400) - One of the request inputs is not valid.")]
    InvalidInput = 400000,
    [Description("Bad Request (400) - One of the request inputs is out of range.")]
    OutOfRange = 400001,
    [Description("Unauthorized (401)")]
    Unauthorized = 401000,
    [Description("Forbidden")]
    Forbidden = 403000,
    [Description("Username or Password is incorrect.")]
    UsernameOrPasswordIncorrect = 403001,
    [Description("The specified account is disabled.")]
    AccountIsDisabled = 403002,
    [Description("Two-factor authentication is required.")]
    RequiredTwoFactorAuthentication = 403003,
    [Description("Two-factor authentication failed.")]
    TwoFactorAuthenticationFailed = 403004,
    [Description("Two-factor authentication binding is required.")]
    RequiredTwoFactorAuthenticationBinding = 403005,
    [Description("Required to change your password.")]
    ChangePasswordRequired = 403006,
    [Description("The specified resource does not exist.")]
    ResourceNotFound = 404000,
    [Description("The specified resource dependency does not exist.")]
    ResourceDependencyNotFound = 404001,
    [Description("This resource can not acceptable to operated.")]
    ResourceNotAcceptable = 406000,
    [Description("This request takes too long to process.")]
    RequestTimeout = 408000,
    [Description("Conflict (409) - The specified resource already exist.")]
    ResourceAlreadyExists = 409000,
    [Description("Request body too large.")]
    PayloadTooLarge = 413000,
    [Description("Internal Server Error")]
    InternalServerError = 500000
}