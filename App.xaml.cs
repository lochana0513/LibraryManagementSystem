using System.Configuration;
using System.Data;
using System.Windows;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Service;
using LibraryManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagementSystem
{

    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set up the DI container
            var serviceCollection = new ServiceCollection();

            // Register application services
            serviceCollection.AddSingleton<IBookService, BookService>(); 
            serviceCollection.AddSingleton<IMemberService, MemberService>();
            serviceCollection.AddSingleton<IGenreService, GenreService>(); 
            serviceCollection.AddSingleton<ITransactionService, TransactionService>();

            // Register DbContext
            serviceCollection.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer("Data Source=(localdb)\\localhost;Initial Catalog=testDB;Integrated Security=True;"));



            // Register ViewModels 
            serviceCollection.AddTransient<BookViewModel>();
            serviceCollection.AddTransient<MemberViewModel>();
            serviceCollection.AddTransient<GenreViewModel>();
            serviceCollection.AddTransient<TransactionViewModel>();


            // Build the provider
            ServiceProvider = serviceCollection.BuildServiceProvider();

        }

    }

}
