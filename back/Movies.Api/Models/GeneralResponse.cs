using Microsoft.AspNetCore.Mvc;

namespace Movies.Api.Models;

public class GeneralResponse<T>
{
    public T Data { get; set; }
    public int Status { get; set; }
    public string[]? Errors { get; set; }
}