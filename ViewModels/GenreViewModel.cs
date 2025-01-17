using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public ObservableCollection<Genre> Genre
        {
            get => _genre;
            set => SetProperty(ref _genre, value);
        }

        public Genre SelectedGenre
        {
            get => _selectedgenre;
            set => SetProperty(ref _selectedgenre, value);
        }

        private bool _isPopupOpen;
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetProperty(ref _isPopupOpen, value); // Ensure this notifies the UI
        }


        public ICommand OpenPopupCommand { get; }
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

            OpenPopupCommand = new RelayCommand(parameter => OpenPopup(), null);
            SaveCommand = new RelayCommand(parameter => Save(), CanSave);
            CancelCommand = new RelayCommand(parameter => Cancel(), null);
            ClosePopupCommand = new RelayCommand(parameter => ClosePopup(), null);
            AddGenreCommand = new RelayCommand(async parameter => await AddGenre(parameter), CanExecuteAddGenre);
            UpdateGenreCommand = new RelayCommand(async parameter => await UpdateGenre(parameter), CanExecuteUpdateGenre);
            DeleteGenreCommand = new RelayCommand(async parameter => await DeleteGenre(parameter), CanExecuteDeleteGenre);

            LoadGenre();


        }

        private void OpenPopup()
        {
            IsPopupOpen = true;
        }

        private void Save()
        {
            IsPopupOpen = false;
        }

        private bool CanSave(object parameter)
        {
            return SelectedGenre != null && !string.IsNullOrEmpty(SelectedGenre.GenreName);
        }

        private void Cancel()
        {
            IsPopupOpen = false;
        }

        private void ClosePopup()
        {
            IsPopupOpen = false;
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

        // Updated AddGenre Method
        private async Task AddGenre(object param)
        {
            if (SelectedGenre == null || string.IsNullOrEmpty(SelectedGenre.GenreName)) return;

            try
            {
                await _genreService.AddGenre(SelectedGenre);
                Genre.Add(SelectedGenre);
                IsPopupOpen = false;
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
