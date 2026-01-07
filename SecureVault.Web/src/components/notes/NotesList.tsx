import React from 'react';
import { Note, ApiResult } from '../../types';
import NoteCard from './NoteCard';
import Button from '../common/Button';

interface NotesListProps {
  notes: Note[];
  onUpdate: (noteId: number, content: string) => Promise<ApiResult<Note>>;
  onDelete: (noteId: number) => Promise<ApiResult<void>>;
  onRefresh: () => void;
  deleting: number | null;
}

const NotesList: React.FC<NotesListProps> = ({
  notes,
  onUpdate,
  onDelete,
  onRefresh,
  deleting
}) => {
  if (notes.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-icon">📝</div>
        <h3>No notes yet</h3>
        <p>Create your first encrypted note to get started!</p>
      </div>
    );
  }

  return (
    <div className="notes-list-container">
      <div className="notes-list-header">
        <h2>Your Secure Notes ({notes.length})</h2>
        <Button variant="secondary" size="small" onClick={onRefresh}>
          🔄 Refresh
        </Button>
      </div>

      <div className="notes-list">
        {notes.map((note) => (
          <NoteCard
            key={note.id}
            note={note}
            onUpdate={onUpdate}
            onDelete={onDelete}
            isDeleting={deleting === note.id}
          />
        ))}
      </div>
    </div>
  );
};

export default NotesList;