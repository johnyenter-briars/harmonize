namespace Harmonize.Client.Model.Response;

public class BaseResponse<T>
{
    public required string Message { get; set; }
    public required int StatusCode { get; set; }
    public required T? Value { get; set; }
    public bool Success => StatusCode == 201 || StatusCode == 200;
    public string SuccessMessage => Success ? "Success" : "Error";
}
