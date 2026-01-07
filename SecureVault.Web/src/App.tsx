import React from 'react';
import Layout from './components/layout/Layout';
import NotesView from './components/notes/NotesView';
import './styles/index.css';
import './styles/components.css';

const App: React.FC = () => {
  const userId: string = 'user_123';

  return (
    <Layout>
      <NotesView userId={userId} />
    </Layout>
  );
};

export default App;