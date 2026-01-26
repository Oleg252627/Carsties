using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions(string? date)
    {
        var query = context.Auctions.AsNoTracking().OrderBy(z => z.Item.Make).AsQueryable();

        if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var parsedDate))
        {
            var parsedDateUtc = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
            query = query.Where(z => z.UpdatedAt > parsedDateUtc);
        }

        return await query.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
    {
        var auction = await context.Auctions
        .Include(z => z.Item)
        .FirstOrDefaultAsync(z => z.Id == id);

        if (auction is null) return NotFound();

        return mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto dto)
    {
        var auction = mapper.Map<Auction>(dto);
        auction.Seller = "test";

        context.Auctions.Add(auction);

        var auctionDto = mapper.Map<AuctionDto>(auction);

        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(auctionDto));

        var result = await context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Failed to create auction");

        return CreatedAtAction(nameof(GetAuction), new { id = auction.Id }, auctionDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto dto)
    {
        var auction = await context.Auctions.Include(z => z.Item).FirstOrDefaultAsync(z => z.Id == id);

        if (auction is null) return NotFound();

        auction.Item.Make = dto.Make ?? auction.Item.Make;
        auction.Item.Model = dto.Model ?? auction.Item.Model;
        auction.Item.Year = dto.Year ?? auction.Item.Year;
        auction.Item.Color = dto.Color ?? auction.Item.Color;
        auction.Item.Mileage = dto.Mileage ?? auction.Item.Mileage;

        await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auction));

        var result = await context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest("Failed to update auction");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await context.Auctions.FindAsync(id);

        if (auction is null) return NotFound();

        context.Auctions.Remove(auction);

        await publishEndpoint.Publish(new AuctionDeleted { Id = auction.Id.ToString() });
        var result = await context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest("Failed to delete auction");
    }
}