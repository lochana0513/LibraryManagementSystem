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
using System.Windows.Data;

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

        public ICollectionView FilteredMembers
        {
            get => _filteredMembers;
            private set => SetProperty(ref _filteredMembers, value);
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    FilteredMembers.Refresh();
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

            OpenAddPopupCommand = new RelayCommand(parameter => OpenAddPopup());
            OpenEditPopupCommand = new RelayCommand(parameter => OpenEditPopup(), CanExecuteEditPopup);
            CancelCommand = new RelayCommand(parameter => CloseAllPopups());
            ClosePopupCommand = new RelayCommand(parameter => CloseAllPopups());
            AddMemberCommand = new RelayCommand(async parameter => await AddMember(parameter), CanExecuteAddMember);
            UpdateMemberCommand = new RelayCommand(async parameter => await UpdateMember(parameter), CanExecuteUpdateMember);
            DeleteMemberCommand = new RelayCommand(async parameter => await DeleteMember(parameter), CanExecuteDeleteMember);

            LoadMembers();
        }

        private void OpenAddPopup()
        {
            NewMember = new Member(); // Initialize a new book
            IsAddPopupOpen = true;
        }

        private void OpenEditPopup()
        {
            IsEditPopupOpen = true;
        }

        private bool CanExecuteEditPopup(object parameter)
        {
            return SelectedMember != null;
        }

        private void CloseAllPopups()
        {
            IsAddPopupOpen = false;
            IsEditPopupOpen = false;
        }

        private void InitializeFilteredMembers()
        {
            if (Member != null)
            {
                FilteredMembers = CollectionViewSource.GetDefaultView(Member);
                FilteredMembers.Filter = FilterMember;
            }
        }

        private bool FilterMember(object obj)
        {
            if (obj is Member member)
            {
                return string.IsNullOrWhiteSpace(SearchTerm) || member.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);
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
                Console.WriteLine($"Error loading books: {ex.Message}");
            }
        }

        public async Task AddMember(object param)
        {
            if (NewMember == null) return;

            try
            {
                // Call the service to add the member
                await _memberService.AddMember(NewMember);
                Member.Add(NewMember);
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
            IsAddPopupOpen = false;
        }

        private bool CanExecuteAddMember(object param)
        {
            return NewMember != null && !string.IsNullOrEmpty(NewMember.Name);
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
            IsEditPopupOpen = false;
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
