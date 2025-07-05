import { ReactKeycloakProvider } from '@react-keycloak/web';
import Keycloak from 'keycloak-js';

const keycloak = new Keycloak({
  url: 'https://mock-keycloak.local/',
  realm: 'mock-realm',
  clientId: 'mock-client',
});

export default function TestKeycloak() {
  return (
    <ReactKeycloakProvider authClient={keycloak}>
      <div>Hello Keycloak Test</div>
    </ReactKeycloakProvider>
  );
} 