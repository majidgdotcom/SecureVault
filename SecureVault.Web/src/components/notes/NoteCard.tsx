import React, { useState } from 'react';
import { Note, ApiResult } from '../../types';
import Button from '../common/Button';
import NoteForm from './NoteForm';

interface NoteCardProps {
  note: Note;
  onUpdate: (noteId: number, content: string) => Promise<ApiResult<Note>>;
  onDelete: (noteId: number) => Promise<ApiResult<void>>;
  isDeleting: boolean;
}

const NoteCard: React.FC<NoteCardProps> = ({ note, onUpdate, onDelete, isDeleting }) => {
  const [isEditing, setIsEditing] = useState<boolean>(false);
  const [editContent, setEditContent] = useState<string>(note.content);
  const [isUpdating, setIsUpdating] = useState<boolean>(false);

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const handleEdit = () => {
    setEditContent(note.content);
    setIsEditing(true);
  };

  const handleCancelEdit = () => {
    setEditContent(note.content);
    setIsEditing(false);
  };

  const handleSaveEdit = async () => {
    setIsUpdating(true);
    const result = await onUpdate(note.id, editContent);
    setIsUpdating(false);

    if (result.success) {
      setIsEditing(false);
    }
  };

  const handleDelete = async () => {
    if (window.confirm('Are you sure you want to delete this note?')) {
      await onDelete(note.id);
    }
  };

  if (isEditing) {
    return (
      <div className="note-card editing">
        <NoteForm
          value={editContent}
          onChange={setEditContent}
          onSubmit={handleSaveEdit}
          onCancel={handleCancelEdit}
          loading={isUpdating}
          submitLabel="Update Note"
          isEditing={true}
        />
      </div>
    );
  }

  return (
    <div className="note-card">
      <div className="note-header">
        <span className="note-date">{formatDate(note.createdAt)}</span>
      </div>

      <div className="note-content">
        <p>{note.content}</p>
      </div>

      <div className="note-updated">
        {note.updatedAt
          ? `Last updated: ${formatDate(note.updatedAt)}`
          : '—'}
      </div>

      <div className="note-actions">
        <Button
          variant="secondary"
          size="small"
          onClick={handleEdit}
          disabled={isDeleting}
        >
          ✏️ Edit
        </Button>

        <Button
          variant="danger"
          size="small"
          onClick={handleDelete}
          loading={isDeleting}
          disabled={isDeleting}
        >
          🗑️ Delete
        </Button>
      </div>
    </div>
  );
};

export default NoteCard;