using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using LibraryManagementSystem.Commands;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Pages;
using LibraryManagementSystem.Service;

namespace LibraryManagementSystem.ViewModels
{
    public class TransactionViewModel : ViewModelBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IMemberService _memberService;
        private readonly IBookService _bookService;
        private ObservableCollection<BorrowingTransaction> _transactions;
        private ObservableCollection<Member> _members;
        private ObservableCollection<Book> _books;
        private Book _selectedBook;
        private ObservableCollection<Book> _selectedBooks = new ObservableCollection<Book>();


        private BorrowingTransaction _selectedTransaction;
        private BorrowingTransaction _newTransaction;

        private string _searchTerm;
        private ICollectionView _filteredTransaction;

        public ICommand OpenAddPopupCommand { get; }
        public ICommand OpenEditPopupCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ClosePopupCommand { get; }
        public ICommand AddTransactionCommand { get; }
        public ICommand UpdateTransactionCommand { get; }
        public ICommand DeleteTransactionCommand { get; }
        public ICommand AddBookCommand { get; }
        public ICommand DeleteBookCommand { get; }


        public BorrowingTransaction NewTransaction
        {
            get => _newTransaction;
            set
            {
                SetProperty(ref _newTransaction, value);

                // Initialize default BorrowDate and DueDate
                if (_newTransaction != null)
                {
                    _newTransaction.BorrowDate = DateTime.Now;
                    _newTransaction.DueDate = DateTime.Now.AddDays(14); // Default to 2 weeks from now
                }

                ((RelayCommand)AddTransactionCommand).RaiseCanExecuteChanged();
            }
        }



        public ObservableCollection<BorrowingTransaction> BorrowingTransaction
        {
            get => _transactions;
            set
            {
                if (SetProperty(ref _transactions, value))
                {

                    InitializeFilteredTransaction();
                }
            }
        }

        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                if (SetProperty(ref _selectedBook, value))
                {
                    ((RelayCommand)AddBookCommand).RaiseCanExecuteChanged();
                }
            }
        }


        public ObservableCollection<Book> SelectedBooks
        {
            get => _selectedBooks;
            set => SetProperty(ref _selectedBooks, value);
        }

        public ObservableCollection<Member> Member
        {
            get => _members;
            set => SetProperty(ref _members, value);
        }

        public ObservableCollection<Book> Book
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        public ICollectionView FilteredTransaction
        {
            get => _filteredTransaction;
            private set => SetProperty(ref _filteredTransaction, value);
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    FilteredTransaction.Refresh();
                }
            }
        }

        public BorrowingTransaction SelectedTransaction
        {
            get => _selectedTransaction;
            set
            {
                if (SetProperty(ref _selectedTransaction, value))
                {
                    ((RelayCommand)OpenEditPopupCommand).RaiseCanExecuteChanged();
                }
            }
        }




        private bool _isAddPopupOpen;
        public bool IsAddPopupOpen
        {
            get => _isAddPopupOpen;
            set => SetProperty(ref _isAddPopupOpen, value);
        }

        private bool _isEditPopupOpen;
        public bool IsEditPopupOpen
        {
            get => _isEditPopupOpen;
            set => SetProperty(ref _isEditPopupOpen, value);
        }

        public TransactionViewModel()
        {

            BorrowingTransaction = new ObservableCollection<BorrowingTransaction>();
            Member = new ObservableCollection<Member>();
            Book = new ObservableCollection<Book>();
        }

        public TransactionViewModel(ITransactionService transactionServic,IMemberService memberService,IBookService bookService)
        {
            _transactionService = transactionServic;
            _memberService = memberService;
            _bookService = bookService;
            OpenAddPopupCommand = new RelayCommand(parameter => OpenAddPopup());
            OpenEditPopupCommand = new RelayCommand(parameter => OpenEditPopup(), CanExecuteEditPopup);
            CancelCommand = new RelayCommand(parameter => CloseAllPopups());
            ClosePopupCommand = new RelayCommand(parameter => CloseAllPopups());
            AddTransactionCommand = new RelayCommand(ExecuteAddTransaction, CanExecuteAddTransaction);
            AddBookCommand = new RelayCommand(AddSelectedBook, CanAddBook);
            DeleteBookCommand = new RelayCommand(DeleteBook, CanDeleteBook);

            LoadTransaction();


        }

        private async Task OpenAddPopup()
        {
            NewTransaction = new BorrowingTransaction();
            await LoadMembers();
            await LoadBooks();
            IsAddPopupOpen = true;
        }

        private void OpenEditPopup()
        {
            IsEditPopupOpen = true;
            
        }

        private bool CanExecuteEditPopup(object parameter)
        {
            return SelectedTransaction != null;
        }

        private void CloseAllPopups()
        {
            IsAddPopupOpen = false;
            IsEditPopupOpen = false;
        }
        private void InitializeFilteredTransaction()
        {
            if (BorrowingTransaction != null)
            {
                FilteredTransaction = CollectionViewSource.GetDefaultView(BorrowingTransaction);
                FilteredTransaction.Filter = FilterTransaction;
            }
        }

        private bool FilterTransaction(object obj)
        {
            if (obj is BorrowingTransaction borrowingTransaction)
            {
                // Check if SearchTerm is an integer and matches MemberID
                if (int.TryParse(SearchTerm, out int searchMemberID))
                {
                    return borrowingTransaction.MemberID == searchMemberID;
                }

                // Otherwise, check if the SearchTerm matches other string properties
                return borrowingTransaction.MemberID.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private async Task LoadMembers()
        {
            try
            {
                var membersFromDb = await _memberService.GetAllMembers();
                Member = new ObservableCollection<Member>(membersFromDb);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading Members: {ex.Message}");
            }
        }

        private async Task LoadBooks()
        {
            try
            {
                var booksFromDb = await _bookService.GetAllAvailableBooks();
                Book = new ObservableCollection<Book>(booksFromDb);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading Books: {ex.Message}");
            }
        }

        private async Task LoadTransaction()
        {
            try
            {
                var transactionFromDb = await _transactionService.GetAllLendTransactions();
                BorrowingTransaction = new ObservableCollection<BorrowingTransaction>(transactionFromDb);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading books: {ex.Message}");
            }
        }

        private bool CanExecuteAddTransaction(object parameter) =>
            NewTransaction != null && SelectedBooks.Count > 0;

        private void ExecuteAddTransaction(object parameter)
        {
            try
            {
                // Map selected books to transaction books
                var transactionBooks = new List<TransactionBook>();
                foreach (var book in SelectedBooks)
                {
                    transactionBooks.Add(new TransactionBook
                    {
                        BookID = book.BookID,
                        BorrowingTransaction = NewTransaction
                    });
                }

                // Add transaction
                _transactionService.AddLendTransaction(NewTransaction, transactionBooks);
                IsAddPopupOpen = false;
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., log or display a message)
            }
        }


        private void AddSelectedBook(object parameter)
        {
            if (SelectedBook != null && !_selectedBooks.Contains(SelectedBook))
            {
                SelectedBooks.Add(SelectedBook);
            }
        }

        private bool CanAddBook(object parameter)
        {
            return SelectedBook != null;
        }

        private void DeleteBook(object parameter)
        {
            if (parameter is Book book && SelectedBooks.Contains(book))
            {
                SelectedBooks.Remove(book);
            }
        }

        private bool CanDeleteBook(object parameter)
        {
            return parameter is Book book && SelectedBooks.Contains(book);
        }



    }
}
