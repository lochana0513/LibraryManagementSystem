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
    public class GenreViewModel : ViewModelBase
    {
        private readonly IGenreService _genreService;
        private ObservableCollection<Genre> _genre;
        private Genre _selectedgenre;
        private Genre _newGenre;

        private string _searchTerm;
        private ICollectionView _filteredGenre;

        public Genre NewGenre
        {
            get => _newGenre;
            set
            {
                SetProperty(ref _newGenre, value);
                ((RelayCommand)AddGenreCommand).RaiseCanExecuteChanged();
            }
        }

        public Genre SelectedGenre
        {
            get => _selectedgenre;
            set => SetProperty(ref _selectedgenre, value);
        }

        public ObservableCollection<Genre> Genre
        {
            get => _genre;
            set
            {
                if (SetProperty(ref _genre, value))
                {
                    // Refresh the filtered view when books collection changes
                    InitializeFilteredGenre();
                }
            }
        }


        public ICollectionView FilteredGenre
        {
            get => _filteredGenre;
            private set => SetProperty(ref _filteredGenre, value);
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    FilteredGenre.Refresh();
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
        public ICommand AddGenreCommand { get; }
        public ICommand UpdateGenreCommand { get; }
        public ICommand DeleteGenreCommand { get; }

        public GenreViewModel()
        {
            // Initialize default values or set up services here
            Genre = new ObservableCollection<Genre>();
        }
        public GenreViewModel(IGenreService genreService)
        {
            _genreService = genreService;
            Genre = new ObservableCollection<Genre>();


            OpenAddPopupCommand = new RelayCommand(parameter => OpenAddPopup());
            OpenEditPopupCommand = new RelayCommand(parameter => OpenEditPopup(), CanExecuteEditPopup);
            CancelCommand = new RelayCommand(parameter => CloseAllPopups());
            ClosePopupCommand = new RelayCommand(parameter => CloseAllPopups());
            AddGenreCommand = new RelayCommand(async parameter => await AddGenre(parameter), CanExecuteAddGenre);
            UpdateGenreCommand = new RelayCommand(async parameter => await UpdateGenre(parameter), CanExecuteUpdateGenre);
            DeleteGenreCommand = new RelayCommand(async parameter => await DeleteGenre(parameter), CanExecuteDeleteGenre);

            LoadGenre();


        }

        private void OpenAddPopup()
        {
            NewGenre = new Genre(); // Initialize a new book
            IsAddPopupOpen = true;
        }

        private void OpenEditPopup()
        {
            IsEditPopupOpen = true;
        }

        private bool CanExecuteEditPopup(object parameter)
        {
            return SelectedGenre != null;
        }

        private void CloseAllPopups()
        {
            IsAddPopupOpen = false;
            IsEditPopupOpen = false;
        }

        private async Task LoadGenre()
        {
            try
            {
                var genreFromDb = await _genreService.GetAllGenre();
                Genre = new ObservableCollection<Genre>(genreFromDb);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading benre: {ex.Message}");
            }
        }

        private void InitializeFilteredGenre()
        {
            if (Genre != null)
            {
                FilteredGenre = CollectionViewSource.GetDefaultView(Genre);
                FilteredGenre.Filter = FilterGenre;
            }
        }

        private bool FilterGenre(object obj)
        {
            if (obj is Genre genre)
            {
                return string.IsNullOrWhiteSpace(SearchTerm) || genre.GenreName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        // Updated AddGenre Method
        private async Task AddGenre(object param)
        {
            if (NewGenre == null || string.IsNullOrEmpty(NewGenre.GenreName)) return;

            try
            {
                await _genreService.AddGenre(NewGenre);
                Genre.Add(NewGenre);
                Console.WriteLine("Genre added successfully.");
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

        private bool CanExecuteAddGenre(object param)
        {
            return SelectedGenre != null && !string.IsNullOrEmpty(SelectedGenre.GenreName);
        }

        // Update a genre
        private async Task UpdateGenre(object param)
        {
            if (SelectedGenre == null) return;

            try
            {
                await _genreService.UpdateGenre(SelectedGenre);
                Console.WriteLine("Genre updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            IsEditPopupOpen = false;  
        }

        // Can Update Genre be executed
        private bool CanExecuteUpdateGenre(object param)
        {
            return SelectedGenre != null && !string.IsNullOrEmpty(SelectedGenre.GenreName);
        }

        // Delete a genre
        private async Task DeleteGenre(object param)
        {
            if (SelectedGenre == null) return;

            try
            {
                await _genreService.DeleteGenre(SelectedGenre.GenreID);
                Genre.Remove(SelectedGenre);
                Console.WriteLine("Genre deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Can Delete Genre be executed
        private bool CanExecuteDeleteGenre(object param)
        {
            return SelectedGenre != null;
        }




    }
}
