import React from 'react';
import Layout from './components/Layout/Layout';
import 'bootstrap/dist/css/bootstrap.min.css';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Events from './components/Event/Events';
import Login from './components/Account/Login';
import Register from './components/Account/Register';
import { Container } from 'reactstrap';
import CreateEvent from './components/Event/CreateEvent';
import NotPublishedEvents from './components/Event/NotPublishedEvents';
import '@eonasdan/tempus-dominus/dist/css/tempus-dominus.css';
import { library, dom } from '@fortawesome/fontawesome-svg-core';
import { fas } from '@fortawesome/free-solid-svg-icons';
import ProtectedRoute from './components/Account/ProtectedRoute';

library.add(fas);
dom.watch();

function App() {
  return (
    <Router>
      <Layout />
      <Container>
        <Routes>
          <Route path="/" element={<Events />} />
          <Route path="/Account/Login" element={<Login />} />
          <Route path="/Account/Register" element={<Register />} />
          <Route
            path="/Event/CreateEvent"
            element={
              <ProtectedRoute roles={['Event manager']}>
                <CreateEvent />
              </ProtectedRoute>
            }
          />
          <Route
            path="/Event/NotPublishedEvents"
            element={
              <ProtectedRoute roles={['Event manager']}>
                <NotPublishedEvents />
              </ProtectedRoute>
            }
          />
        </Routes>
      </Container>
    </Router>
  );
}

export default App;
