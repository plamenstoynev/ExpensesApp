using ExpenseTracker.Application.Features.Budgets.Commands.CreateBudget;
using ExpenseTracker.Application.Features.Budgets.Commands.DeleteBudget;
using ExpenseTracker.Application.Features.Budgets.Queries.GetBudgets;
using ExpenseTracker.Application.Features.Categories.Commands.CreateCategory;
using ExpenseTracker.Application.Features.Categories.Commands.DeleteCategory;
using ExpenseTracker.Application.Features.Categories.Queries.GetCategories;
using ExpenseTracker.Application.Features.Reports.Queries.GetMonthlyReport;
using ExpenseTracker.Application.Features.Transactions.Commands.CreateTransaction;
using ExpenseTracker.Application.Features.Transactions.Commands.DeleteTransaction;
using ExpenseTracker.Application.Features.Transactions.Commands.UpdateTransaction;
using ExpenseTracker.Application.Features.Transactions.Queries.GetTransactionById;
using ExpenseTracker.Application.Features.Transactions.Queries.GetTransactions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register FluentValidation validators
        services.AddValidatorsFromAssemblyContaining<CreateTransactionCommandValidator>();

        // Transactions
        services.AddScoped<CreateTransactionCommandHandler>();
        services.AddScoped<DeleteTransactionCommandHandler>();
        services.AddScoped<UpdateTransactionCommandHandler>();
        services.AddScoped<GetTransactionsQueryHandler>();
        services.AddScoped<GetTransactionByIdQueryHandler>();

        // Categories
        services.AddScoped<CreateCategoryCommandHandler>();
        services.AddScoped<DeleteCategoryCommandHandler>();
        services.AddScoped<GetCategoriesQueryHandler>();

        // Budgets
        services.AddScoped<CreateBudgetCommandHandler>();
        services.AddScoped<DeleteBudgetCommandHandler>();
        services.AddScoped<GetBudgetsQueryHandler>();

        // Reports
        services.AddScoped<GetMonthlyReportQueryHandler>();

        return services;
    }
}
