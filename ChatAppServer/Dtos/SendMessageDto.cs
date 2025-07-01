using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ChatAppServer.Dtos;

public record SendMessageDto(Guid UserId,Guid ToUserId,string message);

