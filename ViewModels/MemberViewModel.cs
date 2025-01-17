using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Service;
using System.Windows.Input;
using LibraryManagementSystem.Commands;
using System.ComponentModel;
using LibraryManagementSystem.Pages;

namespace LibraryManagementSystem.ViewModels
{
    public class MemberViewModel : ViewModelBase
    {
        private readonly IMemberService _memberService;
        private ObservableCollection<Member> _members;
        private Member _selectedMember;
        private Member _newMember;

        private string _searchTerm;
        private ICollectionView _filteredMembers;

        public ObservableCollection<Member> Member
        {
            get => _members;
            set
            {
                if (SetProperty(ref _members, value))
                {
                    InitializeFilteredMembers();
                }
            }
        }

        public Member NewMember
        {
            get => _newMember;
            set
            {
                SetProperty(ref _newMember, value);
                ((RelayCommand)AddMemberCommand).RaiseCanExecuteChanged();
            }
        }

        public Member SelectedMember
        {
            get => _selectedMember;
            set => SetProperty(ref _selectedMember, value);
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
        public ICommand AddMemberCommand { get; }
        public ICommand UpdateMemberCommand { get; }
        public ICommand DeleteMemberCommand { get; }

        public MemberViewModel()
        {
            // Initialize default values or set up services here
            // For example, you could initialize the Books collection to an empty ObservableCollection:
            Member = new ObservableCollection<Member>();
        }
        public MemberViewModel(IMemberService memberService)
        {
            _memberService = memberService;

            OpenPopupCommand = new RelayCommand(parameter => OpenPopup(), null);
            SaveCommand = new RelayCommand(parameter => Save(), CanSave);
            CancelCommand = new RelayCommand(parameter => Cancel(), null);
            ClosePopupCommand = new RelayCommand(parameter => ClosePopup(), null);
            AddMemberCommand = new RelayCommand(async parameter => await AddMember(parameter), CanExecuteAddMember);
            UpdateMemberCommand = new RelayCommand(async parameter => await UpdateMember(parameter), CanExecuteUpdateMember);
            DeleteMemberCommand = new RelayCommand(async parameter => await DeleteMember(parameter), CanExecuteDeleteMember);

            LoadMembers();
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
            return SelectedMember != null && !string.IsNullOrEmpty(SelectedMember.Name);
        }

        private void Cancel()
        {
            IsPopupOpen = false;
        }

        private void ClosePopup()
        {
            IsPopupOpen = false;
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
                Console.WriteLine($"Error loading books: {ex.Message}");
            }
        }

        public async Task AddMember(object param)
        {
            if (SelectedMember == null) return;

            try
            {
                // Call the service to add the member
                await _memberService.AddMember(SelectedMember);
                Member.Add(SelectedMember);
                Console.WriteLine("Member added successfully.");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // You could also show an error dialog or notification in the UI
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Validation Error: {ex.Message}");
                // Show error message to the user (UI notification)
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Inform the user that the contact info is already taken
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                // Show a generic error message to the user
            }
        }

        private bool CanExecuteAddMember(object param)
        {
            return SelectedMember != null && !string.IsNullOrEmpty(SelectedMember.Name);
        }

        public async Task UpdateMember(object param)
        {
            if (SelectedMember == null) return;

            try
            {
                await _memberService.UpdateMember(SelectedMember);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private bool CanExecuteUpdateMember(object param)
        {
            return SelectedMember != null && !string.IsNullOrEmpty(SelectedMember.Name);
        }

        public async Task DeleteMember(object param)
        {
            if (SelectedMember == null) return;

            try
            {
                await _memberService.DeleteMember(SelectedMember.MemberID);
                Member.Remove(SelectedMember);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private bool CanExecuteDeleteMember(object param)
        {
            return SelectedMember != null;
        }



    }
}
