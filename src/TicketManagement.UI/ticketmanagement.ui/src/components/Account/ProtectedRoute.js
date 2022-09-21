import React from 'react';
import { Navigate } from 'react-router-dom';

import AuthService from '../../services/AuthService';

const ProtectedRoute = ({ roles, children }) => {
    const currentUser = AuthService.getCurrentUser();

    if (!AuthService.isAuthenticated()) {
      return <Navigate to="/Account/Login" />;
    }

    if (roles && roles.indexOf(currentUser.role) === -1) {
        return <Navigate to="/" />;
    }
  
    return children;
  };

  export default ProtectedRoute;