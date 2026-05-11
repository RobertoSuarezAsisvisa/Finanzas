using FinanzasMCP.Application.Budgets.Commands;
using FinanzasMCP.Application.Budgets.Handlers;
using FinanzasMCP.Application.Budgets.Queries;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Transactions.Commands;
using FinanzasMCP.Application.Transactions.Handlers;
using FinanzasMCP.Application.Transactions.Queries;
using FinanzasMCP.Domain.Transactions;
using FinanzasMCP.McpServer.Storage;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.McpServer.Api;

public static class CashflowEndpoints
{
    public static void MapCashflowEndpoints(this RouteGroupBuilder api)
    {
        MapTransactions(api.MapGroup("/transactions"));
        MapBudgets(api.MapGroup("/budgets"));
        api.MapTransactionAttachmentEndpoints();
    }

    private static void MapTransactions(RouteGroupBuilder group)
    {
        group.MapGet("", async (Guid? accountId, TransactionType? type, Guid? categoryId, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string? search, int? page, int? pageSize, GetTransactionsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetTransactionsQuery(accountId, type, categoryId, dateFrom, dateTo, search, page ?? 1, pageSize ?? 10), ct)));

        group.MapGet("{id:guid}/attachments", async (Guid id, HttpContext httpContext, IFinanzasMCPDbContext dbContext, CancellationToken ct) =>
        {
            var exists = await dbContext.Set<Transaction>().AnyAsync(x => x.Id == id, ct);
            if (!exists)
            {
                return Results.NotFound();
            }

            var attachments = await dbContext.TransactionAttachments
                .AsNoTracking()
                .Where(x => x.TransactionId == id)
                .ToListAsync(ct);

            return Results.Ok(attachments
                .OrderByDescending(x => x.UploadedAt)
                .Select(x => new TransactionAttachmentSummary(
                x.Id,
                x.TransactionId,
                x.FileName,
                x.ContentType,
                x.SizeBytes,
                x.UploadedAt,
                $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/v1/transaction-attachments/{x.Id}/content")));
        });

        group.MapPost("", async (CreateTransactionRequest request, CreateTransactionHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateTransactionCommand(request.Type, request.Amount, request.Currency, request.AccountId, request.ToAccountId, request.CategoryId, request.BudgetId, request.Description, request.Reference, request.TransactionDate, request.RecurringRuleId, request.TagIds ?? Array.Empty<Guid>()), ct);
            return Results.Created($"/api/v1/transactions/{result.Id}", result);
        });

        group.MapPost("{id:guid}/attachments", async (Guid id, HttpContext httpContext, IFinanzasMCPDbContext dbContext, ITransactionAttachmentProcessor processor, ITransactionAttachmentStorage storage, CancellationToken ct) =>
        {
            var transaction = await dbContext.Set<Transaction>().FirstOrDefaultAsync(x => x.Id == id, ct);
            if (transaction is null)
            {
                return Results.NotFound();
            }

            var form = await httpContext.Request.ReadFormAsync(ct);
            var files = form.Files;
            if (files.Count == 0)
            {
                throw new InvalidOperationException("Select at least one file.");
            }

            if (files.Count > 6)
            {
                throw new InvalidOperationException("You can upload up to 6 files at a time.");
            }

            var summaries = new List<TransactionAttachmentSummary>(files.Count);
            foreach (var file in files)
            {
                ValidateAttachment(file);

                await using var prepared = await processor.PrepareAsync(file, ct);
                var path = await storage.UploadAsync(transaction.UserId, transaction.Id, prepared.FileName, prepared.ContentType, prepared.Content, ct);
                var attachment = TransactionAttachment.Create(transaction.Id, prepared.FileName, prepared.ContentType, prepared.SizeBytes, path);
                attachment.AssignUser(transaction.UserId);
                dbContext.TransactionAttachments.Add(attachment);
                summaries.Add(new TransactionAttachmentSummary(
                    attachment.Id,
                    transaction.Id,
                    attachment.FileName,
                    attachment.ContentType,
                    attachment.SizeBytes,
                    attachment.UploadedAt,
                    $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/v1/transaction-attachments/{attachment.Id}/content"));
            }

            await dbContext.SaveChangesAsync(ct);
            return Results.Ok(summaries);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateTransactionRequest request, UpdateTransactionHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateTransactionCommand(id, request.Type, request.Amount, request.Currency, request.AccountId, request.ToAccountId, request.CategoryId, request.BudgetId, request.Description, request.Reference, request.TransactionDate, request.RecurringRuleId, request.TagIds ?? Array.Empty<Guid>()), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, IFinanzasMCPDbContext dbContext, ITransactionAttachmentStorage storage, DeleteTransactionHandler handler, CancellationToken ct) =>
        {
            var attachmentPaths = await dbContext.TransactionAttachments
                .AsNoTracking()
                .Where(x => x.TransactionId == id)
                .Select(x => x.StoragePath)
                .ToListAsync(ct);

            await handler.Handle(new DeleteTransactionCommand(id), ct);

            foreach (var storagePath in attachmentPaths)
            {
                await storage.DeleteAsync(storagePath, ct);
            }

            return Results.NoContent();
        });
    }

    private static void ValidateAttachment(IFormFile file)
    {
        var allowed = new[]
        {
            "image/jpeg",
            "image/png",
            "image/webp",
            "image/heic",
            "application/pdf"
        };

        if (file.Length <= 0)
        {
            throw new InvalidOperationException("The uploaded file is empty.");
        }

        if (file.Length > 10 * 1024 * 1024)
        {
            throw new InvalidOperationException("Each attachment must be 10 MB or less.");
        }

        if (!allowed.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only JPG, PNG, WEBP, HEIC or PDF files are allowed.");
        }
    }

    private static void MapTransactionAttachmentEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/transaction-attachments");

        group.MapGet("{id:guid}/content", async (Guid id, IFinanzasMCPDbContext dbContext, ITransactionAttachmentStorage storage, CancellationToken ct) =>
        {
            var attachment = await dbContext.TransactionAttachments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            if (attachment is null)
            {
                return Results.NotFound();
            }

            var file = await storage.DownloadAsync(attachment.StoragePath, ct);
            return Results.File(file.Content, file.ContentType, attachment.FileName);
        });

        group.MapDelete("{id:guid}", async (Guid id, IFinanzasMCPDbContext dbContext, ITransactionAttachmentStorage storage, CancellationToken ct) =>
        {
            var attachment = await dbContext.TransactionAttachments.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (attachment is null)
            {
                return Results.NotFound();
            }

            attachment.SoftDelete();
            await dbContext.SaveChangesAsync(ct);
            await storage.DeleteAsync(attachment.StoragePath, ct);
            return Results.NoContent();
        });
    }

    private static void MapBudgets(RouteGroupBuilder group)
    {
        group.MapGet("", async (GetBudgetsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetBudgetsQuery(), ct)));

        group.MapPost("", async (CreateBudgetRequest request, CreateBudgetHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateBudgetCommand(request.Name, request.LimitAmount, request.PeriodType, request.ValidityType, request.PeriodStart, request.PeriodEnd, request.CategoryId), ct);
            return Results.Created($"/api/v1/budgets/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateBudgetRequest request, UpdateBudgetHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateBudgetCommand(id, request.Name, request.LimitAmount, request.PeriodType, request.ValidityType, request.PeriodStart, request.PeriodEnd, request.IsActive), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteBudgetHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteBudgetCommand(id), ct);
            return Results.NoContent();
        });
    }
}
