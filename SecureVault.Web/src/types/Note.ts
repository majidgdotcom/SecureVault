export interface Note {
  id: number;
  content: string;
  createdAt: string;
  updatedAt?: string;
  userId: string;
}

export interface CreateNoteDto {
  userId: string;
  content: string;
}

export interface UpdateNoteDto {
  content: string;
}

export interface NoteFormData {
  content: string;
}