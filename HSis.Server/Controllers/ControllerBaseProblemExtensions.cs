using Microsoft.AspNetCore.Mvc;

namespace HSis.Server.Controllers;

internal static class ControllerBaseProblemExtensions
{
    // Problem(extensions: ...) falla si la extension ya existe, y ProblemDetailsFactory
    // agrega "code" en CustomizeProblemDetails; por eso el codigo especifico se asigna despues.
    public static ObjectResult ProblemConCodigo(
        this ControllerBase controller,
        int statusCode,
        string title,
        string detail,
        string code)
    {
        var problem = controller.ProblemDetailsFactory.CreateProblemDetails(
            controller.HttpContext,
            statusCode,
            title,
            detail: detail);
        problem.Extensions["code"] = code;

        return new ObjectResult(problem) { StatusCode = problem.Status ?? statusCode };
    }
}