import { useState, useEffect, useCallback } from 'react';
import notesService from '../services/notesService';
import { Note, ApiResult } from '../types';


// Notes ViewModel State
interface NotesViewModelState {
  notes: Note[];
  loading: boolean;
  error: string | null;
  creating: boolean;
  updating: boolean;
  deleting: number | null;
}


// Notes ViewModel Return Type
interface NotesViewModelReturn {
  // State
  notes: Note[];
  loading: boolean;
  error: string | null;
  creating: boolean;
  updating: boolean;
  deleting: number | null;

  // Actions
  createNote: (content: string) => Promise<ApiResult<Note>>;
  updateNote: (noteId: number, content: string) => Promise<ApiResult<Note>>;
  deleteNote: (noteId: number) => Promise<ApiResult<void>>;
  refreshNotes: () => void;
  clearError: () => void;
}

export const useNotesViewModel = (userId: string): NotesViewModelReturn => {
  const [state, setState] = useState<NotesViewModelState>({
    notes: [],
    loading: false,
    error: null,
    creating: false,
    updating: false,
    deleting: null,
  });


  // Fetch all notes for the user
  const fetchNotes = useCallback(async () => {
    setState((prev) => ({ ...prev, loading: true, error: null }));

    const result = await notesService.getUserNotes(userId);

    if (result.success && result.data) {
      setState((prev) => ({
        ...prev,
        notes: result.data!,
        loading: false,
      }));
    } else {
      setState((prev) => ({
        ...prev,
        error: result.error || 'Failed to fetch notes',
        loading: false,
      }));
    }
  }, [userId]);


  // Create a new note
  const createNote = useCallback(
    async (content: string): Promise<ApiResult<Note>> => {
      if (!content.trim()) {
        const errorMsg = 'Note content cannot be empty';
        setState((prev) => ({ ...prev, error: errorMsg }));
        return { success: false, error: errorMsg };
      }

      setState((prev) => ({ ...prev, creating: true, error: null }));

      const result = await notesService.createNote({ userId, content });

      if (result.success && result.data) {
        setState((prev) => ({
          ...prev,
          creating: false,
          notes: [result.data!, ...prev.notes],
        }));
      } else {
        setState((prev) => ({
          ...prev,
          error: result.error || 'Failed to create note',
          creating: false,
        }));
      }

      return result;
    },
    [userId]
  );


  // Update an existing note
  const updateNote = useCallback(
    async (noteId: number, content: string): Promise<ApiResult<Note>> => {
      if (!content.trim()) {
        const errorMsg = 'Note content cannot be empty';
        setState((prev) => ({ ...prev, error: errorMsg }));
        return { success: false, error: errorMsg };
      }

      setState((prev) => ({ ...prev, updating: true, error: null }));

      const result = await notesService.updateNote(noteId, { content });

      if (result.success && result.data) {
        setState((prev) => ({
          ...prev,
          updating: false,
          notes: prev.notes.map((note) =>
            note.id === noteId ? result.data! : note
          ),
        }));
      } else {
        setState((prev) => ({
          ...prev,
          error: result.error || 'Failed to update note',
          updating: false,
        }));
      }

      return result;
    },
    []
  );


  // Delete a note
  const deleteNote = useCallback(
    async (noteId: number): Promise<ApiResult<void>> => {
      setState((prev) => ({ ...prev, deleting: noteId, error: null }));

      const result = await notesService.deleteNote(noteId);

      if (result.success) {
        setState((prev) => ({
          ...prev,
          deleting: null,
          notes: prev.notes.filter((note) => note.id !== noteId),
        }));
      } else {
        setState((prev) => ({
          ...prev,
          error: result.error || 'Failed to delete note',
          deleting: null,
        }));
      }

      return result;
    },
    []
  );


  // Clear error message
  const clearError = useCallback(() => {
    setState((prev) => ({ ...prev, error: null }));
  }, []);


  // Refresh notes (manual refresh)
  const refreshNotes = useCallback(() => {
    fetchNotes();
  }, [fetchNotes]);

  // Load notes on mount or when userId changes
  useEffect(() => {
    if (userId) {
      fetchNotes();
    }
  }, [userId, fetchNotes]);

  return {
    // State
    notes: state.notes,
    loading: state.loading,
    error: state.error,
    creating: state.creating,
    updating: state.updating,
    deleting: state.deleting,

    // Actions
    createNote,
    updateNote,
    deleteNote,
    refreshNotes,
    clearError,
  };
};