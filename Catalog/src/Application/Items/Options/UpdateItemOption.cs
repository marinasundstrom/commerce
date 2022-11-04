using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Options;
using YourBrand.Catalog.Domain;
using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Application.Items.Options;

public record UpdateItemOption(string ItemId, string OptionId, ApiUpdateItemOption Data) : IRequest<OptionDto>
{
    public class Handler : IRequestHandler<UpdateItemOption, OptionDto>
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OptionDto> Handle(UpdateItemOption request, CancellationToken cancellationToken)
        {
            var item = await _context.Items
            .AsNoTracking()
            .FirstAsync(x => x.Id == request.ItemId);

        var option = await _context.Options
            .Include(x => x.Values)
            .Include(x => x.Group)
            .FirstAsync(x => x.Id == request.OptionId);

        var group = await _context.OptionGroups
            .FirstOrDefaultAsync(x => x.Id == request.Data.GroupId);

        option.Name = request.Data.Name;
        option.Description = request.Data.Description;
        option.ItemId = request.Data.ItemId;
        option.Group = group;
        option.IsSelected = request.Data.IsSelected;
        option.Price = request.Data.Price;
        option.OptionType = (Domain.Enums.OptionType)request.Data.OptionType;

        foreach (var v in request.Data.Values)
        {
            if (v.Id == null)
            {
                var value = new OptionValue(v.Name)
                {
                    ItemId = v.ItemId,
                    Price = v.Price
                };

                option.Values.Add(value);
                _context.OptionValues.Add(value);
            }
            else
            {
                var value = option.Values.First(x => x.Id == v.Id);

                value.Name = v.Name;
                value.ItemId = v.ItemId;
                value.Price = v.Price;
            }
        }

        option.DefaultValueId = option.Values.FirstOrDefault(x => x.Id == request.Data.DefaultOptionValueId)?.Id;

        foreach (var v in option.Values.ToList())
        {
            if (_context.Entry(v).State == EntityState.Added)
                continue;

            var value = request.Data.Values.FirstOrDefault(x => x.Id == v.Id);

            if (value is null)
            {
                option.Values.Remove(v);
            }
        }

        await _context.SaveChangesAsync();

        return new OptionDto(option.Id, option.Name, option.Description, (Application.OptionType)option.OptionType, option.Group == null ? null : new OptionGroupDto(option.Group.Id, option.Group.Name, option.Group.Description, option.Group.Seq, option.Group.Min, option.Group.Max), option.ItemId, option.Price, option.IsSelected,
            option.Values.Select(x => new OptionValueDto(x.Id, x.Name, x.ItemId, x.Price, x.Seq)),
            option.DefaultValue == null ? null : new OptionValueDto(option.DefaultValue.Id, option.DefaultValue.Name, option.DefaultValue.ItemId, option.DefaultValue.Price, option.DefaultValue.Seq), option.MinNumericalValue, option.MaxNumericalValue, option.DefaultNumericalValue, option.TextValueMinLength, option.TextValueMaxLength, option.DefaultTextValue);
    
        }
    }
}
