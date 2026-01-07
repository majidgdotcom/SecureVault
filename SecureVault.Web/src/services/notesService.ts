import axios, { AxiosError } from 'axios';
import { Note, CreateNoteDto, UpdateNoteDto, ApiResult } from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:5043/api';

// Configure axios instance
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    console.error('API Error:', error);
    return Promise.reject(error);
  }
);

// Notes Service - Handles all API communication
class NotesService {

  async getUserNotes(userId: string): Promise<ApiResult<Note[]>> {
    try {
      const response = await apiClient.get<Note[]>(`/notes/user/${userId}`);
      return { success: true, data: response.data };
    } catch (error) {
      return {
        success: false,
        error: this.extractErrorMessage(error),
      };
    }
  }

  async getNote(noteId: number): Promise<ApiResult<Note>> {
    try {
      const response = await apiClient.get<Note>(`/notes/${noteId}`);
      return { success: true, data: response.data };
    } catch (error) {
      return {
        success: false,
        error: this.extractErrorMessage(error),
      };
    }
  }

  async createNote(dto: CreateNoteDto): Promise<ApiResult<Note>> {
    try {
      const response = await apiClient.post<Note>('/notes', dto);
      return { success: true, data: response.data };
    } catch (error) {
      return {
        success: false,
        error: this.extractErrorMessage(error),
      };
    }
  }

  async updateNote(noteId: number, dto: UpdateNoteDto): Promise<ApiResult<Note>> {
    try {
      const response = await apiClient.put<Note>(`/notes/${noteId}`, dto);
      return { success: true, data: response.data };
    } catch (error) {
      return {
        success: false,
        error: this.extractErrorMessage(error),
      };
    }
  }

  async deleteNote(noteId: number): Promise<ApiResult<void>> {
    try {
      await apiClient.delete(`/notes/${noteId}`);
      return { success: true };
    } catch (error) {
      return {
        success: false,
        error: this.extractErrorMessage(error),
      };
    }
  }
   // Extract error message from various error types
  private extractErrorMessage(error: unknown): string {
    if (axios.isAxiosError(error)) {
      const axiosError = error as AxiosError<{ error?: string }>;
      return (
        axiosError.response?.data?.error ||
        axiosError.message ||
        'An unexpected error occurred'
      );
    }
    
    if (error instanceof Error) {
      return error.message;
    }
    
    return 'An unexpected error occurred';
  }
}

export default new NotesService();