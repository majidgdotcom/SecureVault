import React, { useState } from 'react';
import { useNotesViewModel } from '../../viewmodels/useNotesViewModel';
import NotesList from './NotesList';
import NoteForm from './NoteForm';
import ErrorMessage from '../common/ErrorMessage';
import LoadingSpinner from '../common/LoadingSpinner';

interface NotesViewProps {
  userId?: string;
}

const NotesView: React.FC<NotesViewProps> = ({ userId = 'user_123' }) => {
  const [newNoteContent, setNewNoteContent] = useState<string>('');

  const {
    notes,
    loading,
    error,
    creating,
    deleting,
    createNote,
    updateNote,
    deleteNote,
    refreshNotes,
    clearError,
  } = useNotesViewModel(userId);

  const handleCreateNote = async (): Promise<void> => {
    const result = await createNote(newNoteContent);
    if (result.success) {
      setNewNoteContent(''); // Clear form on success
    }
  };

  return (
    <div className="notes-view">
      <header className="notes-header">
        <div className="header-content">

        </div>
      </header>

      {error && <ErrorMessage message={error} onDismiss={clearError} />}

      <section className="create-note-section">
        <NoteForm
          value={newNoteContent}
          onChange={setNewNoteContent}
          onSubmit={handleCreateNote}
          loading={creating}
          submitLabel="Create Encrypted Note"
        />
      </section>

      <section className="notes-section">
        {loading && notes.length === 0 ? (
          <LoadingSpinner message="Loading your secure notes..." />
        ) : (
          <NotesList
            notes={notes}
            onUpdate={updateNote}
            onDelete={deleteNote}
            onRefresh={refreshNotes}
            deleting={deleting}
          />
        )}
      </section>

      <footer className="notes-footer">
        <p>
          All notes are encrypted on the server using AES-256 encryption with
          unique initialization vectors. Your data is secure.
        </p>
      </footer>
    </div>
  );
};

export default NotesView;