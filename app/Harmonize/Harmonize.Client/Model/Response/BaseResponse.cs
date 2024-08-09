using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.Client.Model.Response;

public class BaseResponse<T>
{
    public required string Message { get; set; }
    public required int StatusCode { get; set; }
    public required T Value { get; set; }
}
