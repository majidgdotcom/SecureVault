import React from 'react';

const Header: React.FC = () => {
  return (
    <nav className="app-header">
      <div className="header-container">
        <div className="logo">
          <span className="logo-icon">🔒</span>
          <span className="logo-text">SecureVault</span>
        </div>
        <div className="header-info">
          <span className="encryption-badge">AES-256 Encrypted</span>
        </div>
      </div>
    </nav>
  );
};

export default Header;