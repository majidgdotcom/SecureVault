import React from 'react';
import Button from '../common/Button';

interface NoteFormProps {
  value: string;
  onChange: (value: string) => void;
  onSubmit: () => void;
  onCancel?: () => void;
  disabled?: boolean;
  loading?: boolean;
  placeholder?: string;
  submitLabel?: string;
  isEditing?: boolean;
}

const NoteForm: React.FC<NoteFormProps> = ({
  value,
  onChange,
  onSubmit,
  onCancel,
  disabled = false,
  loading = false,
  placeholder = 'Write your secure note...',
  submitLabel = 'Save Note',
  isEditing = false,
}) => {
  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (value.trim() && onSubmit) {
      onSubmit();
    }
  };

  const handleTextareaChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    onChange(e.target.value);
  };

  return (
    <form onSubmit={handleSubmit} className="note-form">
      <div className="form-group">
        <label htmlFor="note-content" className="form-label">
          <span>{isEditing ? 'Edit Note' : 'Create New Note'}</span>
        </label>
        <textarea
          id="note-content"
          className="form-textarea"
          value={value}
          onChange={handleTextareaChange}
          placeholder={placeholder}
          rows={5}
          disabled={disabled}
          required
          maxLength={5000}
        />
        <div className="char-count">
          {value.length} / 5000 characters
        </div>
      </div>

      <div className="form-actions">
        <Button
          type="submit"
          variant="primary"
          loading={loading}
          disabled={disabled || !value.trim()}
        >
          {submitLabel}
        </Button>

        {isEditing && onCancel && (
          <Button
            type="button"
            variant="secondary"
            onClick={onCancel}
            disabled={loading}
          >
            Cancel
          </Button>
        )}
      </div>
    </form>
  );
};

export default NoteForm;