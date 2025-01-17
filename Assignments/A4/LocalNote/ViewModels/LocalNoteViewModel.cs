﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalNote.Models;
using LocalNote.Repositories;
using LocalNote.Commands;

namespace LocalNote.ViewModels
{
    public class LocalNoteViewModel : INotifyPropertyChanged
    {
        public SaveCommand SaveCommand { get; }
        public EditCommand EditCommand { get; }
        public DeleteCommand DeleteCommand { get; }
        public AboutCommand AboutCommand { get; }
        public ExitCommand ExitCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<NoteModel> Notes { get; set; }
        public List<NoteModel> _allNotes = new List<NoteModel>();
        private NoteModel _selectedNote;

        public string Content { get; set; }
        public string Title { get; set; }
        private string _filter;

        public LocalNoteViewModel ()
        {
            SaveCommand = new SaveCommand(this, _allNotes);
            SaveCommand.OnNoteCreated += Save_OnNoteCreated;
            EditCommand = new EditCommand(this);
            EditCommand.OnNoteChanged += Edit_OnNoteChanged;
            DeleteCommand = new DeleteCommand(this, _allNotes);
            DeleteCommand.OnNoteDeleted += Delete_OnNoteDeleted;
            AboutCommand = new AboutCommand();
            ExitCommand = new ExitCommand();

            Notes = new ObservableCollection<NoteModel>();
            //LocalNoteRepo.GetNotesFromFolder(_allNotes);
            LocalNoteRepo.GetNotesFromDB(_allNotes);

            // Initial data
            NoteModel first = new NoteModel("First", "this is the first note");
            NoteModel Another = new NoteModel("Another", "this is another note");
            _allNotes.Add(first);
            _allNotes.Add(Another);

            PerformFiltering();
        }
        private void Save_OnNoteCreated(object sender, EventArgs e)
        {
            if (SaveCommand.NewNote != null)
            {
                _allNotes.Add(SaveCommand.NewNote);
                PerformFiltering();
            }
        }
        private void Edit_OnNoteChanged(object sender, EventArgs e)
        {
            if (SelectedNote != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Content"));
                // enabling save button now that it is in edit mode
                SaveCommand.FireCanExecuteChanged();
            }
        }
        private void Delete_OnNoteDeleted(object sender, EventArgs e)
        {
            SelectedNote = null;
            PerformFiltering();
        }

        public NoteModel SelectedNote
        {
            get { return _selectedNote; }
            set
            {
                _selectedNote = value;
                if(value == null)
                {
                    // new note has empty editable content and title of untitled note
                    MainPage.EditContentTextbox.IsReadOnly = false;
                    Title = "Untitled Note";
                    Content = "";

                } 
                else
                {
                    // User can't edit unless in edit mode
                    MainPage.EditContentTextbox.IsReadOnly = true;
                    MainPage.EditAppBarButton.IsEnabled = true;
                    
                    Content = value.Content;
                    Title = value.Title;
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Content"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Title"));

                // Change which commands are enabled
                SaveCommand.FireCanExecuteChanged();
                EditCommand.FireCanExecuteChanged();
                DeleteCommand.FireCanExecuteChanged();
            }
        }
        
        public string Filter { 
            get { return _filter; } 
            set
            {
                if (value == _filter) { return; }
                _filter = value;
                PerformFiltering();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Filter)));
            }
        }

        private void PerformFiltering()
        {
            if (_filter == null)
            {
                _filter = "";
            }
            //If _filter has a value (ie. user entered something in Filter textbox)
            //Lower-case and trim string
            var lowerCaseFilter = Filter.ToLowerInvariant().Trim();

            //Use LINQ query to get all personmodel names that match filter text, as a list
            var result =
                _allNotes.Where(note => note.Title.ToLowerInvariant()
                .Contains(lowerCaseFilter))
                .ToList();

            //Get list of values in current filtered list that we want to remove
            //(ie. don't meet new filter criteria)
            var toRemove = Notes.Except(result).ToList();

            //Loop to remove items that fail filter
            foreach (var x in toRemove)
            {
                Notes.Remove(x);
            }

            var resultCount = result.Count;
            // Add back in correct order.
            for (int i = 0; i < resultCount; i++)
            {
                var resultItem = result[i];
                if (i + 1 > Notes.Count || !Notes[i].Equals(resultItem))
                {
                    Notes.Insert(i, resultItem);
                }
            }
        }
    }
}
