import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router';
import { QueryClientProvider } from '@tanstack/react-query';

import App from './App';
import { queryClient } from './api/queryClient';

import './index.css';

const rootElement =
  document.getElementById('root');

if (!rootElement) {
  throw new Error(
    'Elemento root não encontrado.',
  );
}

createRoot(rootElement).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </QueryClientProvider>
  </StrictMode>,
);