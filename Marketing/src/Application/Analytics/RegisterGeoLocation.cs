using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Analytics.Commands;

public record RegisterGeoLocation(string ClientId, string SessionId, YourBrand.Marketing.Domain.ValueObjects.Coordinates Coordinates) : IRequest
{
    public class Handler : IRequestHandler<RegisterGeoLocation>
    {
        private readonly IApplicationDbContext context;

        public Handler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(RegisterGeoLocation request, CancellationToken cancellationToken)
        {
            var session = await context.Sessions
                .FirstAsync(x => x.Id == request.SessionId && x.ClientId == request.ClientId, cancellationToken);

            session.Coordinates = request.Coordinates;

            await context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
