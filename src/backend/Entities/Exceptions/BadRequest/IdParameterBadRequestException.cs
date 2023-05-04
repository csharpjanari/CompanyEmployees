namespace Entities.Exceptions.BadRequest;

public sealed class IdParameterBadRequestException : BadRequestException
{
    public IdParameterBadRequestException() : 
        base("Parameter ids is null")
    { }
}