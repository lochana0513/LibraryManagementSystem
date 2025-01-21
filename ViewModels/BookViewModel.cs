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
                    // Refresh the filtered view when books collection changes
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
            // Initialize default values or set up services here
            // For example, you could initialize the Books collection to an empty ObservableCollection:
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
            NewBook = new Book(); // Initialize a new book
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

        // Add a book
        public async Task AddBook(object param)
        {
            if (NewBook == null) return;

            try
            {
                // Call the service to add the book
                await _bookService.AddBook(NewBook);

                // Add the book to the observable collection for UI updates
                Book.Add(NewBook);
                // Set a success message               
                Console.WriteLine("Book added successfully."); // Log the message (optional)
            }
            catch (ArgumentNullException ex)
            {
                // Handle validation error: Null or missing data
                Console.WriteLine($"Validation Error: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                // Handle validation error: Invalid arguments
                Console.WriteLine($"Validation Error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                // Handle validation error: Duplicate book
                Console.WriteLine($"Validation Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle other unexpected errors
                Console.WriteLine($"Error: {ex.Message}");
            }
            IsAddPopupOpen = false;
            
        }


        // Can Add Book be executed
        private bool CanExecuteAddBook(object param)
        {
            return NewBook != null && !string.IsNullOrEmpty(NewBook.Title);
        }

        // Update a book
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

        // Can Update Book be executed
        private bool CanExecuteUpdateBook(object param)
        {
            return SelectedBook != null && !string.IsNullOrEmpty(SelectedBook.Title);
        }

        // Delete a book
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

        // Can Delete Book be executed
        private bool CanExecuteDeleteBook(object param)
        {
            return SelectedBook != null;
        }

        protected override void Validate(string propertyName)
        {
            if (propertyName == "Title")
            {
                if (string.IsNullOrEmpty(SelectedBook?.Title))
                {
                    AddValidationError("Title", "Title is required.");
                }
                else if (SelectedBook.Title.Length > 255)
                {
                    AddValidationError("Title", "Title cannot exceed 255 characters.");
                }
                else
                {
                    RemoveValidationError("Title");
                }
            }
            else if (propertyName == "Author")
            {
                if (string.IsNullOrEmpty(SelectedBook?.Author))
                {
                    AddValidationError("Author", "Author is required.");
                }
                else if (SelectedBook.Author.Length > 255)
                {
                    AddValidationError("Author", "Author cannot exceed 255 characters.");
                }
                else
                {
                    RemoveValidationError("Author");
                }
            }
            else if (propertyName == "ISBN")
            {
                if (!string.IsNullOrEmpty(SelectedBook?.ISBN) && SelectedBook.ISBN.Length != 13)
                {
                    AddValidationError("ISBN", "ISBN must be 13 characters long.");
                }
                else
                {
                    RemoveValidationError("ISBN");
                }
            }
            else if (propertyName == "GenreID")
            {
                if (SelectedBook?.GenreID == null) // Check if GenreID is null
                {
                    AddValidationError("Genre", "Genre cannot be null");
                }
                else
                {
                    RemoveValidationError("GenreID");
                }
            }

            else if (propertyName == "Availability")
            {
                if (SelectedBook?.Availability == null)
                {
                    AddValidationError("Availability", "Availability is required.");
                }
                else
                {
                    RemoveValidationError("Availability");
                }
            }
        }


    }
}
