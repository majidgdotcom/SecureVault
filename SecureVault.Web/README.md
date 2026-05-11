# SecureVault — Frontend

React + TypeScript frontend for SecureVault. Communicates with the .NET 8 API to create, read, update, and delete encrypted notes.

## Pattern

The frontend uses **MVVM** via custom hooks:

- `useNotesViewModel` — manages all notes state and exposes typed actions (`createNote`, `updateNote`, `deleteNote`, `refreshNotes`)
- Components are purely presentational; they receive state and callbacks from the ViewModel hook

## Setup

```bash
cp .env.example .env    # set REACT_APP_API_URL to your API base URL
npm install
npm start
```

## Scripts

| Command | Description |
|---|---|
| `npm start` | Dev server at http://localhost:3000 |
| `npm test` | Run tests |
| `npm run build` | Production build |
