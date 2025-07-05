import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import { ReactKeycloakProvider } from '@react-keycloak/web';
import Keycloak from 'keycloak-js';

const keycloak = new Keycloak({
  url: 'http://localhost:8080/',
  realm: 'devrealm',          
  clientId: 'frontend',      
});

const eventLogger = (event, error) => {
  console.log('Keycloak event:', event, error);
};

const tokenLogger = (tokens) => {
  console.log('Keycloak tokens:', tokens);
};

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <ReactKeycloakProvider
    authClient={keycloak}
    onEvent={eventLogger}
    onTokens={tokenLogger}
  >
    <App />
  </ReactKeycloakProvider>
); 