using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JustAnother.Model;

public class ApiResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string[] ErrorMessages { get; set; }
    public object Result { get; set; }
    public bool IsSuccess { get; set; }
}
