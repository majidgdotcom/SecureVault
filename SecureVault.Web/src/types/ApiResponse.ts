export interface ApiResult<T> {
  success: boolean;
  data?: T;
  error?: string;
}

export interface ErrorResponse {
  error: string;
  validationErrors?: string[];
}