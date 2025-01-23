using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Commands;
using LibraryManagementSystem.Pages;
using LibraryManagementSystem.Service;
using LibraryManagementSystem.Models;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;


namespace LibraryManagementSystem.ViewModels
{
    public class BookViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly IGenreService _genreService;
        private ObservableCollection<Book> _books;
        private ObservableCollection<Genre> _genres;
        private Book _selectedBook;
        private Book _newBook;

        private string _searchTerm;
        private ICollectionView _filteredBooks;

        public Book NewBook
        {
            get => _newBook;
            set
            {
                SetProperty(ref _newBook, value);
                ((RelayCommand)AddBookCommand).RaiseCanExecuteChanged();
            }
        }



        public ObservableCollection<Book> Book
        {
            get => _books;
            set
            {
                if (SetProperty(ref _books, value))
                {
                   
                    InitializeFilteredBooks();
                }
            }
        }

        public ObservableCollection<Genre> Genre
        {
            get => _genres;
            set => SetProperty(ref _genres, value);
        }

        public ICollectionView FilteredBooks
        {
            get => _filteredBooks;
            private set => SetProperty(ref _filteredBooks, value);
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    FilteredBooks.Refresh();
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





        public ICommand OpenAddPopupCommand { get; }
        public ICommand OpenEditPopupCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ClosePopupCommand { get; }
        public ICommand AddBookCommand { get; }
        public ICommand UpdateBookCommand { get; }
        public ICommand DeleteBookCommand { get; }


        public BookViewModel()
        {

            Book = new ObservableCollection<Book>();
            Genre = new ObservableCollection<Genre>();

        }
        public BookViewModel(IBookService bookService, IGenreService genreService)
        {
            _bookService = bookService;
            _genreService = genreService;

            OpenAddPopupCommand = new RelayCommand(parameter => OpenAddPopup());
            OpenEditPopupCommand = new RelayCommand(parameter => OpenEditPopup(), CanExecuteEditPopup);
            CancelCommand = new RelayCommand(parameter => CloseAllPopups());
            ClosePopupCommand = new RelayCommand(parameter => CloseAllPopups());
            AddBookCommand = new RelayCommand(async parameter => await AddBook(parameter), CanExecuteAddBook);
            UpdateBookCommand = new RelayCommand(async parameter => await UpdateBook(parameter), CanExecuteUpdateBook);
            DeleteBookCommand = new RelayCommand(async parameter => await DeleteBook(parameter), CanExecuteDeleteBook);

            LoadBooks();
            
        }

        private void OpenAddPopup()
        {
            NewBook = new Book(); 
            LoadGenres();
            IsAddPopupOpen = true;
        }

        private void OpenEditPopup()
        {
            IsEditPopupOpen = true;
            LoadGenres();
        }

        private bool CanExecuteEditPopup(object parameter)
        {
            return SelectedBook != null;
        }

        private void CloseAllPopups()
        {
            IsAddPopupOpen = false;
            IsEditPopupOpen = false;
        }

        private async Task LoadBooks()
        {
            try
            {
                var booksFromDb = await _bookService.GetAllBooks();
                Book = new ObservableCollection<Book>(booksFromDb);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading books: {ex.Message}");
            }
        }

        private async Task LoadGenres()
        {
            try
            {
                var genresFromDb = await _genreService.GetAllGenre();
                Genre = new ObservableCollection<Genre>(genresFromDb);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading genres: {ex.Message}");
            }
        }


        private void InitializeFilteredBooks()
        {
            if (Book != null)
            {
                FilteredBooks = CollectionViewSource.GetDefaultView(Book);
                FilteredBooks.Filter = FilterBooks;
            }
        }

        private bool FilterBooks(object obj)
        {
            if (obj is Book book)
            {
                return string.IsNullOrWhiteSpace(SearchTerm) || book.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }


        public async Task AddBook(object param)
        {
            if (NewBook == null) return;

            try
            {
               
                await _bookService.AddBook(NewBook);

                
                Book.Add(NewBook);
                         
                Console.WriteLine("Book added successfully.");
            }
            catch (ArgumentNullException ex)
            {
                
                Console.WriteLine($"Validation Error: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
               
                Console.WriteLine($"Validation Error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                
                Console.WriteLine($"Validation Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");
            }
            IsAddPopupOpen = false;
            
        }


     
        private bool CanExecuteAddBook(object param)
        {
            return NewBook != null && !string.IsNullOrEmpty(NewBook.Title);
        }


        public async Task UpdateBook(object param)
        {
            if (SelectedBook == null) return;

            try
            {
                await _bookService.UpdateBook(SelectedBook);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            IsEditPopupOpen = false;
        }


        private bool CanExecuteUpdateBook(object param)
        {
            return SelectedBook != null && !string.IsNullOrEmpty(SelectedBook.Title);
        }


        public async Task DeleteBook(object param)
        {
            if (SelectedBook == null) return;

            try
            {
                await _bookService.DeleteBook(SelectedBook.BookID);
                Book.Remove(SelectedBook);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        private bool CanExecuteDeleteBook(object param)
        {
            return SelectedBook != null;
        }

 


    }
}
