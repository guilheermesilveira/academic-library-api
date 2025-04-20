using System.Net;
using Newtonsoft.Json;

namespace Library.Api.Responses;

public class BadRequestResponse : Response
{
    [JsonProperty(Order = 3)] public List<string>? Errors { get; private set; }

    public BadRequestResponse(List<string>? errors)
    {
        Title = "One or more validation errors occurred";
        Status = (int)HttpStatusCode.BadRequest;
        Errors = errors ?? new List<string>();
    }
}