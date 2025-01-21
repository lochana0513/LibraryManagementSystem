﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibraryManagementSystem.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagementSystem.Pages
{
    /// <summary>
    /// Interaction logic for LendBooks.xaml
    /// </summary>
    public partial class LendBooks : Page
    {
        public LendBooks()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetRequiredService<TransactionViewModel>();
        }
    }
}
