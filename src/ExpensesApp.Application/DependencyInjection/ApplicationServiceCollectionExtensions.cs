using ExpensesApp.Application.Features.Budgets.Commands.CreateBudget;
using ExpensesApp.Application.Features.Budgets.Commands.DeleteBudget;
using ExpensesApp.Application.Features.Budgets.Queries.GetBudgets;
using ExpensesApp.Application.Features.Categories.Commands.CreateCategory;
using ExpensesApp.Application.Features.Categories.Commands.DeleteCategory;
using ExpensesApp.Application.Features.Categories.Queries.GetCategories;
using ExpensesApp.Application.Features.Reports.Queries.GetMonthlyReport;
using ExpensesApp.Application.Features.Transactions.Commands.CreateTransaction;
using ExpensesApp.Application.Features.Transactions.Commands.DeleteTransaction;
using ExpensesApp.Application.Features.Transactions.Commands.UpdateTransaction;
using ExpensesApp.Application.Features.Transactions.Queries.GetTransactionById;
using ExpensesApp.Application.Features.Transactions.Queries.GetTransactions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ExpensesApp.Application.DependencyInjection;

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
